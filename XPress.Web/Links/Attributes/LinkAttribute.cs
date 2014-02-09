using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace XPress.Web.Links.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
    public class LinkAttribute : Attribute
    {
        /// <summary>
        /// Constructs the link argument.
        /// </summary>
        /// <param name="url">
        /// The url associated with the link. 
        /// In case of an embedded link this is the embedded name, amd the assembly is taken from
        /// the type the link associates to.
        /// </param>
        public LinkAttribute(string url, LinkOrigin origin = LinkOrigin.File)
        {
            m_Url = url.Trim();
            Type = LinkType.Script;
            LoadType = LinkLoadType.Inline;
            LoadIndex = 100;
            Compress = true;
            Origin = origin;
            PageStatic = true;
        }

        string m_Url;

        /// <summary>
        /// The url associated with the link. 
        /// In case of an embedded link this is the embedded name, amd the assembly is taken from
        /// the type the link associates to.
        /// </summary>
        public string Url
        {
            get { return m_Url; }
        }

        /// <summary>
        /// The type of the current string. 
        /// This type will be used to compression. Use other if no compression is needed.
        /// </summary>
        public LinkType Type
        {
            get;
            set;
        }

        /// <summary>
        /// If true the activation of a object with this attribute will be forced.
        /// </summary>
        public bool ForceActivation { get; set; }

        /// <summary>
        /// The parent type that requested the specific link. (The type where the link was defined).
        /// </summary>
        public Type ParentType { get; set; }

        /// <summary>
        /// Determines when to load the script. Note that when marking a script to be loaded with the head,
        /// the script must be persent at the first load of the page. i.e. in the first response to the page
        /// Otherwise the script is ignored.
        /// </summary>
        public LinkLoadType LoadType { get; set; }

        /// <summary>
        /// If the current is marked for LoadWhenPageLoad, the index determines the position of the script in
        /// The loading scenario. Default is 100
        /// </summary>
        public int LoadIndex
        {
            get;
            set;
        }

        /// <summary>
        /// If true the current link will be compressed if possible.
        /// </summary>
        public bool Compress { get; set; }

        /// <summary>
        /// If true the link requires validation before the link is called. (True for now only for scripts).
        /// </summary>
        public bool RequriesValidationBeforeCall { get { return Type == LinkType.Constructor || Type == LinkType.InitScriptFunction || Type == LinkType.Script; } }

        string m_uniqueId = null;

        /// <summary>
        /// The associated name of the current link. If not directly defined returns the url.
        /// To similar unqiue ids will override each other.
        /// </summary>
        public string UniqueId
        {
            get
            {
                if (m_uniqueId == null)
                {
                    // getting a decimal hash.
                    return Url;// Encoding.ASCII.GetString(System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(Url)));
                }
                return m_uniqueId;
            }
            set
            {
                m_uniqueId = value;
            }
        }

        /// <summary>
        /// Where the search the link. If a file then we would search the link using the url.
        /// Otherwise it is assumed that the link is an embedded resource with the same name, from the calling
        /// assembly. For types, the link origin is assumed to be embedded.
        /// </summary>
        public LinkOrigin Origin { get; set; }

        /// <summary>
        /// If true this link would not be removed in the case where the current rendering of the page exculeds the current link.
        /// i.e. when no conntrol calls this link anymore. Default is true.
        /// </summary>
        public bool PageStatic { get; set; }

        /// <summary>
        /// Depending on the link origin, the link data can be taken from the specified source.
        /// </summary>
        public string DynamicSource { get; set; }

        /// <summary>
        /// True if the link is a partial file url.
        /// </summary>
        public bool IsPartialFileUrl { get { return Origin == LinkOrigin.File && !Url.StartsWith("~"); } }

        /// <summary>
        /// Validated that the link url is complient with the root url associated with the type the link originated from.
        /// </summary>
        /// <param name="root"></param>
        internal void ValidateRootUrl(LinkFilesRootUrlAttribute root)
        {
            // validated the root url accoding to the type.
            if (!IsPartialFileUrl)
                return;
            m_Url = root.RootUrl +
                (m_Url.StartsWith("/") ? "" : "/") + m_Url;
        }
    }

}
