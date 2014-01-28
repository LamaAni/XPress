using XPress.Web.Links.Bank;
using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XPress.Strings;

namespace XPress.Web.Html.Response
{
    /// <summary>
    /// Implements a link command to be sent to the client.
    /// </summary>
    public class LinkCommand : Core.XPressResponseCommand
    {
        public LinkCommand(LinkInfo info, HttpContext context)
            : base("link", Core.CommandExecutionType.Pend)
        {
            Url = XPress.Web.Links.LinkHandler.WriteLinkCommand(info.Link.UniqueId, context);
            UniqueId = info.GetMD5Key(context).Escape();//.Replace('%', '_').Replace("*","__");
            Name =
#if DEBUG
 info.Link.Origin == XPress.Web.Links.LinkOrigin.AsIs ? info.Link.ParentType.Name : info.Link.Url;
#else
            null;
#endif
            LinkType = info.Link.Type == Links.LinkType.Css ? LinkElementType.Css : LinkElementType.Script;
            Info = info;
        }

        #region link command info

        [XPressIgnore]
        public LinkInfo Info { get; private set; }

        public string Url { get; private set; }
        public string UniqueId { get; private set; }
        public LinkElementType LinkType { get; private set; }
        public string Name { get; private set; }

        /// <summary>
        /// The relation between the link and the page.
        /// </summary>
        public string Relation { get; private set; }

        #endregion
    }

    public enum LinkElementType { Script, Css, Link, Meta };
}
