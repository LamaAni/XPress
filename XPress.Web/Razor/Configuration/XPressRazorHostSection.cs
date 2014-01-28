using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Razor.Configuration;

namespace XPress.Web.Razor.Configuration
{
    public class XPressRazorHostSection : HostSection
    {
        public XPressRazorHostSection()
            : base()
        {
            this.FactoryType = "XPress.Web.Razor.RmcRazorHostFactory";
        }
    }
}
