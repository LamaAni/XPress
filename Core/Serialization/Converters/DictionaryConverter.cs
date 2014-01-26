using XPress.Serialization.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Map;

namespace XPress.Serialization.Converters
{
    /// <summary>
    /// Implements a dictionary converter that allows writing and reading dictionaries.
    /// </summary>
    public class DictionaryConverter<T> : JsonObjectConverter<T, IDictionary, JsonObject<T>>
    {
        public DictionaryConverter()
        {
        }

        public override void PopulateJsonValue(SerializationTypeMap<T> stm, IDictionary o, JsonObject<T> val, SerializationContext<T> context)
        {
            IDictionaryEnumerator dicEnum = o.GetEnumerator();
            while (dicEnum.MoveNext())
            {
                val.Add(new JsonPair<T>(context.GetJsonValue(dicEnum.Key, dicEnum.Key.GetType()), context.GetJsonValue(dicEnum.Value, dicEnum.Value.GetType())));
            }
        }

        public override void PopulateObjectValue(SerializationTypeMap<T> stm, IDictionary o, JsonObject<T> val, SerializationContext<T> context)
        {
            o.Clear();
            foreach (JsonPair<T> pair in val)
            {
                object key = context.GetObject(pair.Key, stm.Info.GenericTypeArguments[0]);
                object value = context.GetObject(pair.Value, stm.Info.GenericTypeArguments[1]);
                o.Add(key, value);
            }
        }

        public override IDictionary CreateUninitializedInstance(SerializationTypeMap<T> stm, JsonObject<T> val, SerializationContext<T> context)
        {
            return Activator.CreateInstance(stm.MappedType) as IDictionary;
        }
    }
}
