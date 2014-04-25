using PropertyChanged;
using System;
using System.Diagnostics.Contracts;

namespace Voltmeter.Model
{
    /// <summary>
    /// Represents Direct Current electrical readings.
    /// Performs all values calculations and updates.
    /// </summary>
    [ImplementPropertyChanged]
    public sealed class ElectricReadings
    {
        public ElectricReadings(double resistance = 1000.0, double voltageScale = 1.0)
        {
            Contract.Requires<ArgumentOutOfRangeException>(resistance > 0.0);
            Contract.Requires<ArgumentOutOfRangeException>(voltageScale > 0.0);
            Reset(resistance, voltageScale);
        }

        /// <summary>
        /// Gets Voltage (Volts) measured by device. Used for other values calculation.
        /// </summary>
        public double Voltage { get; private set; }

        /// <summary>
        /// Gets Voltage scale coefficient, used to adjust input data to the real value.
        /// </summary>
        public double VoltageScale { get; private set; }

        /// <summary>
        /// Gets/Sets Voltage measuring contour Resistance (Ohms). Used for other values calculation.
        /// </summary>
        public double Resistance { get; private set; }

        /// <summary>
        /// Gets Direct Current value (Ampers), calculated as Voltage / Resistance.
        /// </summary>
        public double Current { get; private set; }

        /// <summary>
        /// Gets Electrical Power (Watts), calculated as Voltage * Current.
        /// </summary>
        public double Power { get; private set; }

        /// <summary>
        /// Gets Energy (Watts * sec) measured over time. Calculated as (Power(n) + Power(n-1)) / 2 * (time(n) - time(n-1)).
        /// </summary>
        public double Energy { get; private set; }

        /// <summary>
        /// Gets/sets measured voltage threshold (Volts).
        /// </summary>
        public double NoiseThreshold { get; set; }

        /// <summary>
        /// Gets UTC timestamp of the last data sample.
        /// </summary>
        public DateTimeOffset? TimeStamp { get; private set; }

        /// <summary>
        /// Resets all values using except <see cref="VoltageScale" /> and <see cref="Resistance" />.
        /// </summary>
        public void Reset()
        {
            Contract.Ensures(TimeStamp == null);
            Contract.Ensures(Energy == 0.0);

            Reset(Resistance, VoltageScale);
        }

        /// <summary>
        /// Recalculates and updates readings according to the new sample.
        /// </summary>
        /// <param name="sample">Measured sample to process</param>
        public void AddSample(DataSample sample)
        {
            Contract.Requires<ArgumentOutOfRangeException>(TimeStamp == null || TimeStamp < sample.Timestamp);
            Contract.Ensures(TimeStamp == sample.Timestamp);
            
            TimeSpan interval = TimeStamp.HasValue
                ? sample.Timestamp - TimeStamp.Value
                : TimeSpan.Zero;

            TimeStamp = sample.Timestamp;
            var measuredVoltage = sample.Voltage <= NoiseThreshold ? 0.0 : sample.Voltage;
            CalculateAndUpdateReadings(measuredVoltage, Resistance, VoltageScale, interval);
        }

        /// <summary>
        /// Changes <see cref="Resistance"/> value and adjusts depending values on request.
        /// </summary>
        /// <param name="resistance">New resistance value (Ohms)</param>
        /// <param name="adjustValues">true to adjust depending values, otherwise false</param>
        public void SetResistance(double resistance, bool adjustValues = true)
        {
            Contract.Requires<ArgumentOutOfRangeException>(resistance > 0.0);
            Contract.Ensures(Resistance == resistance);

            if (adjustValues)
            {
                AdjustReadingsForResistance(Resistance, resistance);
            }
            Resistance = resistance;
        }

        /// <summary>
        /// Changes <see cref="VoltageScale"/> factor and adjusts depending values on request.
        /// </summary>
        /// <param name="voltageScale">New voltage scale factor</param>
        /// <param name="adjustValues">true to adjust depending values, otherwise false</param>
        public void SetVoltageScale(double voltageScale, bool adjustValues = true)
        {
            Contract.Requires<ArgumentOutOfRangeException>(voltageScale > 0.0);
            Contract.Ensures(VoltageScale == voltageScale);

            if (adjustValues)
            {
                AdjustReadingsForVoltageScale(VoltageScale, voltageScale);
            }
            VoltageScale = voltageScale;
        }

        void Reset(double resistance, double voltageScale)
        {
            TimeStamp = null;
            CalculateAndUpdateReadings(0.0, resistance, voltageScale, TimeSpan.Zero);
            Energy = 0.0;
        }

        void CalculateAndUpdateReadings(double measuredVoltage, double resistance, double voltageScale, TimeSpan interval)
        {
            Voltage = measuredVoltage * voltageScale;
            VoltageScale = voltageScale;
            Resistance = resistance;
            Current = Voltage / resistance;
            double newPower = Voltage * Current;
            Energy = Energy + (Power + newPower) * 0.5 * interval.TotalSeconds;
            Power = newPower;
        }

        void AdjustReadingsForResistance(double oldR, double newR)
        {
            if (oldR != newR)
            {
                double adjustmentFactor = oldR / newR;
                Current = Current * adjustmentFactor;
                Energy = Energy * adjustmentFactor;
                Power = Voltage * Current;
            }
        }

        void AdjustReadingsForVoltageScale(double oldS, double newS)
        {
            if (oldS != newS)
            {
                double adjustmentFactor = newS / oldS;
                Voltage = Voltage * adjustmentFactor;
                Current = Voltage / Resistance;
                Power = Voltage * Current;
                Energy = Energy * adjustmentFactor * adjustmentFactor;
            }
        }

        [ContractInvariantMethod]
        void Invariant()
        {
            Contract.Invariant(Resistance > 0.0);
            Contract.Invariant(VoltageScale > 0.0);
            Contract.Invariant(NoiseThreshold >= 0.0);
        }
    }
}