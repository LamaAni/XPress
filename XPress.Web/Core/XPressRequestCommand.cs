using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Core
{
    /// <summary>
    /// Command that was sent from the client to the server, usually using jcom.
    /// </summary>
    public abstract class XPressRequestCommand
    {
        /// <summary>
        /// The command
        /// </summary>
        public XPressRequestCommand()
        {
            Priority = 100;
        }

        [XPressMember("_t", IgnoreMode = XPressIgnoreMode.IfNull)]
        /// <summary>
        /// The type of the command.
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// The priority in which to order the commands.
        /// </summary>
        [XPressMember("_priority", IgnoreMode = XPressIgnoreMode.IfNull)]
        public virtual int Priority { get; set; }

        /// <summary>
        /// Executed command on the client side.
        /// </summary>
        public abstract void ExecuteCommand();

        /// <summary>
        /// The responce object to request. This object will be sent back as part of the responce command.
        /// </summary>
        [XPressMember(IgnoreMode= XPressIgnoreMode.IfNull)]
        public object ResponseValue { get; set; }

        /// <summary>
        /// The command id associated with the command.
        /// </summary>
        [XPressMember("_cmndid")]
        public ulong CommandId { get; private set; }
    }
}
