using System;
using System.Collections.Generic;
using Voltmeter.Model;
using Voltmeter.Resources;

namespace Voltmeter.Services.Emulation
{
    public class PseudoDeviceManager : IDeviceManager
    {
        readonly IDeviceStream EmulatorStream;
        readonly DeviceInformation EmulatorDeviceInfo;

#pragma warning disable 067
        public event EventHandler<DeviceChangeArgs> DeviceChanged;
#pragma warning restore 067

        public PseudoDeviceManager() : this
            (
                DeviceConstants.MaxMeasuredVoltage * 0.05,
                DeviceConstants.MaxMeasuredVoltage * 0.95,
                TimeSpan.FromSeconds(DeviceConstants.MaxPollingInterval.TotalSeconds * 0.8)
            )
        {            
        }

        public PseudoDeviceManager(double min, double max, TimeSpan period)
        {
            EmulatorStream = new PseudoDeviceStream(min, max, period);
            EmulatorDeviceInfo = new DeviceInformation(DeviceConstants.SearchPattern, Strings.Emulated_Device_Description);
        }

        public IEnumerable<DeviceInformation> EnumerateAllDevices()
        {
            yield return EmulatorDeviceInfo;
        }

        public IDeviceStream OpenDeviceStream(DeviceInformation deviceInfo)
        {
            return EmulatorStream;
        }
    }
}