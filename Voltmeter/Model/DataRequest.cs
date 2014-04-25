
using System.Diagnostics.Contracts;

namespace Voltmeter.Model
{
    public struct DataRequest
    {
        public readonly byte[] RawBytes;
        private DataRequest(byte[] rawBytes)
        {
            Contract.Assert(rawBytes.Length == DeviceConstants.OutputReportLength);
            RawBytes = rawBytes;
        }

        public static readonly DataRequest Frequent = new DataRequest(new byte[] { 0, 0, 0 });
        public static readonly DataRequest Periodic = new DataRequest(new byte[] { 0, 0, 1 });
    }
}