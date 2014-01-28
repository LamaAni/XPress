using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.StorageProviders;

namespace XPress.Serialization.Javascript
{

    public class JsonStringRefrenceBankStorageUnit : JsonRefrenceBankStorageUnit<string>
    {
        public JsonStringRefrenceBankStorageUnit(Reference.JsonRefrenceBank<string> bank = null)
            : base(bank == null ? new Reference.JsonRefrenceBank<string>(new Javascript.JsonStringArrayDataProvider()) : bank)
        {
        }
    }
}
