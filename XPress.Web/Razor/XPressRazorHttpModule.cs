using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XPress.Serialization;
using XPress.Strings;
using XPress.Web.Razor;
using XPress.Web.Razor.Extentions;

namespace XPress.Web.Razor
{
    public class XPressRazorHttpModule : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
            context.Error += context_Error;
        }

        static XPressRazorHttpModule()
        {
            ExecuteOnError = new List<Action<Exception[]>>();
        }

        #endregion

        #region static elements

        /// <summary>
        /// A static list to execute when an error occurs.
        /// </summary>
        public static List<Action<Exception[]>> ExecuteOnError { get; private set; }

        #endregion

        #region Event methods

        void context_Error(object sender, EventArgs e)
        {
            // called on application error.
            if (HttpContext.Current.Request.GetRmcRequestFlags().HasFlag(RmcRazorRequestFlags.RespondAsJson))
            {
                Exception[] errs = HttpContext.Current.AllErrors.ToArray();

                // called to execute on an error.
                ExecuteOnError.ForEach(f => f(errs));

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Write(errs.ToJsonErrorString());
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
        }


        void context_BeginRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.GetRmcRequestFlags().HasFlag(RmcRazorRequestFlags.NoSession))
            {
                HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Disabled);
            }
        }

        #endregion
    }
}
