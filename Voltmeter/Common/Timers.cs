using System;
using System.ComponentModel;

namespace Voltmeter.Common
{
    /// <summary>
    /// Common interface for timers.
    /// </summary>
    public interface ITimer : IComponent
    {
        /// <summary> 
        /// Gets or sets a value indicating whether the Timer raises the Tick event each time the specified
        /// Interval has elapsed, when Enabled is set to true.
        /// </summary> 
        bool AutoReset { get; set; }

        /// <summary>
        /// <para>Gets or sets a value indicating whether the <see cref='ITimer'/> 
        /// is able to raise events at a defined interval.</para>
        /// <para>The default value should be false.</para>
        /// </summary> 
        bool Enabled { get; set; }

        /// <summary>
        /// Starts the timing by setting <see cref='ITimer.Enabled'/> to <see langword='true'/>.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the timing by setting <see cref='ITimer.Enabled'/> to <see langword='false'/>.
        /// </summary>
        void Stop();

        /// <summary>
        /// Gets or sets the interval on which to raise events.
        /// </summary> 
        TimeSpan Interval { get; set; }

        /// <summary>
        /// Occurs when the specified timer interval has elapsed and the timer is enabled.
        /// </summary> 
        event EventHandler Tick;
    }

    public class AsyncTimer : Component, ITimer
    {
        readonly System.Timers.Timer _timer;

        public AsyncTimer()
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += (s, e) => OnTick(EventArgs.Empty);
        }

        public bool AutoReset
        {
            get { return _timer.AutoReset; }
            set { _timer.AutoReset = value; }
        }

        public bool Enabled
        {
            get { return _timer.Enabled; }
            set { _timer.Enabled = value; }
        }

        public void Start()
        {
            Enabled = true;
        }

        public void Stop()
        {
            Enabled = false;
        }

        public TimeSpan Interval
        {
            get { return TimeSpan.FromMilliseconds(_timer.Interval); }
            set { _timer.Interval = value.TotalMilliseconds; }
        }

        void OnTick(EventArgs args)
        {
            var handler = Tick;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public event EventHandler Tick;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class UiTimer : Component, ITimer
    {
        readonly System.Windows.Forms.Timer _timer;

        public UiTimer()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Tick += (s, e) =>
            {
                _timer.Enabled = AutoReset;
                OnTick(EventArgs.Empty);
            };
        }

        public bool AutoReset
        {
            get;
            set;
        }

        public void Start()
        {
            Enabled = true;
        }

        public void Stop()
        {
            Enabled = false;
        }

        public bool Enabled
        {
            get { return _timer.Enabled; }
            set { _timer.Enabled = value; }
        }

        public TimeSpan Interval
        {
            get { return TimeSpan.FromMilliseconds(_timer.Interval); }
            set { _timer.Interval = (int)value.TotalMilliseconds; }
        }

        void OnTick(EventArgs args)
        {
            var handler = Tick;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public event EventHandler Tick;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
