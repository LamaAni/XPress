using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace XPress.Web.Html.Collections
{
    public class StyleCollection : IEnumerable<KeyValuePair<string, string>>, ISerializable
    {
        internal StyleCollection()
        {
            CollectionChanged = false;
            CollectionInvalidated = false;
        }

        #region ISerializable Members

        protected StyleCollection(SerializationInfo info, StreamingContext context)
        {
            UnphrasedValue = info.GetString("uv");
            ValidteCollection();
            CollectionChanged = false;
            CollectionInvalidated = false;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ValidateUnpharsedValue();
            info.AddValue("uv", UnphrasedValue);
        }

        #endregion

        #region serializable members

        protected Dictionary<string,string> Dic { get; private set; }
        private string UnphrasedValue { get; set; }
        private bool CollectionInvalidated { get; set; }
        private bool CollectionChanged { get; set; }

        #endregion

        #region members

        public string this[string css]
        {
            get
            {
                // check that there is no unpharsed value.
                ValidteCollection();
                if (Dic == null)
                    return null;
                return GetValue(css);
            }
            set
            {
                ValidteCollection();

                if (Dic == null)
                {
                    if (value == null)
                        return;
                    Dic = new Dictionary<string, string>();
                }

                if (Dic.ContainsKey(css))
                {
                    if (Dic[css] == value)
                        return;
                    if (value == null)
                        Dic.Remove(css);
                    else Dic[css] = value;
                }
                else if(value!=null)
                {
                    Dic[css] = value;
                }

                CollectionChanged = true;
            }
        }

        public string Value
        {
            get
            {
                ValidateUnpharsedValue();
                return UnphrasedValue;
            }
            set
            {
                UnphrasedValue = value;
                Dic = null;
                CollectionInvalidated = true;
                CollectionChanged = false;
            }
        }

        public int Count { get { ValidteCollection(); return Dic == null ? 0 : Dic.Count; } }

        public bool HasValue
        {
            get
            {
                if(CollectionChanged && CollectionInvalidated)
                    throw new Exception("Cannot have both collection invalidated and collection changed. Internal error.");
                if (CollectionInvalidated)
                {
                    return UnphrasedValue != null;
                }
                else if (!CollectionChanged)
                {
                    // both collections are update.
                    return UnphrasedValue != null;
                }

                return Dic != null && Dic.Count > 0;
            }
        }

        #endregion

        #region methods

        private string GetValue(string css)
        {
            string val;
            if (Dic.TryGetValue(css, out val))
                return val;
            return null;
        }

        private void Phrase(string value)
        {
            if (value == null)
                Dic = null;
            else Dic = value.Split(';').Select(v => v.Split(':')).ToDictionary(v => v[0], v => v[1]);
            CollectionInvalidated = false;
            CollectionChanged = false;
            UnphrasedValue = value;
        }

        public override string ToString()
        {
            ValidateUnpharsedValue();
            return UnphrasedValue;
        }

        private void ValidateUnpharsedValue()
        {
            if (CollectionChanged)
            {
                UnphrasedValue = string.Join(";", Dic.Select(kvp => kvp.Key + ":" + kvp.Value));
                CollectionChanged = false;
            }
        }

        private void ValidteCollection()
        {
            if (CollectionInvalidated)
            {
                Phrase(UnphrasedValue);
            }
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            ValidteCollection();
            return Dic.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}
