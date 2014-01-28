using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.Controls;
using XPress.Web.JavascriptClient;

namespace XPress.Web.Razor
{
    public class RmcRazorPageFactory : Serialization.Map.ITypeConverter
    {
        #region static

        static RmcRazorPageFactory()
        {
            DefaultFactory = new RmcRazorPageFactory();
        }

        public static RmcRazorPageFactory DefaultFactory { get; private set; }

        static System.Collections.Concurrent.ConcurrentDictionary<Type, string> m_typeToUrl =
            new System.Collections.Concurrent.ConcurrentDictionary<Type, string>();

        static System.Collections.Concurrent.ConcurrentDictionary<string, Type> m_urlToType =
            new System.Collections.Concurrent.ConcurrentDictionary<string, Type>();

        /// <summary>
        /// Creates a template instance from 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T CreateInstanceFromUrl<T>(string url, System.Web.HttpContext context = null)
        {
            context = context == null ? System.Web.HttpContext.Current : context;

            // to server root url.
            url = context.Server.MapToSiteRootUrl(url).ToLower();

            // mapping the type to url.
            T t = (T)System.Web.Compilation.BuildManager.CreateInstanceFromVirtualPath(url, typeof(T));
            
            // registering the template type if needed.
            Type tt = t.GetType();
            if (!m_typeToUrl.ContainsKey(tt))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("utli:");
                builder.Append(url);
                string identity = builder.ToString();
                m_typeToUrl[tt] = identity;
                m_urlToType[identity] = tt;
            }

            return t;
        }


        /// <summary>
        /// Creats a template from url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T FromUrl<T>(string url, System.Web.HttpContext context = null)
            where T : IRazorTemplate
        {
            return XPress.Web.Razor.RmcRazorPageFactory.CreateInstanceFromUrl<T>(url, context);
        }

        #endregion

        #region ITypeConverter Members

        public bool CanConvert(Type t)
        {
            return m_typeToUrl.ContainsKey(t);
        }

        public bool CanConvert(string identity)
        {
            return identity.StartsWith("utli:");
        }

        public Type ToType(string identity)
        {
            // getting the url.
            if (!m_urlToType.ContainsKey(identity))
            {
                // validating instance exists.
                CreateInstanceFromUrl<object>(identity.Substring(5));
            }
            return m_urlToType[identity];
        }

        public string ToIdentitiy(Type t)
        {
            return m_typeToUrl[t];
        }

        #endregion
    }
}
