using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.JCom.Attributes;
using XPress.Web.Html.Linq;
using System.ComponentModel;
using XPress.Web.Html.Rendering;
using XPress.Web.Html;

namespace XPress.Web.Controls.Generic
{
    /// <summary>
    /// Represents a generic html button. (with edit mode and edit selection, controlled by theaming).
    /// </summary>
    [XPress.Web.Links.Attributes.LinkConstructor("XPress.Web.Controls.Generic.Button.js", Links.LinkOrigin.Embedded)]
    public class Button : XPressControl, IGenericControl
    {
        public Button(string text = null, Action<object, EventArgs> onClick = null)
            : base("div")
        {
            Enabled = true;
            Text = text;
            this.Css().Class("genericButton");
            if (onClick != null)
               this.Bind("click", (s, e) => onClick(s, e));
        }

        #region attributes

        /// <summary>
        /// If true the button is enabled.
        /// </summary>
        [ClientSideProperty]
        [DefaultValue(true)]
        public bool Enabled { get; set; }

        /// <summary>
        /// If true this is a default button (and will effect the 
        /// </summary>
        [DefaultValue(false)]
        public bool IsDefault { get; set; }

        /// <summary>
        /// The text in the button.
        /// </summary>
        public string Text { get; set; }

        #endregion

        #region methods

        [ClientSideMethod]
        public void Click()
        {
            // invokes the click from the client side.
            if (this.Events.Contains("click"))
                this.Trigger(this, "click");
        }

        #endregion

        #region rendering override

        protected override void RenderChildren(Html.Rendering.HtmlWriter writer)
        {
            base.RenderChildren(writer);
            if (Text != null)
            {
                //writer.Write("<div style='width:100%; height:100%; overflow:hidden; display:inline-block;'>");
                writer.Write(Context.Server.HtmlEncode(Text));
                //writer.Write("</div>");
            }
        }

        #endregion
    }
}

namespace XPress.Web
{
    public static class gloabl_Generic_Button_Extention_Functions
    {
        /// <summary>
        /// Binds the click command.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="onClick"></param>
        public static HtmlElement Click(this XPress.Web.Controls.Generic.Button ctrl, Action<object, EventArgs> onClick = null)
        {
            ctrl.Bind("click", onClick);
            return ctrl;
        }
    }
}
