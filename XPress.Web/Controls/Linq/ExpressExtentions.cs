using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.Html;

namespace XPress.Web.Controls.Linq
{
    public static class ExpressExtentions
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
            if (c is IXPressControl)
                (c as IXPressControl).RequiresUpdate = true;
            else if (c.Parent != null)
            {
                // try and get the parent to update.
                c.Parent.UpdateClient();
            }
            return c;
        }

        /// <summary>
        /// Returns the client command $$($.FromId('[id]')). using the control's id.
        /// </summary>
        /// <typeparam name="T">The type of the control</typeparam>
        /// <param name="c">The control</param>
        /// <returns>the string command</returns>
        public static string ClientGetScript<T>(this T c)
            where T : HtmlElement
        {
            return "$$($.FromId('" + c.Id + "')";
        }

        /// <summary>
        /// Registers a responce command to be added to the control, via a unique id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static T RegisterResponseCommand<T>(this T c, string commandUniqueId, XPress.Web.Core.XPressResponseCommand cmnd)
            where T : IXPressControl
        {
            if (c.CallContext == null)
                throw new Exception("Cannot register responce in a non XPressTemplate source. (CallContext==null).");
            c.CallContext.Call.Response.CommandsByUniqueId[commandUniqueId] = cmnd;
            return c;
        }

        /// <summary>
        /// Deletes a responce command to be added to the control, via a unique id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static T ClearResponseCommand<T>(this T c, string commandUniqueId)
            where T : IXPressControl
        {
            if (c.CallContext == null)
                throw new Exception("Cannot register responce in a non XPressTemplate source. (CallContext==null).");
            c.CallContext.Call.Response.CommandsByUniqueId.TryRemove(commandUniqueId);
            return c;
        }

        /// <summary>
        /// Registers a responce command to be added to the control.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static T RegisterResponseCommand<T>(this T c, XPress.Web.Core.XPressResponseCommand cmnd)
            where T : IXPressControl
        {
            if (c.CallContext == null)
                throw new Exception("Cannot register responce in a non XPressTemplate source. (CallContext==null).");
            c.CallContext.Call.Response.Commands.Add(cmnd);
            return c;
        }
    }
}
