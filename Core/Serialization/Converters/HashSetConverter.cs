using XPress.Serialization.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Map;

namespace XPress.Serialization.Converters
{
    public class HashSetConverter<T> : JsonObjectConverter<T, IEnumerable, JsonArray<T>>
    {
        /// <summary>
        /// Converts the object value into a set.
        /// </summary>
        public HashSetConverter()
        {
        }

        static System.Collections.Concurrent.ConcurrentDictionary<Type, Action<object, IEnumerable<object>>> SetAndClearByType =
            new System.Collections.Concurrent.ConcurrentDictionary<Type, Action<object, IEnumerable<object>>>();

        static Action<object, IEnumerable<object>> GetSetAndClear(Type t)
        { 
            if(!SetAndClearByType.ContainsKey(t))
            {
                MethodInfo add = t.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
                MethodInfo clear = t.GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance);
                Action<object, IEnumerable<object>> ac = (o,data) =>
                {
                    clear.Invoke(o, dummyEmpty);
                    object[] prs = new object[1];
                    foreach(object v in data)
                    {
                        prs[0] = v;
                        add.Invoke(o, prs);
                    }
                };
                SetAndClearByType.TryAdd(t, ac);
            }
            return SetAndClearByType[t];
        }

        static object[] dummyEmpty = new object[0];

        public override void PopulateJsonValue(SerializationTypeMap<T> stm, IEnumerable o, JsonArray<T> val, SerializationContext<T> context)
        {
            // populating the set.
            val.Clear();
            foreach (object io in o)
                val.Add(context.GetJsonValue(io, io.GetType()));
        }

        public override void PopulateObjectValue(SerializationTypeMap<T> stm, IEnumerable o, JsonArray<T> val, SerializationContext<T> context)
        {
            GetSetAndClear(o.GetType())(o, val.Select(v => context.GetObject(v, stm.Info.GenericTypeArguments[0])));
        }

        /// <summary>
        /// Creates the unitialzied object instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="defintions"></param>
        /// <returns></returns>
        public override IEnumerable CreateUninitializedInstance(SerializationTypeMap<T> stm, JsonArray<T> val, SerializationContext<T> context)
        {
            return Activator.CreateInstance(stm.MappedType) as IEnumerable;// base.CreateUninitializedInstance(stm, val, context);
        }
    }
}
