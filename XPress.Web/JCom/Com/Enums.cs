using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JCom.Com
{
    /// <summary>
    /// The command type to execute
    /// </summary>
    public enum JComCommandType
    {
        /// <summary>
        /// The default command, this is a null value.
        /// </summary>
        Unknown,
        /// <summary>
        /// Gets the data value from the server and sends it back to the client.
        /// </summary>
        Get,
        /// <summary>
        /// Updates the data value sent from the client.
        /// </summary>
        Set,
        /// <summary>
        /// Inokes a method on the server and returns the invocation command value.
        /// </summary>
        Invoke
    }
}
