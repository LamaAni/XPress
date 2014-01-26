using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web
{
    public static class HttpContextExtentions
    {
        /// <summary>
        /// Maps a path back to a site root url.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapUrl(this System.Web.HttpServerUtility server, string path)
        {
            string appPath = server.MapPath("~");
            return "~/" + path.Replace(appPath, "").Replace("\\", "/");
        }

        /// <summary>
        /// Maps a url back to a site root url.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string MapToSiteRootUrl(this System.Web.HttpServerUtility server, string url)
        {
            if (url[0] == '~')
                return url;
            return GetCurrentUrl(server) + (url[0] == '/' ? url.Substring(1) : url);
        }

        /// <summary>
        /// Gets the current url.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static string GetCurrentUrl(this System.Web.HttpServerUtility server)
        {
            return server.MapUrl(server.MapPath("") + "\\");
        }
    }
}
