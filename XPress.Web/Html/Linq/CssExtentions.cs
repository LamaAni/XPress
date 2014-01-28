using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPress.Web.Html.Linq
{
    public static class CssExtentions
    {
        /// <summary>
        /// Gets the css style value of the first element.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="name"></param>
        /// <returns>The query unmodified</returns>
        public static string Css<T>(this T q, string name)
            where T : IQuery
        {
            if (q.GetLinqEnumrable().IsEmpty())
                return null;
            return q.GetLinqEnumrable().First().Style[name];
        }

        /// <summary>
        /// Sets the css styles and values for the current query.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>The query unmodified</returns>
        public static T Css<T>(this T q, string name, string value)
            where T : IQuery
        {
            q.GetLinqEnumrable().ForEach(c => c.Style[name] = value);
            return q;
        }

        /// <summary>
        /// Sets the css styles and values for the current query.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="args">A list of groups of two arguments to set the css values. For example "Css([name],[value],[name],[value]...)"</param>
        /// <returns>The query unmodified</returns>
        public static T Css<T>(this T q, params string[] args)
            where T : IQuery
        {
            if (args.Length * 1.0 % 2 != 0)
                throw new Exception("Css params can only be called with multiples of two.");

            for (int i = 0; i < args.Length; i += 2)
            {
                q.Css(args[i], args[i + 1]);
            }

            return q;
        }

        /// <summary>
        /// Toggles css classes that are dependent on the query state. The css classes are allowed
        /// </summary>
        /// <param name="ex">The query extender to add from </param>
        /// <param name="classes">The class names to add</param>
        /// <param name="add">If null, then adds if dose not exist and remove when exists. Otherwise add=true -> only adds, exc.</param>
        /// <returns>The query</returns>
        public static T ToggleClass<T>(this CssExtneder<T> ex, string classes, bool? add = null)
            where T : IQuery
        {
            HashSet<string> newClassNames = new HashSet<string>(classes.Split(' ').Select(c => c.Trim()));
            HashSet<string> classNames = new HashSet<string>(ex.Class().Split(' ').Select(c => c.Trim()));
            newClassNames.ForEach(n =>
            {
                if (add == true || add == null)
                {
                    if (!classNames.Contains(n))
                        classNames.Add(n);
                }
                if (add == false || add == null)
                {
                    if (classNames.Contains(n))
                        classNames.Remove(n);
                }
            });
            ex.Class(string.Join(" ", classNames));

            return ex.Query;
        }

        /// <summary>
        /// Set the class sttribute
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public static T Class<T>(this CssExtneder<T> ex, string cssClass)
            where T : IQuery
        {
            return ex.Query.Attr("class", cssClass);
        }

        /// <summary>
        /// Gets the class attribute
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string Class<T>(this CssExtneder<T> ex)
            where T : IQuery
        {
            return ex.Query.Attr("class");
        }

        /// <summary>
        /// Adds the classes to the class attribute
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="classes"></param>
        /// <returns></returns>
        public static T AddClass<T>(this CssExtneder<T> ex, string classes)
            where T : IQuery
        {
            return ex.ToggleClass(classes, true);
        }

        /// <summary>
        /// Removes class from the class attribute.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="classes"></param>
        /// <returns></returns>
        public static T RemoveClass<T>(this CssExtneder<T> ex, string classes)
            where T : IQuery
        {
            return ex.ToggleClass(classes, false);
        }

        /// <summary>
        /// Returns true if the class are in the class attribute.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="classes"></param>
        /// <param name="all">If true then demans that all the classes are in the class attribute.</param>
        /// <returns></returns>
        public static bool HasClass<T>(this CssExtneder<T> ex, string classes, bool all = true)
            where T : IQuery
        {
            string[] names = ex.Class().Split(' ').Select(c => c.Trim()).ToArray();
            IEnumerable<string> intersected = names.Intersect(classes.Split(' ').Select(c => c.Trim()));
            return all ? intersected.Count() == names.Length : intersected.Count() > 0;
        }
    }

}