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

        #region Tree commands

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
            return q.Invoke((on) => { on.Events.Trigger(sender, e, info); return true; }, once, dir);
        }

        #endregion
    }

}