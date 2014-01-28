using XPress.Web.Links.Attributes;
using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.Html.Linq;
using XPress.Web.Links.Bank;
using System.Web;
using XPress.Web.JCom.Map;

namespace XPress.Web.Html.Collections
{
    /// <summary>
    /// A collection of values that applies to a specific type. Like links, JCom type defs, exc...
    /// </summary>
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public class TypeDependentCacheCollection
    {
        /// <summary>
        /// The links collection associated with the page.
        /// </summary>
        public TypeDependentCacheCollection()
        {
        }

        #region members

        [XPressMember]
        HashSet<Type> m_loadedTypes = new HashSet<Type>();

        [XPressMember]
        HashSet<string> m_loadedTypeIds = new HashSet<string>();

        #endregion

        #region methods

        /// <summary>
        /// Types loaded
        /// </summary>
        /// <returns></returns>
        public bool TypeLoaded(Type t)
        {
            return m_loadedTypes.Contains(t);
        }

        /// <summary>
        /// Returns collections of missing type dependent values that need to be loaded on the client side.
        /// </summary>
        /// <param name="types"></param>
        /// <param name="missingLinks"></param>
        /// <param name="missingTypeDefs"></param>
        /// <param name="markMissingAsDone"></param>
        public void GetMissingDefinitions(IEnumerable<Type> types, out LinkInfo[] missingLinks, out JCom.Map.JComTypeInfo[] missingTypeDefs, bool markMissingAsDone = true)
        {
            Type[] missing = types.Distinct().Except(m_loadedTypes).ToArray();
            List<LinkInfo> mLinks = new List<LinkInfo>();
            List<JComTypeInfo> mTypeDefs = new List<JCom.Map.JComTypeInfo>();
            missing.ForEach(t =>
            {
                // adding the type.
                if (markMissingAsDone)
                    m_loadedTypes.Add(t);

                JCom.Map.JComTypeInfo jti = JCom.Map.JComTypeInfo.Get(t);
                if(jti.RequiresClientSideDefinition)
                    mTypeDefs.Add(jti);

                // adding the attributes to the collection.
                LinkMapInfo lmi = Mapper.GetMapInfo(t);
                LinkInfo[] links = lmi.Links.Where(link => !m_loadedTypeIds.Contains(link.UniqueId)).ToArray().ForEach(link =>
                {
                    if (markMissingAsDone)
                        m_loadedTypeIds.Add(link.UniqueId);
                    LinkBank.Global.RegisterLink(link);
                }).Select(link => LinkBank.Global.GetLinkInfo(link.UniqueId)).ToArray();
                mLinks.AddRange(links);
            }).ToArray();

            missingTypeDefs = mTypeDefs.Distinct().ToArray();
            missingLinks = mLinks.Distinct(l => l.Link.UniqueId).ToArray();
        }

        /// <summary>
        /// Creates a collection of commands that will respond to update the client on its type dependent values.
        /// </summary>
        /// <param name="context">The current HTTP context</param>
        /// <param name="types">The types to take into account.</param>
        /// <param name="markMissingAsDone">If true, types that are marked as done will not be taken into account the next time this process runs.</param>
        /// <returns>A collection of commands.</returns>
        public XPress.Web.Core.XPressResponseCommand[] CreateTypeDependentDefinitionCommands(HttpContext context, IEnumerable<Type> types, bool markMissingAsDone = true)
        {
            LinkInfo[] missingLinks;
            JCom.Map.JComTypeInfo[] missingTypeDefs;

            this.GetMissingDefinitions(types, out missingLinks, out missingTypeDefs, markMissingAsDone);

            // ordering.
            missingLinks = missingLinks.OrderBy(l => l.Link.LoadIndex).ToArray();

            return missingLinks.Select<LinkInfo, Core.XPressResponseCommand>(lmi => new Response.LinkCommand(lmi, context))
                .Concat(missingTypeDefs.Select<JComTypeInfo, Core.XPressResponseCommand>(
                jmi => new JCom.Com.Response.JComTypeDefResponce(jmi, JCom.Compilers.Specialized.JavaScriptCompiler.Global))).ToArray();
        }
        #endregion
    }
}
