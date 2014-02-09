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
        /// A collection of attributes of the control. Id and style attributes are handled internally.
        /// </summary>
        public override Html.Collections.AttributeCollection Attributes
        {
            get
            {
                return base.Attributes;
            }
        }

        /// <summary>
        /// The client id that will be used when the object is rendered.
        /// </summary>
        public ulong ClientId
        {
            get
            {
                return JavascriptClient.JClientCallContext.Current.Client.ReferenceBank.Store(this);
            }
        }

        #endregion

        #region overriden members

        /// <summary>
        /// Called when the object is pre rendered.
        /// </summary>
        /// <param name="writer"></param>
        public override void PreRender(Html.Rendering.HtmlWriter writer)
        {
            if (this.Attr("id") == null)
                this.Attr("id", writer.ObjectSource.GetObjectId(this).ToString());
            base.PreRender(writer);
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
    }
}
