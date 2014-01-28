using XPress.Coding.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.StorageBank
{
    /// <summary>
    /// Holds information and allows for caching of specific objects.
    /// </summary>
    public class Bank<SUType>
        where SUType : BankStorageUnit
    {
        public Bank(IStorageProvider<SUType> provider, TimeSpan? keepInMemoryInterval = null, TimeSpan? cleanCacheInterval = null)
        {
            UnitCache = new CacheCollection<SUType>(cleanCacheInterval == null ? GlobalCleanCacheInterval : cleanCacheInterval.Value);
        }

        #region static members

        public static TimeSpan GlobalCleanCacheInterval = new TimeSpan(0, 0, 1);

        #endregion

        #region members

        /// <summary>
        /// Provides a source to store and load the service (page and all controls).
        /// </summary>
        public IStorageProvider<SUType> StorageProvider { get; set; }

        /// <summary>
        /// A collection of memory cached units.
        /// </summary>
        public CacheCollection<SUType> UnitCache { get; private set; }

        /// <summary>
        /// Internal for memory clearing.
        /// </summary>
        DateTime m_lastMemoryClear = DateTime.MinValue;

        /// <summary>
        /// A collection of locking objects by id that will allow to lock the objects in the bank when creating or destroing.
        /// </summary>
        ConcurrentDictionary<string, object> m_lockBankStorableHelpers = new ConcurrentDictionary<string, object>();

        #endregion

        #region locking

        /// <summary>
        /// Gets the locking object or creates one if one dose not exist.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected object GetLockObject(string id)
        {
            object o;
            if (!m_lockBankStorableHelpers.TryGetValue(id, out o))
            {
                m_lockBankStorableHelpers.TryAdd(id, new object());
                o = m_lockBankStorableHelpers[id];
            }
            return o;
        }

        /// <summary>
        /// Clears the locking object from memory.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected bool ClearLockObject(string id)
        {
            return m_lockBankStorableHelpers.TryRemove(id);
        }

        #endregion

        #region storing/loading methods

        /// <summary>
        /// Stores a bank object to the correct storage provider.
        /// </summary>
        /// <param name="unit">The obj to store</param>
        /// <param name="appendix">in case the object dose not have an id, this appendix will be added to the generated id.</param>
        /// <param name="prefex">in case the object dose not have an id, this prefex will be added to the generated id.</param>
        public void Store(SUType unit, string prefex = null, string appendix = null, bool forceDirectStorage = false)
        {
            if (unit.Id == null)
                unit.Id = StorageProvider.GetNewUnitId(prefex, appendix);

            if (!forceDirectStorage && unit.IsCached)
            {
                UnitCache.Set(unit.Id, unit);

                // This object is cached and therefore it can allow itself to be pended. (Better preformance).
                StorageProvider.PendUnit(unit.Id, unit);
                StorageProvider.UpdatePending();
            }
            else StorageProvider.WriteUnit(unit.Id, unit);
        }

        /// <summary>
        /// Loads the object from the storage provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="binder"></param>
        /// <returns></returns>
        public SUType Load(string id)
        {
            SUType o;
            if (!UnitCache.TryGet(id, out o))
            {
                // can only be loaded once, all else must wait for deserialize.
                lock (GetLockObject(id))
                {
                    // Second check to see whethere another process has already loaded.
                    if (!UnitCache.TryGet(id, out o))
                    {
                        o = StorageProvider.ReadUnit(id);
                        if (o != null)
                        {
                            o.LastDeserialized = DateTime.UtcNow;
                            UnitCache.Set(id, o);
                        }
                    }
                    ClearLockObject(id);
                }
            }

            if (o != null)
                o.LastAccessed = DateTime.UtcNow;

            // cleaning cache if needed.
            this.UnitCache.CleanCacheIfNeeded();

            return o;
        }

        /// <summary>
        /// Called to collect the cache garbage.
        /// </summary>
        /// <param name="force">If true, all the cache will be immidietly cleared.</param>
        public void CleanCache(bool force = false)
        {
            if (force)
                this.UnitCache.Clear();
            else this.UnitCache.ForceCleanCache();
        }

        /// <summary>
        /// Returns all the objects in the collection that match the specific id.
        /// </summary>
        /// <param name="MatchId">The function to match the id.</param>
        /// <returns>The object collection</returns>
        public KeyValuePair<string, SUType>[] LoadById(Func<string, bool> predict)
        {
            string[] validIds = this.StorageProvider.GetAllIds().Where(id => predict(id)).ToArray();
            return validIds.Select(id => new KeyValuePair<string, SUType>(id, Load(id))).ToArray();
        }

        /// <summary>
        /// Returns true if the object is in memory.
        /// </summary>
        /// <param name="id"></param>
        public bool IsInMemory(string id)
        {
            return UnitCache.Contains(id);
        }

        /// <summary>
        /// Deletes the object data from the bank;
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            StorageProvider.DeleteUnit(id);
        }

        #endregion
    }
}
