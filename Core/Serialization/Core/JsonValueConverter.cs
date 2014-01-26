using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Core
{
    /// <summary>
    /// Implements a json value converter that allows converting json specific values.
    /// </summary>
    public abstract class JsonValueConverter<T>
    {
        /// <summary>
        /// Creates a converter.
        /// </summary>
        /// <param name="name">UNIQUE VALUE. (For deserialziation) only one will be allowed.</param>
        public JsonValueConverter(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the converter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// If true the current can convert this raw value.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public abstract bool CanConvert(T raw);

        /// <summary>
        /// Converts the raw value to object.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public abstract object FromRaw(T raw);

        /// <summary>
        /// Converts the object to raw value.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public abstract T ToRaw(object o);
    }

    /// <summary>
    /// Creates a lambda expression or delegate value converter.
    /// </summary>
    /// <typeparam name="T">The type of the source.</typeparam>
    /// <typeparam name="OType">The type of the object</typeparam>
    public class JsonInlineValueConverter<T,OType> : JsonValueConverter<T>
    {
        public JsonInlineValueConverter(string name, Func<T, bool> canConvert, Func<OType, T> toRaw, Func<T, OType> fromRaw)
            : base(name)
        {
            CanConvertDelegate = canConvert;
            ToRawDelegate = toRaw;
            ToObjectDelegate = fromRaw;
        }

        public Func<T, bool> CanConvertDelegate { get; private set; }
        public Func<OType,T> ToRawDelegate { get; private set; }
        public Func<T, OType> ToObjectDelegate { get; private set; }

        public override bool CanConvert(T raw)
        {
            return CanConvertDelegate(raw);
        }

        public override object FromRaw(T raw)
        {
            return ToObjectDelegate(raw);
        }

        public override T ToRaw(object o)
        {
            return ToRawDelegate((OType)o);
        }
    }
}
