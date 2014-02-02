using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using XPress.Serialization.Attributes;
using XPress.Serialization.Javascript;
using XPress.Web.Razor.Storage;

namespace XPress.Web.JavascriptClient
{
    /// <summary>
    /// Represents the generic json client state, allows for mapping of the request state and the client state.
    /// This client object must be allowed to be serialzied via the Json serialization. (XPress.Serialization)
    /// </summary>
    [Serialization.Attributes.XPressMemberSelection(Serialization.Attributes.XPressMemberSelectionType.OptIn)]
    public class JClientState
    {
        /// <summary>
        /// Creates a new client state that allows for creation of a base service.
        /// </summary>
        /// <param name="clientId">The client id, that will be controlled by this state.</param>
        internal JClientState()
        {
            LastHartbeat = DateTime.Now;
        }

        #region static members

        /// <summary>
        /// The time to keep the service alive when no contact from the client is made. This parameter has an effect on memory.
        /// </summary>
        public static TimeSpan ServiceNoContactKeepAliveTime = new TimeSpan(0, 5, 0);

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
        public XPress.StorageBank.BankStorageSingleValueUnit StorageUnit { get; internal set; } 

        /// <summary>
        /// The JClient id.
        /// </summary>
        [XPressMember]
        public string ClientId { get; internal set; }

        /// <summary>
        /// If the client has been changed in any way, this will be true to allow for updates.
        /// </summary>
        [XPressMember]
        public bool HasPendingResponses { get; internal set; }

        /// <summary>
        /// The last time this client has recived a call from the client.
        /// </summary>
        [XPressMember]
        public DateTime LastHartbeat { get; private set; }

        /// <summary>
        /// returns true if the current service is still alive.
        /// </summary>
        public bool IsAlive { get { return (DateTime.Now - LastHartbeat) <= ServiceNoContactKeepAliveTime; } }

        /// <summary>
        /// The currently executing request id, if -1, no request is exceuting.
        /// EXISTS ONLY IN BUFFERED REQUEST MODE. (i.e. client calls PostCommand).
        /// </summary>
        [XPressMember]
        public int ExecutingRequestId { get; internal set; }

        /// <summary>
        /// The id of the last request, any request lower in id then this request will be considered old and obsolete.
        /// EXISTS ONLY IN BUFFERED REQUEST MODE. (i.e. client calls PostCommand).
        /// </summary>
        [XPressMember]
        public int LastRequestId { get; internal set; }

        /// <summary>
        /// If true, the client has resent a request telling the server it has entered wait for response mode. In this mode, no result should be 
        /// returned to the client - insted the response commands should be stored for the next json command that comes to the sever and join with that.
        /// EXISTS ONLY IN BUFFERED REQUEST MODE. (i.e. client calls PostCommand).
        /// </summary>
        [XPressMember]
        [XPressIgnore(XPressIgnoreMode.IfNull)]
        public bool IsInWaitForResponseMode { get; internal set; }

        /// <summary>
        /// A string that represents a response waiting to be sent to the client.
        /// </summary>
        [XPressMember]
        [XPressIgnore(XPressIgnoreMode.IfNull)]
        public string WaitingResponse { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Called to mark the current time as the last hartbeat time.
        /// </summary>
        public void MarkHartbeat()
        {
            LastHartbeat = DateTime.Now;
        }

        /// <summary>
        /// Stores the storage unit to the bank.
        /// </summary>
        /// <param name="bank"></param>
        public void Store(XPressRazorCacheBanks bank)
        {
            bank.SerializationBank.Store(StorageUnit);
        }

        #endregion

        #region static methods

        /// <summary>
        /// The interval in which the state will collect its garbage.
        /// </summary>
        public static TimeSpan StateGarbageCollectionIntervalTime = new TimeSpan(0, 20, 0);

        /// <summary>
        /// The last time when the state garbage was collected.
        /// </summary>
        public static DateTime LastStateGarabgeCollection { get; private set; }

        /// <summary>
        /// If true currenty collecting the state garbage.
        /// </summary>
        public static bool IsCollectingStateGarbage { get; private set; }

        /// <summary>
        /// Collects the state garbage from the bank collection.
        /// </summary>
        /// <param name="bank"></param>
        public static void CollectStateGarbage(Razor.Storage.XPressRazorCacheBanks bank = null)
        {
            if (IsCollectingStateGarbage || LastStateGarabgeCollection + StateGarbageCollectionIntervalTime > DateTime.Now) return;
            IsCollectingStateGarbage = true;
            Task.Run(() =>
            {
                __collectStateGarbage(bank);
            });
        }

        static void __collectStateGarbage(Razor.Storage.XPressRazorCacheBanks bank)
        {
            if (bank == null)
                bank = Razor.Storage.XPressRazorCacheBanks.Global;

            bank.SerializationBank.LoadById(id=>id.EndsWith(".state")).ForEach(kvp =>
            {
                if (kvp.Value == null)
                {
                    bank.SerializationBank.Delete(kvp.Key);
                }
                else
                {
                    JClientState state = kvp.Value.Value as JClientState;
                    state.StorageUnit = kvp.Value;
                    if (!state.IsAlive)
                        JClientState.DestroyClient(state, bank);
                }
            });

            IsCollectingStateGarbage = false;
            LastStateGarabgeCollection = DateTime.Now;
        }

        /// <summary>
        /// Loads the client state and makes this client state the current running client state.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bank"></param>
        /// <returns></returns>
        public static JClientState LoadClientState(string id, XPressRazorCacheBanks bank = null)
        {
            if (bank == null)
                bank = XPressRazorCacheBanks.Global;
            CollectStateGarbage(bank);
            XPress.StorageBank.BankStorageSingleValueUnit unit = bank.SerializationBank.Load(id);
            if (unit == null)
                return null;
            JClientState state = unit.Value as JClientState;
            state.StorageUnit = unit;
            return state;
        }

        /// <summary>
        /// Destroies a specific client using the storage bank.
        /// </summary>
        /// <param name="state">The object state associated with the client.</param>
        /// <returns>The object state</returns>
        public static void DestroyClient(JClientState state, XPressRazorCacheBanks bank = null)
        {
            if (bank == null)
                bank = XPressRazorCacheBanks.Global;

            // destroing the state.
            bank.SerializationBank.Delete(state.StorageUnit.Id);
            bank.SerializationBank.Delete(state.ClientId);
        }

        /// <summary>
        /// Creates the client for the JClient template, and initialzies the client and client state.
        /// </summary>
        /// <param name="bank">The storage bank associated with the client.</param>
        /// <param name="markAsCurrent">Mark JClient and JClient state as the curent client and state.</param>
        /// <param name="template">The template to create the client for.</param>
        /// <returns></returns>
        internal static JClient InitClient(JClientTemplate template, XPressRazorCacheBanks bank = null, bool markAsCurrent = true)
        {
            if (bank == null)
                bank = XPressRazorCacheBanks.Global;
            CollectStateGarbage(bank);

            // Creates a new instance of jclient.
            JClient client = template.CreateClient();

            if (client == null)
                throw new Exception("Generated client must not be null.");

            // Crete the client state.
            JClientState state = new JClientState();
            state.StorageUnit = new StorageBank.BankStorageSingleValueUnit(state, false);
            bank.SerializationBank.Store(state.StorageUnit, "jclient.", ".state");

            // Creating and storing the client.
            client.ClientStateId = state.Id;
            client.State = state;
            client.StorageUnit = new Serialization.Javascript.JsonStringRefrenceBankStorageUnit();
            client.StorageUnit.ReferenceBank.Store(client, true);
            bank.SerialziedRefrenceBank.Store(client.StorageUnit, state.Id + ".", ".client");

            // restoring the state to include the client id. Now they are connected.
            state.ClientId = client.Id;
            state.Store(bank);

            return client;
        }

        /// <summary>
        /// Loads the client from the client state.
        /// </summary>
        /// <param name="state">The client state to load from.</param>
        /// <returns></returns>
        internal static JClient LoadClient(JClientState state, XPressRazorCacheBanks bank = null, bool makeCurrent = true)
        {
            if (bank == null) bank = XPressRazorCacheBanks.Global;
            CollectStateGarbage(bank);
            XPress.Serialization.StorageProviders.JsonRefrenceBankStorageUnit<string> unit = bank.SerialziedRefrenceBank.Load(state.ClientId);
            JClient client = unit.ReferenceBank.Load(0) as JClient;
            client.StorageUnit = unit;
            client.State = state;
            return client;
        }


        #endregion
    }
}
