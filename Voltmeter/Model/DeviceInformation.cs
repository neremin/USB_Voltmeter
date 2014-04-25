using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Voltmeter.Model
{
    [DebuggerDisplay("{Description} {Path}")]
    public struct DeviceInformation : IEquatable<DeviceInformation>
    {
        public readonly string Path;
        public readonly string Description;

        public DeviceInformation(string path, string description = null)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(path));
            Path = path.ToLowerInvariant();
            Description = description ?? string.Empty;
        }

        public bool Equals(DeviceInformation other)
        {
            return Path == other.Path;
        }
        public override bool Equals(object other)
        {
            if (other is DeviceInformation)
            {
                return Equals((DeviceInformation)other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public override string ToString()
        {
            return Description;
        }
    }
}