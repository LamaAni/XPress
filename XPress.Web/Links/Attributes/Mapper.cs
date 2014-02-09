using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using XPress.NetExtentions;

namespace XPress.Web.Links.Attributes
{
    public static class Mapper
    {
        /// <summary>
        /// The map that holds the type information
        /// </summary>
        static ConcurrentDictionary<Type, LinkMapInfo> Map = new ConcurrentDictionary<Type, LinkMapInfo>();

        public static LinkMapInfo GetMapInfo(Type t)
        {
            if (!Map.ContainsKey(t))
            {
                Map.TryAdd(t, new LinkMapInfo(t));
            }
            return Map[t];
        }
    }

    public class LinkMapInfo : IEnumerable<LinkAttribute>
    {
        public LinkMapInfo(Type t)
        {
            MappedType = t;
            ScanType();
        }

        void ScanType()
        {
            // gettting all the types that are needed to be ignored. 
            LinkIgnoreSourcesFromTypeAttribute[] ignore =
                MappedType.GetAllCustomAttributes<LinkIgnoreSourcesFromTypeAttribute>().ToArray();

            HashSet<Type> ignoreTypes = new HashSet<System.Type>(ignore.SelectMany(i => i.Types));

            // Getting all attributes except the types we are to ignore.
            IEnumerable<LinkAttribute> allAttribs = MappedType.GetAllCustomAttributesWithDeclaringTypes<LinkAttribute>(t =>
            {
                return ignore.Length == 0 ? true : !ignoreTypes.Contains(t);
            }).Distinct(tpl => tpl.Item2.UniqueId).Select(tpl => { tpl.Item2.ParentType = tpl.Item1; return tpl.Item2; }).ToArray();

            // adding the links.
            m_links = allAttribs.ToArray();
            m_fileLinksWithPartialUrl = m_links.Where(l => l.IsPartialFileUrl).ToArray();
            m_linksThatNeedActivation = allAttribs.Where(la => la.ForceActivation || la.Type == LinkType.Constructor || la.Type == LinkType.InitScriptFunction).ToArray();

            //// finding post collection if any is defined.
            //PostCollectionFunction = FindPostCollection();

            // setting the activation protocal for the current object.
            Activation = (LinkActivationEventAttribute)Attribute.GetCustomAttribute(MappedType, typeof(LinkActivationEventAttribute));
            Activation = Activation == null ? new LinkActivationEventAttribute(ActivationEvent.ActiveContext) : Activation;

            // check for activation.
            this.RequiresActivation = m_linksThatNeedActivation.Length > 0;

            // finding root url.
            this.RootUrl = Attribute.IsDefined(MappedType, typeof(LinkFilesRootUrlAttribute)) ?
                (LinkFilesRootUrlAttribute)Attribute.GetCustomAttribute(MappedType, typeof(LinkFilesRootUrlAttribute)) : null;

            if (m_fileLinksWithPartialUrl.Length > 0)
            {
                if (RootUrl == null)
                    throw new Exception("Found link files with partial url, where the class the files are linked to is missing the XPress.Web.Links.Attributes.LinksFileRootUrl attibute. This attribute provides the directory from where to load partial urls.");

                m_fileLinksWithPartialUrl.ForEach(l => l.ValidateRootUrl(RootUrl));
            }
        }

        /// <summary>
        /// The root url associated with the type. Non inherited.
        /// </summary>
        public LinkFilesRootUrlAttribute RootUrl { get; private set; }

        /// <summary>
        /// The activation event associated with the type.
        /// </summary>
        public LinkActivationEventAttribute Activation { get; private set; }

        /// <summary>
        /// The type that was mapped.
        /// </summary>
        public Type MappedType { get; protected set; }

        LinkAttribute[] m_links;

        public IReadOnlyList<LinkAttribute> Links
        {
            get { return m_links; }
        }

        LinkAttribute[] m_fileLinksWithPartialUrl;

        public LinkAttribute[] FileLinksWithPartialUrl
        {
            get { return m_fileLinksWithPartialUrl; }
        }

        LinkAttribute[] m_linksThatNeedActivation;

        /// <summary>
        /// A collection of links that require activation.
        /// </summary>
        public LinkAttribute[] LinksThatNeedActivation
        {
            get { return m_linksThatNeedActivation; }
        }

        public IEnumerator<LinkAttribute> GetEnumerator()
        {
            return m_links.Cast<LinkAttribute>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool RequiresActivation { get; private set; }
    }
}
