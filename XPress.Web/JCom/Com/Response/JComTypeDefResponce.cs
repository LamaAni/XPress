using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JCom.Com.Response
{
    /// <summary>
    /// Defines a command to the client side to declare a type.
    /// </summary>
    [XPressMemberSelection(XPressMemberSelectionType.Properties)]
    public class JComTypeDefResponce : Core.XPressResponseCommand
    {
        public JComTypeDefResponce(Map.JComTypeInfo info, Compilers.Compiler compiler)
            : base("JCOM_TypeDef", Core.CommandExecutionType.Invoke) // typedefs should always be executed immidiatly.
        {
            Name = info.Name;
            Code = compiler.CreateTypeDef(info);
        }

        #region members

        /// <summary>
        /// The code to create the typedef.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The name of the type.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}
