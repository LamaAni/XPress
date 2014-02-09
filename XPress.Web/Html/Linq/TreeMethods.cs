using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html.Linq
{

    public static class TreeMethods
    {
        /// <summary>
        /// Invokes a function along the child tree.
        /// </summary>
        /// <param name="q">The query to invoke on</param>
        /// <param name="action">the action to take</param>
        /// <param name="once">Can a function be called more then once?</param>
        /// <param name="dir">The direction of the bubbling</param>
        /// <returns>The query</returns>
        public static T Invoke<T>(this T q, Action<HtmlElement> action, bool once = true, BubbleDirection dir = BubbleDirection.ToParent)
             where T : IQuery
        {
            return q.Invoke<T>((el) =>
            {
                action(el);
                return BubbleContinueMode.Continue;
            }, once, dir);
        }

        /// <summary>
        /// Invokes a function along the child tree.
        /// </summary>
        /// <param name="q">The query to invoke on</param>
        /// <param name="action">the action to take</param>
        /// <param name="once">Can a function be called more then once?</param>
        /// <param name="dir">The direction of the bubbling</param>
        /// <returns>The query</returns>
        public static T Invoke<T>(this T q, Func<HtmlElement, BubbleContinueMode> action, bool once = true, BubbleDirection dir = BubbleDirection.ToParent)
             where T : IQuery
        {
            HashSet<HtmlElement> invoked = null;
            if (once)
                invoked = new HashSet<HtmlElement>();

            // going through the query and invoking controls
            foreach (HtmlElement c in q.GetLinqEnumrable())
                if (dir == BubbleDirection.ToParent)
                    InvokeUp(c, action, invoked, once);
                else InvokeDown(c, action, invoked, once);

            return q;
        }

        static void InvokeUp(HtmlElement c, Func<HtmlElement, BubbleContinueMode> action, HashSet<HtmlElement> invoked, bool noRepeats)
        {
            // invoking up the parent list.
            if (noRepeats)
            {
                if (invoked.Contains(c))
                    return;
                invoked.Add(c);
            }

            bool stopBubble = action(c) != BubbleContinueMode.Continue;

            if (!stopBubble && c.Parent() != null)
                InvokeUp(c.Parent(), action, invoked, noRepeats);
        }

        static BubbleContinueMode InvokeDown(HtmlElement c, Func<HtmlElement, BubbleContinueMode> action, HashSet<HtmlElement> invoked, bool noRepeats)
        {
            if (noRepeats)
            {
                if (invoked.Contains(c))
                    return BubbleContinueMode.Continue;
                invoked.Add(c);
            }

            BubbleContinueMode bubble = action(c);

            if (bubble == BubbleContinueMode.Continue && c.HasChildren)
            {
                foreach (HtmlElement child in c.Children)
                {
                    BubbleContinueMode mode = InvokeDown(child, action, invoked, noRepeats);
                    if (mode == BubbleContinueMode.Stop)
                    {
                        bubble = BubbleContinueMode.Stop;
                        break;
                    }
                }
            }

            return bubble;
        }

        /// <summary>
        /// Returns all the controls identified by predict.
        /// </summary>
        /// <typeparam name="T">The element to search for</typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IQuery Select<T>(this T c, Func<HtmlElement, bool> predict, BubbleDirection direction = BubbleDirection.ToChildren)
            where T : IQuery
        {
            List<HtmlElement> els = new List<HtmlElement>();
            c.Invoke((el) =>
            {
                if (predict(el))
                    els.Add(el);
            }, true, direction);
            return new HtmlElementQuery(els);
        }

        /// <summary>
        /// Finds the first control that matches predict.
        /// </summary>
        /// <typeparam name="T">The type of the calling control</typeparam>
        /// <param name="c">The control</param>
        /// <param name="predict">predict if true.</param>
        /// <param name="direction">The direction of the bubble.</param>
        /// <returns></returns>
        public static HtmlElement Find<T>(this T c, Func<HtmlElement, bool> predict, BubbleDirection direction = BubbleDirection.ToChildren)
             where T : IQuery
        {
            HtmlElement found = null;
            c.Invoke((el) =>
            {
                if (predict(el))
                {
                    found = el;
                    return BubbleContinueMode.Stop;
                }
                return BubbleContinueMode.Continue;
            }, true, direction);
            return found;
        }
    }


    /// <summary>
    /// The direction of the bubbling.
    /// </summary>
    public enum BubbleDirection
    {
        ToParent = 0,
        ToChildren = 1
    }

    public enum BubbleContinueMode
    {
        /// <summary>
        /// Stops the bubble
        /// </summary>
        Stop,
        /// <summary>
        /// Holds the bubble from continuing to child controls, in the case of BubbleDirection = ToParent, stops the bubble.
        /// </summary>
        DontContinueToChildren,
        /// <summary>
        /// Allows the bubble to continue.
        /// </summary>
        Continue,
    }
}
