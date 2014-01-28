using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XPress.Web.Links
{
    public class LinkHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            // getting the link id.
            string linkMD5 = context.Request.Unvalidated["lid"];
            if (linkMD5 == null)
                throw new Exception("Cannot find link id");
            Bank.LinkBank.Global.WriteAsLinkResponse(context, linkMD5);
            context.Response.Cache.SetCacheability(HttpCacheability.Private);
            context.Response.Cache.SetExpires(DateTime.Now + new TimeSpan(10, 0, 0, 0, 0));
            //context.Response.Headers["host"] = context.Request.Url.Host;
        }

        #endregion

        #region static

        public static string WriteLinkCommand(string uid, HttpContext context)
        {
            Bank.LinkInfo info = Bank.LinkBank.Global.GetLinkInfo(uid);
            return WriteLinkCommand(info, context);
        }

        public static string WriteLinkCommand(Bank.LinkInfo info, HttpContext context)
        {
            info.ValidateLink(context);

            return "_get.cmd.link?" + string.Join("&", new string[]{
#if DEBUG
                "source="+(info.Link.Origin == LinkOrigin.AsIs? info.Link.ParentType.Name : info.Link.Url),
#else
#endif
                "lid="+HttpUtility.UrlEncode(info.GetMD5Key(context))});
        }

        #endregion
    }
}
