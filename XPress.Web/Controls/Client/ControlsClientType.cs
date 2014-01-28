using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using XPress.Web.Links.Attributes;
using System.IO;
using XPress.Strings;

namespace XPress.Web.Controls.Client
{
    /// <summary>
    /// Represents a client type for a control structure. Allows for the activation of a control on the client side.
    /// </summary>
    [XPress.Serialization.Attributes.XPressMemberSelection(Serialization.Attributes.XPressMemberSelectionType.OptIn)]
    public class ControlsClientType
    {
        /// <summary>
        /// Allows for the generation of a client type.
        /// </summary>
        /// <param name="typeName"></param>
        public ControlsClientType(Type original)
        {
            ControlType = original;
            LinkIds = new HashSet<string>();
            Map();
        }

        #region Static

        static ConcurrentDictionary<Type, ControlsClientType> m_clientTypeDefenitions = new ConcurrentDictionary<Type, ControlsClientType>();

        public static ControlsClientType Get(Type t)
        {
            if (!m_clientTypeDefenitions.ContainsKey(t))
            {
                m_clientTypeDefenitions.TryAdd(t, new ControlsClientType(t));
            }

            return m_clientTypeDefenitions[t];
        }

        #endregion

        #region members

        /// <summary>
        /// The type of the control that is associated with this type.
        /// </summary>
        public Type ControlType { get; private set; }

        /// <summary>
        /// The hash name of the client type.
        /// </summary>
        [XPress.Serialization.Attributes.XPressMember("Hash")]
        public string HashName { get; private set; }

        /// <summary>
        /// The client build command.
        /// </summary>
        [XPress.Serialization.Attributes.XPressMember("Command")]
        public string BuildCommand { get; private set; }

        /// <summary>
        /// If true the current type requires a specified type defenition to be downloaded to the client.
        /// </summary>
        public bool RequiresClientTypeDefenition { get; private set; }

        /// <summary>
        /// A collection of the link ids associated with this type.
        /// </summary>
        public HashSet<string> LinkIds { get; private set; }

        /// <summary>
        /// The link mapping infomation associated with the link.
        /// </summary>
        public LinkMapInfo LinkInfo { get; private set; }

        #endregion

        #region mapping

        void Map()
        {
            // adding the link mapping and invoking the activation events.
            LinkInfo = Mapper.GetMapInfo(ControlType);
            
            IEnumerable<LinkAttribute> scriptLinks = LinkInfo.Where(l => l.Type == Web.Links.LinkType.InitScriptFunction || l.Type == Web.Links.LinkType.Constructor).ToArray();
            
            StringWriter commandBuild = new StringWriter();

            scriptLinks.ForEach(l =>
            {
                LinkIds.TryAdd(l.UniqueId);
                commandBuild.WriteLine("$.extend(this, $.XPress.Scripts.Loaded['" + l.UniqueId + "']);if(this.$!=null)this.$();this.$=null;");
            });

            commandBuild.WriteLine("this.$=null;");

            this.RequiresClientTypeDefenition = scriptLinks.Count() > 0;
            BuildCommand = commandBuild.ToString();
            HashName = (ControlType.ToString() + BuildCommand).CreateMD5HashId();
        }

        #endregion
    }
}
