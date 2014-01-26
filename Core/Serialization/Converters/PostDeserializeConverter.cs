using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Documents;

namespace XPress.Serialization.Converters
{
    /// <summary>
    /// Implements a post deserialziation value converter.
    /// </summary>
    public class PostDeserializeConverter<T> : JsonObjectConverter<T,IPostDeserialize,IJsonValue<T>>
    {
        public PostDeserializeConverter()
        {
            IgnoreTypeDirectives = true;
        }

        public override void PopulateJsonValue(Map.SerializationTypeMap<T> stm, IPostDeserialize o, IJsonValue<T> val, SerializationContext<T> context)
        {
            // nothing to do here, object value is the internal object data value, as was created in the initialiations.
        }

        public override void PopulateObjectValue(Map.SerializationTypeMap<T> stm, IPostDeserialize o, IJsonValue<T> val, SerializationContext<T> context)
        {
            // nothing to do here all object data was created at the object creation.
        }

        public override IPostDeserialize CreateUninitializedInstance(Map.SerializationTypeMap<T> stm, IJsonValue<T> val, SerializationContext<T> context)
        {
            return Activator.CreateInstance(stm.MappedType, context, val) as IPostDeserialize;
        }

        public override IJsonValue<T> CreateUinitializedJsonValue(Map.SerializationTypeMap<T> stm, IPostDeserialize o, SerializationContext<T> context)
        {
            if(o.HasValue)
            {
                return context.GetJsonValue(o.GetValue(), stm.Info.GenericTypeArguments[0]);
            }
            else
            {
                IJsonValue<T> val = o.Data as IJsonValue<T>;
                if (val == null)
                    throw new Exception("Attempted to read data source '" + o.Data.GetType() + "' with a '" + typeof(IJsonValue<T>) + "' reader.");
                return val;
            }
        }
    }
}
