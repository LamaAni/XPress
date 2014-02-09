using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.Html.Events;
using XPress.Web.Html;

namespace XPress.Web.Html.Linq
{
    public static class EventExtentions
    {
        #region Event registration and removal

        /// <summary>
        /// Binds an event to the control.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="eventName">The event to bind</param>
        /// <param name="action">The action to take</param>
        /// <returns></returns>
        public static T Bind<T>(this T q, EventDefaults eventName, Action<object, EventArgs> action)
            where T : HtmlElement
        {
            return q.Bind(new EventInfo(eventName), action);
        }

        /// <summary>
        /// Binds an event to the control.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="eventName">The event to bind</param>
        /// <param name="action">The action to take</param>
        /// <returns></returns>
        public static T Bind<T>(this T q, string eventName, Action<object, EventArgs> action)
             where T : HtmlElement
        {
            return q.Bind(new EventInfo(eventName), action);
        }

        /// <summary>
        /// Binds an event to the controls.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="info">The event to bind</param>
        /// <param name="action">The action to take</param>
        /// <returns></returns>
        public static T Bind<T>(this T q, EventInfo info, Action<object, EventArgs> action)
            where T : HtmlElement
        {
            foreach (HtmlElement c in q.GetLinqEnumrable())
                c.Events.Bind(info, action);
            return q;
        }

        /// <summary>
        /// Unbinds an event form the controls.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="eventName">The event name with namespace. If the namespace exists only events with that namespace will be unbound.</param>
        /// <returns></returns>
        public static T UnBind<T>(this T q, EventDefaults eventName)
            where T : HtmlElement
        {
            return q.UnBind(new EventInfo(eventName));
        }

        /// <summary>
        /// Unbinds an event form the controls.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="eventName">The event name with namespace. If the namespace exists only events with that namespace will be unbound.</param>
        /// <returns></returns>
        public static T UnBind<T>(this T q, string eventName)
            where T : HtmlElement
        {
            return q.UnBind(new EventInfo(eventName));
        }

        /// <summary>
        /// Unbinds an event form the controls.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="info">The event name with namespace. If the namespace exists only events with that namespace will be unbound.</param>
        /// <returns></returns>
        public static T UnBind<T>(this T q, EventInfo info)
            where T : HtmlElement
        {
            foreach (HtmlElement c in q.GetLinqEnumrable())
            { 
                c.Events.UnBind(info);
            }

            return q;
        }

        #endregion

        #region client events

        /// <summary>
        /// The event to run on the client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="eventName"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static T OnClientEvent<T>(this T c, string eventName, string code)
            where T : IQuery
        {
            eventName = eventName.Trim().ToLower();
            eventName = eventName.StartsWith("on") ? eventName : "on" + eventName;
            c.GetLinqEnumrable().ForEach(e =>
                {
                    if (e.HasAttributes && e.Attributes[eventName] != null)
                        e.Attributes[eventName] += ";" + code;
                    else e.Attributes[eventName] = code;
                });
            return c;
        }

        /// <summary>
        /// The event to run on the client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="eventName"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static T OnClientEvent<T>(this T c, ClientHtmlEvents ev, string code)
            where T : IQuery
        {
            return c.OnClientEvent(c.ToString(), code);
        }

        #endregion

        #region event invocation

        /// <summary>
        /// Calls a specific event on the query.
        /// </summary>
        /// <param name="q">The query</param>
        /// <param name="sender">The sender</param>
        /// <param name="eventName">The event name</param>
        /// <returns>The query</returns>
        public static T Trigger<T>(this T q, object sender, EventDefaults eventName, EventArgs e = null)
            where T : HtmlElement
        {
            return q.Trigger(sender,new EventInfo(eventName), e);
        }

                /// <summary>
        /// Calls a specific event on the query.
        /// </summary>
        /// <param name="q">The query</param>
        /// <param name="sender">The sender</param>
        /// <param name="eventName">The event name</param>
        /// <returns>The query</returns>
        public static T Trigger<T>(this T q, object sender, string eventName, EventArgs e = null)
            where T : HtmlElement
        {
            return q.Trigger(sender, new EventInfo(eventName), e);
        }

        /// <summary>
        /// Calls a specific event on the query.
        /// </summary>
        /// <param name="q">The query</param>
        /// <param name="sender">The sender</param>
        /// <param name="eventName">The event name</param>
        /// <returns>The query</returns>
        public static T Trigger<T>(this T q, object sender, EventInfo info, EventArgs e = null)
            where T : HtmlElement
        {
            q.GetLinqEnumrable().ForEach(c =>
            {
                c.Events.Trigger(sender, e, info);
            });
            return q;
        }

        #endregion

        #region Events tree commands

        /// <summary>
        /// Invokes all the controls in the current invocation list. All controls are only invoked once.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="once">If true all controls are only invoked once.</param>
        /// <param name="dir">The direction for which to invoke</param>
        /// <param name="e">The event args</param>
        /// <param name="sender">The event sender</param>
        /// <returns></returns>
        public static T Invoke<T>(this T q, object sender, EventDefaults eventName, bool once = true, BubbleDirection dir = BubbleDirection.ToParent)
             where T : HtmlElement
        {
            return q.Invoke(sender, null, eventName, once, dir);
        }

        /// <summary>
        /// Invokes all the controls in the current invocation list. All controls are only invoked once.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="once">If true all controls are only invoked once.</param>
        /// <param name="dir">The direction for which to invoke</param>
        /// <param name="e">The event args</param>
        /// <param name="sender">The event sender</param>
        /// <returns></returns>
        public static T Invoke<T>(this T q, object sender, string eventName, bool once = true, BubbleDirection dir = BubbleDirection.ToParent)
             where T : HtmlElement
        {
            return q.Invoke(sender, null, eventName, once, dir);
        }

        /// <summary>
        /// Invokes all the controls in the current invocation list. All controls are only invoked once.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="once">If true all controls are only invoked once.</param>
        /// <param name="dir">The direction for which to invoke</param>
        /// <param name="e">The event args</param>
        /// <param name="sender">The event sender</param>
        /// <returns></returns>
        public static T Invoke<T>(this T q, object sender, EventArgs e, EventDefaults eventName, bool once = true, BubbleDirection dir = BubbleDirection.ToParent)
             where T : HtmlElement
        {
            return q.Invoke(sender, e, new EventInfo(eventName.ToString()), once, dir);
        }

        /// <summary>
        /// Invokes all the controls in the current invocation list. All controls are only invoked once.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="once">If true all controls are only invoked once.</param>
        /// <param name="dir">The direction for which to invoke</param>
        /// <param name="e">The event args</param>
        /// <param name="sender">The event sender</param>
        /// <returns></returns>
        public static T Invoke<T>(this T q, object sender, EventArgs e, string eventName, bool once = true, BubbleDirection dir = BubbleDirection.ToParent)
             where T : HtmlElement
        {
            return q.Invoke(sender, e, new EventInfo(eventName), once, dir);
        }

        /// <summary>
        /// Invokes all the controls in the current invocation list. All controls are only invoked once.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="info">The event to invoke.</param>
        /// <param name="once">If true all controls are only invoked once.</param>
        /// <param name="dir">The direction for which to invoke</param>
        /// <param name="e">The event args</param>
        /// <param name="sender">The event sender</param>
        /// <returns></returns>
        public static T Invoke<T>(this T q, object sender, EventArgs e, EventInfo info, bool once = true, BubbleDirection dir = BubbleDirection.ToParent)
             where T : HtmlElement
        {
            return q.Invoke((on) => { on.Events.Trigger(sender, e, info); }, once, dir);
        }

        #endregion
    }

    public enum ClientHtmlEvents
    {
        /// <summary>
        /// Fires the moment that the element loses focus
        /// </summery>
        onblur,
        /// <summary>
        /// Fires the moment when the value of the element is changed
        /// </summery>
        onchange,
        /// <summary>
        /// Script to be run when a context menu is triggered
        /// </summery>
        oncontextmenuNew,
        /// <summary>
        /// Fires the moment when the element gets focus
        /// </summery>
        onfocus,
        /// <summary>
        /// Script to be run when a form changes
        /// </summery>
        onformchangeNew,
        /// <summary>
        /// Script to be run when a form gets user input
        /// </summery>
        onforminputNew,
        /// <summary>
        /// Script to be run when an element gets user input
        /// </summery>
        oninputNew,
        /// <summary>
        /// Script to be run when an element is invalid
        /// </summery>
        oninvalidNew,
        /// <summary>
        /// Fires when the Reset button in a form is clicked
        /// </summery>
        onreset,
        /// <summary>
        /// Fires after some text has been selected in an element, Not supported in HTML5
        /// </summery>
        onselect,
        /// <summary>
        /// Fires when a form is submitted, Not supported in HTML5
        /// </summery>
        onsubmit,
        /// <summary>
        /// Fires on a mouse click on the element
        /// </summery>
        onclick,
        /// <summary>
        /// Fires on a mouse double-click on the element
        /// </summery>
        ondblclick,
        /// <summary>
        /// Script to be run when an element is dragged
        /// </summery>
        ondragNew,
        /// <summary>
        /// Script to be run at the end of a drag operation
        /// </summery>
        ondragendNew,
        /// <summary>
        /// Script to be run when an element has been dragged to a valid drop target
        /// </summery>
        ondragenterNew,
        /// <summary>
        /// Script to be run when an element leaves a valid drop target
        /// </summery>
        ondragleaveNew,
        /// <summary>
        /// Script to be run when an element is being dragged over a valid drop target
        /// </summery>
        ondragoverNew,
        /// <summary>
        /// Script to be run at the start of a drag operation
        /// </summery>
        ondragstartNew,
        /// <summary>
        /// Script to be run when dragged element is being dropped
        /// </summery>
        ondropNew,
        /// <summary>
        /// Fires when a mouse button is pressed down on an element
        /// </summery>
        onmousedown,
        /// <summary>
        /// Fires when the mouse pointer moves over an element
        /// </summery>
        onmousemove,
        /// <summary>
        /// Fires when the mouse pointer moves out of an element
        /// </summery>
        onmouseout,
        /// <summary>
        /// Fires when the mouse pointer moves over an element
        /// </summery>
        onmouseover,
        /// <summary>
        /// Fires when a mouse button is released over an element
        /// </summery>
        onmouseup,
        /// <summary>
        /// Script to be run when the mouse wheel is being rotated
        /// </summery>
        onmousewheelNew,
        /// <summary>
        /// Script to be run when an element's scrollbar is being scrolled
        /// </summery>
        onscrollNew,
        /// <summary>
        /// Fires when a user is pressing a key
        /// </summery>
        onkeydown,
        /// <summary>
        /// Fires when a user presses a key
        /// </summery>
        onkeypress,
        /// <summary>
        /// Fires when a user releases a key
        /// </summery>
        onkeyup,
        /// <summary>
        /// Script to be run after the document is printed
        /// </summery>
        onafterprintNew,
        /// <summary>
        /// Script to be run before the document is printed
        /// </summery>
        onbeforeprintNew,
        /// <summary>
        /// Script to be run before the document is unloaded
        /// </summery>
        onbeforeunloadNew,
        /// <summary>
        /// Script to be run when an error occur
        /// </summery>
        onerrorNew,
        /// <summary>
        /// Script to be run when the document has changed
        /// </summery>
        onhaschangeNew,
        /// <summary>
        /// Fires after the page is finished loading
        /// </summery>
        onload,
        /// <summary>
        /// Script to be run when the message is triggered
        /// </summery>
        onmessageNew,
        /// <summary>
        /// Script to be run when the document goes offline
        /// </summery>
        onofflineNew,
        /// <summary>
        /// Script to be run when the document comes online
        /// </summery>
        ononlineNew,
        /// <summary>
        /// Script to be run when the window is hidden
        /// </summery>
        onpagehideNew,
        /// <summary>
        /// Script to be run when the window becomes visible
        /// </summery>
        onpageshowNew,
        /// <summary>
        /// Script to be run when the window's history changes
        /// </summery>
        onpopstateNew,
        /// <summary>
        /// Script to be run when the document performs a redo
        /// </summery>
        onredoNew,
        /// <summary>
        /// Fires when the browser window is resized
        /// </summery>
        onresizeNew,
        /// <summary>
        /// Script to be run when a Web Storage area is updated
        /// </summery>
        onstorageNew,
        /// <summary>
        /// Script to be run when the document performs an undo
        /// </summery>
        onundoNew,
        /// <summary>
        /// Fires once a page has unloaded (or the browser window has been closed)
        /// </summery>
        onunload,
    }

}