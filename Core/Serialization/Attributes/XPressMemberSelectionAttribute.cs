using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Attributes
{
    internal interface IMembersSelectionAttribute
    {
        XPressMemberSelectionType Selection { get; set; }
    }

    /// <summary>
    /// Determines which members to select, if the OptIn is selected then only specifically defined
    /// members are added. Dose not affect the base class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class XPressInheritedMemberSelectionAttribute : Attribute, IMembersSelectionAttribute
    {
        /// <summary>
        /// Set the member selection type of the current, if the OptIn is selected then only specifically defined
        /// members are added. DOSE NOT AFFECT THE BASE CLASS!
        /// </summary>
        /// <param name="selection"></param>
        public XPressInheritedMemberSelectionAttribute(XPressMemberSelectionType selection = XPressMemberSelectionType.Properties)
        {
            Selection = selection;
        }

        /// <summary>
        /// The selection type for the current members.
        /// </summary>
        public XPressMemberSelectionType Selection { get; set; }
    }

    /// <summary>
    /// Set the member selection type of the current, if the OptIn is selected then only specifically defined
    /// members are added. Affectes only the declared class (is not inherited by derived classes). Trumps any decleration
    /// of XPressMemberSelectionAttribute. Use XPressInheritedMemberSelectionAttribute for inherited member selection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class XPressMemberSelectionAttribute : Attribute, IMembersSelectionAttribute
    {
        /// <summary>
        /// Set the member selection type of the current, if the OptIn is selected then only specifically defined
        /// members are added. Affectes only the declared class (is not inherited by derived classes). Trumps any decleration
        /// of XPressMemberSelectionAttribute.
        /// </summary>
        /// <param name="selection"></param>
        public XPressMemberSelectionAttribute(XPressMemberSelectionType selection = XPressMemberSelectionType.Properties)
        {
            Selection = selection;
        }

        /// <summary>
        /// The selection type for the current members.
        /// </summary>
        public XPressMemberSelectionType Selection { get; set; }
    }

    /// <summary>
    /// Determiens which members to select.
    /// </summary>
    [Flags]
    public enum XPressMemberSelectionType { Fields = 1, Properties = 2, ReadOnlyProperties = 4, OptIn = 8 }
}
