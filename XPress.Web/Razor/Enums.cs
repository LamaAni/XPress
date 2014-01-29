using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Razor
{
    /// <summary>
    /// Flags that can be sent using the request, to determine behaviur on the serverside.
    /// </summary>
    [Flags]
    public enum XPressRazorRequestFlags
    {
        /// <summary>
        /// No flags
        /// </summary>
        None = 0,
        /// <summary>
        /// If exists, no session is loaded.
        /// </summary>
        NoSession = 1,
        /// <summary>
        /// If exists, all responses are assumed to come from xmlhttp, and therefore respond as json.
        /// </summary>
        RespondAsJson = 2,
    }
}
