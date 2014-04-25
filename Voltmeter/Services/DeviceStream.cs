using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Voltmeter.Services
{
    public sealed class DeviceStream : IDeviceStream
    {
        Stream _stream;

        public DeviceStream(Stream stream, ushort inputReportLength, ushort outputReportLength)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            MaxResponseSize = inputReportLength;
            MaxRequestSize = outputReportLength;
            _stream = stream;
        }

        public ushort MaxRequestSize { get; private set; }
        public ushort MaxResponseSize { get; private set; }

        public void Write(byte[] buffer, int offset, int count)
        {
            var stream = _stream;
            if (stream == null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            stream.Write(buffer, offset, count);
            stream.Flush();
        }

        public bool Read(byte[] buffer, int offset, int count, TimeSpan waitInterval, CancellationToken cancelToken)
        {
            var stream = _stream;
            if (stream == null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            return Task<int>.Factory
                .FromAsync(stream.BeginRead, stream.EndRead, buffer, 0, buffer.Length, null, TaskCreationOptions.None)
                .Wait((int)waitInterval.TotalMilliseconds, cancelToken);                
        }

        public void Dispose()
        {
            var stream = Interlocked.Exchange(ref _stream, null);
            if (stream != null)
            {
                stream.Dispose();
            }
        }
    }
}