using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Documents
{
    /// <summary>
    /// A json object that represents a single line object that can be read and converted to a json value,
    /// The parse object allows "word" like implementation of json values.
    /// </summary>
    public class JsonObjectPhrase<T> : IJsonValue<T>
    {
        /// <summary>
        /// Represents the word that allows the object to be parsed.
        /// </summary>
        /// <param name="word"></param>
        public JsonObjectPhrase(object identity, object word)
            : this(identity, new object[1] { word })
        {
        }

        public JsonObjectPhrase(object identity, object[] words)
        {
            Identity = identity;
            Words = words;
        }

        /// <summary>
        /// The word that reporesents the parse object.
        /// </summary>
        public object[] Words { get; private set; }

        /// <summary>
        /// The identity of the parsed object (the type marker).
        /// </summary>
        public object Identity { get; private set; }
    }
}
