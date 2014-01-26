using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Attributes
{
    /// <summary>
    /// Sets when to ignore the field/property
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class XPressIgnoreAttribute : Attribute
    {
        /// <summary>
        /// Sets when to ignore.
        /// </summary>
        /// <param name="ignoreMode">In what mode to ignore the value</param>
        public XPressIgnoreAttribute(XPressIgnoreMode ignoreMode = XPressIgnoreMode.NeverIncluded)
        {
            IgnoreMode = ignoreMode;
        }

        /// <summary>
        /// The ignore mode to assign to the property or field.
        /// </summary>
        public XPressIgnoreMode IgnoreMode { get; set; }

        /// <summary>
        /// Returns true if the ignore mode dose not include always.
        /// </summary>
        public bool IsIncluded { get { return (IgnoreMode & XPressIgnoreMode.NeverIncluded) != XPressIgnoreMode.NeverIncluded; } }
    }

    [Flags]
    public enum XPressIgnoreMode { IfNull = 1, IfDefualt = 2, NeverIncluded = 4, NeverIgnored = 8 }
}
