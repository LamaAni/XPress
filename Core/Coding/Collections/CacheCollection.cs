using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Coding.Collections
{
    /// <summary>
    /// Implements a cache collection to allow for saving objects, by refrence or by timeout. 
    /// The object will be kept in memory either if the object has a refence somewhere in the program or a
    /// specified amount of time has not passed.
    /// </summary>
    public class CacheCollection<T>
        where T : class, ICacheCollectable
    {
        public CacheCollection(TimeSpan cleanInterval)
        {
            CleanInterval = cleanInterval;
        }

        #region members

        DateTime m_lastCacheClearTime = DateTime.MinValue;

        /// <summary>
        /// The time between cache clearing if ClearCacheIfNeeded is called.
        /// </summary>
        public TimeSpan CleanInterval { get; private set; }

        System.Collections.Concurrent.ConcurrentDictionary<string, T> m_timeoutKept =
            new System.Collections.Concurrent.ConcurrentDictionary<string, T>();

        System.Collections.Concurrent.ConcurrentDictionary<string, WeakReference> m_refrenceKept =
            new System.Collections.Concurrent.ConcurrentDictionary<string, WeakReference>();

        /// <summary>
        /// True if the collection requires cache cleaning.
        /// </summary>
        public bool RequiresCacheCleaning { get { return m_lastCacheClearTime + CleanInterval < DateTime.Now; } }

        #endregion

        #region methods

        /// <summary>
        /// Returns the chached object count.
        /// </summary>
        /// <returns></returns>
        public int GetCachedObjectCount()
        {
            return m_timeoutKept.Keys.Union(m_refrenceKept.Keys).Count();
        }

        /// <summary>
        /// If true the object is in the cache collection.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            if (key == null)
                return false;
            return m_timeoutKept.ContainsKey(key) || m_refrenceKept.ContainsKey(key);
        }

        /// <summary>
        /// Returns the object from the cache.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(string key)
        {
            T o = null;
            if (TryGet(key, out o))
            {
                return o;
            }
            else throw new Exception("An object with that key dose not exist in the collection.");
        }

        /// <summary>
        /// Attempts to get the object if exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool TryGet(string key, out T o)
        {
            o = null;
            WeakReference wref;
            if (m_refrenceKept.TryGetValue(key, out wref) && wref.IsAlive)
            {
                o = wref.Target as T;
                return true;
            }
            else if (m_timeoutKept.TryGetValue(key, out o))
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Sets a value to the collection and creates a new entery for the refrence.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Set(string key, T val)
        {
            if (val.KeepInMemoryInterval.TotalMilliseconds > 0)
                m_timeoutKept[key] = val;
            m_refrenceKept[key] = new WeakReference(val);
        }

        /// <summary>
        /// Clear the cache for dead objects if CacheClearInterval has elapsed.
        /// </summary>
        public bool CleanCacheIfNeeded(bool async = true)
        {
            if (RequiresCacheCleaning && !_isCacheCleaning)
            {
                m_lastCacheClearTime = DateTime.Now;
                if (async)
                {
                    Task.Run(() =>
                    {
                        ForceCleanCache();
                    });
                }
                else ForceCleanCache();
                return true;
            }
            return false;
        }

        bool _isCacheCleaning = false;

        /// <summary>
        /// Clears the cache for dead objects.
        /// </summary>
        public void ForceCleanCache()
        {
            _isCacheCleaning = true;
            Exception err = null;
            try
            {
                m_lastCacheClearTime = DateTime.Now;
                // clearing dead refrences.
                m_refrenceKept.ToArray().ForEach((kvp) =>
                {
                    if (!kvp.Value.IsAlive)
                        m_refrenceKept.TryRemove(kvp.Key);
                });

                // clearing obsolete bank storable objects.
                m_timeoutKept.ToArray().ForEach(kvp =>
                {
                    if (kvp.Value.LastAccessed + kvp.Value.KeepInMemoryInterval < DateTime.Now)
                        m_timeoutKept.TryRemove(kvp.Key);
                });
            }
            catch (Exception ex)
            {
                err = ex;
            }
            finally
            {
                _isCacheCleaning = false;
                if (err != null)
                    throw err;
            }
        }

        /// <summary>
        /// Empties the collection
        /// </summary>
        public void Clear()
        {
            m_refrenceKept.Clear();
            m_timeoutKept.Clear();
        }

        #endregion
    }
}
