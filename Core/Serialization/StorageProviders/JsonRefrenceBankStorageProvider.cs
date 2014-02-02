using XPress.Serialization;
using XPress.Serialization.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.StorageProviders
{
    /// <summary>
    /// Creates a refrance bank storage provider.
    /// </summary>
    public abstract class JsonRefrenceBankStorageProvider<T> : JsonFileStorageProvider<JsonRefrenceBankStorageUnit<T>, T>
    {
        /// <summary>
        /// Creates the file storage serializer. 
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="extention"></param>
        /// <param name="path">If null, a directory named "Cache\\Serialzier" will be created if possible. Not then throw error.</param>
        public JsonRefrenceBankStorageProvider(IJsonSerializer<T> serialzier, string extention = "cache.dat", string path = null)
            : base(extention, path)
        {
            Serializer = serialzier;
        }

        public IJsonSerializer<T> Serializer { get; private set; }

        protected override byte[] ToByteArray(JsonRefrenceBankStorageUnit<T> unit)
        {
            if (unit.ReferenceBank == null)
                throw new Exception("Cannot find refrence bank in the unit.");

            // updating the bank and writing the bank data.
            unit.ReferenceBank.WriteToSource(true, true, UsePrettyJson);

            return Serializer.ToByteArray(unit.ReferenceBank.DataProvider.GetSource());
        }

        protected override JsonRefrenceBankStorageUnit<T> FromByteArray(byte[] bytes)
        {
            return ReadUnit(Serializer.ParseByteArray(bytes));
        }

        /// <summary>
        /// Creates a new bank unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        protected abstract JsonRefrenceBankStorageUnit<T> CreateNewUnit(T unit);

        /// <summary>
        /// Creates a new bank unit from source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected abstract JsonRefrenceBankStorageUnit<T> ReadUnit(T source);
    }
}
