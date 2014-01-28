using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XPress.Serialization;

namespace XPress.Web.JavascriptClient
{
    public static class __JacascriptClientGlobalExtentions
    {
        /// <summary>
        /// Returns the rmc razor request type.
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        public static JClientRequestType GetRmcRazorRequestType(this HttpRequest rq)
        {
            if (rq.Headers["jClientCmd"] == null)
                return JClientRequestType.Page;
            switch (rq.Headers["jClientCmd"].ToLower())
            {
                case "json": return JClientRequestType.Json;
                case "waitforresponse": return JClientRequestType.WaitForResponse;
                case "beat": return JClientRequestType.Beat;
                case "close": return JClientRequestType.DestroyClient;
                default: return JClientRequestType.Page;
            }
        }

        /// <summary>
        /// Reads the beat command client id from the stream.
        /// </summary>
        /// <returns></returns>
        public static string GetJClientIdFromRequestHeders(this HttpRequest rq)
        {
            return rq.Headers["jclientid"];
        }

        /// <summary>
        /// Reads the client request from the stream.
        /// </summary>
        /// <param name="rq">The request</param>
        /// <returns></returns>
        public static Request.JClientRequest ReadRequestFromStream(this HttpRequest rq)
        {
            rq.InputStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(rq.InputStream);
            string jsonRq = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            return jsonRq.FromJSJson<Request.JClientRequest>();
        }

        /// <summary>
        /// Reads the stream as string. From beginning to end.
        /// </summary>
        /// <param name="rq"></param>
        /// <returns>The string in the stream</returns>
        public static string ReadAsString(this HttpRequest rq)
        {
            rq.InputStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(rq.InputStream);
            string val = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            return val;
        }
    }
}
