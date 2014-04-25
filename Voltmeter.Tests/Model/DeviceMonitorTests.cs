using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Voltmeter.Services;

namespace Voltmeter.Model
{
    [TestFixture]
    public class DeviceMonitorTests
    {
        [Test]
        public void AvailableDevices_should_contain_only_items_matching_search_pattern_and_sorted_by_path()
        {
            // Assign
            const string matchPattern = "item";
            var devices = new List<DeviceInformation>
            {
                new DeviceInformation(matchPattern + 2),
                new DeviceInformation(matchPattern + 3),
                new DeviceInformation("odd_1"), // Should be skipped
                new DeviceInformation(matchPattern + 1),
            };

            var deviceManager = Substitute.For<IDeviceManager>();
            deviceManager.EnumerateAllDevices().Returns(devices);
            
            // Act
            var monitor = new DeviceMonitor(deviceManager, matchPattern);

            // Assert
            monitor.AvailableDevices
                .Should().OnlyContain(d => d.Path.Contains(matchPattern));
            monitor.AvailableDevices
                .Select(d => d.Path).Should().BeInAscendingOrder();
        }

        [Test]
        public void AvailableDevices_should_remain_sorted_by_path_when_new_device_becomes_available
            (
                [Values("1", "3", "5")] string newDevicePath
            )
        {
            // Assign
            var devices = new List<DeviceInformation>
            {
                new DeviceInformation("2"),
                new DeviceInformation("4"),
            };

            var deviceManager = Substitute.For<IDeviceManager>();
            deviceManager.EnumerateAllDevices().Returns(devices);

            var monitor = new DeviceMonitor(deviceManager, string.Empty);

            var newDevice = new DeviceInformation(newDevicePath);
            devices.Add(newDevice);

            // Act
            deviceManager.DeviceChanged += Raise.EventWith(new DeviceChangeArgs(DeviceChange.Added, newDevice.Path));

            // Assert
            monitor.AvailableDevices
                .Should().Contain(devices, newDevice);
            monitor.AvailableDevices
                .Select(d => d.Path).Should().BeInAscendingOrder();
        }
    }
}