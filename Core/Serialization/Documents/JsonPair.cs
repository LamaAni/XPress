using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Documents
{
    public class JsonPair<T> : IJsonValue<T>
    {
        public JsonPair(IJsonValue<T> key, IJsonValue<T> value)
        {
            Key = key;
            Value = value;
        }

        public IJsonValue<T> Key { get; private set; }

        public IJsonValue<T> Value { get; set; }

        public override string ToString()
        {
            return (Key == null ? "null" : Key.ToString()) + ": " + (Value == null ? "null" : Value.ToString());
        }
    }
}
