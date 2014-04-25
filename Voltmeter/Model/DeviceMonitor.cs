using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using Voltmeter.Common;
using Voltmeter.Resources;
using Voltmeter.Services;

namespace Voltmeter.Model
{
    public class DeviceMonitor
    {
        readonly IDeviceManager DeviceManager;
        readonly string SearchPattern;
        
        public readonly BindingList<DeviceInformation> AvailableDevices;
        
        public DeviceMonitor(IDeviceManager deviceManager, string searchPattern)
        {
            Contract.Requires<ArgumentNullException>(deviceManager != null);
            Contract.Requires<ArgumentNullException>(searchPattern != null);
            DeviceManager = deviceManager;
            DeviceManager.DeviceChanged += OnAvailabilityDeviceChanged;
            SearchPattern = searchPattern;

            var availableDevices =
                DeviceManager
                    .EnumerateAllDevices()
                    .Where(d => d.Path.Contains(SearchPattern))
                    .OrderBy(d => d.Path);

            AvailableDevices = new BindingList<DeviceInformation>(availableDevices.ToList());
        }

        void OnAvailabilityDeviceChanged(object sender, DeviceChangeArgs e)
        {
            if (!e.DevicePath.Contains(SearchPattern))
            {
                return;
            }
            
            DeviceInformation deviceInfo;
            switch (e.ChangeType)
            {
                case DeviceChange.Added:
                    deviceInfo = DeviceManager.EnumerateAllDevices().First(d => d.Path == e.DevicePath);
                    Contract.Assert(!AvailableDevices.Contains(deviceInfo));
                    InsertDeviceInfo(deviceInfo);
                    Log.InfoFormat(Strings.USB_Device_Plugged_0, deviceInfo.Description);
                    break;
                case DeviceChange.Removed:
                    deviceInfo = AvailableDevices.First(d => d.Path == e.DevicePath);
                    Contract.Assert(AvailableDevices.Contains(deviceInfo));
                    AvailableDevices.Remove(deviceInfo);
                    Log.InfoFormat(Strings.USB_Device_Unplugged_0, deviceInfo.Description);
                    break;
            }
        }

        void InsertDeviceInfo(DeviceInformation deviceInfo)
        {
            int insertIndex = AvailableDevices.Count;
            for (int i = 0; i < AvailableDevices.Count; ++i)
            {
                if (string.Compare(AvailableDevices[i].Path, deviceInfo.Path, StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    insertIndex = i;
                    break;
                }
            }
            AvailableDevices.Insert(insertIndex, deviceInfo);
        }

        [ContractInvariantMethod]
        void Invariant()
        {
            Contract.Invariant(AvailableDevices != null);
            Contract.Invariant(AvailableDevices.All(d => d.Path.Contains(SearchPattern)));
            Contract.Invariant(AvailableDevices.SequenceEqual(AvailableDevices.OrderBy(d => d.Path)));
        }
    }
}