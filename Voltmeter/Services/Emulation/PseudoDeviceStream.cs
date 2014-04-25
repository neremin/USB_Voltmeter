using System;
using System.Threading;
using Voltmeter.Model;

namespace Voltmeter.Services.Emulation
{
    public class PseudoDeviceStream : IDeviceStream
    {
        readonly WaveSignalGenerator _generator;

        public PseudoDeviceStream(double min, double max, TimeSpan period)
        {
            _generator = new WaveSignalGenerator(min, max, period);
        }

        public ushort MaxRequestSize { get { return DeviceConstants.OutputReportLength; } }
        public ushort MaxResponseSize { get { return DeviceConstants.InputReportLength; } }

        public void Write(byte[] buffer, int offset, int count)
        {
            // Ignore
        }

        public bool Read(byte[] buffer, int offset, int count, TimeSpan waitInterval, CancellationToken cancelToken)
        {
            var sample = new DataSample(DateTimeOffset.UtcNow, _generator.GetCurrentValue());
            var bytes = DataSampleConverter.ToBytes(sample);

            Array.Copy(bytes, 0, buffer, offset, Math.Min(bytes.Length, count));

            return true;
        }

        public void Dispose()
        {
            // Ignore
        }
    }
}