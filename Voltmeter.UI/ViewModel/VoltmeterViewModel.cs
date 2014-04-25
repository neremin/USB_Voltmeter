using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using PropertyChanged;
using System;
using System.Diagnostics.Contracts;
using Voltmeter.Binding;
using Voltmeter.Binding.Extensions;
using Voltmeter.Common;
using Voltmeter.Model;
using Voltmeter.Services;
using Voltmeter.UI.Properties;
using Voltmeter.UI.Resources;
using Voltmeter = Voltmeter.Model.Voltmeter;

namespace Voltmeter.UI.ViewModel
{
    public sealed class VoltmeterViewModel : INotifyPropertyChanged
    {
        readonly Model.Voltmeter Voltmeter;
        readonly Model.DeviceMonitor DeviceMonitor;
        readonly IBindingManager Bindings;
        readonly Stopwatch StopWatch;
        readonly ITimer Timer;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public VoltmeterViewModel(IDeviceManager deviceManager, ITimer timer = null)
        {
            Contract.Requires<ArgumentNullException>(deviceManager != null);
            Bindings = new BindingManager();
            Voltmeter = new Model.Voltmeter(deviceManager, new CurrentContextTaskQueue());
            DeviceMonitor = new DeviceMonitor(deviceManager, DeviceConstants.SearchPattern);
            StopWatch = new Stopwatch();
            Timer = timer ?? new UiTimer
            {
                AutoReset = true,
                Interval = TimeSpan.FromMilliseconds(50),
            };
            Timer.Tick += (s, e) => { if (IsRunning) this.Notify(PropertyChanged, me => me.SessionDuration); };
            AvailableDevices.ListChanged += (s, e) => AutoSelectDevice(forceDeviceChange: false);

            AddDisplayValues();
            AddBindings();
            AutoSelectDevice(forceDeviceChange: true);
        }

        public byte DisplayPrecision { get; set; }
        [DoNotNotify]
        public BindingList<DeviceInformation> AvailableDevices { get { return DeviceMonitor.AvailableDevices; } }
        public DisplayValue Resistance { get; private set; }
        [DoNotNotify]
        public DisplayValue Voltage { get; private set; }
        [DoNotNotify]
        public DisplayValue NoiseThreshold { get; private set; }
        [DoNotNotify]
        public DisplayValue Current { get; private set; }
        [DoNotNotify]
        public DisplayValue Power { get; private set; }
        [DoNotNotify]
        public DisplayValue Energy { get; private set; }
        public double VoltageScale { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsAvailable { get { return AvailableDevices.Count > 0 && DeviceIndex >= 0; } }
        public bool Use10BitVoltage { get; set; }
        public TimeSpan PollingInterval { get; set; }
        [DoNotNotify]
        public TimeSpan MaxPollingInterval { get { return DeviceConstants.MaxPollingInterval; } }
        [DoNotNotify]
        public TimeSpan MinPollingInterval { get { return DeviceConstants.MinPollingInterval; } }
        public DateTimeOffset? LastUpdate { get; private set; }
        public TimeSpan SessionDuration { get { return StopWatch.Elapsed; } }
        public int DeviceIndex { get; set; }
        [DoNotNotify]
        DeviceInformation? SelectedDevice { get { return IsAvailable ? AvailableDevices[DeviceIndex] : (DeviceInformation?) null; } }

        public void LoadSettings()
        {
            try
            {
                Timer.Interval = Settings.Default.DurationUpdateInterval;

                DisplayPrecision = Settings.Default.Precision;

                Resistance.Units.TargetIndex = Math.Min(Settings.Default.ResistanceUnitIndex, Resistance.Units.List.Length - 1);
                Voltage.Units.TargetIndex = Math.Min(Settings.Default.VoltageUnitIndex, Voltage.Units.List.Length - 1);
                Current.Units.TargetIndex = Math.Min(Settings.Default.CurrentUnitIndex, Current.Units.List.Length - 1);
                Power.Units.TargetIndex = Math.Min(Settings.Default.PowerUnitIndex, Power.Units.List.Length - 1);
                Energy.Units.TargetIndex = Math.Min(Settings.Default.EnergyUnitIndex, Energy.Units.List.Length - 1);
                NoiseThreshold.Units.TargetIndex = Math.Min(Settings.Default.NoiseThresholdUnitIndex, NoiseThreshold.Units.List.Length - 1);

                Voltmeter.Use10BitVoltage = Settings.Default.Use10BitsVoltage;
                Voltmeter.PollingInterval = Settings.Default.PollingInterval;
                Voltmeter.Readings.NoiseThreshold = Settings.Default.NoiseThreshold;

                Voltmeter.Readings.SetResistance(Settings.Default.Resistance, true);
                Voltmeter.Readings.SetVoltageScale(Settings.Default.VoltageScale, true);
            }
            catch (Exception ex)
            {
                Log.ExceptionError(ex, Strings.Load_Settings_Error);
            }
        }

        public void SaveSettings()
        {
            try
            {
                Settings.Default.Precision = DisplayPrecision;

                Settings.Default.Resistance = Voltmeter.Readings.Resistance;
                Settings.Default.VoltageScale = Voltmeter.Readings.VoltageScale;
                Settings.Default.NoiseThreshold = Voltmeter.Readings.NoiseThreshold;

                Settings.Default.NoiseThresholdUnitIndex = NoiseThreshold.Units.TargetIndex;
                Settings.Default.ResistanceUnitIndex = Resistance.Units.TargetIndex;
                Settings.Default.VoltageUnitIndex = Voltage.Units.TargetIndex;
                Settings.Default.CurrentUnitIndex = Current.Units.TargetIndex;
                Settings.Default.PowerUnitIndex = Power.Units.TargetIndex;
                Settings.Default.EnergyUnitIndex = Energy.Units.TargetIndex;

                Settings.Default.Use10BitsVoltage = Voltmeter.Use10BitVoltage;
                Settings.Default.PollingInterval = Voltmeter.PollingInterval;

                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Log.ExceptionError(ex, Strings.Save_Settings_Error);
            }
        }

        public void SetResistance(double resistance, bool adjustValues)
        {
            double convertedResistance = UnitConverter.Convert
                (
                    resistance, 
                    from: Resistance.Units.Target,
                    to:   Resistance.Units.Source
                );
            Voltmeter.Readings.SetResistance(convertedResistance, adjustValues);
        }

        public void SetVoltageScale(double voltageScale, bool adjustValues)
        {
            Voltmeter.Readings.SetVoltageScale(voltageScale, adjustValues);
        }

        public void ToggleStartStop()
        {
            Voltmeter.ToggleStartStop();
        }

        public void Reset()
        {
            if (StopWatch.IsRunning)
            {
                StopWatch.Restart();
            }
            else
            {
                StopWatch.Reset();
            }
            Voltmeter.Readings.Reset();
        }

        void AddDisplayValues()
        {
            Resistance = new DisplayValue
                (
                    () => Voltmeter.Readings.Resistance,
                    Units.Resistance.Ohm,
                    Units.Resistance.All
                );

            Voltage = new DisplayValue
                (
                    () => Voltmeter.Readings.Voltage,
                    Units.Voltage.Volts,
                    Units.Voltage.All
                );

            Current = new DisplayValue
                (
                    () => Voltmeter.Readings.Current,
                    Units.Current.Ampers,
                    Units.Current.All
                );

            Power = new DisplayValue
                (
                    () => Voltmeter.Readings.Power,
                    Units.Power.Watts,
                    Units.Power.All
                );

            Energy = new DisplayValue
                (
                    () => Voltmeter.Readings.Energy,
                    Units.Energy.WattSecond,
                    Units.Energy.All
                );

            NoiseThreshold = new DisplayValue
                (
                    () => Voltmeter.Readings.NoiseThreshold,
                    Units.Voltage.Volts,
                    Units.Voltage.All.ToArray(), // use list copy to avoid side-effects with Voltage (while DataBinding)
                    minSourceValue: 0.0,
                    maxSourceValue: DeviceConstants.MaxMeasuredVoltage
                );
        }

        void AddBindings()
        {
            Bindings.BindOneWay
                (
                    () => Voltmeter.Readings.TimeStamp,
                    () => LastUpdate
                )
                .RefreshTarget();

            Bindings.BindOneWay
                (
                    () => Voltmeter.Readings.VoltageScale,
                    () => VoltageScale 
                )
                .RefreshTarget();

            Bindings.BindOneWay
                (
                    () => Voltmeter.IsRunning,
                    () => IsRunning
                )
                .RefreshTarget();

            Bindings.BindTwoWay
                (
                    () => Voltmeter.Use10BitVoltage,
                    () => Use10BitVoltage
                )
                .RefreshTarget();

            Bindings.BindTwoWay
                (
                    () => Voltmeter.PollingInterval,
                    () => PollingInterval
                )
                .RefreshTarget();
        }

        void AutoSelectDevice(bool forceDeviceChange = false)
        {
            DeviceIndex = AvailableDevices.Count == 0 ? -1 : Math.Max(DeviceIndex, 0);
            if (forceDeviceChange)
            {
                OnDeviceIndexChanged();
            }
        }

        /// <devdoc>
        /// Method gets called when IsRunning property changes. 
        /// Method name depends on property name.
        /// https://github.com/Fody/PropertyChanged/wiki/On_PropertyName_Changed
        /// </devdoc>
        void OnIsRunningChanged()
        {
            if (IsRunning)
            {
                StopWatch.Start();
                Timer.Start();
            }
            else
            {
                StopWatch.Stop();
                Timer.Stop();
            }
        }

        void OnDeviceIndexChanged()
        {
            Voltmeter.AttachedDevice = SelectedDevice;
        }

        [ContractInvariantMethod]
        void Invariant()
        {
            Contract.Invariant(AvailableDevices != null);
            Contract.Invariant(IsAvailable && Voltmeter.AttachedDevice.HasValue && DeviceIndex >= 0 || 
                              !IsAvailable && !Voltmeter.AttachedDevice.HasValue && DeviceIndex == -1);
            Contract.Invariant(IsRunning && IsAvailable || !IsRunning);
        }
    }
}