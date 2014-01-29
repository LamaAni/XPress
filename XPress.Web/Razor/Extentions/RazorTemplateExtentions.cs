using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XPress.Web.Razor.Extentions
{
    public static class RazorTemplateExtentions
    {
        /// <summary>
        /// Loads a diffrent page/template into the current page.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="virtualPath"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IRazorTemplate Load(this IRazorTemplate t, string virtualPath, HttpContext context = null)
        {
            return XPressRazorPageFactory.CreateInstanceFromUrl<IRazorTemplate>(virtualPath, context == null ? HttpContext.Current : context);
        }
    }
}
