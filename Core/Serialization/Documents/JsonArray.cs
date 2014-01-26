using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Documents
{
    /// <summary>
    /// Implements a json array to be read as inner
    /// </summary>
    public class JsonArray<T> :JsonEnumrableObject<T,IJsonValue<T>>, IJsonValue<T>
    {
        public JsonArray()
        {
        }

        /// <summary>
        /// Used by the reader and writer to create json objects override this to handle specific object loading.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public virtual void AddRawValue(IJsonValue<T> val, bool isDirective)
        {
            if (isDirective)
            {
                this.AddDirective((JsonData<T>)val);
            }
            else this.Add(val);
        }

        public override string ToString()
        {
            return "Array {" + this.Count + "}";
        }
    }
}
