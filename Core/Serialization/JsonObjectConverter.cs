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
    /// Represents a specialized json value converter, this converter converts a specific json value representation 
    /// into its json counterpart and back from said conversion value.
    /// </summary>
    /// <typeparam name="JVType">The type of json value.</typeparam>
    /// <typeparam name="OType">The type of the object.</typeparam>
    /// <typeparam name="T">The Json source type</typeparam>
    public abstract class JsonObjectConverter<T, OType, JVType> : IJsonObjectConverter<T>
        where JVType : IJsonValue<T>
    {
        /// <summary>
        /// Converts an object from a json representation to a json value representation.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>The Json value</returns>
        public abstract void PopulateJsonValue(SerializationTypeMap<T> stm, OType o, JVType val, SerializationContext<T> context);

        /// <summary>
        /// Converts a json value into a specific object and returns the object value.
        /// </summary>
        /// <param name="val">The object value.</param>
        /// <param name="derivingType"></param>
        /// <returns>The generated object.</returns>
        public abstract void PopulateObjectValue(SerializationTypeMap<T> stm, OType o, JVType val, SerializationContext<T> context);

        /// <summary>
        /// Creates a new instance of the type that is defined by the deriving type.
        /// </summary>
        public virtual OType CreateUninitializedInstance(SerializationTypeMap<T> stm, JVType val, SerializationContext<T> context)
        {
            Type type = stm.MappedType;
            if (!typeof(OType).IsAssignableFrom(type))
                throw new Exception("Cannot populate type " + type + " with a convertor that fits type " + typeof(OType));
            return (OType)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
        }

        /// <summary>
        /// Creates the appropriate json value to contain the data.
        /// </summary>
        /// <param name="defintions">The json definitions.</param>
        /// <returns>The appropriate json value.</returns>
        public virtual JVType CreateUinitializedJsonValue(SerializationTypeMap<T> stm, OType o, SerializationContext<T> context)
        {
            return Activator.CreateInstance<JVType>();
        }

        #region IJsonValueConveter<T> Members

        /// <summary>
        /// Creates the appropriate json value to contain the data.
        /// </summary>
        /// <returns>The appropriate json value.</returns>
        public IJsonValue<T> GetUinitializedJsonValue(SerializationTypeMap<T> stm, object o, SerializationContext<T> context)
        {
            return CreateUinitializedJsonValue(stm, (OType)o, context);
        }

        /// <summary>
        /// Creates a new instance of the type that is defined by the deriving type.
        /// </summary>
        public object GetUninitializedInstance(SerializationTypeMap<T> stm, IJsonValue<T> val, SerializationContext<T> context)
        {
            return CreateUninitializedInstance(stm, (JVType)val, context);
        }

        /// <summary>
        /// Converts a json value into a specific object and returns the object value.
        /// </summary>
        /// <param name="val">The object value.</param>
        /// <returns>The generated object.</returns>
        public void PopulateObjectValue(SerializationTypeMap<T> stm, object o, IJsonValue<T> val, SerializationContext<T> context)
        {
            PopulateObjectValue(stm, (OType)o, (JVType)val, context);
        }

        /// <summary>
        /// Converts an object from a json representation to a json value representation.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>The Json value</returns>
        public void PopulateJsonValue(SerializationTypeMap<T> stm, object o, IJsonValue<T> val, SerializationContext<T> context)
        {
            PopulateJsonValue(stm, (OType)o, (JVType)val, context);
        }

        /// <summary>
        /// If ture then no type directive will be added to the result of this conveter.
        /// </summary>
        public bool IgnoreTypeDirectives
        {
            get;
            protected set;
        }

        #endregion
    }

    public interface IJsonObjectConverter<T>
    {
        /// <summary>
        /// If ture then no type directive will be added to the result of this conveter.
        /// </summary>
        bool IgnoreTypeDirectives { get; }

        /// <summary>
        /// Creates the appropriate json value to contain the data.
        /// </summary>
        /// <returns>The appropriate json value.</returns>
        IJsonValue<T> GetUinitializedJsonValue(SerializationTypeMap<T> stm, object o, SerializationContext<T> context);

        /// <summary>
        /// Creates a new instance of the type that is defined by the deriving type.
        /// </summary>
        object GetUninitializedInstance(SerializationTypeMap<T> stm, IJsonValue<T> val, SerializationContext<T> context);
        /// <summary>
        /// Converts a json value into a specific object and returns the object value.
        /// </summary>
        /// <param name="val">The object value.</param>
        /// <returns>The generated object.</returns>
        void PopulateObjectValue(SerializationTypeMap<T> stm, object o, IJsonValue<T> val, SerializationContext<T> context);
        /// <summary>
        /// Converts an object from a json representation to a json value representation.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>The Json value</returns>
        void PopulateJsonValue(SerializationTypeMap<T> stm, object o, IJsonValue<T> val, SerializationContext<T> context);
    }

}
