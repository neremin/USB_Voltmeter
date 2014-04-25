using System;
using FluentAssertions;
using NUnit.Framework;

namespace Voltmeter.Model
{
    [TestFixture]
    public class DataSampleConverterTests
    {
        const double TOLERANCE = 0.015d;

        [Test]
        public void FromBytes_values_conversion_for_10_or_8bit( [Values(true, false)] bool voltage10bit )
        {
            DataSampleConverter.FromBytes(new byte[7] { 0, 133, 128, 1, 0, 0, 0 }, DateTimeOffset.UtcNow, voltage10bit)
                .Voltage
                .Should().BeApproximately(2.609d, TOLERANCE);
        }

        [Test]
        public void ToBytes_FromBytes_roundtrip_conversion_for_10_or_8bit([Values(true, false)] bool voltage10bit )
        {
            var inputSample = new DataSample(DateTimeOffset.UtcNow, 4.679d);

            var bytes = DataSampleConverter.ToBytes(inputSample, voltage10bit);
            var resultSample = DataSampleConverter.FromBytes(bytes, DateTimeOffset.UtcNow, voltage10bit);
            
            resultSample.Voltage
                .Should().BeApproximately(inputSample.Voltage, TOLERANCE);
        }
    }
}
