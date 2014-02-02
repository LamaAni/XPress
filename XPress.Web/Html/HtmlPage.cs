using XPress.Web.Html.Linq;
using XPress.Web.Links;
using XPress.Web.Links.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html
{
    /// <summary>
    /// Implements a most basic html page to be rendered to the client.
    /// </summary>
    public class HtmlPage : HtmlTemplate
    {
        public HtmlPage()
        {
            CanBePage = true;
        }

        /// <summary>
        /// On the client side all $.Vebrose=VebroseClientJavascript;
        /// </summary>
        public bool VebroseClientJavascript { get; set; }

        protected override void CreateInitScript(StringBuilder initBuilder)
        {
            if (VebroseClientJavascript)
                initBuilder.Append("$.Vebrose=true;");
            base.CreateInitScript(initBuilder);
        }
    }
}
