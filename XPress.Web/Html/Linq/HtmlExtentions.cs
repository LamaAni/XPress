using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace XPress.Web.Html.Linq
{
    public static class HtmlExtentions
    {
        /// <summary>
        /// Changes the tag name of the specified conteol.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T TagName<T>(this T q, string name)
            where T : IQuery
        {
            q.GetLinqEnumrable().First().TagName = name;
            return q;
        }

        /// <summary>
        /// Returns the parent of the first control of the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static HtmlElement Parent<T>(this T q)
            where T : IQuery
        {
            return q.GetLinqEnumrable().First().Parent;
        }

        /// <summary>
        /// Returns the attached page.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static HtmlTemplate Page<T>(this T q)
            where T : IQuery
        {
            return FindPage(q.GetLinqEnumrable().First());
        }

        static HtmlTemplate FindPage(HtmlElement c)
        {
            if (c is HtmlTemplate && (c as HtmlTemplate).AsPage)
                return c as HtmlTemplate;
            if (c.Parent == null)
                return null;
            return FindPage(c.Parent);
        }

        /// <summary>
        /// Returns the children collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Collections.ChildCollection Children<T>(this T q)
            where T:IQuery
        {
            return q.GetLinqEnumrable().First().Children;
        }


        /// <summary>
        /// Applies to any http context call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static HttpContext HttpContext<T>(this T c)
            where T : Html.HtmlElement
        {
            return System.Web.HttpContext.Current;
        }

        /// <summary>
        /// The active http session state if any. If context is null then returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static HttpSessionState Session<T>(this T c)
            where T : Html.HtmlElement
        {
            HttpContext context = c.HttpContext();
            return context == null ? null : context.Session;
        }

    }
}
