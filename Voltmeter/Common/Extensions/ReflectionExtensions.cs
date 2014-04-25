using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Voltmeter.Common.Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Find attribute for the given type/member.
        /// </summary>
        /// <typeparam name="T">Type of the attribute to find.</typeparam>
        /// <param name="provider">Attributes provider (type/member, etc.)</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute</param>
        /// <returns>First attribute instance if found, otherwise null.</returns>
        public static T GetAttribute<T>(this ICustomAttributeProvider provider, bool inherit = false)
            where T : Attribute
        {
            Contract.Requires<ArgumentNullException>(provider != null);
            var attributes = provider.GetCustomAttributes(typeof(T), inherit);
            return attributes.Length > 0 ? attributes[0] as T : null;
        }
    }
}
