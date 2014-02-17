using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.JCom.Attributes;
using XPress.Web.Controls.Linq;
using XPress.Web.Html.Linq;
using XPress.Web.Controls.Generic;
using System.ComponentModel;

namespace XPress.Web.Controls.Generic
{
    /// <summary>
    /// Implements a floating box, used to create a floating windows and other floating controls.
    /// </summary>
    [XPress.Web.Links.Attributes.LinkConstructor("XPress.Web.Controls.Generic.FloatingBox.js", Links.LinkOrigin.Embedded)]
    [XPress.Web.Links.Attributes.LinkScript("XPress.Web.Controls.Generic.FloatingBoxGlobals.js", Links.LinkOrigin.Embedded, LoadIndex=90)]
    public class FloatingBox : XPress.Web.Controls.XPressControlTemplate, IGenericControl
    {
        /// <summary>
        /// Creates a floating box
        /// </summary>
        public FloatingBox(string tagName = "div", bool isShown = false)
            :base(tagName)
        {
            IsShown = isShown;
            this.Style["display"] = "none";
            this.MoveWindowToTopInStackWhenShown = true;
            HideOnLoseContext = false;
            this.ZStack = 1;
        }

        #region members

        /// <summary>
        /// Integer number representing the stack index for the float, the stack index will determine
        /// the z index group the floating box belongs to. Default is 1. (zindex=1000+).
        /// </summary>
        [ClientSideProperty]
        [DefaultValue(1)]
        public int ZStack { get; set; }

        /// <summary>
        /// Moves the window to the top of the window stack when showing the window.
        /// </summary>
        [ClientSideProperty]
        [DefaultValue(true)]
        public bool MoveWindowToTopInStackWhenShown { get; set; }

        /// <summary>
        /// If true the current is shown.
        /// </summary>
        [ClientSideProperty]
        [DefaultValue(false)]
        public bool IsShown { get; private set; }

        /// <summary>
        /// The id of the control is bound to. When a box is bound to a control, it will show near that control according to
        /// the BindState.
        /// </summary>
        [ClientSideProperty]
        public string BoundQuery { get; private set; }

        /// <summary>
        /// If true then hides the floating box when context is lost (focus to any of the child elements).
        /// In order for this to work the control must be able to take focus, on any child. (Set CanHaveFocus = true).
        /// </summary>
        [ClientSideProperty]
        [DefaultValue(true)]
        public bool HideOnLoseContext { get; set; }

        #endregion

        #region rendering

        public override void PreRender(Html.Rendering.HtmlWriter writer)
        {
            // shows the values if nesscesary.

            // cleras the responce command to show if any.
            if (this.CallContext != null)
                this.ClearResponseCommand(this.Id + "_show"); // clearing the responce command to show.
            if (IsShown)
                writer.InitCommands.Add(new XPress.Web.Core.JScriptCommandResponce(this.ClientGetScript() + ".Show();", Core.CommandExecutionType.Post));

            base.PreRender(writer);
        }

        #endregion

        #region float commands

        /// <summary>
        /// Shows the control on the client side.
        /// </summary>
        /// <param name="isShown"></param>
        public void Show(bool isShown)
        {
            // creates the responce command.
            if (this.RequiresUpdate || this.CallContext == null)
                return;

            var cmndId = this.Id + "_show";

            if (isShown && !IsShown)
            {
                this.RegisterResponseCommand(cmndId, new XPress.Web.Core.JScriptCommandResponce(this.ClientGetScript() + ".Show();"));
            }
            else if (!isShown && IsShown)
            {
                this.RegisterResponseCommand(cmndId, new XPress.Web.Core.JScriptCommandResponce(this.ClientGetScript() + ".Hide();"));
            }

            IsShown = isShown;
        }

        #endregion
    }

    /// <summary>
    /// The bind state where 
    /// </summary>
    public enum BindState { TopLeft, TopRight, BottomRight,BottomLeft};
}

namespace XPress.Web.Controls.Linq
{
    public static class _generic_Extention_methods_FloatingBox
    {
        /// <summary>
        /// Shows or hides the box
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="isShown"></param>
        /// <returns></returns>
        public static T Show<T>(this T c, bool isShown)
            where T : FloatingBox
        {
            c.Show(isShown);
            return c;
        }

        /// <summary>
        /// Hides the control when focus is lost, toggle on off.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="isShown"></param>
        /// <returns></returns>
        public static T HideBoxOnLoseFocus<T>(this T c, bool hideOnContextLost)
            where T : FloatingBox
        {
            c.HideOnLoseContext = hideOnContextLost;
            return c;
        }
    }
}
