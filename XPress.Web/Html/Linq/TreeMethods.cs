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
        public static T Invoke<T>(this T q, Func<HtmlElement, bool?> action, bool once = true, BubbleDirection dir = BubbleDirection.ToParent)
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

        static void InvokeUp(HtmlElement c, Func<HtmlElement, bool?> action, HashSet<HtmlElement> invoked, bool noRepeats)
        {
            // invoking up the parent list.
            if (noRepeats)
            {
                if (invoked.Contains(c))
                    return;
                invoked.Add(c);
            }

            bool bubble = action(c) != false;

            if (bubble && c.Parent() != null)
                InvokeUp(c.Parent(), action, invoked, noRepeats);
        }

        static void InvokeDown(HtmlElement c, Func<HtmlElement, bool?> action, HashSet<HtmlElement> invoked, bool noRepeats)
        {
            if (noRepeats)
            {
                if (invoked.Contains(c))
                    return;
                invoked.Add(c);
            }

            bool bubble = action(c) != false;

            if (bubble && c.HasChildren)
                c.Children.ForEach(child => InvokeDown(child, action, invoked, noRepeats));
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
}
