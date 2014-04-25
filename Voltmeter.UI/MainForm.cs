using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Voltmeter.Binding.Extensions;
using Voltmeter.UI.Resources;
using Voltmeter.UI.ViewModel;

namespace Voltmeter.UI
{
    public partial class MainForm : Form
    {
        readonly Func<VoltmeterViewModel> CreateViewModel;
        VoltmeterViewModel ViewModel { get; set; }

        public MainForm(Func<VoltmeterViewModel> viewModelFactory)
        {
            Contract.Requires<ArgumentNullException>(viewModelFactory != null);
            CreateViewModel = viewModelFactory;
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ViewModel = CreateViewModel();
            ViewModel.LoadSettings();

            BindDataSources();
            BindDisplayValues();
            BindControlsState();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ViewModel.SaveSettings();
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Shortcuts
            if (keyData == (Keys.Control | Keys.Delete))
            {
                OnReset(this, EventArgs.Empty);
                return true;
            }
            if (keyData == (Keys.Control | Keys.Space) && ViewModel.IsAvailable)
            {
                OnToggleStartStop(this, EventArgs.Empty);
                return true;
            }
            if (keyData == (Keys.F1))
            {
                new About().ShowDialog(this);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #region Bindings
        void BindDataSources()
        {
            _cmbAvailableDevices.DataSource = ViewModel.AvailableDevices;
            _cmbResistanceUnits.DataSource  = ViewModel.Resistance.Units.List;
            _cmbVoltageUnits.DataSource     = ViewModel.Voltage.Units.List;
            _cmbCurrentUnits.DataSource     = ViewModel.Current.Units.List;
            _cmbPowerUnits.DataSource       = ViewModel.Power.Units.List;
            _cmbEnergyUnits.DataSource      = ViewModel.Energy.Units.List;
            _cmdVoltageNoiseUnits.DataSource = ViewModel.NoiseThreshold.Units.List;
        }

        void BindDisplayValues()
        {
            _numResistance.AddDataBinding(v => v.Value, ViewModel.Resistance, vm => vm.Value, DataSourceUpdateMode.OnValidation);
            _numResistance.AddDataBinding(v => v.DecimalPlaces, ViewModel, vm => vm.DisplayPrecision);
            _cmbResistanceUnits.AddDataBinding(v => v.SelectedIndex, ViewModel.Resistance.Units, vm => vm.TargetIndex);

            _txtVoltage.AddDataBinding(v => v.Value, ViewModel.Voltage, vm => vm.Value);
            _txtVoltage.AddDataBinding(v => v.DecimalPlaces, ViewModel, vm => vm.DisplayPrecision);
            _cmbVoltageUnits.AddDataBinding(v => v.SelectedIndex, ViewModel.Voltage.Units, vm => vm.TargetIndex);
           
            _txtCurrent.AddDataBinding(v => v.Value, ViewModel.Current, vm => vm.Value);
            _txtCurrent.AddDataBinding(v => v.DecimalPlaces, ViewModel, vm => vm.DisplayPrecision);
            _cmbCurrentUnits.AddDataBinding(v => v.SelectedIndex, ViewModel.Current.Units, vm => vm.TargetIndex);

            _txtPower.AddDataBinding(v => v.Value, ViewModel.Power, vm => vm.Value);
            _txtPower.AddDataBinding(v => v.DecimalPlaces, ViewModel, vm => vm.DisplayPrecision);
            _cmbPowerUnits.AddDataBinding(v => v.SelectedIndex, ViewModel.Power.Units, vm => vm.TargetIndex);

            _txtEnergy.AddDataBinding(v => v.Value, ViewModel.Energy, vm => vm.Value);
            _txtEnergy.AddDataBinding(v => v.DecimalPlaces, ViewModel, vm => vm.DisplayPrecision);
            _cmbEnergyUnits.AddDataBinding(v => v.SelectedIndex, ViewModel.Energy.Units, vm => vm.TargetIndex);


            _numVoltageScale.AddDataBinding(v => v.Value, ViewModel, vm => vm.VoltageScale, DataSourceUpdateMode.OnValidation);
            _numVoltageScale.AddDataBinding(v => v.DecimalPlaces, ViewModel, vm => vm.DisplayPrecision);

            _numVoltageNoiseThreshold.AddDataBinding(v => v.Value, ViewModel.NoiseThreshold, vm => vm.Value);
            _numVoltageNoiseThreshold.AddDataBinding(v => v.Minimum, ViewModel.NoiseThreshold, vm => vm.MinValue);
            _numVoltageNoiseThreshold.AddDataBinding(v => v.Maximum, ViewModel.NoiseThreshold, vm => vm.MaxValue);
            _numVoltageNoiseThreshold.AddDataBinding(v => v.DecimalPlaces, ViewModel, vm => vm.DisplayPrecision);
            _cmdVoltageNoiseUnits.AddDataBinding(v => v.SelectedIndex, ViewModel.NoiseThreshold.Units, vm => vm.TargetIndex);

            _numPollingInterval.AddDataBinding(v => v.Value, ViewModel, vm => vm.PollingInterval)
                .Formatting
                    .Format(t => (decimal) t.TotalSeconds)
                    .Parse(s => TimeSpan.FromSeconds((double) s))
                .Binding
                    .ReadValue();
            
            _numPollingInterval.Minimum = (decimal)ViewModel.MinPollingInterval.TotalSeconds;
            _numPollingInterval.Maximum = (decimal)ViewModel.MaxPollingInterval.TotalSeconds;

            
            _btn10BitVoltage.AddDataBinding(v => v.Checked, ViewModel, vm => vm.Use10BitVoltage);

            _numPrecision.AddDataBinding(v => v.Value, ViewModel, vm => vm.DisplayPrecision);
            _lblLastUpdate.AddDataBinding(v => v.Text, ViewModel, vm => vm.LastUpdate)
                .NullValue(Strings.Unknown)
                .Formatting
                    .FormatString("dd-MMM-yyyy  hh:mm:ss.fff")
                    .FormatProvider(CultureInfo.CurrentUICulture);

            _lblSessionDuration.AddDataBinding(v => v.Text, ViewModel, vm => vm.SessionDuration)
                .NullValue(TimeSpan.Zero.ToString("G"))
                .DataSourceNullValue(TimeSpan.Zero)
                .Formatting
                    .Format(v => v.ToString("G"));

            _cmbAvailableDevices.AddDataBinding(v => v.SelectedIndex, ViewModel, vm => vm.DeviceIndex);
        }

        void BindControlsState()
        {
            this.AddDataBinding(v => v.IsRunning, ViewModel, vm => vm.IsRunning);
            this.AddDataBinding(v => v.IsAvailable, ViewModel, vm => vm.IsAvailable);
            this.AddDataBinding(v => v.Use10BitVoltage, ViewModel, vm => vm.Use10BitVoltage)
                .Binding.ReadValue();
        }

        public bool IsRunning
        {
            get { return default(bool); } // Does not matter. This is placeholder to enable binding
            set
            {
                SetReadOnly(_numResistance, value);
                SetReadOnly(_numVoltageScale, value);
                SetReadOnly(_numVoltageNoiseThreshold, value);
                SetReadOnly(_numPollingInterval, value);
                _btnStartStop.Text = value ? Strings.Stop : Strings.Start;
                _cmbAvailableDevices.Enabled = !value;
            }
        }

        public bool IsAvailable
        {
            get { return default(bool); } // Does not matter. This is placeholder to enable binding
            set
            {
                _btnStartStop.Enabled = value;
                if (value)
                {
                    _lblDeviceStatus.Text = Strings.Device_Available;
                    _lblDeviceStatus.ForeColor = Color.Green;
                }
                else
                {
                    _lblDeviceStatus.Text = Strings.Device_Unavailable;
                    _lblDeviceStatus.ForeColor = Color.Red;
                }
            }
        }

        public bool Use10BitVoltage
        {
            get { return default(bool); } // Does not matter. This is placeholder to enable binding
            set
            {
                if (value)
                {
                    _btn10BitVoltage.Checked = true;                    
                }
                else
                {
                    _btn8BitVoltage.Checked = true;
                }
            }
        }

        #endregion

        static void SetReadOnly(NumericUpDown control, bool readOnly)
        {
            control.ReadOnly = readOnly;
            control.Controls[0].Enabled = !readOnly; // UpDown buttons
            control.InterceptArrowKeys = !readOnly;
        }

        void OnNumberEntered(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}"); // Causes focus lost
            }
        }

        void OnReset(object sender, EventArgs e)
        {
            ViewModel.Reset();
        }

        void OnToggleStartStop(object sender, EventArgs e)
        {
            ViewModel.ToggleStartStop();
        }

        void OnResistanceValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var newValue = (double) _numResistance.Value;
            if (newValue == ViewModel.Resistance.Value)
            {
                return;
            }

            e.Cancel = true; // Cancel to skip default update mechanism

            var result = MessageBox.Show
                (
                    this,
                    Strings.Adjust_Values_For_Resistance, 
                    Text, 
                    MessageBoxButtons.YesNoCancel, 
                    MessageBoxIcon.Exclamation
                );
            if (result == DialogResult.Cancel)
            {
                return;
            }

            ViewModel.SetResistance(newValue, adjustValues: result == DialogResult.Yes);
        }

        void OnVoltageScaleValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var newValue = (double)_numVoltageScale.Value;
            if (newValue == ViewModel.VoltageScale)
            {
                return;
            }

            e.Cancel = true; // Cancel to skip default update mechanism

            var result = MessageBox.Show
                (
                    this,
                    Strings.Adjust_Values_For_VoltageScale,
                    Text,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation
                );
            if (result == DialogResult.Cancel)
            {
                return;
            }

            ViewModel.SetVoltageScale(newValue, adjustValues: result == DialogResult.Yes);
        }
    }
}