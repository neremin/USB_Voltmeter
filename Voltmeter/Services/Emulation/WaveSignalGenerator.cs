using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Voltmeter.Model;

namespace Voltmeter.Services.Emulation
{   
    public class WaveSignalGenerator
    {
        public readonly double Min;
        public readonly double Max;
        public readonly TimeSpan Period;

        public WaveSignalGenerator(double min, double max, TimeSpan period)
        {
            Contract.Requires<ArgumentOutOfRangeException>(0.0 <= min && min < max && max < DeviceConstants.MaxMeasuredVoltage);
            Contract.Requires<ArgumentOutOfRangeException>(TimeSpan.Zero < period);
            
            Min = min;
            Max = max;
            Period = period;
        }

        public double GetCurrentValue()
        {
            var time = TimeSpan.FromTicks(Stopwatch.GetTimestamp());
            return CalculateScaledPositiveWaveSignal(time, Period, Min, Max);
        }

        public static double CalculateScaledPositiveWaveSignal(TimeSpan time, TimeSpan period, double min, double max)
        {
            return min + (max - min) * CalculateNormalizedPositiveWaveSignal(time.Ticks, period.Ticks);
        }

        public static double CalculateNormalizedPositiveWaveSignal(double value, double period)
        {
            return 0.5 * (Math.Sin(Math.PI * 2.0 * value / period) + 1);
        }
    }
}