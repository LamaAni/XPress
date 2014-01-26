using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Documents;
using XPress.Serialization.Map;

namespace XPress.Serialization.Converters
{
    public class ArrayConverter<T> : JsonObjectConverter<T, Array, JsonArray<T>>
    {
        public override void PopulateJsonValue(SerializationTypeMap<T> stm, Array o, JsonArray<T> val, SerializationContext<T> context)
        {
            for (int i = 0; i < o.Length; i++)
            {
                object iv = o.GetValue(i);
                val.Add(context.GetJsonValue(iv, iv.GetType()));
            }
        }

        public override void PopulateObjectValue(SerializationTypeMap<T> stm, Array o, JsonArray<T> val, SerializationContext<T> context)
        {
            for (int i = 0; i < val.Count; i++)
            {
                o.SetValue(context.GetObject(val[i], stm.Info.ArrayElementType), i);
            }
        }

        public override Array CreateUninitializedInstance(SerializationTypeMap<T> stm, JsonArray<T> val, SerializationContext<T> context)
        {
            return Array.CreateInstance(stm.Info.ArrayElementType, val.Count);
        }
    }
}
