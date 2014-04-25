using System;
using FluentAssertions;
using NUnit.Framework;

namespace Voltmeter.Services.Emulation
{
    [TestFixture]
    public class WaveSignalGeneratorTests
    {
        const double TOLERANCE = 0.0000000001d;

        [Test]
        public void CalculateScaledPositiveWaveSignal_periodic_results_check()
        {
            TimeSpan
                period               = TimeSpan.FromSeconds(2),
                minSignalTime        = TimeSpan.FromSeconds(0.75 * period.TotalSeconds),
                maxSignalTime        = TimeSpan.FromSeconds(0.25 * period.TotalSeconds),
                halfSignalTime       = TimeSpan.Zero;

            const double
                min = 1.4,
                max = 6.5,
                half = (max + min) * 0.5;

            WaveSignalGenerator.CalculateScaledPositiveWaveSignal(minSignalTime, period, min, max)
                .Should().BeApproximately(min, TOLERANCE);

            WaveSignalGenerator.CalculateScaledPositiveWaveSignal(maxSignalTime, period, min, max)
                .Should().BeApproximately(max, TOLERANCE);

            WaveSignalGenerator.CalculateScaledPositiveWaveSignal(halfSignalTime, period, min, max)
                .Should().BeApproximately(half, TOLERANCE);
        }

        [Test]
        public void CalculateNormalizedPositiveWaveSignal_periodic_result_check()
        {
            const double period = 4, min = 0, max = 1, half = 0.5;
            
            WaveSignalGenerator.CalculateNormalizedPositiveWaveSignal(0, period)
                .Should().BeApproximately(half, TOLERANCE);

            WaveSignalGenerator.CalculateNormalizedPositiveWaveSignal(period * 0.25, period)
                .Should().BeApproximately(max, TOLERANCE);

            WaveSignalGenerator.CalculateNormalizedPositiveWaveSignal(period * 0.5, period)
                .Should().BeApproximately(half, TOLERANCE);

            WaveSignalGenerator.CalculateNormalizedPositiveWaveSignal(period * 0.75, period)
                .Should().BeApproximately(min, TOLERANCE);

            WaveSignalGenerator.CalculateNormalizedPositiveWaveSignal(period, period)
                .Should().BeApproximately(half, TOLERANCE);

            WaveSignalGenerator.CalculateNormalizedPositiveWaveSignal(period * 10, period)
                .Should().BeApproximately(half, TOLERANCE);
        }
    }
}
