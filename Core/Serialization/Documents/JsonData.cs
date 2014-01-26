using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Core;

namespace XPress.Serialization.Documents
{
    /// <summary>
    /// Represents a single json value that has no other value members.
    /// </summary>
    public class JsonData<T> : IJsonValue<T>
    {
        /// <summary>
        /// The source of the json value.
        /// </summary>
        /// <param name="source">the string source to create the value from</param>
        public JsonData(Core.ElementData<T> data, JsonDefinitions<T> def)
        {
            RawData = data;
            Definitions = def;
            HasValue = false;
        }

        /// <summary>
        /// Creats a json data object from a value.
        /// </summary>
        /// <param name="value"></param>
        public JsonData(object value)
        {
            m_Value = value;
            HasValue = true;
        }

        /// <summary>
        /// The language associated with the source. Only exist if this object was read from json.
        /// </summary>
        protected JsonDefinitions<T> Definitions { get; private set; }

        /// <summary>
        /// The raw data in source form.
        /// </summary>
        public Core.ElementData<T> RawData { get; private set; }

        /// <summary>
        /// if true a value exist (can be null). In the case of objects read from json this value will become true
        /// when raw value is converted to object value.
        /// </summary>
        public bool HasValue { get; private set; }

        object m_Value = null;

        /// <summary>
        /// The object value.
        /// </summary>
        public object Value
        {
            get
            {
                if (!HasValue)
                {
                    m_Value = Definitions.GetObject(RawData);
                    HasValue = true;
                }
                return m_Value;
            }
        }

        public override string ToString()
        {
            return Value == null ? "null" : Value.ToString();
        }
    }
}
