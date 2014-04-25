using System;
using System.Diagnostics.Contracts;

namespace Voltmeter.Model
{
    public struct DataSample
    {
        public readonly DateTimeOffset Timestamp;
        public readonly double Voltage;
        public readonly bool IsTimeBased;
        public readonly double Frequency;

        public DataSample(DateTimeOffset timestamp, double voltage, bool isTimeBased = false, double frequency = 0.0)
        {
            Timestamp = timestamp;
            Voltage = voltage;
            IsTimeBased = isTimeBased;
            Frequency = frequency;
        }
    }

    public sealed class DataSampleArgs : EventArgs
    {
        public readonly DataSample Sample;

        public DataSampleArgs(DataSample sample)
        {
            Sample = sample;
        }
    }

    public static class DataSampleConverter
    {
        public static DataSample FromBytes(byte[] bytes, DateTimeOffset timestamp, bool voltage10bit = false, bool polling1sec = false)
        {
            Contract.Requires<ArgumentNullException>(bytes != null);
            Contract.Requires<ArgumentException>(bytes.Length >= DeviceConstants.InputReportLength);
            double voltage = voltage10bit
                //? (bytes[1] * 4 + bytes[2] / 64) / 1023.0d * 5.0d // original conversion
                ? (bytes[1] << 2 | bytes[2] >> 6) / 1023.0 * DeviceConstants.MaxMeasuredVoltage
                : (bytes[1] / 255.0) * DeviceConstants.MaxMeasuredVoltage;
            bool isTimeBased = (bytes[3] == 0);
            double frequency = isTimeBased
                ? 0.0
                //: (bytes[4] + bytes[5] * 256 + bytes[6] * 65536) * // original conversion 
                : (bytes[4] | bytes[5] << 8 | bytes[6] << 16) *
                  (polling1sec
                      ? 1.04037d /* for 1 sec polling interval */
                      : 11.4441d /* for 100 msec pollig interval */);

            return new DataSample(timestamp, voltage, isTimeBased, frequency);
        }

        /// <devdoc>
        /// This is approximate reverse algo.
        /// For emulation purposes only.
        /// </devdoc>
        public static byte[] ToBytes(DataSample sample, bool voltage10bit = true)
        {
            var bytes = new byte[DeviceConstants.InputReportLength];
            if (voltage10bit)
            {
                var value = (ushort)(sample.Voltage * 1023 / DeviceConstants.MaxMeasuredVoltage);
                bytes[1] = (byte)(value >> 2);
                bytes[2] = (byte)(value * 64);
            }
            else
            {
                bytes[1] = (byte)(sample.Voltage / DeviceConstants.MaxMeasuredVoltage * 255.0);
            }
            bytes[3] = (byte)(sample.IsTimeBased ? 0 : 1);
            // Skip frequency. Never used
            return bytes;
        }
    }
}