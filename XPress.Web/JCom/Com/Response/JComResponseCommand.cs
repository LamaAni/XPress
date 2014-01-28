using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JCom.Com.Response
{
    /// <summary>
    /// Abstract class that represent the jcom command.
    /// </summary>
    public abstract class JComResponseCommand : Core.XPressResponseCommand
    {
        /// <summary>
        /// Build a jcom responce command, to be sent to the client for execution.
        /// </summary>
        /// <param name="execType">The type of execution to run on the client.</param>
        public JComResponseCommand(JComCommandType commandType, Core.CommandExecutionType execType = Core.CommandExecutionType.Post)
            : base("JCom", execType)
        {
            CommandType = commandType;
        }

        /// <summary>
        /// The command type associated with the responce.
        /// </summary>
        public JComCommandType CommandType { get; private set; }
    }

}
