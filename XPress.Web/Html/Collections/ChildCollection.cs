using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPress.Web.Html.Collections
{
    /// <summary>
    /// Implements a controls collection that is attached to a control.
    /// </summary>
    public class ChildCollection : IEnumerable<HtmlElement>
    {
        public ChildCollection(HtmlElement element, IEnumerable<HtmlElement> kids = null)
        {
            Kids = kids == null ? new List<HtmlElement>() : new List<HtmlElement>(kids);
            Element = element;
        }

        #region Members

        protected List<HtmlElement> Kids { get; private set; }

        public HtmlElement Element { get; private set; }

        public int Count { get { return Kids.Count; } }

        #endregion

        #region Adding methods

        /// <summary>
        /// Adds the control to the collection. Calls the function "BindToParent" on the control.
        /// </summary>
        /// <param name="control">The control to insert</param>
        /// <param name="index">Add at index. -1 means append to end.</param>
        /// <returns>True if the control was removed from another parent</returns>
        public bool Insert(int index, HtmlElement ctrl)
        {
            bool hadParent = ctrl.BindToParent(Element);
            if (ctrl.Parent != Element)
            {
                throw new Exception("The bind command did not bind the Collection's control as the parent of ctrl.");
            }
            if (index < 0)
                Kids.Add(ctrl);
            else Kids.Insert(index, ctrl);
            return hadParent;
        }

        /// <summary>
        /// Adds the control to the collection. Calls the function "Bind" on the control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns>True if the control was removed from another parent</returns>
        public bool Append(HtmlElement ctrl)
        {
            return Insert(-1, ctrl);
        }

        /// <summary>
        /// Removed the current control from the collection.
        /// </summary>
        /// <param name="ctrl"></param>
        public bool Remove(HtmlElement ctrl)
        {
            int i = 0;
            foreach (HtmlElement c in Kids)
            {
                if (object.ReferenceEquals(c, ctrl))
                {
                    RemoveAt(i);
                    return true;
                }
                i += 1;
            }
            return false;
        }

        /// <summary>
        /// Removes the contorl at the specified index.
        /// </summary>
        /// <param name="index"></param>
        public bool RemoveAt(int index)
        {
            if (Kids.Count <= index || index < 0)
                return false;
            Kids.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Retuens the last control in the list.
        /// </summary>
        /// <returns></returns>
        public HtmlElement Last()
        {
            return Kids.Count == 0 ? null : Kids[Kids.Count - 1];
        }

        /// <summary>
        ///  Clears the control collection.
        /// </summary>
        public void Clear()
        {
            Kids.Clear();
        }

        #endregion

        #region IEnumerable<IHtmlElement> Members

        public IEnumerator<HtmlElement> GetEnumerator()
        {
            return Kids.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
