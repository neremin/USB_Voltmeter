using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Voltmeter.Common
{
    [TestFixture]
    public class TimeStatsTests
    {
        [Test]
        public void AddSample_should_correctly_update_min_max_avg_for_sample_sequence()
        {
            // Assign
            TimeSpan[] samples =
            {
                TimeSpan.FromMilliseconds(310),
                TimeSpan.FromMilliseconds(300),
                TimeSpan.FromMilliseconds(350),
                TimeSpan.FromMilliseconds(330),
            };

            TimeSpan
                min = TimeSpan.FromTicks((long)samples.Min(t => t.Ticks)),
                max = TimeSpan.FromTicks((long)samples.Max(t => t.Ticks)),
                avg = TimeSpan.FromTicks((long)samples.Average(t => t.Ticks));

            // Act
            var stats = CreateStats(samples);

            // Assert
            stats.SamplesCount
                .Should().Be(samples.Length);
            stats.Min
                .Should().Be(min);
            stats.Max
                .Should().Be(max);
            stats.Avg
                .Should().Be(avg);
        }

        [Test]
        public void Combine_calculations_cross_verification()
        {
            // Assign
            TimeSpan[] samples1 =
            {
                TimeSpan.FromMilliseconds(310),
                TimeSpan.FromMilliseconds(300),
                TimeSpan.FromMilliseconds(350),
                TimeSpan.FromMilliseconds(330),
            };

            TimeSpan[] samples2 =
            {
                TimeSpan.FromMilliseconds(315),
                TimeSpan.FromMilliseconds(290),
                TimeSpan.FromMilliseconds(325),
            };

            var emptyStats = new TimeStats();
            var stats1 = CreateStats(samples1);
            var stats2 = CreateStats(samples2);
            var common = CreateStats(samples1.Concat(samples2).ToArray());

            // Act
            var combined = emptyStats + stats1 + stats2 + emptyStats;

            // Assert
            combined.ShouldBeEquivalentTo(common);
        }

        static TimeStats CreateStats(params TimeSpan[] samples)
        {
            var stats = new TimeStats();
            foreach (var sample in samples)
            {
                stats.AddSample(sample);
            }
            return stats;
        }
    }
}
