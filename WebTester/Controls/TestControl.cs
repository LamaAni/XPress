using XPress.Web.Html;
using XPress.Web.Links.Attributes;
using XPress.Web.Html.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XPress.Web.JCom.Attributes;
using XPress.Web.Controls;

namespace WebTester.Controls
{
    [LinkConstructor("~/Controls/constructor.js")]
    [LinkInitScript("~/Controls/initscript.js")]
    [LinkActivationEvent(XPress.Web.Links.ActivationEvent.ActiveContext)]
    public class TestControl : RemoteControl
    {
        public TestControl()
            : base("div")
        {
            this.Children.Append(new HtmlLiteral("test..."));
        }

        /// <summary>
        /// Called from the client side.
        /// </summary>
        [ClientSideMethod]
        public string ClientSideTestMethod()
        {
            //this.RequiresUpdate = true;
            // called from the client sid
            GC.Collect();
            return "Called GC.Collect, Current client side property value " + ClientSideProperty;
        }

        /// <summary>
        /// Example for client side property.
        /// </summary>
        [ClientSideProperty]
        public int ClientSideProperty { get; private set; }
    }
}