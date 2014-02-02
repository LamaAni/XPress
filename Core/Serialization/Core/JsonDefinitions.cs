using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Core
{
    public class JsonDefinitions<T>
    {
        /// <summary>
        /// The collection of reader definitions that help the reader to determine the reading structure.
        /// </summary>
        /// <param name="getElementData">Returns the element data object in the form of a ElementData value."/></param>
        public JsonDefinitions(Func<ElementData<T>, T> getElementData, Func<T, T> prepareValue, T nullValue, 
            JsonValueConverter<T> directiveConveter, JsonValueConverter<T> parseObjectConverter)
        {
            GetElementData = getElementData;
            PrepareValue = prepareValue;
            ReturnUnknownDataTypesAsRawData = true;
            NullValue = nullValue;
            DirectiveConverter = directiveConveter;
            ParseObjectConverter = parseObjectConverter;
            m_convertersByType = new Dictionary<Type, JsonValueConverter<T>>();

            // adding the default converters.
            this.AddConverter(typeof(JsonDirective<T>), directiveConveter);
            this.AddConverter(typeof(JsonObjectPhrase<T>), parseObjectConverter);
        }

        /// <summary>
        /// The directive converter that writes and reads directives.
        /// </summary>
        public JsonValueConverter<T> DirectiveConverter { get; private set; }

        /// <summary>
        /// The parse object converter that writes and reads parse objects.
        /// </summary>
        public JsonValueConverter<T> ParseObjectConverter { get; private set; }

        /// <summary>
        /// The null value for the json.
        /// </summary>
        public T NullValue { get; private set; }

        /// <summary>
        /// Helps with the preperation of the object value. (like trimming... exc);
        /// </summary>
        public Func<T,T> PrepareValue { get; private set; }

        /// <summary>
        /// Returns the element data object in the form of a ElementData value.
        /// </summary>
        public Func<ElementData<T>, T> GetElementData { get; private set; }

        Dictionary<Type, JsonValueConverter<T>> m_convertersByType = new Dictionary<Type, JsonValueConverter<T>>();

        /// <summary>
        /// A collection of converters by type.
        /// </summary>
        public IReadOnlyDictionary<Type, JsonValueConverter<T>> ConvertersByType
        {
            get { return m_convertersByType; }
        }


        JsonValueConverter<T>[] m_converters = new JsonValueConverter<T>[0];

        /// <summary>
        /// A list of all json value converters (unique by name).
        /// </summary>
        public JsonValueConverter<T>[] Converters
        {
            get { return m_converters; }
        }

        /// <summary>
        /// If true when the converter dose not find a matching data type, it returns the taw data as the value.
        /// </summary>
        public bool ReturnUnknownDataTypesAsRawData { get; set; }

        /// <summary>
        /// Adds a converter and binds it to the type.
        /// </summary>
        /// <param name="converter"></param>
        public void AddConverter(Type t, JsonValueConverter<T> converter)
        {
            AddConverter(new Type[] { t }, converter);
        }

        /// <summary>
        /// Adds a converter and binds it to the types.
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="converter"></param>
        public void AddConverter(Type[] ts, JsonValueConverter<T> converter)
        {
            foreach(Type t in ts)
            {
                m_convertersByType[t] = converter;
            }

            m_converters = m_converters.Concat(new JsonValueConverter<T>[] { converter }).Distinct(c => c.Name).ToArray();
        }

        /// <summary>
        /// Adds a converter and binds it to the type.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="name"></param>
        /// <param name="canConvert"></param>
        /// <param name="toRaw"></param>
        /// <param name="fromRaw"></param>
        public void AddConverter<OType>(Type t, string name, Func<T, bool> canConvert, Func<OType, T> toRaw, Func<T, OType> fromRaw)
        {
            AddConverter<OType>(new Type[1] { t }, name, canConvert, toRaw, fromRaw);
        }

        /// <summary>
        /// Adds a converter and binds it to the types.
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="name"></param>
        /// <param name="canConvert"></param>
        /// <param name="toRaw"></param>
        /// <param name="fromRaw"></param>
        public void AddConverter<OType>(Type[] ts, string name, Func<T, bool> canConvert, Func<OType, T> toRaw, Func<T, OType> fromRaw)
        {
            AddConverter(ts, new JsonInlineValueConverter<T, OType>(name, canConvert, toRaw, fromRaw));
        }

        /// <summary>
        /// Returns the value according to the data value.
        /// </summary>
        /// <param name="data">The raw data.</param>
        /// <returns>The value</returns>
        public object GetObject(ElementData<T> data)
        {
            if (data.IsEmpty)
                return null;

            T raw = PrepareValue(data.GetRawData());

            if (raw.Equals(NullValue))
                return null;

            object o = GetObjectFromRawValue(raw, data.IsDirective);

            if (o != null)
                return o;
            if (ReturnUnknownDataTypesAsRawData)
                return raw;
#if DEBUG
            throw new Exception("No convertor found for data " + data.GetRawData());
#endif

            return o; // no valid convertor found.
        }

        /// <summary>
        /// Returns the value according to the data value.
        /// </summary>
        /// <param name="data">The raw data.</param>
        /// <returns>The value</returns>
        public object GetObjectFromRawValue(T raw, bool isDirective = false)
        {
            if (raw.Equals(NullValue))
                return null;

            if (isDirective)
                return DirectiveConverter.FromRaw(raw);

            if (raw.Equals(NullValue))
                return null;

            foreach (JsonValueConverter<T> c in m_converters)
            {
                if (c.CanConvert(raw))
                    return c.FromRaw(raw);
            }

            return null;
        }

        /// <summary>
        /// Converts the object into raw value if possible.
        /// </summary>
        /// <param name="o">The object to get the raw value for</param>
        /// <returns></returns>
        public T GetValue(object o)
        {
            if (o == null)
                return NullValue;

            Type t = o.GetType();
            JsonValueConverter<T> converter = null;
            if (m_convertersByType.TryGetValue(t, out converter))
            {
                return converter.ToRaw(o);
            }
            else return default(T);
        }
    }
}
