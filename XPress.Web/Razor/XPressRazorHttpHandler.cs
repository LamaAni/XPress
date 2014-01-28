using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using XPress.Serialization;
using XPress.Web.JavascriptClient;
using XPress.Web.JavascriptClient.Request;
using XPress.Web.Razor.Storage;

namespace XPress.Web.Razor
{
    public class XPressRazorHttpHandler : IHttpHandler, IRequiresSessionState
    {
        public XPressRazorHttpHandler()
        {
            Bank = XPressRazorCacheBanks.Global;
        }

        #region Static

        static XPressRazorHttpHandler()
        {
            //Serialization.Map.TypeConversionsMap.DefaultMap.Converters.Insert(0, RmcRazorPageFactory.DefaultFactory);
        }

        #endregion

        #region IHttpHandler Members

        [ThreadStatic]
        static XPressRazorCacheBanks m_Current;

        /// <summary>
        /// The thread associated storage bank.
        /// </summary>
        public static XPressRazorCacheBanks Bank
        {
            get { return XPressRazorHttpHandler.m_Current; }
            internal set { XPressRazorHttpHandler.m_Current = value; }
        }

        /// <summary>
        /// If true this handler is resusable.
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
#if DEBUG
            XPress.Coding.CodeTimer timer = new XPress.Coding.CodeTimer();
#endif
            // loading the page as the IRazorTemplate.
            IRazorTemplate page = RmcRazorPageFactory.FromUrl<IRazorTemplate>("~" + context.Request.Url.LocalPath, context);
#if DEBUG
            timer.Mark("Created page template");
#endif

            if (page == null)
            {
                throw new Exception("Path not found or request is unknown.");
            }

            // setting the context.
            HttpContext.Current = context;

            // processes the request via the page.
            page.ProcessRequest(context);
#if DEBUG
            timer.Mark("Request completated.");
            context.Response.Headers.Add("timing", timer.ToTraceCommand("RMC total timing"));
#endif
        }

        #endregion
    }

}
