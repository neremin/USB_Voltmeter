using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Voltmeter.Resources;

namespace Voltmeter.Common
{
    [ContractClass(typeof(ITaskQueueContract))]
    public interface ITaskQueue
    {
        void Enqueue(Action task);
    }

    [ContractClassFor(typeof(ITaskQueue))]
    abstract class ITaskQueueContract : ITaskQueue
    {
        public void Enqueue(Action task)
        {
            Contract.Requires<ArgumentNullException>(task != null);            
        }
    }

    public class InterlockedTaskQueue : ITaskQueue
    {
        readonly object _syncRoot = new object();

        public void Enqueue(Action task)
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_syncRoot, Timeout.Infinite, ref lockTaken);
                task();
            }
            catch (Exception ex)
            {
                Log.ExceptionError(ex, Errors.Task_Execution_Failed);
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_syncRoot);
                }
            }
        }
    }

    public class AsyncTaskQueue : ITaskQueue
    {
        Task _lastTask;
        readonly TaskScheduler _taskScheduler;
        readonly object _lastTaskLock = new object();

        public AsyncTaskQueue(TaskScheduler taskScheduler)
        {
            _taskScheduler = taskScheduler;
            _lastTask = Task.Factory.StartNew(() => { });
        }

        public void Enqueue(Action task)
        {
            Action<Task> taskAction = delegate
            {
                try
                {
                    task();
                }
                catch (Exception ex)
                {
                    Log.ExceptionError(ex, Errors.Task_Execution_Failed);
                }
            };

            lock (_lastTaskLock)
            {
                _lastTask = _lastTask.ContinueWith(taskAction, _taskScheduler);
            }
        }
    }

    public class CurrentContextTaskQueue : AsyncTaskQueue
    {
        public CurrentContextTaskQueue()
            : base(TaskScheduler.FromCurrentSynchronizationContext())
        {
        }
    }

    public class PooledTaskQueue : AsyncTaskQueue
    {
        public PooledTaskQueue()
            : base(TaskScheduler.Default)
        {
        }
    }
}
