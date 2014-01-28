using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XPress.Web.Links.Bank
{
    /// <summary>
    /// Represents a general link bank that stores links to unique ids.
    /// </summary>
    public class LinkBank
    {
        protected LinkBank()
        {
            LinksByUniqueId = new System.Collections.Concurrent.ConcurrentDictionary<string, LinkInfo>();
            LinkMD5ToLinkId = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
        }

        static LinkBank()
        {
            Global = new LinkBank();
        }

        #region static

        /// <summary>
        /// The global link bank that is associated with the basic application.
        /// </summary>
        public static LinkBank Global { get; private set; }

        #endregion

        #region Members

        /// <summary>
        /// A collection of links by the link's unique id.
        /// </summary>
        protected System.Collections.Concurrent.ConcurrentDictionary<string, LinkInfo> LinksByUniqueId { get; private set; }

        /// <summary>
        /// Maps the link md5 to link id.
        /// </summary>
        protected System.Collections.Concurrent.ConcurrentDictionary<string, string> LinkMD5ToLinkId { get; private set; }

        public LinkInfo GetLinkInfo(string uid)
        {
            if (!LinksByUniqueId.ContainsKey(uid))
                throw new Exception("Link must be in the collection");
            return LinksByUniqueId[uid];
        }

        #endregion

        #region Link registration

        public bool Contains(string linkId)
        {
            return LinksByUniqueId.ContainsKey(linkId);
        }

        /// <summary>
        /// Registers a new link to the collection.
        /// </summary>
        /// <param name="link"></param>
        public void RegisterLink(Attributes.LinkAttribute link)
        {
            // registes a new link if possible.
            if (LinksByUniqueId.ContainsKey(link.UniqueId))
                return; // nothing to define, this link exists.

            LinkInfo info = new LinkInfo(link);
            LinksByUniqueId[info.Link.UniqueId] = info;
        }

        /// <summary>
        /// Registeres the link info MD5 with the link id.
        /// </summary>
        /// <param name="info"></param>
        internal void RegisterLinkMD5(LinkInfo info, string md5)
        {
            if (!LinkMD5ToLinkId.ContainsKey(md5))
                LinkMD5ToLinkId.TryAdd(md5, info.Link.UniqueId);
        }

        #endregion

        #region write

        public void WriteAsLinkResponse(HttpContext context, string md5Id)
        {
            // clearing the current response.
            context.Response.Clear();
            string uninqueId;
            if (!LinkMD5ToLinkId.TryGetValue(md5Id, out uninqueId))
                throw new Exception("Cannot find the specified link id " + md5Id);

            if (!LinksByUniqueId.ContainsKey(uninqueId))
                throw new Exception("Cannot find the specified link of source " + uninqueId);
            LinkInfo info = LinksByUniqueId[uninqueId];
            context.Response.Write(info.GetLinkBody(context));
            context.Response.ContentEncoding = info.GetEncoding(context);
            context.Response.ContentType = info.GetContentType(context);
        }

        #endregion
    }
}
