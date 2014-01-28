using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JavascriptClient.Request
{
    /// <summary>
    /// Implements a jclient request command
    /// </summary>
    public abstract class JClientRequestCommand :Core.XPressRequestCommand
    {
        /// <summary>
        /// Creates a jclient request command.
        /// </summary>
        /// <param name="call"></param>
        public JClientRequestCommand()
        {
        }

        public JClientCall Call { get { return JClientCallContext.Current.Call; } }
    }
}
