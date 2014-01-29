using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Core
{
    /// <summary>
    /// The general response command object to be handed by the XPressCore commands collection.
    /// </summary>
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public abstract class XPressResponseCommand
    {
        public XPressResponseCommand(string type, CommandExecutionType cmndType)
        {
            Priority = 100;
            Type = type;
            ExecutionType = cmndType;
        }

        [XPressMember("_t", IgnoreMode = XPressIgnoreMode.IfNull)]
        /// <summary>
        /// The type of the command
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// The priority in which to order the commands.
        /// </summary>
        [XPressIgnore]
        public virtual int Priority { get; set; }

        /// <summary>
        /// The execution type that determines the behaviur on the client side. Default is Post.
        /// </summary>
        [XPressMember("_et", IgnoreMode = XPressIgnoreMode.IfNull)]
        public virtual CommandExecutionType ExecutionType { get; set; }
    }

    public enum CommandExecutionType { 
        /// <summary>
        /// Posted commands must wait for the all pending comman validations to compleate.
        /// </summary>
        Post, 
        /// <summary>
        /// Pend commands return validations that must be compleated before any posted commands
        /// </summary>
        Pend, 
        /// <summary>
        /// Invoked commands are called before the pend commands and have a specific internal order.
        /// </summary>
        Invoke,
    }
}
