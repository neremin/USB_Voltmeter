using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using Voltmeter.Common;
using Voltmeter.Resources;
using Voltmeter.Services;
using ThreadState = System.Threading.ThreadState;

namespace Voltmeter.Model
{
    public sealed class PollingLoop
    {
        const int MaxReadAttempts = 5;

        readonly IDeviceStream                              DeviceStream;
        readonly ITaskQueue                                 EventsQueue;
        readonly CancellationTokenSource                    Cancellation;
        readonly TimeSpan                                   PollingInterval;
        readonly byte[]                                     RequestBytes;
        readonly Func<DateTimeOffset, byte[], DataSample>   SampleParse;

        Thread LoopThread;

        public event EventHandler<DataSampleArgs>           DataReceived;
        public event EventHandler<EventArgs>                Disposed;

        public PollingLoop
            (
                IDeviceStream deviceStream, 
                DataRequest dataRequest, 
                TimeSpan pollingInterval, 
                Func<DateTimeOffset, byte[], DataSample> sampleParse, 
                ITaskQueue eventsQueue
            )
        {
            Contract.Requires<ArgumentNullException>(deviceStream != null);
            Contract.Requires<ArgumentNullException>(sampleParse != null);
            Contract.Requires<ArgumentNullException>(eventsQueue != null);
            
            DeviceStream = deviceStream;
            EventsQueue = eventsQueue;
            Cancellation = new CancellationTokenSource();
            PollingInterval = pollingInterval;
            SampleParse = sampleParse;
            RequestBytes = new byte[DeviceStream.MaxRequestSize];

            Contract.Assert(dataRequest.RawBytes.Length == DeviceStream.MaxRequestSize);

            Array.Copy(dataRequest.RawBytes, RequestBytes, Math.Min(RequestBytes.Length, dataRequest.RawBytes.Length));

            LoopThread = new Thread(RunLoop)
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal,
                Name = Strings.USB_Device_Polling_Loop,
            };            
        }

        public void RunAsync()
        {
            var thread = LoopThread;
            if (thread == null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if ((thread.ThreadState & ThreadState.Unstarted) != ThreadState.Unstarted)
            {
                throw new InvalidOperationException(Errors.Polling_Device_Already);
            }
            
            thread.Start();
            Contract.Assert(thread.IsAlive);
        }

        public bool IsRunning
        {
            get
            {
                var thread = LoopThread;
                return thread != null && thread.IsAlive;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void EnqueueEvent<TEventArgs>(EventHandler<TEventArgs> handler, TEventArgs args)
            where TEventArgs : EventArgs
        {
            if (handler != null)
            {
                EventsQueue.Enqueue(() => handler(this, args));
            }
        }
        
        void Dispose(bool disposing)
        {
            var thread = Interlocked.Exchange(ref LoopThread, null);
            if (thread != null)
            {
                Cancellation.Cancel(true);
                DeviceStream.Dispose();
                Cancellation.Dispose();
                if (disposing)
                {
                    EnqueueEvent(Disposed, EventArgs.Empty);
                }
            }
        }

        ~PollingLoop()
        {
            Dispose(false);
        }

        void RunLoop()
        {
            Log.Info(Strings.Device_Polling_Loop_Started);

            var stream = DeviceStream;
            var cancel = Cancellation.Token;

            var responseWaitInterval = TimeSpan.FromMilliseconds(PollingInterval.TotalMilliseconds * 0.9);

            var responseBuffer = new byte[stream.MaxResponseSize];
                
            var timeStats = new TimeStats();
            uint failedAttemptsCount = 0;

            while (!cancel.IsCancellationRequested && failedAttemptsCount < MaxReadAttempts)
            {
                try
                {
                    timeStats.BeginSample();

                    // Send request
                    stream.Write(RequestBytes, 0, RequestBytes.Length);

                    // Receive response
                    bool succeed = stream.Read(responseBuffer, 0, responseBuffer.Length, responseWaitInterval, cancel);

                    var timeElapsed = timeStats.EndSample();

                    // Log failed attempts
                    if (!succeed)
                    {
                        ++failedAttemptsCount;
                        continue;
                    }

                    // Dispatch response
                    EnqueueEvent(DataReceived, new DataSampleArgs(SampleParse(DateTimeOffset.Now, responseBuffer)));
                    failedAttemptsCount = 0;

                    // Wait before next request if needed
                    if (timeElapsed < PollingInterval)
                    {
                        cancel.WaitHandle.WaitOne(PollingInterval - timeElapsed);
                        Debug.WriteLine("Device polling waited {0:c}", PollingInterval - timeElapsed);
                    }
                }
                catch (OperationCanceledException)
                {
                    // This must be caused by StopPolling() and/or Dispose() call
                    Contract.Assert(cancel.IsCancellationRequested);
                    break;
                }
                catch (ObjectDisposedException)
                {
                    // This must be caused by StopPolling() and/or Dispose() call
                    Contract.Assert(cancel.IsCancellationRequested);
                    break;
                }
                catch (Exception exception)
                {
                    Log.ExceptionError(exception, Errors.Data_Request_Failed);
                    break;
                }
            }

            // Stopped after MaxReadAttempts consequent failed requests
            if (MaxReadAttempts == failedAttemptsCount)
            {
                Log.WarningFormat(Errors.Too_Many_Failed_Attempts_1, failedAttemptsCount, responseWaitInterval);
            }

            // Notify of this Stop event
            Log.Info(Strings.Device_Polling_Loop_Stopped);

            // Log time stats if needed
            if (timeStats.Avg > PollingInterval)
            {
                Log.Warning(
                    string.Concat(
                        string.Format(Strings.Device_Too_Slow_0, PollingInterval),
                            Environment.NewLine,
                            timeStats.ToString(Environment.NewLine, "g")
                    )
                );
            }

            // Final cleanup
            Dispose();
        }
    }
}