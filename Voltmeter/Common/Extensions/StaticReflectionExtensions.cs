using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace Voltmeter.Common.Extensions
{
    public static class StaticReflectionExtensions
    {
        [Pure]
        public static string MemberName<T, V>(
                this T instance,
                Expression<Func<T, V>> expression)
        {
            return StaticReflection.GetMemberName(instance, expression);
        }

        [Pure]
        public static string MemberName<T, V>(
            this Expression<Func<T, V>> expression)
        {
            return StaticReflection.GetMemberName(expression);
        }

        [Pure]
        public static string MemberName<T>(
            this T instance,
            Expression<Action<T>> expression)
        {
            return StaticReflection.GetMemberName(instance, expression);
        }

        [Pure]
        public static string MemberName<T>(
            this Expression<Action<T>> expression)
        {
            return StaticReflection.GetMemberName(expression);
        }

        [Pure]
        public static string MemberName<T>(
            this Expression<Func<T>> expression)
        {
            return StaticReflection.GetMemberName(expression);
        }

        [Pure]
        public static MemberInfo MemberInfo<T, V>(
            this T instance,
            Expression<Func<T, V>> expression)
        {
            return StaticReflection.GetMemberInfo(instance, expression);
        }

        [Pure]
        public static MemberInfo MemberInfo<T, V>(
            this Expression<Func<T, V>> expression)
        {
            return StaticReflection.GetMemberInfo(expression);
        }

        [Pure]
        public static MemberInfo MemberInfo<T>(
            this T instance,
            Expression<Action<T>> expression)
        {
            return StaticReflection.GetMemberInfo(instance, expression);
        }

        [Pure]
        public static MemberInfo MemberInfo<T>(
            this Expression<Action<T>> expression)
        {
            return StaticReflection.GetMemberInfo(expression);
        }

        [Pure]
        public static MemberInfo MemberInfo<T>(
            this Expression<Func<T>> expression)
        {
            return StaticReflection.GetMemberInfo(expression);
        }

        [Pure]
        public static MemberInfo MemberInfo(this Expression expression)
        {
            return StaticReflection.GetMemberInfo(expression);
        }
       
    }
}