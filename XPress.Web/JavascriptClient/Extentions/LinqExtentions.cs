using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace XPress.Web.JavascriptClient.Extentions
{
    /// <summary>
    /// Collection of linq methods that textend JTemplate.
    /// </summary>
    public static class LinqExtentions
    {
        /// <summary>
        /// Returns the active JCLientCall. Only true if the current call was made through a JClientTemplate page
        /// or the JClientCallContext was set manually.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static JavascriptClient.JClientCallContext CallContext<T>(this T c)
            where T : Html.HtmlElement
        {
            return JavascriptClient.JClientCallContext.Current;
        }

        /// <summary>
        /// Adds a responce command to retrun to the client side.
        /// </summary>
        /// <param name="c">Root control.</param>
        /// <param name="cmnd">The command to add.</param>
        /// <returns></returns>
        public static T AddResponceCommand<T>(this T c, Core.XPressResponseCommand cmnd)
            where T : JavascriptClient.JClientCallContext
        {
            c.Call.Response.Commands.Add(cmnd);
            return c;
        }
    }
}
