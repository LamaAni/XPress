using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JavascriptClient.Response
{
    public class JClientSystemCommand : Core.XPressResponseCommand
    {
        public JClientSystemCommand(JClientSystemResponseCommandEnum cmnd)
            : base("System", Core.CommandExecutionType.Invoke)
        {
            Command = cmnd.ToString();
        }

        public string Command { get; private set; }
    }

    public enum JClientSystemResponseCommandEnum { Disconnect, Obsolete, WaitForResponse };
}
