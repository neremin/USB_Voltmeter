using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Windows.Forms;
using Voltmeter.Common.Extensions;

namespace Voltmeter.Binding.Extensions
{
    public static class WinFormsBindingExtensions
    {
        public static BindingBuilder<T, V> AddDataBinding<TControl, TModel, T, V>
            (
                this TControl control, Expression<Func<TControl, T>> controlProperty,
                TModel model, Expression<Func<TModel, V>> modelProperty,
                DataSourceUpdateMode updateMode = DataSourceUpdateMode.OnPropertyChanged
            )
            where TControl : Control
            where TModel : class
        {
            Contract.Requires<ArgumentNullException>(control != null);
            Contract.Requires<ArgumentNullException>(controlProperty != null);
            Contract.Requires<ArgumentNullException>(model != null);
            Contract.Requires<ArgumentNullException>(modelProperty != null);
            Contract.Assert(controlProperty.MemberInfo().ReflectedType.IsInstanceOfType(control));
            Contract.Assert(modelProperty.MemberInfo().ReflectedType.IsInstanceOfType(model));

            return new BindingBuilder<T, V>(
                    control.DataBindings.Add(controlProperty.MemberName(), model, modelProperty.MemberName(), true, updateMode)
                );
        }

        public sealed class BindingBuilder<T, V>
        {
            readonly System.Windows.Forms.Binding _binding;
            internal BindingBuilder(System.Windows.Forms.Binding binding)
            {
                Contract.Requires<ArgumentNullException>(binding != null);
                _binding = binding;
            }

            public BindingBuilder<T, V> DataSourceUpdateMode(DataSourceUpdateMode updateMode)
            {
                _binding.DataSourceUpdateMode = updateMode;
                return this;
            }
            public BindingBuilder<T, V> ControlUpdateMode(ControlUpdateMode updateMode)
            {
                _binding.ControlUpdateMode = updateMode;
                return this;
            }

            public BindingBuilder<T, V> NullValue(object value)
            {
                _binding.NullValue = value;
                return this;
            }
            public BindingBuilder<T, V> DataSourceNullValue(object value)
            {
                _binding.DataSourceNullValue = value;
                return this;
            }

            public FormattingBindingBuilder<T, V> Formatting
            {
                get { return new FormattingBindingBuilder<T, V>(_binding); }
            }

            public System.Windows.Forms.Binding Binding
            {
                get { return _binding; }
            }

            public static implicit operator System.Windows.Forms.Binding(BindingBuilder<T, V> builder)
            {
                return builder._binding;
            }
        }

        public sealed class FormattingBindingBuilder<T, V>
        {
            readonly System.Windows.Forms.Binding _binding;

            internal FormattingBindingBuilder(System.Windows.Forms.Binding binding)
            {
                Contract.Requires<ArgumentNullException>(binding != null);
                _binding = binding;
                _binding.FormattingEnabled = true;
            }

            public FormattingBindingBuilder<T, V> FormatString(string formatString)
            {
                Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(formatString));
                _binding.FormatString = formatString;
                return this;
            }

            public FormattingBindingBuilder<T, V> FormatProvider(IFormatProvider formatProvider)
            {
                Contract.Requires<ArgumentNullException>(formatProvider != null);
                _binding.FormatInfo = formatProvider;
                return this;
            }

            public FormattingBindingBuilder<T, V> Format(Func<V, T> forwardConverter)
            {
                Contract.Requires<ArgumentNullException>(forwardConverter != null);
                _binding.Format += (s, e) =>
                {
                    Contract.Requires<ArgumentException>(e.DesiredType == typeof (T));
                    Contract.Requires<ArgumentException>(e.Value is V);
                    e.Value = forwardConverter((V) e.Value);
                };
                return this;
            }

            public FormattingBindingBuilder<T, V> Parse(Func<T, V> reverseConverter)
            {
                Contract.Requires<ArgumentNullException>(reverseConverter != null);
                _binding.Parse += (s, e) =>
                {
                    Contract.Requires<ArgumentException>(e.DesiredType == typeof (V));
                    Contract.Requires<ArgumentException>(e.Value is T);
                    e.Value = reverseConverter((T)e.Value);
                };
                return this;
            }

            public System.Windows.Forms.Binding Binding
            {
                get { return _binding; }
            }

            public static implicit operator System.Windows.Forms.Binding(FormattingBindingBuilder<T, V> builder)
            {
                return builder._binding;
            }
        }
    }
}
