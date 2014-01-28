using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XPress.Serialization;
using XPress.Serialization.Attributes;
using XPress.StorageBank;
using XPress.Web.JavascriptClient.Request;
using XPress.Web.JavascriptClient.Response;
using System.Collections.Concurrent;
using XPress.Web.JCom.Com;
using XPress.Web.Razor.Storage;

namespace XPress.Web.JavascriptClient
{
    /// <summary>
    /// Implements a basic json client to handle client server communications.
    /// THIS OBJECT WILL BE SERIALIZED TO JSON!! according to the XPress.Serialization.XPressMemberSelection rules.
    /// </summary>
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public class JClient
    {
        /// <summary>
        /// Used as constructor to create an old instance of the object state.
        /// </summary>
        public JClient()
            : base()
        {
            KeepInMemoryInterval = new TimeSpan(0, 0, 1);
            ClientRegisterMessage = "Hi, and welcome.";
        }

        static JClient()
        {
            JClientRequest.AddTranslator("System", (doc, client) =>
            {
                Request.SystemCommands cmnd = SystemCommands.Unknown;
                Dictionary<string, string> dic = doc.FromJSJson<Dictionary<string, string>>();
                if (dic.ContainsKey("Command"))
                    Enum.TryParse<Request.SystemCommands>(dic["Command"], out cmnd);
                return new Request.JClientSystemRequestCommand(cmnd);
            });

            // call to validate translators for jcom.
            JComClient.ValidateInitialized();
        }

        #region helper classes
        class JComObjectSource : IJComObjectSource
        {
            public JComObjectSource(JClient client)
            {
                Client = client;
            }

            public JClient Client { get; private set; }

            #region IJComObjectSource Members

            /// <summary>
            /// Gets the object id. Adds the object to the collection if dose not exist.
            /// </summary>
            /// <param name="o"></param>
            /// <returns></returns>
            public ulong GetObjectId(object o)
            {
                return Client.Cache.Store(o);
            }

            /// <summary>
            /// Returns the object from the id.
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public object GetObject(uint id)
            {
                return Client.Cache.Load(id);
            }

            #endregion
        }

        #endregion

        #region Jcom objects members

        /// <summary>
        /// A collection of loaded type def that were sent to the client side.
        /// </summary>
        [XPressMember]
        internal HashSet<Type> LoadedTypeDefs = new HashSet<Type>();

        /// <summary>
        /// A collection of new type defenitions that need registration on the client side.
        /// </summary>
        [XPressIgnore]
        internal HashSet<Type> NewTypeDefs = new HashSet<Type>();

        #endregion

        #region members

        /// <summary>
        /// If id exists.
        /// </summary>
        [XPressIgnore]
        public string Id
        {
            get { return StorageUnit.Id; }
        }

        /// <summary>
        /// The storage unit associated with the bank.
        /// </summary>
        [XPressIgnore]
        public XPress.Serialization.StorageProviders.JsonRefrenceBankStorageUnit<string> StorageUnit { get; internal set; } 

        /// <summary>
        /// The refrence bank associated with the client.
        /// </summary>
        [XPressIgnore]
        public XPress.Serialization.Reference.JsonRefrenceBank<string> ReferenceBank { get { return StorageUnit.ReferenceBank; } }

        /// <summary>
        /// The interval for which to keep the JClient in memory.
        /// </summary>
        public TimeSpan KeepInMemoryInterval { get; private set; }

        /// <summary>
        /// Internval value for the jcom client.
        /// </summary>
        [XPressIgnore]
        JComClient _JComClien = null;

        /// <summary>
        /// The jcom client associated with the current javascript client.
        /// </summary>
        [XPressIgnore]
        public virtual JComClient JComClient { get { if (_JComClien == null)_JComClien = new JComClient(new JComObjectSource(this), JCom.Compilers.Specialized.JavaScriptCompiler.Global); return _JComClien; } }

        /// <summary>
        /// The client registration message that appears when the client is first registered.
        /// </summary>
        [XPressIgnore]
        public string ClientRegisterMessage { get; set; }

        [XPressIgnore]
        JClientOptions m_Options;

        /// <summary>
        /// The options collection to initialzie the client with, initialzied with the client
        /// </summary>
        [XPressMember]
        public JClientOptions Options
        {
            get { return m_Options; }
            set { m_Options = value; }
        }

        /// <summary>
        /// The id of the client state associated with this client. 
        /// </summary>
        [XPressMember]
        public string ClientStateId { get; internal set; }

        /// <summary>
        /// The client state associated with this client.
        /// </summary>
        [XPressIgnore]
        public JClientState State { get; internal set; }

        /// <summary>
        /// The context objects associated with the client object. (XPress.Serialization handled) - partiall objects serialization.
        /// </summary>
        [XPressIgnore]
        public XPress.Serialization.Reference.JsonRefrenceBank<string> Cache { get; internal set; }

        #endregion

        #region Construction of call objects

        /// <summary>
        /// Creates a new client call to handle information called from the client. 
        /// All command handling will be done by this object.
        /// </summary>
        /// <returns></returns>
        public virtual JClientCall CreateClientCall(JClientRequest request, JClientResponse response)
        {
            return new JClientCall(this, request, response);
        }

        #endregion

        #region JCom object managment

        /// <summary>
        /// Registers a new type definition to be
        /// </summary>
        /// <param name="t"></param>
        internal void RegisterJComObjectType(Type t)
        {
            // adding the new types if nesscecary.
            if (LoadedTypeDefs.Contains(t))
                return;
            LoadedTypeDefs.TryAdd(t);
            NewTypeDefs.TryAdd(t);
        }

        #endregion

        #region storage to banks

        /// <summary>
        /// Stores the storage unit to the bank.
        /// </summary>
        /// <param name="bank"></param>
        public void Store(XPressRazorCacheBanks bank)
        {
            bank.SerialziedRefrenceBank.Store(StorageUnit);
        }

        #endregion
    }
}
