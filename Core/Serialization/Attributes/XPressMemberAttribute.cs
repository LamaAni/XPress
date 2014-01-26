using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Attributes
{
    /// <summary>
    /// Defines this value as a serializeable member of the current type.
    /// (Similar to DataMember)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class XPressMemberAttribute : Attribute
    {
        /// <summary>
        /// Defines this value as member of the current type.
        /// (Similar to DataMember)
        /// </summary>
        /// <param name="name"></param>
        public XPressMemberAttribute()
            :this(null)
        {
        }

        public XPressMemberAttribute(string name)
        {
            Name = name;
            IgnoreMode = XPressIgnoreMode.IfNull | XPressIgnoreMode.IfDefualt;
        }

        public string Name { get; set; }
        public bool IsRequired { get; set; }
        public int Order { get; set; }
        public XPressIgnoreMode IgnoreMode { get; set; }
    }
}
