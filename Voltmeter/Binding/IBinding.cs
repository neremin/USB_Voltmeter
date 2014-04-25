using System;

namespace Voltmeter.Binding
{
    public interface IBinding : IDisposable
    {
        void RefreshTarget();
    }
}
