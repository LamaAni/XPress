using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.Controls.Generic;
using XPress.Web;
using XPress.Web.Html.Linq;

namespace XPress.Web.Controls.Generic
{
    /// <summary>
    /// Implements a context menu control.
    /// </summary>
    public class ContextMenu : XPress.Web.Controls.RemoteControl
    {
        public ContextMenu()
            :base("div")
        {
            this.Style["display"] = "none";
        }
    }
}

namespace XPress.Web
{
    [XPress.Web.Links.Attributes.LinkConstructor("XPress.Web.Controls.Generic.Button.js", Links.LinkOrigin.Embedded)]
    public static class __base_ContextMenu_ExtentionMethods
    {
    }
}
