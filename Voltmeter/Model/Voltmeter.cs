using System.ComponentModel;
using PropertyChanged;
using System;
using System.Diagnostics.Contracts;
using System.Threading;
using Voltmeter.Binding.Extensions;
using Voltmeter.Common;
using Voltmeter.Services;
using Voltmeter.Resources;

namespace Voltmeter.Model
{
    /// <summary>
    /// Represents USB HID Voltmeter 
    /// </summary>
    public sealed class Voltmeter : INotifyPropertyChanged
    {
        readonly IDeviceManager DeviceManager;
        readonly ITaskQueue     EventsQueue;
        PollingLoop             PollingLoop;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public Voltmeter(IDeviceManager deviceManager, ITaskQueue eventsQueue)
        {
            Contract.Requires<ArgumentNullException>(deviceManager != null);
            Contract.Requires<ArgumentNullException>(eventsQueue != null);

            DeviceManager = deviceManager;
            EventsQueue = eventsQueue;
            Readings = new ElectricReadings();
            Use10BitVoltage = true;
            PollingInterval = DeviceConstants.MinPollingInterval;
        }

        /// <summary>
        /// Gets voltmeter readings in SI units
        /// </summary>
        [DoNotNotify]
        public ElectricReadings Readings { get; private set; }

        /// <summary>
        /// Indicates if data polling is in progress
        /// </summary>
        public bool IsRunning  { get { return AttachedDevice.HasValue && PollingLoop != null && PollingLoop.IsRunning; } }

        /// <summary>
        /// Gets/sets if voltage data should be interpreted as 10-bit (true) or 8-bit (false)
        /// </summary>
        public bool Use10BitVoltage { get; set; }

        /// <summary>
        /// Gets/sets attached USB device which is for data query
        /// </summary>
        public DeviceInformation? AttachedDevice { get; set; }

        /// <summary>
        /// Gets/sets device polling interval
        /// </summary>
        public TimeSpan PollingInterval { get; set; }
        
        /// <summary>
        /// Starts/stops available voltmeter polling
        /// </summary>
        public void ToggleStartStop()
        {
            Contract.Requires<InvalidOperationException>(AttachedDevice.HasValue);
            if (IsRunning)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        void Stop()
        {
            var pollingLoop = Interlocked.Exchange(ref PollingLoop, null);
            if (pollingLoop != null)
            {
                pollingLoop.Dispose();
                this.Notify(PropertyChanged, me => me.IsRunning);
            }
        }

        void Start()
        {
            Contract.Requires<InvalidOperationException>(AttachedDevice.HasValue && !IsRunning);

            var loop = CreatePollingLoop();
            if (Interlocked.CompareExchange(ref PollingLoop, loop, null) != null)
            {
                loop.Dispose();
                throw new InvalidOperationException(Errors.Device_Already_Opened);
            }
                
            loop.RunAsync();
            this.Notify(PropertyChanged, me => me.IsRunning);
        }

        PollingLoop CreatePollingLoop()
        {
            Contract.Assert(AttachedDevice.HasValue);
            
            bool useMaxPollingInterval = (DeviceConstants.MaxPollingInterval == PollingInterval);
            var request = useMaxPollingInterval ? DataRequest.Periodic : DataRequest.Frequent;
            
            var stream = DeviceManager.OpenDeviceStream(AttachedDevice.Value);

            var pollingLoop = new PollingLoop
                (
                    stream, request, PollingInterval,
                    (timeStamp, bytes) =>
                        DataSampleConverter.FromBytes
                            (
                                bytes, timeStamp,
                                Use10BitVoltage,
                                useMaxPollingInterval
                            ),
                    EventsQueue
                );

            pollingLoop.DataReceived += (s, args) => Readings.AddSample(args.Sample);
            pollingLoop.Disposed += (s, args) => Stop();

            return pollingLoop;
        }

        /// <devdoc>
        /// Method gets called when AttachedDevice property changes. 
        /// Method name depends on property name.
        /// https://github.com/Fody/PropertyChanged/wiki/On_PropertyName_Changed
        /// </devdoc>
        void OnAttachedDeviceChanged()
        {
            Stop();
        }
        
        [ContractInvariantMethod]
        void Invariant()
        {
            Contract.Invariant(IsRunning && AttachedDevice.HasValue && PollingLoop != null || !IsRunning && PollingLoop == null);
            Contract.Invariant(DeviceConstants.MinPollingInterval <= PollingInterval && PollingInterval <= DeviceConstants.MaxPollingInterval);
        }
    }
}