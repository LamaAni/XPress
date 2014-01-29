using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JavascriptClient
{
    /// <summary>
    /// Represents the request type as read by the XPressRazorHttpHandler.
    /// </summary>
    public enum JClientRequestType
    {
        /// <summary>
        /// A page request, dose not hold internal data for commands.
        /// </summary>
        Page,
        /// <summary>
        /// Json request, commands are arranged in json form.
        /// </summary>
        Json,
        /// <summary>
        /// HartBeat request (internal).
        /// </summary>
        Beat,
        /// <summary>
        /// Means that the client on the serverside waits for response of the last command.
        /// There is a current command being executed and therefore the response will be loaded into the client until such time that that response could 
        /// be returned to the client side.
        /// </summary>
        WaitForResponse,
        /// <summary>
        /// Means that the client has closed on the client side.
        /// </summary>
        DestroyClient,
    }
}
