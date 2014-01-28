using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.StorageProviders;
using XPress.StorageBank;

namespace XPress.Web.Razor.Storage
{
    /// <summary>
    /// Implements a class for diffrent storage banks required for XPress razor to run.
    /// </summary>
    public class XPressRazorCacheBanks
    {
        /// <summary>
        /// Creates a cache bank collection.
        /// </summary>
        /// <param name="directory"></param>
        public XPressRazorCacheBanks(string cacheDirectory)
        {
            SerializationBank = new Bank<BankStorageUnit>(
                new XPress.Serialization.StorageProviders.JsonSerializerFileStorageProvider<string>(
                    XPress.Serialization.Javascript.JsonStringSerializer.Global, "cache.ser.dat", cacheDirectory));

            SerialziedRefrenceBank = new Bank<JsonRefrenceBankStorageUnit<string>>(
                new XPress.Serialization.Javascript.JsonStringArrayRefrenceBankStorageProvider(
                    XPress.Serialization.Javascript.JsonStringSerializer.Global, "cache.ref.dat", cacheDirectory));
        }

        #region static

        ~XPressRazorCacheBanks()
        {
            CacheDirectory = "Cache\\";
        }

        /// <summary>
        /// The cache directory associated with the XPRess razor system, inside this directory all cache values will be stored.
        /// </summary>
        public static string CacheDirectory { get; set; }

        static XPressRazorCacheBanks _global;

        /// <summary>
        /// The global cache banks.
        /// </summary>
        public static XPressRazorCacheBanks Global
        {
            get
            {
                if (_global == null)
                    _global = new XPressRazorCacheBanks(CacheDirectory);
                return _global;
            }
        }

        #endregion

        #region members

        /// <summary>
        /// Implements a serialization bank that serizalizes its objects via the JsonSerializer. (Slow non refrence serialization).
        /// </summary>
        public Bank<BankStorageUnit> SerializationBank { get; private set; }

        /// <summary>
        /// Implements a serialization bank that serizalizes its objects via XPress.Serialization.Reference.JsonRefrenceBank.
        /// Values are loaded via id.
        /// </summary>
        public Bank<JsonRefrenceBankStorageUnit<string>> SerialziedRefrenceBank { get; private set; }
        
        #endregion
    }
}
