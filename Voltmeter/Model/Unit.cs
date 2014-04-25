using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Voltmeter.Common;

namespace Voltmeter.Model
{
    // Inspired by http://www.codeproject.com/Articles/30569/Unit-Conversions-in-C-WPF   
    [Serializable]
    public struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        public readonly string Type;
        public readonly string Abbreviation;
        public readonly string Name;
        public readonly double Scale;
        public readonly double Shift;

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Abbreviation);
        }

        public Unit(string type, string name, string abbreviation, double scale, double shift = 0.0d)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(type));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(abbreviation));
            Contract.Requires<ArgumentException>(scale != 0.0d);

            Name         = name;
            Abbreviation = abbreviation;
            Type         = type;
            Scale        = scale;
            Shift        = shift;
        }

        public int CompareTo(Unit that)
        {
            Contract.Assert(this.Type == that.Type);
            return Comparer<double>.Default.Compare(this.Scale + this.Shift, that.Scale + that.Shift);
        }

        public bool Equals(Unit that)
        {
            return this.Type == that.Type &&
                   this.Scale == that.Scale &&
                   this.Shift == that.Shift;
        }

        public override bool Equals(object obj)
        {
            if (obj is Unit)
            {
                return Equals((Unit) obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCodeCombiner.CombineHashCodes(
                    Type.GetHashCode(),
                    Scale.GetHashCode(),
                    Shift.GetHashCode());
        }

    }
}