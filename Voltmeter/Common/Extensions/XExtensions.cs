using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace Voltmeter.Common.Extensions
{
    public static class XExtensions
    {
        public static XElement GetOrAddElement(this XContainer parent, XName name)
        {
            Contract.Requires<ArgumentNullException>(parent != null);
            Contract.Requires<ArgumentNullException>(name != null);
            
            var element = parent.Element(name);
            if (element == null)
            {
                element = new XElement(name);
                parent.Add(element);
            }
            return element;
        }
    }
}