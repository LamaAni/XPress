using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html.Linq
{
    public static class ConstructionExtentions
    {
        #region control creation

        /// <summary>
        /// Loads a template from a url provided.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="url">The url to load.</param>
        /// <returns></returns>
        public static HtmlTemplate Load<T>(this T parent, string url)
            where T : HtmlElement
        {
            HtmlTemplate template = XPress.Web.Razor.RmcRazorPageFactory.FromUrl<HtmlTemplate>(url, parent.Context);
            template.Execute();
            return template;
        }

        #endregion

    }
}

