using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Documents
{
    /// <summary>
    /// Implements a json object. Members are loaded into the collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonObject<T> : JsonEnumrableObject<T, JsonPair<T>>, IJsonValue<T>
    {
        public JsonObject()
        {
        }

        /// <summary>
        /// Used by the reader and writer to create json objects override this to handle specific object loading.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public virtual void AddRawValue(IJsonValue<T> key, IJsonValue<T> val, bool isDirective)
        {
            if (isDirective)
                this.AddDirective(key as JsonData<T>); //value is ignored.
            else this.Add(new JsonPair<T>(key, val));
        }

        public override string ToString()
        {
            return "Object {" + this.Count + "}";
        }
    }
}
