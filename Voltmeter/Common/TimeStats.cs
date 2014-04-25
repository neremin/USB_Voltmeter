using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using Voltmeter.Resources;

namespace Voltmeter.Common
{
    [DebuggerDisplay("Avg = {Avg}, Max = {Max}, Min = {Min}, Samples = {Samples}")]
    public sealed class TimeStats
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        
        public TimeSpan Max { get; private set; }
        public TimeSpan Min { get; private set; }
        public TimeSpan Avg { get; private set; }
        public long SamplesCount { get; private set;}

        public void BeginSample()
        {
            _stopwatch.Restart();
        }
        
        public TimeSpan EndSample()
        {
            var sample = _stopwatch.Elapsed;
            AddSample(sample);
            return sample;
        }

        public void Combine(TimeStats that)
        {
            if (that.SamplesCount == 0)
            {
                return;
            }
            if (this.SamplesCount == 0)
            {
                this.Max = that.Max;
                this.Min = that.Min;
                this.Avg = that.Avg;
                this.SamplesCount = that.SamplesCount;
            }
            else
            {
                var totalSamplesCount = this.SamplesCount + that.SamplesCount;
                this.Max = TimeSpan.FromTicks(Math.Max(this.Max.Ticks, that.Max.Ticks));
                this.Min = TimeSpan.FromTicks(Math.Min(this.Min.Ticks, that.Min.Ticks));
                this.Avg = TimeSpan.FromTicks((long)
                       (this.Avg.Ticks * ((double)this.SamplesCount / totalSamplesCount)
                      + that.Avg.Ticks * ((double)that.SamplesCount / totalSamplesCount))
                    );
                this.SamplesCount = totalSamplesCount;
            }
        }

        public static TimeStats operator+(TimeStats stats1, TimeStats stats2)
        {
            var combinedStats = new TimeStats();
            combinedStats.Combine(stats1);
            combinedStats.Combine(stats2);
            return combinedStats;
        }

        public void AddSample(TimeSpan sample)
        {
            var samplesCount = ++SamplesCount;
            if (samplesCount == 1)
            {
                Max = sample;
                Min = sample;
                Avg = sample;
            }
            else
            {
                Max = TimeSpan.FromTicks(Math.Max(Max.Ticks, sample.Ticks));
                Min = TimeSpan.FromTicks(Math.Min(Min.Ticks, sample.Ticks));
                Avg = TimeSpan.FromTicks((Avg.Ticks * (samplesCount - 1) + sample.Ticks) / samplesCount);                
            }
        }

        public string ToString(string separator = ", ", string format = "c")
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(separator));
            var sb = new StringBuilder();

            sb.AppendFormat(Strings.TimeStats_Avg_0, Avg.ToString(format));
            sb.Append(separator);

            sb.AppendFormat(Strings.TimeStats_Max_0, Max.ToString(format));
            sb.Append(separator);

            sb.AppendFormat(Strings.TimeStats_Min_0, Min.ToString(format));
            sb.Append(separator);
            
            sb.AppendFormat(Strings.TimeStats_Samples_0, SamplesCount);

            return sb.ToString();
        }
    }
}