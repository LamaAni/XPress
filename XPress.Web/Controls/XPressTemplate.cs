using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using XPress.Web.Html.Linq;
using XPress.Serialization.Attributes;
using XPress.Serialization;
using XPress.Strings;
using XPress.Web.JavascriptClient;

namespace XPress.Web.Controls
{
    /// <summary>
    ///A basic template for remote control. Note that the template itself may act as a remote control only in the case it is not a page.
    /// </summary>
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    [XPressInheritedMemberSelection(XPressMemberSelectionType.Properties)]
    public class XPressTemplate : XPress.Web.JavascriptClient.JClientTemplate
    {
        /// <summary>
        /// Generates a new template.
        /// </summary>
        /// <param name="TagName"></param>
        public XPressTemplate(string tagName = null)
            : base(tagName)
        {
        }

        protected override void ProcessRequestAsPage(System.Web.HttpContext context)
        {
            base.ProcessRequestAsPage(context);

            // marking all the first IRemoteControl children as roots.
            this.Invoke((el) =>
            {
                if (el is IXPressControl)
                {
                    JavascriptClient.JClientCallContext.Current.Client.ReferenceBank.Store(el, true);
                    return BubbleContinueMode.DontContinueToChildren;
                }
                else return BubbleContinueMode.Continue;
            }, true, BubbleDirection.ToChildren);
        }

        #region IRemoteControl Members

        /// <summary>
        /// The rmc client.
        /// </summary>
        public new Client.ControlsClient Client
        {
            get { return base.Client as Client.ControlsClient; }
        }

        /// <summary>
        /// A collection of type dependent cache members.
        /// </summary>
        public override Html.Collections.TypeDependentCacheCollection TypeDependentCache
        {
            get
            {
                return Client.TypeDependentCache;
            }
        }

        #endregion

        #region overrident members

        /// <summary>
        /// Overrident to create the basic rmc client.
        /// </summary>
        /// <returns></returns>
        public override JClient CreateClient()
        {
            return new Client.ControlsClient();
        }

        #endregion
    }
}
