using System.Security.AccessControl;

namespace Voltmeter.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._lblResistanceCaption = new System.Windows.Forms.Label();
            this._numResistance = new System.Windows.Forms.NumericUpDown();
            this._cmbResistanceUnits = new System.Windows.Forms.ComboBox();
            this._cmbVoltageUnits = new System.Windows.Forms.ComboBox();
            this._lblVoltageCaption = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._txtVoltage = new Voltmeter.Controls.NumericBox();
            this._txtCurrent = new Voltmeter.Controls.NumericBox();
            this._lblCurrentCaption = new System.Windows.Forms.Label();
            this._lblPowerCaption = new System.Windows.Forms.Label();
            this._txtPower = new Voltmeter.Controls.NumericBox();
            this._cmbCurrentUnits = new System.Windows.Forms.ComboBox();
            this._lblEnergyCaption = new System.Windows.Forms.Label();
            this._txtEnergy = new Voltmeter.Controls.NumericBox();
            this._cmbPowerUnits = new System.Windows.Forms.ComboBox();
            this._cmbEnergyUnits = new System.Windows.Forms.ComboBox();
            this._lblLastUpdate = new System.Windows.Forms.Label();
            this._lblSessionDuration = new System.Windows.Forms.Label();
            this._lblSessionDurationCaption = new System.Windows.Forms.Label();
            this._lblLastUpdateCaption = new System.Windows.Forms.Label();
            this._numVoltageScale = new System.Windows.Forms.NumericUpDown();
            this._lblVoltageScaleCaption = new System.Windows.Forms.Label();
            this._lblVoltageNoiseThresholdCaption = new System.Windows.Forms.Label();
            this._numVoltageNoiseThreshold = new System.Windows.Forms.NumericUpDown();
            this._cmdVoltageNoiseUnits = new System.Windows.Forms.ComboBox();
            this._lblPrecisionCaption = new System.Windows.Forms.Label();
            this._numPrecision = new System.Windows.Forms.NumericUpDown();
            this._lblVoltageResolutionCaption = new System.Windows.Forms.Label();
            this._lblPollingIntervalCaption = new System.Windows.Forms.Label();
            this._numPollingInterval = new System.Windows.Forms.NumericUpDown();
            this._lblDeviceStatus = new System.Windows.Forms.Label();
            this._panelResolution = new System.Windows.Forms.Panel();
            this._btn10BitVoltage = new System.Windows.Forms.RadioButton();
            this._btn8BitVoltage = new System.Windows.Forms.RadioButton();
            this._btnReset = new System.Windows.Forms.Button();
            this._btnStartStop = new System.Windows.Forms.Button();
            this._lblDeviceCaption = new System.Windows.Forms.Label();
            this._cmbAvailableDevices = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this._numResistance)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numVoltageScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._numVoltageNoiseThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._numPrecision)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._numPollingInterval)).BeginInit();
            this._panelResolution.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lblResistanceCaption
            // 
            resources.ApplyResources(this._lblResistanceCaption, "_lblResistanceCaption");
            this._lblResistanceCaption.Name = "_lblResistanceCaption";
            // 
            // _numResistance
            // 
            resources.ApplyResources(this._numResistance, "_numResistance");
            this._numResistance.Maximum = new decimal(new int[] {
            1874919423,
            2328306,
            0,
            0});
            this._numResistance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            983040});
            this._numResistance.Name = "_numResistance";
            this._numResistance.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this._numResistance.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnNumberEntered);
            this._numResistance.Validating += new System.ComponentModel.CancelEventHandler(this.OnResistanceValidating);
            // 
            // _cmbResistanceUnits
            // 
            resources.ApplyResources(this._cmbResistanceUnits, "_cmbResistanceUnits");
            this._cmbResistanceUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbResistanceUnits.Name = "_cmbResistanceUnits";
            // 
            // _cmbVoltageUnits
            // 
            resources.ApplyResources(this._cmbVoltageUnits, "_cmbVoltageUnits");
            this._cmbVoltageUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbVoltageUnits.Name = "_cmbVoltageUnits";
            // 
            // _lblVoltageCaption
            // 
            resources.ApplyResources(this._lblVoltageCaption, "_lblVoltageCaption");
            this._lblVoltageCaption.Name = "_lblVoltageCaption";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this._lblVoltageCaption, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._txtVoltage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._txtCurrent, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._lblCurrentCaption, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._cmbVoltageUnits, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this._lblPowerCaption, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this._txtPower, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this._cmbCurrentUnits, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this._lblEnergyCaption, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this._txtEnergy, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this._cmbPowerUnits, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this._lblResistanceCaption, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this._numResistance, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this._cmbEnergyUnits, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this._cmbResistanceUnits, 2, 10);
            this.tableLayoutPanel1.Controls.Add(this._lblLastUpdate, 0, 16);
            this.tableLayoutPanel1.Controls.Add(this._lblSessionDuration, 2, 16);
            this.tableLayoutPanel1.Controls.Add(this._lblSessionDurationCaption, 2, 15);
            this.tableLayoutPanel1.Controls.Add(this._lblLastUpdateCaption, 0, 15);
            this.tableLayoutPanel1.Controls.Add(this._numVoltageScale, 0, 14);
            this.tableLayoutPanel1.Controls.Add(this._lblVoltageScaleCaption, 0, 13);
            this.tableLayoutPanel1.Controls.Add(this._lblVoltageNoiseThresholdCaption, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this._numVoltageNoiseThreshold, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this._cmdVoltageNoiseUnits, 2, 12);
            this.tableLayoutPanel1.Controls.Add(this._lblPrecisionCaption, 4, 11);
            this.tableLayoutPanel1.Controls.Add(this._numPrecision, 4, 12);
            this.tableLayoutPanel1.Controls.Add(this._lblVoltageResolutionCaption, 4, 7);
            this.tableLayoutPanel1.Controls.Add(this._lblPollingIntervalCaption, 4, 9);
            this.tableLayoutPanel1.Controls.Add(this._numPollingInterval, 4, 10);
            this.tableLayoutPanel1.Controls.Add(this._lblDeviceStatus, 4, 14);
            this.tableLayoutPanel1.Controls.Add(this._panelResolution, 4, 8);
            this.tableLayoutPanel1.Controls.Add(this._btnReset, 4, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // _txtVoltage
            // 
            this._txtVoltage.DecimalPlaces = ((byte)(0));
            resources.ApplyResources(this._txtVoltage, "_txtVoltage");
            this._txtVoltage.ForeColor = System.Drawing.SystemColors.WindowText;
            this._txtVoltage.Name = "_txtVoltage";
            this._txtVoltage.ReadOnly = true;
            this._txtVoltage.TabStop = false;
            // 
            // _txtCurrent
            // 
            this._txtCurrent.DecimalPlaces = ((byte)(0));
            resources.ApplyResources(this._txtCurrent, "_txtCurrent");
            this._txtCurrent.Name = "_txtCurrent";
            this._txtCurrent.ReadOnly = true;
            this._txtCurrent.TabStop = false;
            // 
            // _lblCurrentCaption
            // 
            resources.ApplyResources(this._lblCurrentCaption, "_lblCurrentCaption");
            this._lblCurrentCaption.Name = "_lblCurrentCaption";
            // 
            // _lblPowerCaption
            // 
            resources.ApplyResources(this._lblPowerCaption, "_lblPowerCaption");
            this._lblPowerCaption.Name = "_lblPowerCaption";
            // 
            // _txtPower
            // 
            this._txtPower.DecimalPlaces = ((byte)(0));
            resources.ApplyResources(this._txtPower, "_txtPower");
            this._txtPower.Name = "_txtPower";
            this._txtPower.ReadOnly = true;
            this._txtPower.TabStop = false;
            // 
            // _cmbCurrentUnits
            // 
            resources.ApplyResources(this._cmbCurrentUnits, "_cmbCurrentUnits");
            this._cmbCurrentUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbCurrentUnits.Name = "_cmbCurrentUnits";
            // 
            // _lblEnergyCaption
            // 
            resources.ApplyResources(this._lblEnergyCaption, "_lblEnergyCaption");
            this._lblEnergyCaption.Name = "_lblEnergyCaption";
            // 
            // _txtEnergy
            // 
            this._txtEnergy.DecimalPlaces = ((byte)(0));
            resources.ApplyResources(this._txtEnergy, "_txtEnergy");
            this._txtEnergy.Name = "_txtEnergy";
            this._txtEnergy.ReadOnly = true;
            this._txtEnergy.TabStop = false;
            // 
            // _cmbPowerUnits
            // 
            resources.ApplyResources(this._cmbPowerUnits, "_cmbPowerUnits");
            this._cmbPowerUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbPowerUnits.Name = "_cmbPowerUnits";
            // 
            // _cmbEnergyUnits
            // 
            resources.ApplyResources(this._cmbEnergyUnits, "_cmbEnergyUnits");
            this._cmbEnergyUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbEnergyUnits.Name = "_cmbEnergyUnits";
            // 
            // _lblLastUpdate
            // 
            resources.ApplyResources(this._lblLastUpdate, "_lblLastUpdate");
            this._lblLastUpdate.Name = "_lblLastUpdate";
            // 
            // _lblSessionDuration
            // 
            resources.ApplyResources(this._lblSessionDuration, "_lblSessionDuration");
            this._lblSessionDuration.Name = "_lblSessionDuration";
            // 
            // _lblSessionDurationCaption
            // 
            resources.ApplyResources(this._lblSessionDurationCaption, "_lblSessionDurationCaption");
            this._lblSessionDurationCaption.Name = "_lblSessionDurationCaption";
            // 
            // _lblLastUpdateCaption
            // 
            resources.ApplyResources(this._lblLastUpdateCaption, "_lblLastUpdateCaption");
            this._lblLastUpdateCaption.Name = "_lblLastUpdateCaption";
            // 
            // _numVoltageScale
            // 
            resources.ApplyResources(this._numVoltageScale, "_numVoltageScale");
            this._numVoltageScale.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this._numVoltageScale.Maximum = new decimal(new int[] {
            1874919423,
            2328306,
            0,
            0});
            this._numVoltageScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            983040});
            this._numVoltageScale.Name = "_numVoltageScale";
            this._numVoltageScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._numVoltageScale.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnNumberEntered);
            this._numVoltageScale.Validating += new System.ComponentModel.CancelEventHandler(this.OnVoltageScaleValidating);
            // 
            // _lblVoltageScaleCaption
            // 
            resources.ApplyResources(this._lblVoltageScaleCaption, "_lblVoltageScaleCaption");
            this._lblVoltageScaleCaption.Name = "_lblVoltageScaleCaption";
            // 
            // _lblVoltageNoiseThresholdCaption
            // 
            resources.ApplyResources(this._lblVoltageNoiseThresholdCaption, "_lblVoltageNoiseThresholdCaption");
            this._lblVoltageNoiseThresholdCaption.Name = "_lblVoltageNoiseThresholdCaption";
            // 
            // _numVoltageNoiseThreshold
            // 
            resources.ApplyResources(this._numVoltageNoiseThreshold, "_numVoltageNoiseThreshold");
            this._numVoltageNoiseThreshold.Maximum = new decimal(new int[] {
            1874919423,
            2328306,
            0,
            0});
            this._numVoltageNoiseThreshold.Name = "_numVoltageNoiseThreshold";
            // 
            // _cmdVoltageNoiseUnits
            // 
            resources.ApplyResources(this._cmdVoltageNoiseUnits, "_cmdVoltageNoiseUnits");
            this._cmdVoltageNoiseUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmdVoltageNoiseUnits.Name = "_cmdVoltageNoiseUnits";
            // 
            // _lblPrecisionCaption
            // 
            resources.ApplyResources(this._lblPrecisionCaption, "_lblPrecisionCaption");
            this._lblPrecisionCaption.Name = "_lblPrecisionCaption";
            // 
            // _numPrecision
            // 
            resources.ApplyResources(this._numPrecision, "_numPrecision");
            this._numPrecision.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this._numPrecision.Name = "_numPrecision";
            this._numPrecision.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this._numPrecision.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnNumberEntered);
            // 
            // _lblVoltageResolutionCaption
            // 
            resources.ApplyResources(this._lblVoltageResolutionCaption, "_lblVoltageResolutionCaption");
            this._lblVoltageResolutionCaption.Name = "_lblVoltageResolutionCaption";
            // 
            // _lblPollingIntervalCaption
            // 
            resources.ApplyResources(this._lblPollingIntervalCaption, "_lblPollingIntervalCaption");
            this._lblPollingIntervalCaption.Name = "_lblPollingIntervalCaption";
            // 
            // _numPollingInterval
            // 
            this._numPollingInterval.DecimalPlaces = 3;
            resources.ApplyResources(this._numPollingInterval, "_numPollingInterval");
            this._numPollingInterval.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this._numPollingInterval.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this._numPollingInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this._numPollingInterval.Name = "_numPollingInterval";
            this._numPollingInterval.Value = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this._numPollingInterval.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnNumberEntered);
            // 
            // _lblDeviceStatus
            // 
            resources.ApplyResources(this._lblDeviceStatus, "_lblDeviceStatus");
            this._lblDeviceStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this._lblDeviceStatus.Name = "_lblDeviceStatus";
            this.tableLayoutPanel1.SetRowSpan(this._lblDeviceStatus, 3);
            // 
            // _panelResolution
            // 
            this._panelResolution.Controls.Add(this._btn10BitVoltage);
            this._panelResolution.Controls.Add(this._btn8BitVoltage);
            resources.ApplyResources(this._panelResolution, "_panelResolution");
            this._panelResolution.Name = "_panelResolution";
            // 
            // _btn10BitVoltage
            // 
            resources.ApplyResources(this._btn10BitVoltage, "_btn10BitVoltage");
            this._btn10BitVoltage.Name = "_btn10BitVoltage";
            this._btn10BitVoltage.UseVisualStyleBackColor = true;
            // 
            // _btn8BitVoltage
            // 
            resources.ApplyResources(this._btn8BitVoltage, "_btn8BitVoltage");
            this._btn8BitVoltage.Name = "_btn8BitVoltage";
            this._btn8BitVoltage.UseVisualStyleBackColor = true;
            // 
            // _btnReset
            // 
            resources.ApplyResources(this._btnReset, "_btnReset");
            this._btnReset.Name = "_btnReset";
            this._btnReset.TabStop = false;
            this._btnReset.UseVisualStyleBackColor = true;
            this._btnReset.Click += new System.EventHandler(this.OnReset);
            // 
            // _btnStartStop
            // 
            resources.ApplyResources(this._btnStartStop, "_btnStartStop");
            this._btnStartStop.Name = "_btnStartStop";
            this._btnStartStop.TabStop = false;
            this._btnStartStop.UseVisualStyleBackColor = true;
            this._btnStartStop.Click += new System.EventHandler(this.OnToggleStartStop);
            // 
            // _lblDeviceCaption
            // 
            resources.ApplyResources(this._lblDeviceCaption, "_lblDeviceCaption");
            this._lblDeviceCaption.Name = "_lblDeviceCaption";
            // 
            // _cmbAvailableDevices
            // 
            this._cmbAvailableDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this._cmbAvailableDevices, "_cmbAvailableDevices");
            this._cmbAvailableDevices.FormattingEnabled = true;
            this._cmbAvailableDevices.Name = "_cmbAvailableDevices";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._btnStartStop);
            this.Controls.Add(this._cmbAvailableDevices);
            this.Controls.Add(this._lblDeviceCaption);
            this.Controls.Add(this.tableLayoutPanel1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            ((System.ComponentModel.ISupportInitialize)(this._numResistance)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numVoltageScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._numVoltageNoiseThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._numPrecision)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._numPollingInterval)).EndInit();
            this._panelResolution.ResumeLayout(false);
            this._panelResolution.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lblResistanceCaption;
        private System.Windows.Forms.NumericUpDown _numResistance;
        private System.Windows.Forms.ComboBox _cmbResistanceUnits;
        private System.Windows.Forms.ComboBox _cmbVoltageUnits;
        private System.Windows.Forms.Label _lblVoltageCaption;
        private Voltmeter.Controls.NumericBox _txtVoltage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label _lblCurrentCaption;
        private Voltmeter.Controls.NumericBox _txtCurrent;
        private System.Windows.Forms.ComboBox _cmbCurrentUnits;
        private System.Windows.Forms.Label _lblPowerCaption;
        private Voltmeter.Controls.NumericBox _txtPower;
        private System.Windows.Forms.ComboBox _cmbPowerUnits;
        private System.Windows.Forms.Label _lblEnergyCaption;
        private Voltmeter.Controls.NumericBox _txtEnergy;
        private System.Windows.Forms.ComboBox _cmbEnergyUnits;
        private System.Windows.Forms.Label _lblLastUpdateCaption;
        private System.Windows.Forms.Label _lblLastUpdate;
        private System.Windows.Forms.Button _btnStartStop;
        private System.Windows.Forms.Button _btnReset;
        private System.Windows.Forms.Label _lblPrecisionCaption;
        private System.Windows.Forms.NumericUpDown _numPrecision;
        private System.Windows.Forms.Label _lblDeviceStatus;
        private System.Windows.Forms.Label _lblVoltageScaleCaption;
        private System.Windows.Forms.NumericUpDown _numVoltageScale;
        private System.Windows.Forms.Label _lblPollingIntervalCaption;
        private System.Windows.Forms.NumericUpDown _numPollingInterval;
        private System.Windows.Forms.Label _lblVoltageResolutionCaption;
        private System.Windows.Forms.Panel _panelResolution;
        private System.Windows.Forms.RadioButton _btn10BitVoltage;
        private System.Windows.Forms.RadioButton _btn8BitVoltage;
        private System.Windows.Forms.Label _lblSessionDurationCaption;
        private System.Windows.Forms.Label _lblSessionDuration;
        private System.Windows.Forms.Label _lblVoltageNoiseThresholdCaption;
        private System.Windows.Forms.NumericUpDown _numVoltageNoiseThreshold;
        private System.Windows.Forms.ComboBox _cmdVoltageNoiseUnits;
        private System.Windows.Forms.Label _lblDeviceCaption;
        private System.Windows.Forms.ComboBox _cmbAvailableDevices;


    }
}

