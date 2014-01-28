using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JCom.Com.Response
{
    /// <summary>
    /// Registers a command to build a client side object in JCom form. (Default Priority 50).
    /// </summary>
    public class JComBuildObjectResponse : Core.XPressResponseCommand
    {
        /// <summary>
        /// Builds a new object on the client side.
        /// </summary>
        /// <param name="anchor">If true, this object will be anchored to the client side memory until manually deleted.</param>
        /// <param name="code">The code to be executed as the build command.</param>
        /// <param name="oid">The object id (of the object that needs building).</param>
        /// <param name="overwriteIfExists">If true, the old object should be deleted and a new object created in its place.</param>
        public JComBuildObjectResponse(object o, ulong oid, bool overwriteIfExists, Map.JComTypeInfo info, Compilers.Compiler compiler)
            : base("JCOM_Construct", Core.CommandExecutionType.Post)
        {
            Priority = 50;
            OverwriteIfExists = overwriteIfExists;
            Data = compiler.GetObjectData(info, o);
            TypeId = info.Name;
            OId = oid;
        }

        /// <summary>
        /// If true, this object will be anchored to the client side memory until manually deleted
        /// </summary>
        [DefaultValue(false)]
        [XPressIgnore(XPressIgnoreMode.IfDefualt)]
        public bool Anchor { get; private set; }

        /// <summary>
        /// Overwrite this object it it exists.
        /// </summary>
        [DefaultValue(false)]
        [XPressIgnore(XPressIgnoreMode.IfDefualt)]
        public bool OverwriteIfExists { get; private set; }

        /// <summary>
        /// The object that represents the client side data members.
        /// </summary>
        public object Data { get; private set; }

        /// <summary>
        /// The type of the member to generate.
        /// </summary>
        public string TypeId { get; private set; }

        /// <summary>
        /// The object id associated with the object.
        /// </summary>
        public ulong OId { get; private set; }
    }
}
