using XPress.Serialization.Documents;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization
{
    /// <summary>
    /// Represents a collection of thisinitions that determine the way the objects are serialzied.
    /// </summary>
    public class SerializationDefinitions<T>
    {
        public SerializationDefinitions(Core.JsonDefinitions<T> definitions, T typeDirectiveMarker)
        {
            JsonDefinitions = definitions;
            
            // serialisers by order
            this.AddConverter(typeof(IJsonSerializable), new Converters.JsonSerializeableConverter<T>());

            // Some basics serializers.
            this.AddConverter(typeof(Array), new Converters.ArrayConverter<T>());
            this.AddConverter(typeof(Delegate), new Converters.DelegateConvertor<T>());
            

            // adding interface serializers.
            this.AddConverter(typeof(IList), new Converters.ListConveter<T>());
            this.AddConverter(typeof(IDictionary), new Converters.DictionaryConverter<T>());
            this.AddConverter(typeof(IPostDeserialize), new Converters.PostDeserializeConverter<T>());
            //this.AddConverter(typeof(HashSet<>), new Convertors.HashSetConverter<T>());

            // adding fallback serialization
            this.AddConverter(typeof(System.Runtime.Serialization.ISerializable), new Converters.NetSerializationConverter<T>());
            this.AddConverter(typeof(object), new Converters.ObjectMembersConvertor<T>());

            this.AddParser(typeof(Type), new Parsers.TypeParser<T>());
            this.AddParser(typeof(DateTime), new Parsers.DateTimeParser<T>());
            this.AddParser(typeof(MemberInfo), new Parsers.MemberInfoParser<T>());
            this.AddParser(typeof(Reference.RefrenceId), new Parsers.RefrenceIdParser<T>());

            TypeDirectiveMarker = typeDirectiveMarker;
        }

        #region Members

        /// <summary>
        /// The directive marker that matches a type directive.
        /// </summary>
        public T TypeDirectiveMarker { get; private set; }

        /// <summary>
        /// The collection of json thisinitions that apply to the current serializer.
        /// </summary>
        public Core.JsonDefinitions<T> JsonDefinitions { get; private set; }

        #endregion

        #region converters collection;

        /// <summary>
        /// conversions collection.
        /// </summary>
        List<KeyValuePair<Type, IJsonObjectConverter<T>>> m_typeConverters = new List<KeyValuePair<Type, IJsonObjectConverter<T>>>();

        /// <summary>
        /// A collection if convertsion types that convert an object into its json value representation.
        /// </summary>
        public IReadOnlyList<KeyValuePair<Type, IJsonObjectConverter<T>>> TypeConverters { get { return m_typeConverters; } }

        /// <summary>
        /// Adds a type converter for the types.
        /// </summary>
        /// <param name="types"></param>
        /// <param name="converter"></param>
        public virtual void AddConverter(Type[] types, IJsonObjectConverter<T> converter, int insertIndex = -1)
        {
            types.ForEach(t => AddConverter(t, converter));
        }

        /// <summary>
        /// Adds a type converter for the type.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="converter"></param>
        public virtual void AddConverter(Type t, IJsonObjectConverter<T> converter, int insertIndex = -1)
        {
            KeyValuePair<Type, IJsonObjectConverter<T>> binding = new KeyValuePair<Type, IJsonObjectConverter<T>>(t, converter);
            if (insertIndex < 0 || insertIndex >= m_typeConverters.Count)
                m_typeConverters.Add(binding);
            else m_typeConverters.Insert(insertIndex, binding);
        }

        Dictionary<object, IJsonObjectParser<T>> m_parserByIdentity = new Dictionary<object, IJsonObjectParser<T>>();
        List<KeyValuePair<Type, IJsonObjectParser<T>>> m_typeParsers = new List<KeyValuePair<Type, IJsonObjectParser<T>>>();

        /// <summary>
        /// The type parser collection.
        /// </summary>
        public IReadOnlyList<KeyValuePair<Type, IJsonObjectParser<T>>> TypeParsers
        {
            get { return m_typeParsers; }
        }

        /// <summary>
        /// Adds a parser for type t.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="parser"></param>
        public virtual void AddParser(Type t, IJsonObjectParser<T> parser, int insertIndex = -1)
        {
            if (insertIndex < 0 || insertIndex >= m_typeParsers.Count)
                m_typeParsers.Add(new KeyValuePair<Type, IJsonObjectParser<T>>(t, parser));
            else m_typeParsers.Insert(insertIndex, new KeyValuePair<Type, IJsonObjectParser<T>>(t, parser));

            m_parserByIdentity[parser.Identity] = parser;
        }

        /// <summary>
        /// Adds a parser for types ts.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="parser"></param>
        public virtual void AddParser(Type[] ts, IJsonObjectParser<T> parser, int insertIndex=-1)
        {
            ts.ForEach(t => this.AddParser(t, parser, insertIndex));
        }

        #endregion

        #region Convertor mappings

        ConcurrentDictionary<Type, IJsonObjectConverter<T>> __mappedTypeConverters = new ConcurrentDictionary<Type, IJsonObjectConverter<T>>();
        ConcurrentDictionary<Type, IJsonObjectParser<T>> __mappedTypeParsers = new ConcurrentDictionary<Type, IJsonObjectParser<T>>();

        /// <summary>
        /// Returns the type convertor for the specified type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IJsonObjectConverter<T> GetConvertor(Map.TypeMapInfo tmi)
        {
            Type t = tmi.MappedType;

            IJsonObjectConverter<T> convertor = null;
            if (__mappedTypeConverters.TryGetValue(t, out convertor))
            {
                return convertor;
            }

            // getting the generic interfaces for t.
            IEnumerable<Type> genericInterfaces = new Type[0];
            if (t.IsGenericType)
                genericInterfaces = t.GetInterfaces().Where(it => it.IsGenericType).Select(it => it.GetGenericTypeDefinition());

            // adding list values to the converters.
            convertor = m_typeConverters.FirstOrDefault(tpl =>
            {
                return tpl.Key.IsAssignableFrom(t) ||
                    t.IsGenericType &&
                    (tpl.Key.IsAssignableFrom(t.GetGenericTypeDefinition()) || genericInterfaces.Any(it => tpl.Key.IsAssignableFrom(it)));
            }).Value;

            __mappedTypeConverters.TryAdd(t, convertor);

            return convertor;
        }

        //private int OrderTypes(Type a, Type b)
        //{
        //    // always last.
        //    bool aobject = a == typeof(object);
        //    bool bobject = b == typeof(object);
        //    if (aobject)
        //    {
        //        if (bobject)
        //            return 0;
        //        else return -1;
        //    }

        //    if (bobject)
        //        return 1;

        //    if (a.IsInterface)
        //    {
        //        if (b.IsInterface)
        //            return 0;
        //        return -1;
        //    }
        //    if (b.IsInterface)
        //        return 1;

        //    if (a == b)
        //        return 0;
        //    if (a.IsSubclassOf(b)) // a>b
        //        return 1;
        //    else return -1;
        //}

        //private KeyValuePair<Type, IJsonObjectConverter<T>>[] GetValidConvertors(Type type)
        //{
        //    return new Type[] { type }.Concat(type.GetInterfaces()).SelectMany(t =>
        //    {
        //        return TypeConverters
        //            .Where(kvp =>
        //            {
        //                if (kvp.Key.IsAssignableFrom(t))
        //                    return true;
        //                if (t.IsGenericType && kvp.Key.IsAssignableFrom(t.GetGenericTypeDefinition()))
        //                    return true;
        //                return false;
        //            }).ToArray();
        //    }).Distinct(kvp => kvp.Key).ToArray();
        //}

        /// <summary>
        /// Returns the correct parser for the specifc prase.
        /// </summary>
        /// <param name="parse"></param>
        /// <returns></returns>
        public IJsonObjectParser<T> GetParser(JsonObjectPhrase<T> parse)
        {
            IJsonObjectParser<T> parser = null;
            m_parserByIdentity.TryGetValue(parse.Identity, out parser);
            return parser;
        }

        /// <summary>
        /// Returns the parser by the type. This is a long action.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IJsonObjectParser<T> GetParser(Map.TypeMapInfo tmi)
        {
            Type t = tmi.MappedType;

            IJsonObjectParser<T> parser = null;

            if (__mappedTypeParsers.TryGetValue(t, out parser))
            {
                return parser;
            }
            
            // adding list values to the converters.
            parser = m_typeParsers.FirstOrDefault(tpl => tpl.Key.IsAssignableFrom(t)).Value;

            __mappedTypeParsers.TryAdd(t, parser);
            return parser;
        }

        #endregion
    }
}
