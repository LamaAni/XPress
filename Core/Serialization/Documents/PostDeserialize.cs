using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Documents
{
    /// <summary>
    /// Implements a post deserialized value that will be deserialized only when the paramter is called.
    /// Use this member to hide/postpone heavy serialziation values.
    /// </summary>
    /// <typeparam name="T">The type of object to post deserialize</typeparam>
    public class PostDeserialize<T> : IPostDeserialize
    {
        /// <summary>
        /// Create a post deserialzied object form value.
        /// </summary>
        /// <param name="value"></param>
        public PostDeserialize(T value)
        {
            _value = value;
            m_deserializaer = null;
            m_data = null;
            m_HasBeenDeserialzied = true;
        }

        /// <summary>
        /// Constructs a post deserialized object that has not yet to deserialize.
        /// </summary>
        /// <param name="data">The object json data.</param>
        /// <param name="source">The deserialization source</param>
        /// <param name="t">The object type</param>
        public PostDeserialize(IDeserialziationSource source, IJsonRawData data)
        {
            _value = default(T);
            m_deserializaer = source;
            m_data = data;
            m_HasBeenDeserialzied = false;
        }

        #region members

        /// <summary>
        /// Deserialziation source
        /// </summary>
        IDeserialziationSource m_deserializaer;

        /// <summary>
        /// The deserialization data.
        /// </summary>
        IJsonRawData m_data;

        bool m_HasBeenDeserialzied;

        T _value;

        #endregion

        #region members

        /// <summary>
        /// Deserializes the object value if needed.
        /// </summary>
        public void ValidateValueDeserialzied()
        {
            if (m_HasBeenDeserialzied)
                return;
            m_HasBeenDeserialzied = true;
            _value = (T)m_deserializaer.GetObjectFromRawData(m_data, typeof(T));
        }

        /// <summary>
        /// The object value.
        /// </summary>
        public T Value
        {
            get
            {
                ValidateValueDeserialzied();
                return _value;
            }
        }

        #endregion

        #region IPostDeserialize Members

        /// <summary>
        /// False only if the object has not been deserialzied.
        /// </summary>
        public bool HasValue
        {
            get { return m_HasBeenDeserialzied; }
        }

        /// <summary>
        /// The object data. Null if not from serialziation.
        /// </summary>
        public IJsonRawData Data
        {
            get { return m_data; }
        }

        /// <summary>
        /// Retrns the object value - same as "Value".
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return Value;
        }

        #endregion
    }

    /// <summary>
    /// General interface for handling post deserialization.
    /// </summary>
    public interface IPostDeserialize
    {
        /// <summary>
        /// The value associated with the json data.
        /// </summary>
        object GetValue();

        /// <summary>
        /// The raw data for the post deserialzied.
        /// </summary>
        IJsonRawData Data{get;}

        /// <summary>
        /// True if the object has been deserialzied or the object has a value.
        /// </summary>
        bool HasValue { get; }
    }
}
