using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XPress.Serialization;

namespace XPress.Web.Razor
{
    public static class Razor_Global_Extentions
    {
        /// <summary>
        /// Returns the request flags loaded by the request headers.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static RmcRazorRequestFlags GetRmcRequestFlags(this HttpRequest request)
        {
            if (request.Headers["_rmcrazor"] == null)
                return RmcRazorRequestFlags.None;
            string[] flags = request.Headers["_rmcrazor"].Split(';');
            RmcRazorRequestFlags rslt = RmcRazorRequestFlags.None;
            flags.ForEach(f =>
            {
                if (f == "")
                    return;
                object flag = Enum.Parse(typeof(RmcRazorRequestFlags), f);
                if (flag == null)
                    return;
                RmcRazorRequestFlags rrf = (RmcRazorRequestFlags)flag;
                if ((rrf & rslt) != rrf)
                    rslt = rslt | rrf;
            });
            return rslt;
        }

        public static object ToJsonErrorObject(this Exception ex)
        {
            return new
            {
                Trace = ex.StackTrace,
                Location = "From " + ex.TargetSite.DeclaringType.ToString() + " at " + ex.TargetSite.Name,
                Message = ex.Message,
                Inner = ex.InnerException == null ? null : ToJsonErrorObject(ex.InnerException),
            };
        }

        public static string ToJsonErrorString(this Exception[] errs)
        {
#if DEBUG
            return new
            {
                Errors = errs.Select(err => err.ToJsonErrorObject()).ToArray(),
                IsError = true,
                Message = "Error on serverside!",
            }.ToJSJson();
#else
                return "{\"Message\": \"Error on serverside. Please run in debug mode to view this error details.\", \"IsError\" : true}";
#endif
        }
    }
}
