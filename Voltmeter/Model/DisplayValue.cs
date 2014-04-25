using PropertyChanged;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using Voltmeter.Binding;

namespace Voltmeter.Model
{
    [DebuggerDisplay("{Value} {Units.Target}")]
    [ImplementPropertyChanged]
    public sealed class DisplayValue
    {
        readonly IBindingManager Bindings;
        readonly double MinSourceValue;
        readonly double MaxSourceValue;

        public DisplayValue
            (
                Expression<Func<double>> source,
                Unit sourceUnit,
                Unit[] units,
                double minSourceValue = double.NaN,
                double maxSourceValue = double.NaN
            )
        {
            Bindings = new BindingManager();
            Units = new DisplayUnits(this, sourceUnit, units);

            Bindings.BindTwoWay
            (
                source,
                () => Value,
                sourceValue => UnitConverter.Convert(sourceValue, from: Units.Source, to: Units.Target),
                targetValue => UnitConverter.Convert(targetValue, from: Units.Target, to: Units.Source)
            );

            MinSourceValue = minSourceValue;
            MaxSourceValue = maxSourceValue;

            Refresh();
        }

        public double Value { get; set; }
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }

        [DoNotNotify]
        public DisplayUnits Units { get; private set; }

        public void Refresh()
        {
            var newMinValue = UnitConverter.Convert(MinSourceValue, from: Units.Source, to: Units.Target);
            var newMaxValue = UnitConverter.Convert(MaxSourceValue, from: Units.Source, to: Units.Target);

            // Set range to maximum to avoid value truncation while target update
            MinValue = Math.Min(MinValue, newMinValue);
            MaxValue = Math.Max(MaxValue, newMaxValue);

            Bindings.GetBinding(() => Value).RefreshTarget();

            // Set range to actual values
            MinValue = newMinValue;
            MaxValue = newMaxValue;
        }
        
        [ContractInvariantMethod]
        void Invariant()
        {
            Contract.Invariant(double.IsNaN(MinValue) || MinValue <= Value);
            Contract.Invariant(double.IsNaN(MaxValue) || MaxValue >= Value);
        }

        #region DisplayUnits

        [ImplementPropertyChanged]
        public sealed class DisplayUnits
        {
            private readonly DisplayValue _displayValue;
            internal DisplayUnits(
                        DisplayValue displayValue,
                        Unit source,
                        Unit[] list
                )
            {
                Contract.Requires<ArgumentNullException>(list != null);
                Contract.Requires<ArgumentException>(list.Length > 0);
                Contract.Requires<ArgumentException>(list.All(unit => UnitConverter.IsConvertible(source, unit)));

                Source = source;
                List = list;
                _displayValue = displayValue;
            }

            [DoNotNotify]
            public Unit Source { get; private set; }

            public Unit Target { get { return List[TargetIndex]; } }

            [DoNotNotify]
            public Unit[] List { get; private set; }

            public int TargetIndex { get; set; }

            /// <devdoc>
            /// Method gets called when TargetIndex property changes. 
            /// Method name depends on property name.
            /// https://github.com/Fody/PropertyChanged/wiki/On_PropertyName_Changed
            /// </devdoc>
            void OnTargetIndexChanged()
            {
                _displayValue.Refresh();
            }

            [ContractInvariantMethod]
            void Invariant()
            {
                Contract.Invariant(List != null && List.Length > 0);
                Contract.Invariant(0 <= TargetIndex && TargetIndex < List.Length);
            }
        }

        #endregion
    }
}