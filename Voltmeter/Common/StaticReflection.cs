using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using Voltmeter.Resources;

namespace Voltmeter.Common
{
    /// <summary>
    /// Static reflection methods/extensions.
    /// </summary>
    /// <devdoc>
    /// Original implementation from 
    /// http://joelabrahamsson.com/getting-property-and-method-names-using-static-reflection-in-c/
    /// </devdoc>
    public static class StaticReflection
    {
        #region GetMemberName

        public static string GetMemberName<T, V>(
            T instance,
            Expression<Func<T, V>> expression)
        {
            return GetMemberInfo(instance, expression).Name;
        }

        public static string GetMemberName<T, V>(
            Expression<Func<T, V>> expression)
        {
            return GetMemberInfo(expression).Name;
        }

        public static string GetMemberName<T>(
            T instance,
            Expression<Action<T>> expression)
        {
            return GetMemberInfo(instance, expression).Name;
        }

        public static string GetMemberName<T>(
            Expression<Action<T>> expression)
        {
            return GetMemberInfo(expression).Name;
        }

        public static string GetMemberName<T>(
            Expression<Func<T>> expression)
        {
            return GetMemberInfo(expression).Name;
        }

        #endregion

        #region GetMemberInfo

        public static MemberInfo GetMemberInfo<T, V>(
            T instance,
            Expression<Func<T, V>> expression)
        {
            return GetMemberInfo(expression);
        }

        public static MemberInfo GetMemberInfo<T, V>(
            Expression<Func<T, V>> expression)
        {
            Contract.Requires<ArgumentNullException>(expression != null);

            return GetMemberInfo(expression.Body);
        }

        public static MemberInfo GetMemberInfo<T>(
            T instance,
            Expression<Action<T>> expression)
        {
            return GetMemberInfo(expression);
        }

        public static MemberInfo GetMemberInfo<T>(
            Expression<Action<T>> expression)
        {
            Contract.Requires<ArgumentNullException>(expression != null);

            return GetMemberInfo(expression.Body);
        }

        public static MemberInfo GetMemberInfo<T>(
            Expression<Func<T>> expression)
        {
            Contract.Requires<ArgumentNullException>(expression != null);

            return GetMemberInfo(expression.Body);
        }


        public static MemberInfo GetMemberInfo(Expression expression)
        {
            Contract.Requires<ArgumentNullException>(expression != null);

            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                // Reference type property or field
                return memberExpression.Member;
            }

            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null)
            {
                // Reference type method
                return methodCallExpression.Method;
            }

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                // Property, field of method returning value type
                var methodExpression = unaryExpression.Operand as MethodCallExpression;
                return methodExpression != null ? methodExpression.Method
                    : ((MemberExpression)unaryExpression.Operand).Member;
            }

            throw new ArgumentException(Errors.Invalid_Expression);
        }

        #endregion
    }
}
