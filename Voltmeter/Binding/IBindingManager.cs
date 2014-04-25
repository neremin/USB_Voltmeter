using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using Voltmeter.Common.Extensions;

namespace Voltmeter.Binding
{
    [ContractClass(typeof(IBindingManagerContract))]
    public interface IBindingManager
    {
        IBinding GetBinding<TTarget>
            (
                Expression<Func<TTarget>> target
            );

        IBinding BindOneWay<TSource, TTarget>
            (
                Expression<Func<TSource>> source,
                Expression<Func<TTarget>> target,
                Func<TSource, TTarget> converter = null,
                TTarget defaultValue = default (TTarget)
            );

        IBinding BindTwoWay<TSource, TTarget>
            (
                Expression<Func<TSource>> source,
                Expression<Func<TTarget>> target,
                Func<TSource, TTarget> forwardConverter = null,
                Func<TTarget, TSource> reverseConverter = null,
                TTarget defaultValue = default (TTarget)
            );

        void RemoveBinding<TTarget>(Expression<Func<TTarget>> target);
        void RemoveAll();
    }

    [ContractClassFor(typeof(IBindingManager))]
    abstract class IBindingManagerContract : IBindingManager
    {
        public IBinding GetBinding<TTarget>(Expression<Func<TTarget>> target)
        {
            Contract.Requires<ArgumentNullException>(target != null);
            Contract.Requires<ArgumentException>(target.MemberInfo() is PropertyInfo);

            return default(IBinding);
        }

        public IBinding BindOneWay<TSource, TTarget>
            (
                Expression<Func<TSource>> source,
                Expression<Func<TTarget>> target,
                Func<TSource, TTarget> converter,
                TTarget defaultValue = default(TTarget)
            )
        {
            Contract.Requires<ArgumentNullException>(target != null);
            Contract.Requires<ArgumentException>(target.MemberInfo() is PropertyInfo);

            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentException>(source.MemberInfo() is PropertyInfo);

            Contract.Requires<ArgumentException>(converter != null || typeof(TSource) == typeof(TTarget));
            //Contract.Requires<ArgumentException>(source.MemberInfo().ReflectedType.IsSubclassOf(typeof(INotifyPropertyChanged)));

            return default(IBinding);
        }

        public IBinding BindTwoWay<TSource, TTarget>
            (
                Expression<Func<TSource>> source,
                Expression<Func<TTarget>> target,
                Func<TSource, TTarget> forwardConverter,
                Func<TTarget, TSource> reverseConverter,
                TTarget defaultValue = default(TTarget)
            )
        {
            Contract.Requires<ArgumentNullException>(target != null);
            Contract.Requires<ArgumentException>(target.MemberInfo() is PropertyInfo);
            //Contract.Requires<ArgumentException>(target.MemberInfo().ReflectedType.IsSubclassOf(typeof(INotifyPropertyChanged)));

            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentException>(source.MemberInfo() is PropertyInfo);
            //Contract.Requires<ArgumentException>(source.MemberInfo().ReflectedType.IsSubclassOf(typeof(INotifyPropertyChanged)));

            Contract.Requires<ArgumentException>(forwardConverter != null && reverseConverter != null || typeof(TSource) == typeof(TTarget));

            return default(IBinding);
        }

        public void RemoveBinding<TTarget>(Expression<Func<TTarget>> target)
        {
            Contract.Requires<ArgumentNullException>(target != null);
            Contract.Requires<ArgumentException>(target.MemberInfo() is PropertyInfo);
        }

        public abstract void RemoveAll();

        public abstract void Dispose();

    }
}