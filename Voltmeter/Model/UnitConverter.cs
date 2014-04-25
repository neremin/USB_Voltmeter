using System;
using System.Diagnostics.Contracts;

namespace Voltmeter.Model
{
    public static class UnitConverter
    {
        [Pure]
        public static double Convert(double value, Unit from, Unit to)
        {
            Contract.Requires<InvalidOperationException>(from.Type == to.Type);

            if (from.Shift == to.Shift && from.Scale == to.Scale)
            {
                return value;
            }

            double v = (value - from.Shift) / from.Scale;
            return v * to.Scale + to.Shift;
        }

        [Pure]
        public static bool IsConvertible(Unit from, Unit to)
        {
            return from.Type == to.Type;
        }
    }
}
