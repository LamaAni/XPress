using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Strings;

namespace XPress.Web.Controls
{
    /// <summary>
    /// Implements a template that can be rendered as a control. This template allows the functionallity of a remote control
    /// for a template.
    /// </summary>
    public class XPressControlTemplate : XPressTemplate, IXPressControl
    {
        public XPressControlTemplate(string tagName = null)
            : base(tagName.IsNullOrEmpty(true) ? "div" : tagName.Trim())
        {
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
    }
}
