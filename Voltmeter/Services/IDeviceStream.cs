using System;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Voltmeter.Services
{
    [ContractClass(typeof(IDeviceStreamContract))]
    public interface IDeviceStream : IDisposable
    {
        ushort MaxRequestSize { get; }
        ushort MaxResponseSize { get; }

        void Write(byte[] buffer, int offset, int count);
        bool Read(byte[] buffer, int offset, int count, TimeSpan waitInterval, CancellationToken cancelToken);
    }

    [ContractClassFor(typeof(IDeviceStream))]
    abstract class IDeviceStreamContract : IDeviceStream
    {
        public abstract ushort MaxRequestSize { get; }
        public abstract ushort MaxResponseSize { get; }
        
        public void Write(byte[] buffer, int offset, int count)
        {
            Contract.Requires<ArgumentNullException>(buffer != null);
            Contract.Requires<ArgumentOutOfRangeException>(buffer.Length <= MaxRequestSize);
            Contract.Requires<ArgumentOutOfRangeException>(0 <= offset && offset < buffer.Length);
            Contract.Requires<ArgumentOutOfRangeException>(0 <= count && offset + count <= buffer.Length);
        }

        public bool Read(byte[] buffer, int offset, int count, TimeSpan waitInterval, CancellationToken cancelToken)
        {
            Contract.Requires<ArgumentNullException>(buffer != null);
            Contract.Requires<ArgumentOutOfRangeException>(buffer.Length >= MaxResponseSize);
            Contract.Requires<ArgumentOutOfRangeException>(0 <= offset && offset < buffer.Length);
            Contract.Requires<ArgumentOutOfRangeException>(0 <= count && offset + count <= buffer.Length);

            return false;
        }

        public abstract void Dispose();

        [ContractInvariantMethod]
        void Invariant()
        {
            Contract.Invariant(MaxResponseSize > 0);
            Contract.Invariant(MaxRequestSize > 0);
        }
    }
}