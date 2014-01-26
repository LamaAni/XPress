using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Javascript
{
    public class JsonStringArrayRefrenceBankStorageProvider : StorageProviders.JsonRefrenceBankStorageProvider<string>
    {
        public JsonStringArrayRefrenceBankStorageProvider(IJsonSerializer<string> serializer, string extention = "cache.dat", string path = null)
            : base(serializer, extention, path)
        {
        }

        protected override StorageProviders.JsonRefrenceBankStorageUnit<string> CreateNewUnit(string unit)
        {
            return new StorageProviders.JsonRefrenceBankStorageUnit<string>(new Reference.JsonRefrenceBank<string>(new JsonStringArrayDataProvider(null, Serializer)));
        }

        protected override StorageProviders.JsonRefrenceBankStorageUnit<string> ReadUnit(string source)
        {
            return new StorageProviders.JsonRefrenceBankStorageUnit<string>(new Reference.JsonRefrenceBank<string>(new JsonStringArrayDataProvider(source, Serializer)));
        }
    }
}
