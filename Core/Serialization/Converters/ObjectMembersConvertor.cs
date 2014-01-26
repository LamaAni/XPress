using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Map;

namespace XPress.Serialization.Converters
{
    public class ObjectMembersConvertor<T> : JsonObjectConverter<T, object, JsonObject<T>>
    {
        public override void PopulateJsonValue(SerializationTypeMap<T> stm, object o, JsonObject<T> val, SerializationContext<T> context)
        {
            val.Clear();
            stm.Info.WriteableMembers.ForEach(mmi =>
            {
                object oval = mmi.Getter(o);

                if (!mmi.Required && !mmi.IgnoreMode.HasFlag(Attributes.XPressIgnoreMode.NeverIgnored))
                {
                    if (mmi.IgnoreMode.HasFlag(Attributes.XPressIgnoreMode.IfNull) && oval == null)
                        return;
                    if (mmi.IgnoreMode.HasFlag(Attributes.XPressIgnoreMode.IfDefualt) && mmi.DefaultValue != null && mmi.DefaultValue.Value.Equals(oval))
                        return;
                }
                val.AddRawValue(context.GetJsonValue(mmi.Name, typeof(string)), context.GetJsonValue(oval, oval.GetType()), false);
            });
        }

        public override void PopulateObjectValue(SerializationTypeMap<T> stm, object o, JsonObject<T> val, SerializationContext<T> context)
        {
            // getting the names.
            Dictionary<string, IJsonValue<T>> memberValues = new Dictionary<string, IJsonValue<T>>();
            foreach (JsonPair<T> pair in val)
            {
                string name = context.GetObject(pair.Key, typeof(string)) as string;
                if (name == null)
                {
#if DEBUG
                    throw new Exception("Found object member name == null. This is ignored on release.");
#else
                    continue; // nothing to do with null names.
#endif
                }

                if (!stm.Info.MembersByName.ContainsKey(name))
                    continue;

                memberValues[name] = pair.Value;
            }

            stm.Info.WriteableMembers.ForEach(mmi =>
            {
                if (!memberValues.ContainsKey(mmi.Name))
                {
                    if (mmi.Required)
                        throw new Exception("Required field not found in stream");
                    if (mmi.DefaultValue != null && mmi.IgnoreMode.HasFlag(Attributes.XPressIgnoreMode.IfDefualt))
                        try
                        {
                            mmi.Setter(o, mmi.DefaultValue.Value);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            throw new Exception("Cannot set default value on field/property " + mmi.ToString(), ex);
#endif
                        }
                    return;
                }

                mmi.Setter(o, context.GetObject(memberValues[mmi.Name], mmi.MemberType));
            });
        }
    }
}
