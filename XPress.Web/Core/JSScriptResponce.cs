using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Core
{
    /// <summary>
    /// A command to execute a script on the client side.
    /// </summary>
    public class JSScriptResponce : Core.XPressResponseCommand
    {
        public JSScriptResponce(string code, CommandExecutionType type = CommandExecutionType.Post)
            : base("script", type)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
