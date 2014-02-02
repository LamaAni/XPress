using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Map;

namespace XPress.Serialization.Converters
{
    /// <summary>
    /// Implements a converter for the net serialization.
    /// </summary>
    public class NetSerializationConverter<T> : JsonObjectConverter<T, object, JsonArray<T>>
    {
        public NetSerializationConverter()
        {
        }

        /// <summary>
        /// The default formatter to use when formatting a .Net ISerializable interface.
        /// </summary>
        public static FormatterConverter DefaultFormatterConverter = new FormatterConverter();

        public override void PopulateJsonValue(SerializationTypeMap<T> stm, object o, JsonArray<T> val, SerializationContext<T> context)
        {
            ISerializable asSer = o as ISerializable;
            if (asSer == null)
                throw new Exception(".Net serialization demands that the objects is of the interface ISerializable.");

            SerializationInfo info = new SerializationInfo(o.GetType(), DefaultFormatterConverter);
            asSer.GetObjectData(info, context.StreamingContext);

            // writing info to array.
            SerializationInfoEnumerator e = info.GetEnumerator();
            while (e.MoveNext())
            {
                val.Add(context.GetJsonValue(e.Current.Name, typeof(string)));
                Type otype = e.Current.Value == null ? typeof(object) : e.Current.Value.GetType();
                if (!context.IgnoreTypes)
                    val.Add(context.GetJsonValue(otype, typeof(Type)));
                val.Add(context.GetJsonValue(e.Current.Value, otype));
            }
        }

        public override void PopulateObjectValue(SerializationTypeMap<T> stm, object o, JsonArray<T> val, SerializationContext<T> context)
        {
            if (context.IgnoreTypes)
                throw new Exception("Cannot use ISerializeable without type mapping (Ignore types).");

            // populating the streaming constructor.
            SerializationInfo info = new SerializationInfo(o.GetType(), DefaultFormatterConverter);

            // reading the info.
            for (int i = 0; i < val.Count; i += 3)
            {
                string name = context.GetObject(val[i], typeof(string)) as string;
                Type t = context.GetObject(val[i + 1], typeof(Type)) as Type;
                object io = context.GetObject(val[i + 2], t);
                info.AddValue(name, io);
            }

            // invoking the deserializer.
            stm.Info.DeserializationConstructor.Invoke(o, new object[2] { info, context.StreamingContext });
        }
    }
}
