using FluentAssertions;
using NUnit.Framework;

namespace Voltmeter.Model
{
    [TestFixture]
    public class UnitConverterTests
    {
        [Test]
        public void Convert_temperature_between_Celsius_and_Farhengeit()
        {
            var celsius = new Unit("T", "Цельсий", "°C", 1.0);
            var fahrenheit = new Unit("T", "Фаренгейт", "°F", 1.8, 32);

            UnitConverter.Convert(36.6, from: celsius, to: fahrenheit)
                .Should().BeApproximately(97.88, 0.001);

            UnitConverter.Convert(97.88, from: fahrenheit, to: celsius)
                .Should().BeApproximately(36.6, 0.01);
        }

        [Test]
        public void IsConvertible_should_detect_incompatible_units()
        {
            UnitConverter.IsConvertible(from: Units.Current.Ampers, to: Units.Power.Watts)
                .Should().BeFalse();

            UnitConverter.IsConvertible(from: Units.Power.Watts, to: Units.Power.kiloWatts)
                .Should().BeTrue();
        }
        
        [Test]
        public void Convert_NaN_results_NaN()
        {
            var celsius = new Unit("T", "Цельсий", "°C", 1.0);
            var fahrenheit = new Unit("T", "Фаренгейт", "°F", 1.8, 32);

            UnitConverter.Convert(double.NaN, from: celsius, to: fahrenheit)
                .Should().Be(double.NaN);
        }

    }
}
