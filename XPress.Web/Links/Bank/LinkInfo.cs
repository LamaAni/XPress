using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XPress.Strings;
using System.Reflection;

namespace XPress.Web.Links.Bank
{
    public class LinkInfo
    {
        /// <summary>
        /// The link information, in its compressed and uncompressed states.
        /// </summary>
        /// <param name="link">The link info</param>
        public LinkInfo(Attributes.LinkAttribute link)
        {
            Link = link;
        }

        #region members

        /// <summary>
        /// The link associated with the link info.
        /// </summary>
        public Attributes.LinkAttribute Link { get; private set; }

        Func<string> m_getLinkBody = null;

        string m_contentType, m_fileExtention, m_sourceMD5;

        /// <summary>
        /// If true then the current link is valid. (if null then ignore).
        /// </summary>
        Func<bool> m_getLinkValidity = null;

        Encoding m_sourceEncoding;

        #endregion

        #region methods

        /// <summary>
        /// Validates that the current link has been processed by the link info and
        /// the debug and release mode of the file have been generated.
        /// </summary>
        public void ValidateLink(HttpContext context)
        {
            bool needGenerateLink = false;
            if (m_getLinkBody == null || (m_getLinkValidity != null && !m_getLinkValidity()))
                needGenerateLink = true;

            if (!needGenerateLink)
                return;

            StringBuilder source = GetStringSource(context, out m_getLinkValidity);

            m_contentType = "text/javascript";
            m_sourceEncoding = Encoding.UTF8;
            switch (Link.Type)
            {
                case LinkType.Css:
                    {
                        m_contentType = "text/css";
                    }
                    break;
                case LinkType.Constructor:
                    {
#if DEBUG
                        source.Insert(0, "// implementation for " + Link.UniqueId + "\n");
#endif
                        // initializes the constructor;
                        source.Insert(0, "\n$.XPress.Links.Register('" + this.Link.UniqueId + "',");
                        source.Append(");"); 
                    }
                    break;
                case LinkType.Script:
                    {
                        // noting to do, excpet print explanatio in debug mode.
#if DEBUG
                        source.Insert(0, "// implementation for " + Link.UniqueId + "\n");
#endif
                        if (this.Link.LoadType != LinkLoadType.HeadIfPossible)
                        {
                            // adding the register command.
                            source.Append("\n$.XPress.Links.Register('" + this.Link.UniqueId + "',null);");
                        }
                    }
                    break;
                case LinkType.InitScriptFunction:
                    {
#if DEBUG
                        source.Insert(0, "// implementation for " + Link.UniqueId + "\n");
#endif
                        source.Insert(0, "\n$.XPress.Links.Register('" + this.Link.UniqueId + "', {$:function(){\n");

#if DEBUG
                        source.Append("\n");
#endif
                        source.Append(";}});"); 
                    }
                    break;
                default:
                    {
                        throw new NotImplementedException("Unknown file types are not supported yet");
                    }
                    break;
            }

            string strSource = source.ToString();

#if DEBUG
            // no need to compress indebug mode.
#else
            if (Link.Compress)
            {
                switch (Link.Type)
                {
                    case LinkType.Css:
                        strSource = strSource.CompressCss();
                        break;
                    default:
                        strSource = strSource.CompressJavascript();
                        break;
                }
            }
#endif

            m_sourceMD5 = strSource.CreateMD5HashId(m_sourceEncoding);// Encoding.ASCII.GetString(System.Security.Cryptography.MD5.Create().ComputeHash(m_fileEncoding.GetBytes(strSource)));
            LinkBank.Global.RegisterLinkMD5(this, m_sourceMD5);

            // once the source is ready building the get funtion.
            m_getLinkBody = () =>
                {
                    return strSource;
                };
        }

        private StringBuilder GetStringSource(HttpContext context, out Func<bool> getLinkValidity)
        {
            LinkOrigin actualOrigin = Link.Origin;
            StringBuilder source = new StringBuilder();
            getLinkValidity = () => true;
            switch(actualOrigin)
            {
                case LinkOrigin.AsIs:
                    throw new Exception("LinkOrigin.AsIs not implemented at this time.");
                    break;
                case LinkOrigin.Embedded:
                    break;
                case LinkOrigin.File:
                                    // Create a new WebClient instance.
                    if (Link.Url.StartsWith("http://"))
                        using (System.Net.WebClient myWebClient = new System.Net.WebClient())
                        {
                            // Download the Web resource and save it into the current filesystem folder.
                            source.Append(TrimExtras(myWebClient.DownloadString(Link.Url), this.Link.Type));
                        }
                    else
                    {
                        // mapping to the correct path.
                        string filePath = context.Server.MapPath(Link.Url);
                        DateTime dt = File.GetLastWriteTime(filePath);
                        getLinkValidity = () => File.GetLastWriteTime(filePath) == dt;
                        source.Append(TrimExtras(File.ReadAllText(filePath), this.Link.Type));
                    }
                    break;
                default:
                                    if (Link.ParentType == null)
                    throw new Exception("For embedded resource links the parent type (property) of the link must be defined.");

                // getting the specific information form the declaring type info.
                Stream strm = null;
                try
                {
                    strm = Link.ParentType.Assembly.GetManifestResourceStream(Link.Url);
                    if (strm == null)
                        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            strm = asm.GetManifestResourceStream(Link.Url);
                            if (strm != null)
                                break;
                        }

                    if (strm == null)
                        throw new Exception("Manifest resource '" + Link.Url + "' was not found in the resource stream.");
                }
                catch (Exception ex)
                {
                    throw new Exception("For type `" + Link.ParentType.Name + "` the resource \"" + Link.Url + "\" was not found in the type's assembly.", ex);
                }

                StreamReader reader = new StreamReader(strm);
                source.Append(TrimExtras(reader.ReadToEnd(), this.Link.Type));
                    break;
            }
            if (Link.Origin == LinkOrigin.AsIs)
            {
                throw new Exception("LinkOrigin.AsIs not implemented at this time.");
            }
            else if (Link.Origin == LinkOrigin.File)
            {

            }
            else
            {
                if (Link.ParentType == null)
                    throw new Exception("For embedded resource links the parent type (property) of the link must be defined.");

                // getting the specific information form the declaring type info.
                Stream strm = null;
                try
                {
                    strm = Link.ParentType.Assembly.GetManifestResourceStream(Link.Url);
                    if (strm == null)
                        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            strm = asm.GetManifestResourceStream(Link.Url);
                            if (strm != null)
                                break;
                        }

                    if (strm == null)
                        throw new Exception("Manifest resource '" + Link.Url + "' was not found in the resource stream.");
                }
                catch (Exception ex)
                {
                    throw new Exception("For type `" + Link.ParentType.Name + "` the resource \"" + Link.Url + "\" was not found in the type's assembly.", ex);
                }

                StreamReader reader = new StreamReader(strm);
                source.Append(TrimExtras(reader.ReadToEnd(), this.Link.Type));
            }
            return source;
        }

        string TrimExtras(string source, LinkType type)
        {
            if(type == LinkType.Constructor)
            {
                // find the first implementation of {,}
                int firstIndex = source.IndexOf('{');
                int lastIndex = source.LastIndexOf('}');
                return source.Substring(firstIndex, lastIndex - firstIndex + 1);
            }
            return source;
        }

        /// <summary>
        /// Returns the body of the specifed link.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetLinkBody(HttpContext context) 
        {
            if (m_getLinkBody == null)
                ValidateLink(context);
            return m_getLinkBody();
        }

        public string GetContentType(HttpContext context)
        {
            if (m_getLinkBody == null)
                ValidateLink(context);

            return m_contentType;
        }

        public Encoding GetEncoding(HttpContext context)
        {
            if (m_getLinkBody == null)
                ValidateLink(context);
            return m_sourceEncoding;
        }

        public string GetMD5Key(HttpContext context)
        {
            if (m_getLinkBody == null)
                ValidateLink(context);
            return m_sourceMD5;
        }

        #endregion

        #region head link generation

        public string RenderAsHeadLink(HttpContext context)
        {
            StringBuilder wr = new StringBuilder();
            switch (Link.Type)
            {
                case Web.Links.LinkType.Constructor:
                case Web.Links.LinkType.InitScriptFunction:
                case Web.Links.LinkType.Script:
                    wr.Append("<script language='javascript' type='text/javascript' src='");
                    break;
                case Web.Links.LinkType.Css:
                    wr.Append("<link rel='stylesheet' href='");
                    break;
                default: throw new NotImplementedException();
            }

            // validating link (hashing)
            wr.Append(Web.Links.LinkHandler.WriteLinkCommand(this, context));

            switch (Link.Type)
            {
                case Web.Links.LinkType.Constructor:
                case Web.Links.LinkType.InitScriptFunction:
                case Web.Links.LinkType.Script:
                    wr.Append("'></script>");
                    break;
                case Web.Links.LinkType.Css:
                    wr.Append("' />");
                    break;
                default: throw new NotImplementedException();
            }

            return wr.ToString();
        }

        #endregion

    }
}
