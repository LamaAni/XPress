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
            m_linksThatNeedActivation = allAttribs.Where(la => la.ForceActivation || la.Type == LinkType.Constructor || la.Type == LinkType.InitScriptFunction).ToArray();

            //// finding post collection if any is defined.
            //PostCollectionFunction = FindPostCollection();

            // setting the activation protocal for the current object.
            Activation = (LinkActivationEventAttribute)Attribute.GetCustomAttribute(MappedType, typeof(LinkActivationEventAttribute));
            Activation = Activation == null ? new LinkActivationEventAttribute(ActivationEvent.ActiveContext) : Activation;

            // check for activation.
            this.RequiresActivation = m_linksThatNeedActivation.Length > 0;
        }

        //private System.Reflection.MethodInfo FindPostCollection()
        //{
        //    System.Reflection.MethodInfo pcm =
        //        Type.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
        //        .Where(m => Attribute.IsDefined(m, typeof(LinkPostCollectAttribute))).FirstOrDefault();

        //    if (pcm != null)
        //    {
        //        // need activation.
        //        System.Reflection.ParameterInfo[] prs = pcm.GetParameters();
        //        if (prs.Length != 1 || !typeof(LinkCollector).IsAssignableFrom(prs[0].ParameterType))
        //            throw new Exception("Post collect links attribute must recive only one parameter of type LinkCollector");
        //    }

        //    return pcm;
        //}

        //public System.Reflection.MethodInfo PostCollectionFunction { get; private set; }

        public LinkActivationEventAttribute Activation { get; private set; }

        public Type MappedType { get; protected set; }

        LinkAttribute[] m_links;

        public IReadOnlyList<LinkAttribute> Links
        {
            get { return m_links; }
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
