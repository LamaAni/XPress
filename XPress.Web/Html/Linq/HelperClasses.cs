using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html.Linq
{
    public static class ExtentionClasses
    {
        /// <summary>
        /// Internal query, extends css.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static CssExtneder<T> Css<T>(this T q)
            where T : IQuery
        {
            return new CssExtneder<T>(q);
        }

        /// <summary>
        /// Internal query, extends html.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static HtmlExtneder<T> Html<T>(this T q)
            where T : IQuery
        {
            return new HtmlExtneder<T>(q);
        }
    }

    public partial class CssExtneder<T> : QueryExtender<T>
        where T : IQuery
    {
        public CssExtneder(T q)
            : base(q)
        {
        }
    }

    public partial class HtmlExtneder<T> : QueryExtender<T>
        where T : IQuery
    {
        public HtmlExtneder(T q)
            : base(q)
        {
        }
    }

}
