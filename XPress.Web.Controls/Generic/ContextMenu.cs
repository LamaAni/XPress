using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.Controls.Generic;
using XPress.Web;
using XPress.Web.Html.Linq;
using XPress.Web.JCom.Attributes;

namespace XPress.Web.Controls.Generic
{
    /// <summary>
    /// Implements a context menu control.
    /// </summary>
    [XPress.Web.Links.Attributes.LinkConstructor("XPress.Web.Controls.Generic.ContextMenu.js", Links.LinkOrigin.Embedded)]
    public class ContextMenu : XPress.Web.Controls.XPressControl
    {
        public ContextMenu()
            :base("div")
        {
            this.Style["display"] = "none";
            // adding the menu parts control to the child controls.
            this.Children.Append(m_menuParts);
        }

        #region members

        Html.HtmlElement m_menuParts = new Html.HtmlElement();

        #endregion

        #region clientside methods

        /// <summary>
        /// Invokes the clientside menu item.
        /// </summary>
        /// <param name="menuItemId">The menu item to invoke.</param>
        [ClientSideMethod]
        public void InvokeMenuItem(string menuItemId)
        {
        }

        #endregion

        #region rendering methods

        #endregion
    }
}

namespace XPress.Web
{
    
    public static class __base_ContextMenu_ExtentionMethods
    {
    }
}
