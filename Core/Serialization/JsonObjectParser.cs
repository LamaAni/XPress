using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization
{
    /// <summary>
    /// Implements a json object parser that allows conversion of specialized data types (like DateTime)
    /// </summary>
    /// <typeparam name="T">The source type</typeparam>
    public abstract class JsonObjectParser<T, OType> : IJsonObjectParser<T>
    {
        public JsonObjectParser(object identity)
        {
            Identity = identity;
        }

        /// <summary>
        /// The identity of the parser.
        /// </summary>
        public object Identity { get; private set; }

        #region abstract members

        /// <summary>
        /// Return the parsed value.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract JsonObjectPhrase<T> ParseFromObject(OType o, SerializationContext<T> context);

        /// <summary>
        /// Rethrns the object value.
        /// </summary>
        /// <param name="parse"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract OType ObjectFromParse(JsonObjectPhrase<T> parse, SerializationContext<T> context);

        #endregion

        #region IJsonObjectParser<T> Members

        public object FromParse(JsonObjectPhrase<T> parse, SerializationContext<T> context)
        {
            return ObjectFromParse(parse, context);
        }

        public JsonObjectPhrase<T> FromObject(object o, SerializationContext<T> context)
        {
            return ParseFromObject((OType)o, context);
        }

        #endregion
    }

    /// <summary>
    /// An interface representing the object parsing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IJsonObjectParser<T>
    {
        object Identity { get;}
        /// <summary>
        /// Returns the raw
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        object FromParse(JsonObjectPhrase<T> parse, SerializationContext<T> context);

        /// <summary>
        /// Retrhs the json parse value.
        /// </summary>
        JsonObjectPhrase<T> FromObject(object o, SerializationContext<T> context);
    }
}
