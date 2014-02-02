using System;
using System.Collections;
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

        #region IJsonObject Members

        /// <summary>
        /// Converts the json object to key value pairs.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public KeyValuePair<TKey,TValue>[] ToKeyValuePairs<TKey,TValue>()
            where TKey : IJsonValue<T>
            where TValue : IJsonValue<T>
        {
            return this.Select(jp => new KeyValuePair<TKey, TValue>((TKey)jp.Key, (TValue)jp.Value)).ToArray();
        }

        /// <summary>
        /// Converts the json object to key value pairs.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public KeyValuePair<string, TValue>[] ToKeyValuePairs<TValue>()
            where TValue : IJsonValue<T>
        {
            return this.Select(jp => new KeyValuePair<string, TValue>((jp.Key as JsonData<T>).Value as string, (TValue)jp.Value)).ToArray();
        }


        /// <summary>
        /// Converts the json object to key value pairs.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public KeyValuePair<TKey, TValue>[] ToKeyValuePairs<TKey, TValue>(Func<IJsonValue<T>, TKey> convertKey, Func<IJsonValue<T>, TValue> convertVal)
        {
            return this.Select(jp => new KeyValuePair<TKey, TValue>(convertKey(jp.Key), convertVal(jp.Value))).ToArray();
        }

        /// <summary>
        /// Converts the json object to key value pairs.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public KeyValuePair<string, TValue>[] ToKeyValuePairs<TValue>(Func<IJsonValue<T>, TValue> convertVal)
        {
            return this.Select(jp => new KeyValuePair<string, TValue>((jp.Key as JsonData<T>).Value as string, convertVal(jp.Value))).ToArray();
        }

        #endregion

        #region Find commands

        /// <summary>
        /// Finds a pair of values in the case the key is a core type.
        /// Only searches throught json keys that are of type JsonData.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The pair, or null if not found</returns>
        public JsonPair<T> FindPair(object key)
        {
            foreach(JsonPair<T> p in this)
            {
                JsonData<T> k = p.Key as JsonData<T>;
                if (k == null)
                    continue;
                if (k.Value.Equals(key))
                    return p;
            }
            return null;
        }

        #endregion
    }

}
