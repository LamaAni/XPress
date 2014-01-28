using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html.Linq
{
    public static class AttributeExtentions
    {
        #region Attribute set and get.

        /// <summary>
        /// Gets the value of the attribute for the first element in the query.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="name"></param>
        /// <returns>The query unmodified</returns>
        public static string Attr<T>(this T q, string name)
            where T : IQuery
        {
            if (q.GetLinqEnumrable().IsEmpty())
                return null;
            return q.GetLinqEnumrable().First().Attributes[name];
        }

        /// <summary>
        /// Sets the value of the attribute for all the items in the query.
        /// </summary>
        /// <param name="q">The query</param>
        /// <param name="name">The attribute name</param>
        /// <param name="value">The attribute value</param>
        /// <returns>The query unmodified</returns>
        public static T Attr<T>(this T q, string name, string value)
            where T : IQuery
        {
            q.GetLinqEnumrable().ForEach(c => c.Attributes[name] = value);
            return q;
        }

        /// <summary>
        /// Sets one or more attributes to all the itmes in the query.
        /// </summary>
        /// <param name="q">The query</param>
        /// <param name="args">A list of groups of two arguments to set the atribute values. For example - Attr([attr name],[value],[attr name],[value]...)</param>
        /// <returns>The query unmodified</returns>
        /// <example>"Attr([attr name],[value],[attr name],[value]...)"</example>
        public static T Attr<T>(this T q, params string[] args)
            where T : IQuery
        {
            if (args.Length * 1.0 % 2 != 0)
                throw new Exception("Attr params can only be called with multiples of two. i.e. [a],[b]...[a],[b]");

            for (int i = 0; i < args.Length; i += 2)
            {
                q.Attr(args[i], args[i + 1]);
            }

            return q;
        }

        #endregion

        #region specific attributes

        public static string Class<T>(this T q)
            where T : IQuery
        {
            return q.Attr("class");
        }

        public static T Class<T>(this T q, string name)
            where T : IQuery
        {
            return q.Attr("class", name);
        }

        #endregion
    }
}

