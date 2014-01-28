using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(XPress.Web.Razor.XPressRazorApplicationDefnitions), "Start")]

namespace XPress.Web.Razor
{
    public static class XPressRazorApplicationDefnitions
    {
        // initialize the pre application start methods, used to register speicific events.
        public static void Start()
        {
        }
    }
}
