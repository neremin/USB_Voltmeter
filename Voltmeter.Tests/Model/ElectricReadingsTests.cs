using System;
using NUnit.Framework;
using FluentAssertions;
using Voltmeter.Tests.Helpers;

namespace Voltmeter.Model
{
    [TestFixture]
    public class ElectricReadingsTests
    {
        const double TOLERANCE = 0.000000000000001;

        static readonly DateTimeOffset T1 = DateTimeOffset.UtcNow;
        static readonly DateTimeOffset T2 = T1.AddSeconds(1);
        static readonly DateTimeOffset T3 = T2.AddSeconds(1);

        [Test]
        public void SetResistance_should_notify_property_change()
        {
            // Assign
            const double R = 4500, S = 2.1;

            var readings = new ElectricReadings(R, S);
            readings.MonitorEvents();

            // Act
            readings.SetResistance(R * 1.1, adjustValues: false);

            // Assert
            readings.ShouldRaisePropertyChangeFor(m => m.Resistance);
        }

        [Test]
        public void SetResistance_without_adjustment_should_updated_only_Resistance()
        {
            // Assign
            const double R = 1100, Rnew = 4500;

            var expectedReadings = new ElectricReadings(R);
            expectedReadings.AddSample(new DataSample(T1, 3.2));
            expectedReadings.AddSample(new DataSample(T2, 3.1));

            var testedReadings = new ElectricReadings(R);
            testedReadings.AddSample(new DataSample(T1, 3.2));
            testedReadings.AddSample(new DataSample(T2, 3.1));

            testedReadings.MonitorEvents();

            // Act
            testedReadings.SetResistance(Rnew, adjustValues: false);

            // Assert
            testedReadings.ShouldBeEquivalentTo(expectedReadings, options => options
                .Excluding(r => r.Resistance));

            testedReadings.Resistance
                .Should().Be(Rnew);
        }

        [Test]
        public void Initial_values_verification()
        {
            // Assign
            const double S = 2.2, R = 2000;

            // Act
            var readings = new ElectricReadings(R, S);

            // Assert
            readings.Voltage
                .Should().BeApproximately(0.0, TOLERANCE);
            readings.VoltageScale
                .Should().BeApproximately(S, TOLERANCE);
            readings.Resistance
                .Should().BeApproximately(R, TOLERANCE);
            readings.Current
                .Should().BeApproximately(0.0, TOLERANCE);
            readings.Power
                .Should().BeApproximately(0.0, TOLERANCE);
            readings.Energy
                .Should().BeApproximately(0.0, TOLERANCE);
            readings.TimeStamp
                .Should().Be(null);
        }

        [Test]
        public void Initial_values_and_Reset_cross_verification()
        {
            // Assign
            const double S = 2.2, R = 2000;

            var resetReadings = new ElectricReadings(R, S);
            resetReadings.AddSample(new DataSample(T1, 4.0));
            resetReadings.AddSample(new DataSample(T2, 3.0));

            // Act
            resetReadings.Reset();
            var initialReadings = new ElectricReadings(R, S);

            // Assert
            resetReadings.ShouldBeEquivalentTo(initialReadings);
        }

        [Test]
        public void Reset_should_notify_property_changes()
        {
            // Assign
            const double V = 3.2, S = 2.0, R = 1500;

            var readings = new ElectricReadings(R, S);
            readings.AddSample(new DataSample(T1, V + 0.4));
            readings.AddSample(new DataSample(T2, V + 0.5));

            readings.MonitorEvents();

            // Act
            readings.Reset();

            // Assert
            readings.ShouldNotRaisePropertyChangeFor(m => m.Resistance);
            readings.ShouldNotRaisePropertyChangeFor(m => m.VoltageScale);

            readings.ShouldRaisePropertyChangeFor(m => m.TimeStamp);
            readings.ShouldRaisePropertyChangeFor(m => m.Voltage);
            readings.ShouldRaisePropertyChangeFor(m => m.Current);
            readings.ShouldRaisePropertyChangeFor(m => m.Power);
            readings.ShouldRaisePropertyChangeFor(m => m.Energy);
        }

        [Test]
        public void SetResistance_readings_adjustment_cross_verification()
        {
            // Assign
            const double R1 = 1700, 
                         R2 = 1875;

            const double S = 2.3;

            const double V1 = 3.5,
                         V2 = 3.6;

            var expectedReadings = new ElectricReadings(R1, S);
            expectedReadings.AddSample(new DataSample(T1, V1));
            expectedReadings.AddSample(new DataSample(T2, V2));

            var actualReadings = new ElectricReadings(R2, S);
            actualReadings.AddSample(new DataSample(T1, V1));
            actualReadings.AddSample(new DataSample(T2, V2));

            // Act
            actualReadings.SetResistance(R1, adjustValues: true);

            // Assert
            actualReadings.ShouldBeEquivalentTo(expectedReadings, options => options
                .Using(new DoubleAssertionRule(TOLERANCE)));
        }

        [Test]
        public void SetVoltageScale_should_notify_property_change()
        {
            // Assign
            const double R = 4500, S = 2.1;

            var readings = new ElectricReadings(R, S);
            readings.MonitorEvents();

            // Act
            readings.SetVoltageScale(S * 1.1, adjustValues: false);

            // Assert
            readings.ShouldRaisePropertyChangeFor(m => m.VoltageScale);
        }

        [Test]
        public void SetVoltageScale_readings_adjustment_cross_verification()
        {
            // Assign
            const double R = 1730;

            const double S1 = 2.0,
                         S2 = 10.0;

            const double U1 = 220,
                         U2 = 224;

            var electricReadings = new ElectricReadings(R, S1);
            electricReadings.AddSample(new DataSample(T1, U1 / S1));
            electricReadings.AddSample(new DataSample(T2, U2 / S1));

            var actualReadings = new ElectricReadings(R, S2);
            actualReadings.AddSample(new DataSample(T1, U1 / S1));
            actualReadings.AddSample(new DataSample(T2, U2 / S1));

            // Act
            actualReadings.SetVoltageScale(S1, adjustValues: true);

            // Assert
            actualReadings.ShouldBeEquivalentTo(electricReadings, options => options
                .Using(new DoubleAssertionRule(TOLERANCE)));
        }

        [Test]
        public void SetVoltageScale_without_adjustment_should_update_only_VoltageScale()
        {
            // Assign
            const double R = 1730;
            
            const double S1 = 3.0, 
                         S2 = 10.0;

            const double V1 = 3.1, 
                         V2 = 3.2;

            var expectedReadings = new ElectricReadings(R, S1);
            expectedReadings.AddSample(new DataSample(T1, V1));
            expectedReadings.AddSample(new DataSample(T2, V2));

            var actualReadings = new ElectricReadings(R, S1);
            actualReadings.AddSample(new DataSample(T1, V1));
            actualReadings.AddSample(new DataSample(T2, V2));

            // Act
            actualReadings.SetVoltageScale(S2, adjustValues: false);

            // Assert
            actualReadings.ShouldBeEquivalentTo(expectedReadings, options => options
                .Excluding(r => r.VoltageScale));

            actualReadings.VoltageScale
                .Should().Be(S2);
        }

        [Test]
        public void AddSample_on_sequence_calculations_verification()
        {
            // Assign
            const double R = 1700, S = 2.3;

            const double V1 = 2.9,
                         V2 = 3.1,
                         V3 = 3.2;

            const double U1 = V1 * S,
                         U2 = V2 * S,
                         U3 = V3 * S;

            const double I1 = U1 / R,
                         I2 = U2 / R,
                         I3 = U3 / R;

            const double P1 = U1 * I1,
                         P2 = U2 * I2,
                         P3 = U3 * I3;

            double E1 = 0.0,
                   E2 = E1 + (P1 + P2) / 2 * (T2 - T1).TotalSeconds,
                   E3 = E2 + (P2 + P3) / 2 * (T3 - T2).TotalSeconds;

            // Act
            var readings = new ElectricReadings(R, S);
            readings.AddSample(new DataSample(T1, V1));
            readings.AddSample(new DataSample(T2, V2));
            readings.AddSample(new DataSample(T3, V3));

            // Assert
            readings.Resistance
                .Should().BeApproximately(R, TOLERANCE);
            readings.Voltage
                .Should().BeApproximately(U3, TOLERANCE);
            readings.Current
                .Should().BeApproximately(I3, TOLERANCE);
            readings.Power
                .Should().BeApproximately(P3, TOLERANCE);
            readings.Energy
                .Should().BeApproximately(E3, TOLERANCE);
            readings.TimeStamp
                .Should().Be(T3);
        }

        [Test]
        public void AddSample_handles_voltage_as_zero_when_it_is_less_or_equal_than_noise_threshold()
        {
            // Assign
            const double R = 1700, S = 2.3;

            const double noiseThreshold = 1.5;

            const double V1 = 1.1 * noiseThreshold,
                         V2 = 1.0 * noiseThreshold,
                         V3 = 0.9 * noiseThreshold;
            
            var expectedReadings = new ElectricReadings(R, S) { NoiseThreshold = 0.0 };
            expectedReadings.AddSample(new DataSample(T1, V1));
            expectedReadings.AddSample(new DataSample(T2, 0.0));
            expectedReadings.AddSample(new DataSample(T3, 0.0));

            // Act
            var testedReadings = new ElectricReadings(R, S) { NoiseThreshold = noiseThreshold };
            testedReadings.AddSample(new DataSample(T1, V1));
            testedReadings.AddSample(new DataSample(T2, V2));
            testedReadings.AddSample(new DataSample(T3, V3));

            // Assert
            testedReadings.ShouldBeEquivalentTo(expectedReadings, options => options
                .Excluding(r => r.NoiseThreshold));
        }
    }
}