using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Core;
using XPress.Serialization.Documents;
using XPress.Serialization.Map;

namespace XPress.Serialization
{
    /// <summary>
    /// Implements a object to json and object from json serializer.
    /// </summary>
    /// <typeparam name="C">The char type</typeparam>
    /// <typeparam name="T">The source type</typeparam>
    public abstract class JsonSerializer<C, T> : IJsonSerializer<T>
        where T : IEnumerable<C>
    {
        public JsonSerializer(JsonReader<C, T> reader, JsonWriter<C, T> writer, SerializationDefinitions<T> definitions)
        {
            Reader = reader;
            reader.Definitions = definitions.JsonDefinitions;
            Writer = writer;
            Writer.Definitions = definitions.JsonDefinitions;
            Definitions = definitions;
        }

        /// <summary>
        /// The total number of objects processed in the last.
        /// </summary>
        public int LastProcessTotalNumberOfObjects { get; private set; }

        /// <summary>
        /// Returns the directive that marks the binder for this serializer.
        /// </summary>
        public abstract JsonDirective<T> BinderDirective { get; }

        /// <summary>
        /// Reads from the stream.
        /// </summary>
        public JsonReader<C, T> Reader { get; private set; }

        /// <summary>
        /// Writes to the stream.
        /// </summary>
        public JsonWriter<C, T> Writer { get; private set; }

        public OType Deserialize<OType>(T source, bool ignoreTypes = false)
        {
            return (OType)Deserialize(source, typeof(OType), ignoreTypes);
        }

        /// <summary>
        /// Creates a new serialziation context, this object determines the way the serialization is handled.
        /// </summary>
        /// <returns></returns>
        public virtual SerializationContext<T> CreateContext()
        {
            return new SerializationContext<T>(Definitions); 
        }

        private object DeserializeValue(Type otype, SerializationContext<T> context, IJsonValue<T> val)
        {
            // searching for a binder directive.
            JsonArray<T> asArray = val as JsonArray<T>;
            if (asArray != null && asArray.Count == 2 && asArray.HasDirectives && asArray.Directives.Any(d => d.Equals(BinderDirective)))
            {
                // this is a binder array. and the binder needs to be read from the stream.
                bool oldIgnoreTypes = context.IgnoreTypes;
                context.IgnoreTypes = true;
                context.Binder = context.GetObject(asArray[0], typeof(SerializationTypeBinder<T>)) as SerializationTypeBinder<T>;
                context.IgnoreTypes = oldIgnoreTypes;
                val = asArray[1];
            }

            object o = context.GetObject(val, otype);
            LastProcessTotalNumberOfObjects = context.TotalObjectsProcessed;
            return o;
        }

        #region IJsonSerializer<T> Members

        /// <summary>
        /// A collection of values that apply to the json serializer, and determine the definition properties.
        /// </summary>
        public SerializationDefinitions<T> Definitions { get; private set; }

        /// <summary>
        /// Converts an object into a serialization stream, using types and binder.
        /// </summary>
        /// <param name="o">The object to serialize</param>
        /// <param name="isPreety">If true then the json is written with indentation.</param>
        /// <returns>The serialized source</returns>
        /// <returns></returns>
        public virtual T SerializeWithBinder(object o, bool isPreety = false)
        {
            return Serialize(o, false, isPreety, new SerializationTypeBinder<T>());
        }

        /// <summary>
        /// Converts an object into a serialization stream.
        /// </summary>
        /// <param name="o">The object to serialize</param>
        /// <param name="binder">The type binder to use</param>
        /// <param name="isPreety">If true then the json is written with indentation.</param>
        /// <returns>The serialized source</returns>
        public virtual T Serialize(object o, bool ignoreTypes = false, bool isPreety = false, SerializationTypeBinder<T> binder = null)
        {
            SerializationContext<T> context = CreateContext();
            context.Binder = binder;
            context.IgnoreTypes = ignoreTypes;
            IJsonValue<T> val = context.GetJsonValue(o, o.GetType());
            if (binder != null && binder.TypeCount > 0)
            {
                // need to write the binder.
                JsonArray<T> tarray = new JsonArray<T>();

                // adding the binder directive
                tarray.Directives.Add(BinderDirective);

                // adding the data.
                context.IgnoreTypes = true;
                tarray.Add(context.GetJsonValue(binder, binder.GetType()));
                context.IgnoreTypes = false;
                tarray.Add(context.GetJsonValue(o, o.GetType()));
                val = tarray;
            }

            LastProcessTotalNumberOfObjects = context.TotalObjectsProcessed;
            return ToJsonRepresentation(val, isPreety);
        }

        /// <summary>
        /// Converts an object from serialization stream.
        /// </summary>
        /// <param name="source">The json source.</param>
        /// <returns></returns>
        public virtual object Deserialize(T source, Type otype, bool ignoreTypes = false)
        {
            IJsonValue<T> val = FromJsonRepresentation(source);

            return Deserialize(val, otype, ignoreTypes);
        }

                /// <summary>
        /// Converts an object from serialization stream.
        /// </summary>
        /// <param name="val">The json value</param>
        /// <returns></returns>
        public OType Deserialize<OType>(IJsonValue<T> val, bool ignoreTypes = false)
        {
            return (OType)Deserialize(val, typeof(OType), ignoreTypes);
        }

        /// <summary>
        /// Converts an object from serialization stream.
        /// </summary>
        /// <param name="val">The json value</param>
        /// <returns></returns>
        public object Deserialize(IJsonValue<T> val, Type otype, bool ignoreTypes = false)
        {
            SerializationContext<T> context = CreateContext();
            context.IgnoreTypes = ignoreTypes;

            if (val == null)
                return null;

            return DeserializeValue(otype, context, val);
        }

        /// <summary>
        /// Converts the json value to a json representation
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public T ToJsonRepresentation(IJsonValue<T> val, bool isPretty = false)
        {
            return Writer.ToJson(val, isPretty);
        }


        /// <summary>
        /// Converts the json value from json representation.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public IJsonValue<T> FromJsonRepresentation(T val)
        {
            return Reader.FromJson(val);
        }

        /// <summary>
        /// Parses a byte array to the storage medium.
        /// </summary>
        /// <param name="bytes">The bytes to prase.</param>
        /// <returns>The medium</returns>
        public abstract T ParseByteArray(byte[] bytes);

        /// <summary>
        /// Converts the storage medium to a byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract byte[] ToByteArray(T value);

        #endregion
    }
  
    /// <summary>
    /// Interface for json serialziation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IJsonSerializer<T>
    {
        /// <summary>
        /// Converts the json value to a json representation
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        T ToJsonRepresentation(IJsonValue<T> val, bool isPretty = false);

        /// <summary>
        /// Converts the json value from json representation.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        IJsonValue<T> FromJsonRepresentation(T val);

        /// <summary>
        /// Deserialzies an object from the json representation.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        object Deserialize(T val, Type otype, bool ignoreTypes = false);

        /// <summary>
        /// Serializes an object to the json representation.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="isPreety"></param>
        /// <returns></returns>
        T Serialize(object o, bool ignoreTypes = false, bool isPreety = false, SerializationTypeBinder<T> binder = null);

        /// <summary>
        /// A collection of values that apply to the json serializer, and determine the definition properties.
        /// </summary>
        SerializationDefinitions<T> Definitions { get; }

        /// <summary>
        /// Parses a byte array to the storage medium.
        /// </summary>
        /// <param name="bytes">The bytes to prase.</param>
        /// <returns>The medium</returns>
        T ParseByteArray(byte[] bytes);

        /// <summary>
        /// Converts the storage medium to a byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        byte[] ToByteArray(T value);
    }

    public static class JSONSERIALIZEREXTEND
    {
        /// <summary>
        /// Deserializes a value to the correct type.
        /// </summary>
        /// <typeparam name="OType"></typeparam>
        /// <param name="serializer"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static OType Deserialize<T, OType>(this IJsonSerializer<T> serializer, T source, bool ignoreTypes = false)
        {
            return (OType)serializer.Deserialize(source, typeof(OType), ignoreTypes);
        }
    }
}
