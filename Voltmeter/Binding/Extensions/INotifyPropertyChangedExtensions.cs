using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using Voltmeter.Common.Extensions;

namespace Voltmeter.Binding.Extensions
{
    public static class INotifyPropertyChangedExtensions
    {
        public static void Notify<TModel, TValue>
            (
                this TModel model,
                PropertyChangedEventHandler handler, 
                Expression<Func<TModel, TValue>> property
            ) 
            where TModel : INotifyPropertyChanged
        {
            Contract.Requires<ArgumentNullException>(model != null);
            Contract.Requires<ArgumentNullException>(property != null);
            
            Contract.Assert(property.MemberInfo() is PropertyInfo);
            Contract.Assert(property.MemberInfo().ReflectedType != null && property.MemberInfo().ReflectedType.IsInstanceOfType(model));

            if (handler != null)
            {
                handler(model, new PropertyChangedEventArgs(property.MemberName()));
            }
        }
    }
}
