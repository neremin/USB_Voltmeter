using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Voltmeter.Model;

namespace Voltmeter.Services
{
    [ContractClass(typeof(IDeviceManagerContract))]
    public interface IDeviceManager
    {
        /// <summary>
        /// Function to get all USB connected USB devices.
        /// </summary>
        /// <returns>All currently connected USB devices.</returns>
        IEnumerable<DeviceInformation> EnumerateAllDevices();

        /// <summary>
        /// Opens & initializes device stream.
        /// </summary>
        /// <param name="deviceInfo">Device information</param>
        /// <returns>Ready to use USB device stream</returns>
        IDeviceStream OpenDeviceStream(DeviceInformation deviceInfo);

        /// <summary>
        /// Notifies of device state changes (Add/Remove)
        /// </summary>
        event EventHandler<DeviceChangeArgs> DeviceChanged;
    }
    
    [ContractClassFor(typeof(IDeviceManager))]
    abstract class IDeviceManagerContract : IDeviceManager
    {
        public IEnumerable<DeviceInformation> EnumerateAllDevices()
        {
            Contract.Ensures(Contract.Result<IEnumerable<DeviceInformation>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<DeviceInformation>>()
                .All(info => !string.IsNullOrWhiteSpace(info.Path)
                  && info.Path == info.Path.ToLowerInvariant()));
            return null;
        }

        public IDeviceStream OpenDeviceStream(DeviceInformation deviceInfo)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(deviceInfo.Path));
            Contract.Ensures(Contract.Result<IDeviceStream>() != null);
            return null;
        }

        public event EventHandler<DeviceChangeArgs> DeviceChanged { add { } remove { } }
    }

    public enum DeviceChange
    {
        Added = 0,
        Removed = 1
    }

    public class DeviceChangeArgs : EventArgs
    {
        public readonly DeviceChange ChangeType;
        public readonly string DevicePath;

        public DeviceChangeArgs(DeviceChange changeType, string devicePath)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(devicePath));

            ChangeType = changeType;
            DevicePath = devicePath.ToLowerInvariant();
        }
    }
}