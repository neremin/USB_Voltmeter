using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;
using Voltmeter.Common;
using System.IO;
using Microsoft.Win32.SafeHandles;
using Voltmeter.Model;
using Voltmeter.Resources;

namespace Voltmeter.Services
{
    public class DeviceManager : WinUsb, IDeviceManager
    {
        public IEnumerable<DeviceInformation> EnumerateAllDevices()
        {
            Guid hidGuid;
            HidD_GetHidGuid(out hidGuid);	// next, get the GUID from Windows that it uses to represent the HID (Human Interface Device) USB interface

            // This gets a list of all HID devices currently connected to the computer (InfoSet)
            IntPtr infoSetPtr = SetupDiGetClassDevs(ref hidGuid, null, IntPtr.Zero, DeviceInfoFlags.Interface | DeviceInfoFlags.Present);
            try
            {
                DeviceInformation deviceInformation;
                for (uint i = 0; GetDeviceInfoByIndex(infoSetPtr, ref hidGuid, i, out deviceInformation); ++i)
                {
                    Debug.WriteLine(deviceInformation.Path); // list to debugger output window

                    yield return deviceInformation;
                }
            }
            finally
            {
                // Before we go, we have to free up the InfoSet memory reserved by SetupDiGetClassDevs
                SetupDiDestroyDeviceInfoList(infoSetPtr);
            }
        }

        public IDeviceStream OpenDeviceStream(DeviceInformation deviceInfo)
        {
            var deviceHandle = CreateFile(deviceInfo.Path,
                                          FileAccess.Read | FileAccess.Write,
                                          FileShare.None,
                                          IntPtr.Zero,
                                          FileMode.Open,
                                          FileAttributes.NoBuffering | FileAttributes.Overlapped,
                                          IntPtr.Zero);
            if (deviceHandle.IsInvalid)
            {
                var lastError = Marshal.GetLastWin32Error();
                if (ERROR_SHARING_VIOLATION == lastError)
                {
                    throw new WarningException(string.Format(Errors.Device_Sharing_Violation_0, deviceInfo.Description));
                }

                throw new IOException(
                        string.Concat(Errors.Device_Opening_Failed, ":", Environment.NewLine, 
                                      deviceInfo.Description, Environment.NewLine, deviceInfo.Path),
                        new Win32Exception(lastError)
                );
            }

            try
            {
                HidCaps capabilities;
                QueryDeviceCapabilities(deviceHandle, out capabilities);
                return new DeviceStream(
                    new FileStream(deviceHandle, System.IO.FileAccess.ReadWrite, 0x0f, isAsync: true),
                    (ushort)capabilities.InputReportByteLength,
                    (ushort)capabilities.OutputReportByteLength
                );
            }
            catch
            {
                deviceHandle.Close();
                throw;
            }
        }

        readonly ITaskQueue _eventsQueue;
        readonly DeviceMonitorWindow _deviceMonitorWindow;

        public DeviceManager(ITaskQueue eventsQueue)
        {
            Contract.Requires<ArgumentNullException>(eventsQueue != null);

            _eventsQueue = eventsQueue;
            _deviceMonitorWindow = new DeviceMonitorWindow(this);
        }

        public event EventHandler<DeviceChangeArgs> DeviceChanged;

        void EnqueueEvent<TArgs>(EventHandler<TArgs> handler, TArgs args) where TArgs : EventArgs
        {
            if (handler != null)
            {
                _eventsQueue.Enqueue(() => handler(this, args));     
            }
        }

        #region Helpers

        static void QueryDeviceCapabilities(SafeFileHandle deviceHandle, out HidCaps capabilities)
        {
            IntPtr preparsedData;
            if (!HidD_GetPreparsedData(deviceHandle, out preparsedData))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error()) { Source = "WinUSB.HidD_GetPreparsedData" };
            }

            if (!HidP_GetCaps(preparsedData, out capabilities))
            {
                HidD_FreePreparsedData(ref preparsedData);
                throw new Win32Exception(Marshal.GetLastWin32Error()) { Source = "WinUSB.HidP_GetCaps" };
            }

            HidD_FreePreparsedData(ref preparsedData);
        }

        static bool GetDeviceInfoByIndex(IntPtr infoSetPtr, ref Guid hidGuid, uint deviceIndex, out DeviceInformation deviceInfo)
        {
            var interfaceData = new DeviceInterfaceData();
            interfaceData.Size = Marshal.SizeOf(interfaceData); // 28 in 32bit or 32 in 64bit mode
            
            if (SetupDiEnumDeviceInterfaces(infoSetPtr, IntPtr.Zero, ref hidGuid, deviceIndex, ref interfaceData))
            {
                var deviceInfoData = new DeviceInfoData();
                deviceInfoData.Size = (uint) Marshal.SizeOf(deviceInfoData);

                if (SetupDiEnumDeviceInfo(infoSetPtr, deviceIndex, ref deviceInfoData))
                {
                    deviceInfo = new DeviceInformation
                    (
                        path: GetDevicePath(infoSetPtr, ref interfaceData),
                        description: ReadPropertyString(infoSetPtr, ref deviceInfoData, RegistryProperty.DeviceDescription)
                    );
                    return true;
                }
            }

            if (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
            {
                deviceInfo = new DeviceInformation();
                return false; // Do nothing, this just means the device wasn't found
            }

            // Load system error message
            var exception = new Win32Exception(Marshal.GetLastWin32Error()) { Source = "WinUSB.SetupDiEnumDeviceInterfaces" };

            if (ERROR_INVALID_USER_BUFFER == Marshal.GetLastWin32Error())
            {
                // Property infoSetPtr.Size was not set correctly (must be 5 for 32bit or 8 for 64bit)
                throw new Exception(Errors.Invalid_InfoSet_Size, exception);
            }

            throw exception;
        }

        static string ReadPropertyString(IntPtr infoSetPtr, ref DeviceInfoData deviceInfo, RegistryProperty property)
        {
            RegistryValueKind valueKind;
            uint requiredSize;

            SetupDiGetDeviceRegistryProperty(infoSetPtr, ref deviceInfo, property, out valueKind, IntPtr.Zero, 0, out requiredSize);
            var lastError = Marshal.GetLastWin32Error();

            if (ERROR_INSUFFICIENT_BUFFER == lastError)
            {
                Contract.Assert(new[]
                {
                    RegistryValueKind.ExpandString,
                    RegistryValueKind.MultiString,
                    RegistryValueKind.String
                }
                .Contains(valueKind));
                
                IntPtr buffer = Marshal.AllocHGlobal((int)requiredSize);
                try
                {
                    if (SetupDiGetDeviceRegistryProperty(infoSetPtr, ref deviceInfo, property, out valueKind, buffer, requiredSize, out requiredSize))
                    {
                        return Marshal.PtrToStringAuto(buffer);
                    }
                    lastError = Marshal.GetLastWin32Error();
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
            if (ERROR_INVALID_DATA == lastError)
            {
                return string.Empty;
            }
            throw new Win32Exception(lastError) { Source = "WinUSB.SetupDiGetDeviceRegistryProperty" };
        }

        static string GetDevicePath(IntPtr infoSetPtr, ref DeviceInterfaceData interfaceData)
        {
            var interfaceDetailData = new DeviceInterfaceDetailData
            {
                Size = Environment.Is64BitProcess ? 8 : 5
            };
            
            // Query needed structure size
            uint requiredSize = 0;
            if (SetupDiGetDeviceInterfaceDetail(infoSetPtr, ref interfaceData, IntPtr.Zero, 0, ref requiredSize, IntPtr.Zero))
            {
                return string.Empty; // Device has no path (because request was successful without memory for the path). 
                                     // Just skip it. Theoretically, there should be not such devices.
            }

            // Check we have enough space in details structure
            if (requiredSize > Marshal.SizeOf(interfaceDetailData))
            {
                var errorMessage =
                    string.Format(Errors.Insufficient_Path_Buffer_0,
                                  requiredSize - Marshal.SizeOf(interfaceDetailData.Size));
                
                throw new InsufficientMemoryException(errorMessage) { Source = "WinUSB.SetupDiGetDeviceInterfaceDetail" };
            }

            // Request device path
            if (SetupDiGetDeviceInterfaceDetail(infoSetPtr, ref interfaceData, ref interfaceDetailData, 
                                                requiredSize, ref requiredSize, IntPtr.Zero))
            {
                return interfaceDetailData.DevicePath;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error()) { Source = "WinUSB.SetupDiGetDeviceInterfaceDetail" };
        }

        #endregion

        #region DeviceMonitorWindow
        sealed class DeviceMonitorWindow : NativeWindow, IDisposable
        {
            readonly DeviceManager _manager;

            public DeviceMonitorWindow(DeviceManager manager)
            {
                _manager = manager;
                base.CreateHandle(new CreateParams());
                Contract.Assume(Handle != IntPtr.Zero);
                RegisterForUsbEvents(Handle, HIDGuid);
            }

            public void Dispose()
            {
                UnregisterForUsbEvents(Handle);
                base.DestroyHandle();
                GC.SuppressFinalize(this);
            }

            protected override void WndProc(ref Message message)
            {
                base.WndProc(ref message);

                if ((message.Msg == WM_DEVICECHANGE) && (message.LParam != IntPtr.Zero))
                {
                    var header = (DeviceBroadcastHeader)Marshal.PtrToStructure(
                        message.LParam, typeof(DeviceBroadcastHeader));

                    if (header.DeviceType != DeviceType.Interface)
                    {
                        return;
                    }

                    var deviceInterface = (DeviceBroadcastInterface)Marshal.PtrToStructure(
                            message.LParam, typeof(DeviceBroadcastInterface));
                    switch (message.WParam.ToInt32())
                    {
                        case DEVICE_ARRIVAL:
                            SignalDeviceChange(DeviceChange.Added, deviceInterface);
                            break;

                        case DEVICE_REMOVECOMPLETE:
                            SignalDeviceChange(DeviceChange.Removed, deviceInterface);
                            break;
                    }
                }
            }

            void SignalDeviceChange(DeviceChange state, DeviceBroadcastInterface volume)
            {
                _manager.EnqueueEvent(_manager.DeviceChanged, new DeviceChangeArgs(state, volume.Name));
            }
        }

        #endregion DeviceMonitor
    }
}