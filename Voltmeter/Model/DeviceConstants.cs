using System;
using Voltmeter.Services;

namespace Voltmeter.Model
{
    // Analogue input to PIC18F2550 based on code from HID page at www.Lvr.com/hidpage.htm
    // Purpose: demonstrates USB communications with a HID-class device
    // Set these to match the values in the device's firmware.
    public static class DeviceConstants
    {
        /// <summary>
        /// VID
        /// </summary>
        public const int VendorId = 0x4D8;
        /// <summary>
        /// PID
        /// </summary>
        public const int ProductId = 0x3F;

        /// <summary>
        /// Maximum voltage value which can be measured by device without overflow
        /// </summary>
        public const double MaxMeasuredVoltage = 5.0d;

        /// <summary>
        /// Minimum device polling interval
        /// </summary>
        public static readonly TimeSpan MinPollingInterval = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Maximum device polling interval
        /// </summary>
        public static readonly TimeSpan MaxPollingInterval = TimeSpan.FromMilliseconds(1000);

        /// <summary>
        /// Input Report Length in bytes. For emulation purposes only
        /// </summary>
        public const ushort InputReportLength = 7;

        /// <summary>
        /// Output Report Length in bytes. For emulation purposes only
        /// </summary>
        public const ushort OutputReportLength = 3;

        /// <summary>
        /// Device VID & PID combination pattern for matching device path
        /// </summary>
        public static readonly string SearchPattern = WinUsb.FormatVidPid(VendorId, ProductId);
    }
}
