using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Razor
{
    public class XPressRazorHostFactory : System.Web.WebPages.Razor.WebRazorHostFactory
    {
        public XPressRazorHostFactory()
            : base()
        {
        }

        public override System.Web.WebPages.Razor.WebPageRazorHost CreateHost(string virtualPath, string physicalPath)
        {
            return new XPressWebPageRazorHost(virtualPath, physicalPath);
        }

    }
}
