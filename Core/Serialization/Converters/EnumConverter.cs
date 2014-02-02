using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Documents;

namespace XPress.Serialization.Converters
{
    /// <summary>
    /// Implements a simple string converter for enum types.
    /// </summary>
    public class EnumConverter<T> : JsonObjectConverter<T, Enum, JsonData<T>>
    {
        public EnumConverter()
        {
            this.IgnoreTypeDirectives = true;
        }

        public override void PopulateJsonValue(Map.SerializationTypeMap<T> stm, Enum o, JsonData<T> val, SerializationContext<T> context)
        {
            // nothing to do, proceed to enum converter.
        }

        public override void PopulateObjectValue(Map.SerializationTypeMap<T> stm, Enum o, JsonData<T> val, SerializationContext<T> context)
        {
            // nothing to do here, convertsion is done at object creation.
        }

        public override JsonData<T> CreateUinitializedJsonValue(Map.SerializationTypeMap<T> stm, Enum o, SerializationContext<T> context)
        {
            return new JsonData<T>(o.ToString());
        }

        public override Enum CreateUninitializedInstance(Map.SerializationTypeMap<T> stm, JsonData<T> val, SerializationContext<T> context)
        {
            return Enum.Parse(stm.MappedType, val.Value as string) as Enum;
        }
    }
}
