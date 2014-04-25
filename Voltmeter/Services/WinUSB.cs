using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace Voltmeter.Services
{
    /// <summary>
    /// Class wrapping USB API calls, related constants and structures
    /// </summary>
    /// <devdoc>
    /// Based on wrapper from http://www.codeproject.com/Tips/540963/ST-Micro-electronics-Device-Firmware-Upgrade-DFU-D
    /// </devdoc>
    public abstract class WinUsb
    {
        #region Structures

        /// <summary>
        /// Provides details about a single USB device
        /// </summary>
        /// <devdoc>SP_DEVICE_INTERFACE_DATA</devdoc>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct DeviceInterfaceData
        {
            public int Size;
            public Guid InterfaceClassGuid;
            public int Flags;
            private IntPtr Reserved;
        }

        /// <summary>
        /// Defines a device instance that is a member of a device information set
        /// </summary>
        /// <devdoc>SP_DEVINFO_DATA</devdoc>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct DeviceInfoData
        {
           public uint Size;
           public Guid ClassGuid;
           public uint DevInst;
           private IntPtr Reserved;
        }
        
        /// <summary>
        /// Provides the capabilities of a HID device
        /// </summary>
        /// <devdoc>HIDP_CAPS</devdoc>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct HidCaps
        {
            public short Usage;
            public short UsagePage;
            public short InputReportByteLength;
            public short OutputReportByteLength;
            public short FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            private short[] Reserved;
            public short NumberLinkCollectionNodes;
            public short NumberInputButtonCaps;
            public short NumberInputValueCaps;
            public short NumberInputDataIndices;
            public short NumberOutputButtonCaps;
            public short NumberOutputValueCaps;
            public short NumberOutputDataIndices;
            public short NumberFeatureButtonCaps;
            public short NumberFeatureValueCaps;
            public short NumberFeatureDataIndices;
        }
        /// <summary>
        /// Access to the path for a device
        /// </summary>
        /// <devdoc>SP_DEVICE_INTERFACE_DETAIL_DATA</devdoc>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct DeviceInterfaceDetailData
        {
            public int Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        /// <summary>
        /// Device change broadcast message header
        /// </summary>
        /// <devdoc>DEV_BROADCAST_HDR</devdoc>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct DeviceBroadcastHeader
        {
            public uint Size;
            public DeviceType DeviceType;
            private uint Reserved;
        }

        /// <summary>
        /// Used when registering a window to receive messages about devices added or removed from the system.
        /// </summary>
        /// <devdoc>DEV_BROADCAST_DEVICEINTERFACE</devdoc>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        protected class DeviceBroadcastInterface
        {
            public int Size;
            public DeviceType DeviceType;
            private int Reserved;
            public Guid ClassGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Name;
        }
        #endregion

        #region Constants

        /// <summary>
        /// Broadcast data type
        /// </summary>
        protected enum DeviceType : int
        {
            /// <summary>
            /// Class of devices.
            /// </summary>
            /// <devdoc>DBT_DEVTYP_DEVICEINTERFACE. This structure is a DEV_BROADCAST_DEVICEINTERFACE structure.</devdoc>
            Interface = 0x05,
            /// <summary>
            /// File system handle.
            /// </summary>
            /// <devdoc>DBT_DEVTYP_HANDLE. This structure is a DEV_BROADCAST_HANDLE structure.</devdoc>
            Handle = 0x06,
            /// <summary>
            /// OEM- or IHV-defined device type.
            /// </summary>
            /// <devdoc>DBT_DEVTYP_OEM. This structure is a DEV_BROADCAST_OEM structure.</devdoc>
            OEM = 0x00,
            /// <summary>
            /// Port device (serial or parallel).
            /// </summary>
            /// <devdoc>DBT_DEVTYP_PORT. This structure is a DEV_BROADCAST_PORT structure.</devdoc>
            Port = 0x03,
            /// <summary>
            /// Logical volume.
            /// </summary>
            /// <devdoc>DBT_DEVTYP_VOLUME. This structure is a DEV_BROADCAST_VOLUME structure.</devdoc>
            Volume = 0x02
        }

        /// <summary>
        /// Device info flags
        /// </summary>
        [Flags]
        protected enum DeviceInfoFlags : uint
        {
            /// <summary>
            /// Used in SetupDiClassDevs to get devices present in the system
            /// </summary>
            /// <devdoc>DIGCF_PRESENT</devdoc>
            Present = 0x02,
            /// <summary>
            /// Used in SetupDiClassDevs to get device interface details
            /// </summary>
            /// <devdoc>DIGCF_DEVICEINTERFACE</devdoc>
            Interface = 0x10
        }

        /// <summary>
        /// Desired file access. See <see cref="CreateFile"/>.
        /// </summary>
        [Flags]
        protected enum FileAccess : uint
        {
            /// <summary>
            /// <see cref="CreateFile"/> : Open file for read
            /// </summary>
            /// <devdoc>GENERIC_READ</devdoc>
            Read = 0x80000000,
            /// <summary>
            /// <see cref="CreateFile"/> : Open file for write
            /// </summary>
            /// <devdoc>GENERIC_WRITE</devdoc>
            Write = 0x40000000
        }

        [Flags]
        protected enum FileShare : uint
        {
            /// <summary>
            /// Prevents other processes from opening a file or device if they request delete, read, or write access.
            /// </summary>
            /// <devdoc>0</devdoc>
            None = 0,
            /// <summary>
            /// Enables subsequent open operations on a file or device to request read access. 
            /// Otherwise, other processes cannot open the file or device if they request read access.
            /// </summary>
            /// <devdoc>FILE_SHARE_READ</devdoc>
            Read = 0x00000001,
            /// <summary>
            /// If this flag is not specified, but the file or device has been opened for read access, the function fails.
            /// </summary>
            /// <devdoc>FILE_SHARE_WRITE</devdoc>
            Write = 0x00000002,
        }

        /// <summary>
        /// Specifies how the operating system should open a file. See <see cref="CreateFile"/>.
        /// </summary>
        protected enum FileMode : uint
        {
            CreateNew = 1,
            Create = 2,
            Open = 3,
            OpenOrCreate = 4,
            Truncate = 5
        }

        /// <summary>
        /// Additional flags for <see cref="CreateFile"/>.
        /// </summary>
        [Flags]
        protected enum FileAttributes : uint
        {
            /// <summary>
            /// <see cref="CreateFile"/> : Open handle for overlapped operations
            /// </summary>
            Overlapped = 0x40000000,
            /// <summary>
            /// <see cref="CreateFile"/> : No system caching for data reads and writes
            /// </summary>
            NoBuffering = 0x20000000,
            /// <summary>
            /// <see cref="CreateFile"/> : Open handle for overlapped operations
            /// </summary>
            Normal = 0x00000080,
            /// <summary>
            /// <see cref="CreateFile"/> : Write operations will not go through any intermediate cache
            /// </summary>
            WriteThrough = 0x80000000,
        }

        /// <summary>
        /// Type of the recipient handle to be notified of the device changes.
        /// </summary>
        protected enum DeviceNotificationRecipient : uint
        {
            /// <summary>
            /// The notification recipient is a window handle.
            /// </summary>
            /// <devdoc>DEVICE_NOTIFY_WINDOW_HANDLE</devdoc>
            Window = 0x00,
            /// <summary>
            /// The notification recipient is a service status handle.
            /// </summary>
            /// <devdoc>DEVICE_NOTIFY_SERVICE_HANDLE</devdoc>
            Service = 0x01
        }

        /// <summary>
        /// Flags for SetupDiGetDeviceRegistryProperty.
        /// </summary>
        protected enum RegistryProperty : uint
        {
            /// <devdoc>SPDRP_DEVICEDESC</devdoc>
            DeviceDescription = 0x00000000, // DeviceDesc (R/W)
            /// <devdoc>SPDRP_HARDWAREID</devdoc>
            HardwareID = 0x00000001, // HardwareID (R/W)
            /// <devdoc>SPDRP_COMPATIBLEIDS</devdoc>
            CompatibleIDs = 0x00000002, // CompatibleIDs (R/W)
            /// <devdoc>SPDRP_SERVICE</devdoc>
            ServiceName = 0x00000004, // Service (R/W)
            /// <devdoc>SPDRP_CLASS</devdoc>
            SetupClass = 0x00000007, // Class (R--tied to ClassGUID)
            /// <devdoc>SPDRP_CLASSGUID</devdoc>
            SetupClassGuid = 0x00000008, // ClassGUID (R/W)
            /// <devdoc>SPDRP_DRIVER</devdoc>
            DriverKey = 0x00000009, // Driver (R/W)
            /// <devdoc>SPDRP_HARDWAREID</devdoc>
            ConfigFlags = 0x0000000A, // ConfigFlags (R/W)
            /// <devdoc>SPDRP_MFG</devdoc>
            Manufacturer = 0x0000000B, // Mfg (R/W)
            /// <devdoc>SPDRP_FRIENDLYNAME</devdoc>
            FriendlyName = 0x0000000C, // FriendlyName (R/W)
            /// <devdoc>SPDRP_LOCATION_INFORMATION</devdoc>
            HardwareLocation = 0x0000000D, // LocationInformation (R/W)
            /// <devdoc>SPDRP_PHYSICAL_DEVICE_OBJECT_NAME</devdoc>
            PhysicalDeviceObjectName = 0x0000000E, // PhysicalDeviceObjectName (R)
            /// <devdoc>SPDRP_CAPABILITIES</devdoc>
            DeviceCapabilities = 0x0000000F, // Capabilities (R)
            /// <devdoc>SPDRP_UI_NUMBER</devdoc>
            UiNumber = 0x00000010, // UiNumber (R)
            /// <devdoc>SPDRP_UPPERFILTERS</devdoc>
            UpperFilters = 0x00000011, // UpperFilters (R/W)
            /// <devdoc>SPDRP_LOWERFILTERS</devdoc>
            LowerFilters = 0x00000012, // LowerFilters (R/W)
            /// <devdoc>SPDRP_BUSTYPEGUID</devdoc>
            BusTypeGuid = 0x00000013, // BusTypeGUID (R)
            /// <devdoc>SPDRP_LEGACYBUSTYPE</devdoc>
            LegacyBusType = 0x00000014, // LegacyBusType (R)
            /// <devdoc>SPDRP_BUSNUMBER</devdoc>
            BusNumber = 0x00000015, // BusNumber (R)
            /// <devdoc>SPDRP_ENUMERATOR_NAME</devdoc>
            EnumeratorName = 0x00000016, // Enumerator Name (R)
            /// <devdoc>SPDRP_SECURITY</devdoc>
            Security = 0x00000017, // Security (R/W, binary form)
            /// <devdoc>SPDRP_SECURITY_SDS</devdoc>
            SecurityDescriptor = 0x00000018, // Security (W, SDS form)
            /// <devdoc>SPDRP_DEVTYPE</devdoc>
            DeviceType = 0x00000019, // Device Type (R/W)
            /// <devdoc>SPDRP_EXCLUSIVE</devdoc>
            ExclusiveAccess = 0x0000001A, // Device is exclusive-access (R/W)
            /// <devdoc>SPDRP_CHARACTERISTICS</devdoc>
            Characteristics = 0x0000001B, // Device Characteristics (R/W)
            /// <devdoc>SPDRP_ADDRESS</devdoc>
            DeviceAddress = 0x0000001C, // Device Address (R)
            /// <devdoc>SPDRP_UI_NUMBER_DESC_FORMAT</devdoc>
            UiNumberFormatString = 0X0000001D, // UiNumberDescFormat (R/W)
            /// <devdoc>SPDRP_DEVICE_POWER_DATA</devdoc>
            DevicePowerData = 0x0000001E, // Device Power Data (R)
            /// <devdoc>SPDRP_REMOVAL_POLICY</devdoc>
            DeviceRemovalPolicy = 0x0000001F, // Removal Policy (R)
            /// <devdoc>SPDRP_REMOVAL_POLICY_HW_DEFAULT</devdoc>
            HardwareRemovalPolicy = 0x00000020, // Hardware Removal Policy (R)
            /// <devdoc>SPDRP_REMOVAL_POLICY_OVERRIDE</devdoc>
            RemovalPolicyOverride = 0x00000021, // Removal Policy Override (RW)
            /// <devdoc>SPDRP_INSTALL_STATE</devdoc>
            DeviceInstallState = 0x00000022, // Device Install State (R)
            /// <devdoc>SPDRP_LOCATION_PATHS</devdoc>
            DeviceLocationPaths = 0x00000023, // Device Location Paths (R)
        }

        /// <summary>
        /// Windows message sent when a device is inserted or removed
        /// </summary>
        protected const int WM_DEVICECHANGE = 0x0219;
        /// <summary>
        /// WParam for above : A device was inserted
        /// </summary>
        /// <devdoc>DBT_DEVICEARRIVAL</devdoc>
        protected const int DEVICE_ARRIVAL = 0x8000;
        /// <summary>
        /// WParam for above : A device was removed
        /// </summary>
        /// <devdoc>DBT_DEVICEREMOVECOMPLETE</devdoc>
        protected const int DEVICE_REMOVECOMPLETE = 0x8004;
        
        /// <summary>
        /// <see cref="SetupDiEnumDeviceInterfaces"/> : Overlapped operation is incomplete
        /// </summary>
        protected const uint ERROR_NO_MORE_ITEMS = 259;
        /// <summary>
        /// <see cref="SetupDiEnumDeviceInterfaces"/> : Insufficient buffer length
        /// </summary>
        protected const uint ERROR_INVALID_USER_BUFFER = 1784;
        /// <summary>
        /// <see cref="SetupDiGetDeviceRegistryProperty"/> : Property does not exist for or its data is not valid
        /// </summary>
        protected const uint ERROR_INVALID_DATA = 13;
        /// <summary>
        /// <see cref="SetupDiGetDeviceRegistryProperty"/> : Insufficient buffer length
        /// </summary>
        protected const uint ERROR_INSUFFICIENT_BUFFER = 122;
        /// <summary>
        /// <see cref="CreateFile"/> : The process cannot access the file because it is being used by another process.
        /// </summary>
        protected const uint ERROR_SHARING_VIOLATION = 32;

        #endregion

        #region P/Invoke
        /// <summary>
        /// Gets the GUID that Windows uses to represent HID class devices
        /// </summary>
        /// <param name="gHid">An out parameter to take the Guid</param>
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern void HidD_GetHidGuid(out Guid gHid);
        
        /// <summary>
        /// Allocates an InfoSet memory block within Windows that contains details of devices.
        /// </summary>
        /// <param name="gClass">Class guid (e.g. HID guid)</param>
        /// <param name="strEnumerator">Not used</param>
        /// <param name="hParent">Not used</param>
        /// <param name="nFlags">Type of device details required (DIGCF_ constants)</param>
        /// <returns>A reference to the InfoSet</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        protected static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, [MarshalAs(UnmanagedType.LPStr)] string strEnumerator, IntPtr hParent, DeviceInfoFlags nFlags);
        
        /// <summary>
        /// Frees InfoSet allocated in call to above.
        /// </summary>
        /// <param name="lpInfoSet">Reference to InfoSet</param>
        /// <returns>true if successful</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        protected static extern int SetupDiDestroyDeviceInfoList(IntPtr lpInfoSet);
        
        /// <summary>
        /// Gets the DeviceInterfaceData for a device from an InfoSet.
        /// </summary>
        /// <param name="lpDeviceInfoSet">InfoSet to access</param>
        /// <param name="nDeviceInfoData">Not used</param>
        /// <param name="gClass">Device class guid</param>
        /// <param name="nIndex">Index into InfoSet for device</param>
        /// <param name="oInterfaceData">DeviceInterfaceData to fill with data</param>
        /// <returns>True if successful, false if not (e.g. when index is passed end of InfoSet)</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        protected static extern bool SetupDiEnumDeviceInterfaces(IntPtr lpDeviceInfoSet, IntPtr nDeviceInfoData, ref Guid gClass, uint nIndex, ref DeviceInterfaceData oInterfaceData);

        /// <summary>
        /// The SetupDiEnumDeviceInfo function retrieves a context structure for a device information element of the specified
        /// device information set. Each call returns information about one device. The function can be called repeatedly
        /// to get information about several devices.
        /// </summary>
        /// <param name="DeviceInfoSet">A handle to the device information set for which to return an SP_DEVINFO_DATA structure that represents a device information element.</param>
        /// <param name="MemberIndex">A zero-based index of the device information element to retrieve.</param>
        /// <param name="DeviceInfoData">A pointer to an SP_DEVINFO_DATA structure to receive information about an enumerated device information element. The caller must set DeviceInfoData.cbSize to sizeof(SP_DEVINFO_DATA).</param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        protected static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, UInt32 MemberIndex, ref DeviceInfoData DeviceInfoData);
        
        /// <summary>
        /// SetupDiGetDeviceInterfaceDetail - two of these, overloaded because they are used together in slightly different
        /// ways and the parameters have different meanings.
        /// Gets the interface detail from a DeviceInterfaceData. This is pretty much the device path.
        /// You call this twice, once to get the size of the struct you need to send (nDeviceInterfaceDetailDataSize=0)
        /// and once again when you've allocated the required space.
        /// </summary>
        /// <param name="DeviceInfoSet">InfoSet to access</param>
        /// <param name="InterfaceData">DeviceInterfaceData to use</param>
        /// <param name="lpDeviceInterfaceDetailData">DeviceInterfaceDetailData to fill with data</param>
        /// <param name="nDeviceInterfaceDetailDataSize">The size of the above</param>
        /// <param name="nRequiredSize">The required size of the above when above is set as zero</param>
        /// <param name="lpDeviceInfoData">Not used</param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        protected static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref DeviceInterfaceData InterfaceData, IntPtr lpDeviceInterfaceDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, IntPtr lpDeviceInfoData);
        [DllImport("setupapi.dll", SetLastError = true)]
        protected static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref DeviceInterfaceData InterfaceData, ref DeviceInterfaceDetailData oDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, IntPtr lpDeviceInfoData);

        /// <summary>
        /// The SetupDiGetDeviceRegistryProperty  function retrieves the specified device property.
        /// This handle is typically returned by the SetupDiGetClassDevs or SetupDiGetClassDevsEx function.
        /// </summary>
        /// <param Name="DeviceInfoSet">Handle to the device information set that contains the interface and its underlying device.</param>
        /// <param Name="DeviceInfoData">Pointer to an SP_DEVINFO_DATA structure that defines the device instance.</param>
        /// <param Name="Property">Device property to be retrieved. SEE MSDN</param>
        /// <param Name="PropertyRegDataType">Pointer to a variable that receives the registry data Type. This parameter can be NULL.</param>
        /// <param Name="PropertyBuffer">Pointer to a buffer that receives the requested device property.</param>
        /// <param Name="PropertyBufferSize">Size of the buffer, in bytes.</param>
        /// <param Name="RequiredSize">Pointer to a variable that receives the required buffer size, in bytes. This parameter can be NULL.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        protected static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref DeviceInfoData DeviceInfoData, RegistryProperty Property, out RegistryValueKind PropertyRegDataType, IntPtr PropertyBuffer, uint PropertyBufferSize, out uint RequiredSize);

        /// <summary>
        /// Registers a window for device insert/remove messages
        /// </summary>
        /// <param name="hwnd">Handle to the window that will receive the messages</param>
        /// <param name="lpInterface">DeviceBroadcastInterrface structure</param>
        /// <param name="nFlags">set to DEVICE_NOTIFY_WINDOW_HANDLE</param>
        /// <returns>A handle used when unregistering</returns>
        [DllImport("user32.dll", SetLastError = true)]
        protected static extern IntPtr RegisterDeviceNotification(IntPtr hwnd, DeviceBroadcastInterface oInterface, DeviceNotificationRecipient nFlags);

        /// <summary>
        /// Unregister from above.
        /// </summary>
        /// <param name="hHandle">Handle returned in call to RegisterDeviceNotification</param>
        /// <returns>True if success</returns>
        [DllImport("user32.dll", SetLastError = true)]
        protected static extern bool UnregisterDeviceNotification(IntPtr hHandle);

        /// <summary>
        /// Gets details from an open device. Reserves a block of memory which must be freed.
        /// </summary>
        /// <param name="hFile">Device file handle</param>
        /// <param name="lpData">Reference to the preparsed data block</param>
        /// <returns></returns>
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern bool HidD_GetPreparsedData(SafeFileHandle hFile, out IntPtr lpData);
        /// <summary>
        /// Frees the memory block reserved above.
        /// </summary>
        /// <param name="pData">Reference to preparsed data returned in call to GetPreparsedData</param>
        /// <returns></returns>
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern bool HidD_FreePreparsedData(ref IntPtr pData);
        /// <summary>
        /// Gets a device's capabilities from the preparsed data.
        /// </summary>
        /// <param name="lpData">Preparsed data reference</param>
        /// <param name="oCaps">HidCaps structure to receive the capabilities</param>
        /// <returns>True if successful</returns>
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern bool HidP_GetCaps(IntPtr lpData, out HidCaps oCaps);

        // Additional P/Invokes for device firmware update
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern bool HidD_GetFeature(IntPtr lpData, out byte lpReportBuffer, uint uBufferLength);

        [DllImport("hid.dll", SetLastError = true)]
        protected static extern bool HidD_SetFeature(SafeFileHandle hFile, IntPtr Buffer, uint uBufferLength);

        /// <summary>
        /// Set the size of the USB Report IN input buffer
        /// </summary>
        /// <param name="hFile">Handle to device</param>
        /// <param name="nBuffers">Required size of buffer in USB IN Reports</param>
        /// <returns>True if successful</returns>
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern bool HidD_SetNumInputBuffers(IntPtr hFile, uint nBuffers);

        /// <summary>
        /// Get the indexed string. (MMc added)
        /// Remember to initialise the string first so that it has some size, e.g. StringBuilder myValue = new StringBuilder("", 256);
        /// </summary>
        /// <param name="hFile">Handle to HID device</param>
        /// <param name="ulStringIndex">Index for string</param>
        /// <param name="pvBuffer">Buffer for string</param>
        /// <param name="ulBufferLength">Size of buffer</param>
        /// <returns></returns>
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern bool HidD_GetIndexedString(SafeFileHandle hFile, uint uStringIndex, byte[] Buffer, uint uBufferLength);

        [DllImport("hid.dll", SetLastError = true)]
        protected static extern bool HidD_GetInputReport(IntPtr hFile, byte[] Buffer, uint uBufferLength);


        [DllImport("hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        protected static extern bool HidD_GetProductString(IntPtr hDevice, byte[] Buffer, uint BufferLength);

        /// <summary>
        /// Creates/opens a file, serial port, USB device... etc
        /// </summary>
        /// <param name="strName">Path to object to open</param>
        /// <param name="nAccess">Access mode. e.g. Read, write</param>
        /// <param name="nShareMode">Sharing mode</param>
        /// <param name="lpSecurity">Security details (can be null)</param>
        /// <param name="nCreationFlags">Specifies if the file is created or opened</param>
        /// <param name="nAttributes">Any extra attributes? e.g. open overlapped</param>
        /// <param name="lpTemplate">Not used</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        protected static extern SafeFileHandle CreateFile(string strName, FileAccess nAccess, FileShare nShareMode, IntPtr lpSecurity, FileMode nCreationFlags, FileAttributes nAttributes, IntPtr lpTemplate);

        #endregion

        #region Helper methods
        /// <summary>
        /// Registers a window to receive windows messages when a device is inserted/removed. Need to call this
        /// from a form when its handle has been created, not in the form constructor. Use form's OnHandleCreated override.
        /// </summary>
        /// <param name="hWnd">Handle to window that will receive messages</param>
        /// <param name="gClass">Class of devices to get messages for</param>
        /// <returns>A handle used when unregistering</returns>
        protected static IntPtr RegisterForUsbEvents(IntPtr hWnd, Guid gClass)
        {
            var oInterfaceIn = new DeviceBroadcastInterface();
            oInterfaceIn.Size = Marshal.SizeOf(oInterfaceIn);
            oInterfaceIn.ClassGuid = gClass;
            oInterfaceIn.DeviceType = DeviceType.Interface;
            return RegisterDeviceNotification(hWnd, oInterfaceIn, DeviceNotificationRecipient.Window);
        }

        /// <summary>
        /// Unregisters notifications. Can be used in form dispose
        /// </summary>
        /// <param name="hHandle">Handle returned from RegisterForUSBEvents</param>
        /// <returns>True if successful</returns>
        protected static bool UnregisterForUsbEvents(IntPtr hHandle)
        {
            return UnregisterDeviceNotification(hHandle);
        }

        /// <summary>
        /// Helper to get the HID guid.
        /// </summary>
        protected static Guid HIDGuid
        {
            get
            {
                Guid gHid;
                HidD_GetHidGuid(out gHid);
                return gHid;
            }
        }

        public static string FormatVidPid(ushort vendorId, ushort productId)
        {
            return string.Format("vid_{0:x4}&pid_{1:x4}", vendorId, productId);
        }
        #endregion
    }
}