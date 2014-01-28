using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JavascriptClient.Response
{
    /// <summary>
    /// The command response to a beat command to the client.
    /// </summary>
    public enum BeatCommandEnum
    {
        /// <summary>
        /// All ok nothing to do.
        /// </summary>
        OK,
        /// <summary>
        /// Somehting changed on server side, a command is waiting.
        /// </summary>
        Changed,
        /// <summary>
        /// The server side dose not recognise the client side and therefore it is disconnected.
        /// </summary>
        Dead,
    }
}
