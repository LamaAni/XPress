using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using XPress.Web.Html.Linq;
using XPress.Web.Controls.Linq;
using XPress.Serialization.Attributes;

namespace XPress.Web.Controls
{
    /// <summary>
    /// The basis of a remote control, the remote control is a control capable of updating the client side to its current state,
    /// as if the control was a seperated part of the html. The control behaves as if it was a inner frame. 
    /// The id of the control is handled internally.
    /// </summary>
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    [XPressInheritedMemberSelection(XPressMemberSelectionType.Properties)]
    public class XPressControl : Html.HtmlElement, IXPressControl
    {
        /// <summary>
        /// Construct a new control. 
        /// </summary>
        /// <param name="tagName">Cannot be null.</param>
        public XPressControl(string tagName = "div")
            : base(tagName)
        {
            if (tagName == null)
                throw new Exception("The tag name of a control cannot be null.");
            RequiresUpdate = true;
        }

        #region members

        /// <summary>
        /// The call context associated with the client.
        /// </summary>
        public virtual JavascriptClient.JClientCallContext CallContext { get { return JavascriptClient.JClientCallContext.Current; } }

        /// <summary>
        /// The client id that will be used when the object is rendered.
        /// </summary>
        public override string Id
        {
            get
            {
                if (JavascriptClient.JClientCallContext.Current == null)
                    return base.Id;
                return JavascriptClient.JClientCallContext.Current.Client.ReferenceBank.Store(this).ToString();
            }
            set
            {
                if (JavascriptClient.JClientCallContext.Current == null)
                    base.Id = value;
                else throw new Exception("The id for a remote control cannot be set (not implemented), please use the auto generated id.");
            }
        }

        #endregion

        #region IRemoteControl Members

        /// <summary>
        /// If true the current control requires update.
        /// </summary>
        public bool RequiresUpdate
        {
            get;
            set;
        }

        #endregion

        #region generic updating commmands

        /// <summary>
        /// Called when the client side calls for update command.
        /// </summary>
        /// <param name="cmnd"></param>
        [XPress.Web.JCom.Attributes.ClientSideMethod(Name = "Update")]
        protected virtual void OnClientSideUpdate(string cmnd)
        {
            this.Trigger(this, Html.Events.EventDefaults.Update, new XPressControlUpdateEventArgs(cmnd));
        }

        #endregion
    }

    public class XPressControlUpdateEventArgs : EventArgs
    {
        public XPressControlUpdateEventArgs(string cmnd)
        {
            Command = cmnd;
        }

        /// <summary>
        /// The command sent with the update.
        /// </summary>
        public string Command { get; private set; }
    }

    public static class XPressControl_Extentions
    {
        /// <summary>
        /// Bind the on update event to the control, and add update events. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctrl"></param>
        /// <param name="a"></param>
        public static T OnUpdate<T>(this T ctrl, Action<object, EventArgs> func)
            where T : XPressControl
        {
            ctrl.Bind(Html.Events.EventDefaults.Update, func);
            return ctrl;
        }
    }
}
