using XPress.Serialization;
using XPress.StorageBank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.StorageProviders
{
    /// <summary>
    /// Implements a json serializer file storage provider.
    /// </summary>
    public class JsonSerializerFileStorageProvider<SUType, T> : JsonFileStorageProvider<SUType, T>
        where SUType : BankStorageUnit
    {
        /// <summary>
        /// Creates the file storage serializer. 
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="extention"></param>
        /// <param name="path">If null, a directory named "Cache\\Serialzier" will be created if possible. Not then throw error.</param>
        public JsonSerializerFileStorageProvider(IJsonSerializer<T> serialzier, string extention = "cache.dat", string path = null)
            : base(extention, path)
        {
            Serializer = serialzier;
        }

        public IJsonSerializer<T> Serializer { get; private set; }

        protected override byte[] ToByteArray(SUType unit)
        {
            return Serializer.ToByteArray(Serializer.Serialize(unit, false, UsePrettyJson));
        }

        protected override SUType FromByteArray(byte[] bytes)
        {
            return Serializer.Deserialize(Serializer.ParseByteArray(bytes), typeof(SUType)) as SUType;
        }
    }
}
