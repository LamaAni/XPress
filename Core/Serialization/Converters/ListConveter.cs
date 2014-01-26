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
    /// Represents a general list converter that writes a list/reads a list from JSON
    /// </summary>
    public class ListConveter<T> : JsonObjectConverter<T, IList, JsonArray<T>>
    {
        public override void PopulateJsonValue(SerializationTypeMap<T> stm, IList o, JsonArray<T> val, SerializationContext<T> context)
        {
            val.Clear();
            foreach (object io in o)
                val.Add(context.GetJsonValue(io, io.GetType()));
        }

        public override void PopulateObjectValue(SerializationTypeMap<T> stm, IList o, JsonArray<T> val, SerializationContext<T> context)
        {
            o.Clear();
            val.ForEach(jv =>
            {
                o.Add(context.GetObject(jv, stm.Info.GenericTypeArguments[0]));
            });
        }

        public override IList CreateUninitializedInstance(SerializationTypeMap<T> stm, JsonArray<T> val, SerializationContext<T> context)
        {
            return Activator.CreateInstance(stm.MappedType) as IList;
        }
    }
}
