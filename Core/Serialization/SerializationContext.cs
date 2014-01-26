using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Map;

namespace XPress.Serialization
{
    /// <summary>
    /// A collection of mehtods that allow the generation of a serialization context.
    /// The serialization context holds the basic get values for the json stream.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SerializationContext<T> : IDeserialziationSource
    {
        public SerializationContext(SerializationDefinitions<T> definitions, SerializationTypeBinder<T> binder = null)
        {
            Definitions = definitions;
            Binder = binder;
            TypeMapInfos = new Dictionary<Type, SerializationTypeMap<T>>();
            CoreTypes = new HashSet<Type>(Definitions.JsonDefinitions.ConvertersByType.Keys);
            StreamingContext = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.Clone, true);
        }

        public System.Runtime.Serialization.StreamingContext StreamingContext { get; private set; }

        /// <summary>
        /// Type maps for serialization. The type map allows to determine what to do when one reaches the type.
        /// </summary>
        public Dictionary<Type, SerializationTypeMap<T>> TypeMapInfos { get; private set; }

        /// <summary>
        /// The serialization definitions
        /// </summary>
        public SerializationDefinitions<T> Definitions { get; private set; }

        /// <summary>
        /// The core types associated with the serializer. The core types are types the writer can internally handle.
        /// </summary>
        public HashSet<Type> CoreTypes { get; set; }

        /// <summary>
        /// Returns the type binder associated with the current values.
        /// </summary>
        public SerializationTypeBinder<T> Binder { get; set; }

        /// <summary>
        /// The total number of objects that were processed in the context.
        /// </summary>
        public int TotalObjectsProcessed { get; private set; }

        /// <summary>
        /// If this value is true then the current will ingore the type binder when writing.
        /// </summary>
        public bool IgnoreTypes { get; set; }

        #region types

        /// <summary>
        /// Returns the type info accoding to the stem.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public SerializationTypeMap<T> GetMapInfo(Type t)
        {
            SerializationTypeMap<T> stm;
            if(!TypeMapInfos.TryGetValue(t,out stm))
            {
                stm = new SerializationTypeMap<T>(Map.TypeMapInfo.GetInfo(t), this);
                TypeMapInfos.TryAdd(t, stm);
            }
            return stm;
        }

        /// <summary>
        /// Returns the parsing of a type to a single value.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public object ParseTypeToValue(Type t)
        {
            if (this.Binder != null)
            {
                Binder.ValidateRegisterType(t);
                return this.Binder.GetTypeId(t);
            }
            else
            {
                return Map.AssemblyQualifiedNameConvertor.Global.ToIdentitiy(t);
            }
        }

        public Type PraseTypeFromValue(object val)
        {
            JsonNumber<T> number = val as JsonNumber<T>;
            if (number!=null)
            {
                if (this.Binder == null)
                {
                    return typeof(XPress.Serialization.Parsers.TypeParser<T>.UnknownType);
                }
                else
                {
                    return Binder.FromTypeId(number.As<UInt32>());
                }
            }
            else
            {
                return Map.AssemblyQualifiedNameConvertor.Global.ToType(val as string);
            }
        }

        #endregion

        #region serialization

        /// <summary>
        /// Reads the json value defintions.
        /// </summary>
        /// <param name="o">The object to convert</param>
        /// <param name="baseType">The type of the object for conversion of no object type is defined or no converter is defined for the base type.</param>
        /// <returns></returns>
        public virtual IJsonValue<T> GetJsonValue(object o, Type t)
        {
            TotalObjectsProcessed += 1;
            SerializationTypeMap<T> stm = GetMapInfo(t);
            if (stm.IsCoreValue)
                return new JsonData<T>(o);
            else if (stm.IsJsonValue)
                return (IJsonValue<T>)o;
            else if (stm.IsParseValue)
                return new JsonData<T>(stm.Parser.FromObject(o, this));
            else
            {
                IJsonValue<T> val = stm.Converter.GetUinitializedJsonValue(stm, o, this);
                if (stm.Info.InvokeOnSerializing != null)
                    stm.Info.InvokeOnSerializing(o, StreamingContext);
                stm.Converter.PopulateJsonValue(stm, o, val, this);
                if (stm.Info.InvokeOnSerialized != null)
                    stm.Info.InvokeOnSerialized(o, StreamingContext);
                
                if (!stm.Converter.IgnoreTypeDirectives && !IgnoreTypes)
                {
                    IJsonEnumrableObject<T> eo = val as IJsonEnumrableObject<T>;
                    if (eo != null)
                    {
                        // here a type always exists. Now verfy the type has been loaded.
                        eo.AddDirective(new JsonDirective<T>(Definitions.TypeDirectiveMarker, ParseTypeToValue(t)));
                    }
                }

                return val;
            }
        }

        /// <summary>
        /// Returns the object from the json value.
        /// </summary>
        /// <param name="val">The json value</param>
        /// <param name="baseType">The type of the object if not type is found.</param>
        /// <returns></returns>
        public virtual object GetObject(IJsonValue<T> val, Type t)
        {
            TotalObjectsProcessed += 1;
            JsonData<T> asValue = val as JsonData<T>;
            if (asValue != null)
            {
                JsonObjectPhrase<T> parse = asValue.Value as JsonObjectPhrase<T>;
                // general object read. No idea about the object type and therefore we need to check the type marker.
                if (parse != null)
                {
                    // this is an object parser then need to get the parse from the list, and apply its parser to it. (If exist, else null).
                    IJsonObjectParser<T> parser = Definitions.GetParser(parse);
                    return parser.FromParse(parse, this);
                }

                JsonNumber<T> num = asValue.Value as JsonNumber<T>;
                // checking for number. 
                if (num != null)
                {
                    if (typeof(object) == t)
                        return num;
                    // finishing the load.
                    return num.As(t);
                }

                // as the actual value. No more conversions needed. (This can only be a value).
                return asValue.Value;
            }

            // checking for type directive.
            if (!IgnoreTypes)
            {
                IJsonEnumrableObject<T> eo = val as IJsonEnumrableObject<T>;
                if (eo != null)
                {
                    JsonDirective<T> typed = eo.FindDirective(Definitions.TypeDirectiveMarker);
                    if (typed != null)
                    {
                        // getting the type from the value.
                        Type td = PraseTypeFromValue(typed.Data);
                        // only when the type is assignable from the value type. Otherwise try and parse to the given type.
                        if (t.IsAssignableFrom(td))
                            t = td;
                    }
                }
            }

            SerializationTypeMap<T> stm = GetMapInfo(t);
            if (stm.IsCoreValue)
                return asValue.Value;
            else if (stm.IsJsonValue)
                return val;
            else
            {
                if (stm.Converter == null)
                    return null; // reached unknown type converter.

                object o = stm.Converter.GetUninitializedInstance(stm, val, this);
                if (stm.Info.InvokeOnDeserialzing != null)
                    stm.Info.InvokeOnDeserialzing.Invoke(o, StreamingContext);
                stm.Converter.PopulateObjectValue(stm, o, val, this);
                if (stm.Info.InvokeOnDeserialized != null)
                    stm.Info.InvokeOnDeserialized.Invoke(o, StreamingContext);
                return o;
            }
        }

        #endregion

        #region IDeserialziationSource Members

        /// <summary>
        /// Returns the object read from the json raw data. (Deserialization).
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public object GetObjectFromRawData(IJsonRawData data, Type t)
        {
            IJsonValue<T> val = data as IJsonValue<T>;
            if (val == null)
                throw new Exception("Attempted to read data source '" + data.GetType() + "' with a '" + typeof(IJsonValue<T>) + "' reader.");
            return GetObject(val, t);
        }

        #endregion
    }

    public interface IDeserialziationSource
    {
        /// <summary>
        /// Returns the object read from the json raw data. (Deserialization).
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        object GetObjectFromRawData(IJsonRawData data, Type t);
    }
}
