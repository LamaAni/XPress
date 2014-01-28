using XPress.Web.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.Html.Linq;

namespace XPress.Web.Controls.Linq
{
    public static class RemoteControlExtentions
    {
        /// <summary>
        /// Tells the server to update the client for the current control.
        /// </summary>
        /// <typeparam name="T">The type of the control to update</typeparam>
        /// <param name="c">The control to update.</param>
        /// <returns>The control to update.</returns>
        public static T UpdateClient<T>(this T c)
            where T : HtmlElement
        {
            if (c is IRemoteControl)
                (c as IRemoteControl).RequiresUpdate = true;
            else if (c.Parent != null)
            {
                // try and get the parent to update.
                c.Parent.UpdateClient();
            }
            return c;
        }
    }
}
