using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Map;

namespace XPress.Serialization.Converters
{
    /// <summary>
    /// Implements a json serializable conveter, that may be used to convert a serializabtion value.
    /// </summary>
    public class JsonSerializeableConverter<T> : JsonObjectConverter<T, object, JsonArray<T>>
    {
        public override void PopulateJsonValue(SerializationTypeMap<T> stm, object o, JsonArray<T> val, SerializationContext<T> context)
        {
            // irrelevent for the value of the object. This is always an oject array of two values.
            object serO = (o as IJsonSerializable).ToSerializationObject();
            Type ot = serO.GetType();
            if (!context.IgnoreTypes)
                val.Add(context.GetJsonValue(ot, typeof(Type)));
            val.Add(context.GetJsonValue(serO, ot));
        }

        public override void PopulateObjectValue(SerializationTypeMap<T> stm, object o, JsonArray<T> val, SerializationContext<T> context)
        {
            if (context.IgnoreTypes)
                throw new Exception("Cannot use IJsonSerializeable without type mapping (Ignore types).");
            // deserializing.
            Type ot = context.GetObject(val[0], typeof(Type)) as Type;
            object serO = context.GetObject(val[1], ot);
            (o as IJsonSerializable).FromSerialziationObject(o);
        }
    }
}
