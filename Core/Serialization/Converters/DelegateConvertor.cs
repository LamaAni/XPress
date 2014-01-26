using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Documents;
using XPress.Serialization.Map;

namespace XPress.Serialization.Converters
{
    public class DelegateConvertor<T> : JsonObjectConverter<T, Delegate, JsonArray<T>>
    {
        public override void PopulateJsonValue(SerializationTypeMap<T> stm, Delegate o, JsonArray<T> val, SerializationContext<T> context)
        {
            // nothing to do here. this is not, by definition, a refrence type.
        }

        public override void PopulateObjectValue(SerializationTypeMap<T> stm, Delegate o, JsonArray<T> val, SerializationContext<T> context)
        {
            // nothing to do here. this is not, by definition, a refrence type.
        }

        public override JsonArray<T> CreateUinitializedJsonValue(SerializationTypeMap<T> stm, Delegate o, SerializationContext<T> context)
        {
            // creating the uninitialzied json value, that applies to this object.
            JsonArray<T> ar = new JsonArray<T>();
            ar.Add(context.GetJsonValue(o.Method,typeof(MethodInfo)));
            ar.Add(new JsonData<T>(o.Target == null ? null : context.ParseTypeToValue(o.Target.GetType())));
            ar.Add(context.GetJsonValue(o.Target, o.Target == null ? typeof(object) : o.Target.GetType()));
            return ar;
        }

        public override Delegate CreateUninitializedInstance(SerializationTypeMap<T> stm, JsonArray<T> val, SerializationContext<T> context)
        {
            if (val.Count < 3)
                throw new Exception("Incompatible array length for delegate conversion.");
            MethodInfo mi = (MethodInfo)context.GetObject(val[0], typeof(MemberInfo));
            JsonData<T> typeid = val[1] as JsonData<T>;
            Type oType = null; 
            object target = null;
            if (typeid.Value != null)
            {
                oType = context.PraseTypeFromValue(typeid.Value);
                target = context.GetObject(val[2], oType);
            }

            return mi.CreateDelegate(stm.MappedType, target);
        }
    }
}
