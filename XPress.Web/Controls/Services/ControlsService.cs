using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization;

namespace XPress.Web.Controls.Services
{
    /// <summary>
    /// The service that is attached to a specific page.
    /// </summary>
    public class ControlsService
    {
        /// <summary>
        /// Initializes the rmc service.
        /// </summary>
        public ControlsService(Controls.IControl root)
            : this()
        {
            Info = new ServiceInfo();
            ulong iId = Bank.Store(Info, true);
            Info.RootId = Bank.Store(root, true);
        }

        private ControlsService()
            : base()
        {
        }

        public static ControlsService FromJson(string json)
        {
            ControlsService n = new ControlsService();
            n.m_bank = ObjectStream.FromJson(json);
            n.Info = n.Bank.Load(0) as ServiceInfo;
            return n;
        }

        #region Static members

        [ThreadStatic]
        static ControlsService m_sService;

        /// <summary>
        /// The current executing service that is attached to the running thread.
        /// </summary>
        public static ControlsService Current { get { return m_sService; } set { m_sService = value; } }

        #endregion

        #region helper clases

        /// <summary>
        /// An information block for the current service.
        /// </summary>
        [XPress.Serialization.Attributes.XPressInheritedMemberSelectionAttribute(Serialization.Attributes.XPressMemberSelectionType.Properties)]
        public class ServiceInfo
        {
            public ServiceInfo()
            {
                PageId = null;
                LastAccessed = DateTime.MinValue;
                LastDeserialized = DateTime.MinValue;
                MarkedForUpdate = new HashSet<ulong>();
                LoadedClientTypes = new HashSet<uint>();
                LoadedLinks = new HashSet<string>();
            }

            #region members

            /// <summary>
            /// the page id associated with the service.
            /// </summary>
            public string PageId { get; internal set; }

            /// <summary>
            /// Sets the root control id.
            /// </summary>
            public ulong RootId { get; internal set; }

            /// <summary>
            /// The time (utc) this service was last loaded from serialization.
            /// </summary>
            public DateTime LastDeserialized { get; internal set; }

            /// <summary>
            /// The last accessed information.
            /// </summary>
            public DateTime LastAccessed { get; internal set; }

            /// <summary>
            /// A collection of controls that was marked for update in any previus calls or by any prev process.
            /// </summary>
            public HashSet<ulong> MarkedForUpdate { get; internal set; }

            /// <summary>
            /// A collection of links that were loaded.
            /// </summary>
            public HashSet<string> LoadedLinks { get; private set; }

            /// <summary>
            /// A collection of types that have been loaded onto the client side.
            /// </summary>
            public HashSet<uint> LoadedClientTypes { get; private set; }

            /// <summary>
            /// A collection of pending script calls to be invoked at the client.
            /// </summary>
            public Dictionary<string,string> PendingScriptCalls { get; private set; }

            #endregion
        }

        #endregion

        #region members

        public Call.ControlsServiceCall ExecutingCall { get; set; }

        /// <summary>
        /// The information block associated with the service.
        /// </summary>
        public ServiceInfo Info { get; private set; }

        XPress.Serialization.ObjectStream m_bank;

        /// <summary>
        /// The object bank of the service.
        /// </summary>
        public XPress.Serialization.ObjectStream Bank
        {
            get
            {
                if (m_bank == null)
                    m_bank = new ObjectStream();
                return m_bank;
            }
        }


        #endregion

        #region Control methods

        public ulong RegisterControl(Controls.ControlNodeInfo c)
        {
            // setting the unique id for the current control object.
            return Bank.Store(c);
        }

        /// <summary>
        /// Returns the root control associated with the service.
        /// </summary>
        /// <returns></returns>
        public IControl GetRootControl()
        {
            IControl root = Bank.Load(Info.RootId) as IControl;
            if (root is ITemplate)
                (root as ITemplate).RegisterToService(this);
            return root;
        }

        #endregion

        #region control map

        /// <summary>
        /// Returns the controls associated with the unique ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<IControl> GetControls(IEnumerable<ulong> ids)
        {
            return ids.Select(id => GetControl(id));
        }

        /// <summary>
        /// Retunrns the control associated with the unique id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IControl GetControl(ulong id)
        {
            return (Bank.Load(id) as ControlNodeInfo).Owner;
        }

        #endregion

        #region Update commands

        /// <summary>
        /// Mark the specified control as one that requires update.
        /// </summary>
        /// <param name="ni">The control node into that needs update.</param>
        /// <returns></returns>
        public bool MarkForUpdate(ControlNodeInfo ni)
        {
            return this.Info.MarkedForUpdate.TryAdd(ni.UnqiueId);
        }

        #endregion

        #region Loaded types

        /// <summary>
        /// Returns true if this specific client type has been loaded by the client.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool IsClientTypeLoaded(Type t)
        {
            return Info.LoadedClientTypes.Contains(this.Bank.Binder.GetTypeIdentity(t));
        }

        /// <summary>
        /// Marks the type as loaded by the client.
        /// </summary>
        /// <param name="t"></param>
        public void MarkClientTypeAsLoaded(Type t)
        {
            Info.LoadedClientTypes.TryAdd(this.Bank.Binder.GetTypeIdentity(t));
        }

        #endregion
    }
}
