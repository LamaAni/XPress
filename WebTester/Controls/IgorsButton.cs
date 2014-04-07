using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XPress.Web.Controls.Linq;
using XPress.Web.Html;
using XPress.Web.Html.Linq;
using XPress.Web.Links.Attributes;

namespace WebTester.Controls
{
    [LinkCss("~/IgorsButton.css")]
    /// <summary>
    /// Creates the igor button
    /// </summary>
    public class IgorsButton : XPress.Web.Controls.XPressControl
    {
        public IgorsButton(string text = "This is a button")
            : base("div")
        {
            this.Write(text);
            this.Class("buttonclass");
        }
    }
}