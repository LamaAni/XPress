using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace XPress.Web.Html.Collections
{
    /// <summary>
    /// A collection of attributes that applies to a control.
    /// </summary>
    public class AttributeCollection : IEnumerable<KeyValuePair<string, string>>, ISerializable
    {
        public AttributeCollection()
        {
        }

        #region ISerializable Members

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected AttributeCollection(SerializationInfo info, StreamingContext context)
        {
            SerializationInfoEnumerator e = info.GetEnumerator();
            while (e.MoveNext())
            {
                switch (e.Name)
                {
                    case "c":
                        {
                            string[] col = (string[])e.Value;
                            for (int i = 0; i < col.Length; i += 2)
                                SetValue(col[i], col[i + 1]);
                        }
                        break;
                    case "s":
                        {
                            m_Style = (StyleCollection)e.Value;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Serialization data get.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (Dic != null)
            {
                string[] col = new string[Dic.Count * 2];
                Dic.ForEach((kvp, i) =>
                {
                    col[i * 2] = kvp.Key;
                    col[i * 2 + 1] = kvp.Value;
                });
                info.AddValue("c", col);
            }
            if (m_Style != null)
                info.AddValue("s", m_Style);
        }

        #endregion

        #region members

        /// <summary>
        /// Internal dictionary.
        /// </summary>
        protected Dictionary<string, string> Dic { get; private set; }

        /// <summary>
        /// Sets/Gets the attribute value.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[System.Web.UI.HtmlTextWriterStyle name]
        {
            get
            {
                return this[name.ToString()];
            }
            set
            {
                this[name.ToString()] = value;
            }
        }

        /// <summary>
        /// Sets/Gets the attribute value.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                name = name.ToLower();
                if (name == "style")
                    return Style.Value;
                else if (name == "id")
                    throw new Exception("The id of the html element cannot be set/get through the attributes collection and must be set directly on the element or by using the 'Id' extention method.");
                return GetValue(name);
            }
            set
            {
                name = name.ToLower();
                if (name == "style")
                    Style.Value = value;
                else if (name == "id")
                    throw new Exception("The id of the html element cannot be set/get through the attributes collection and must be set directly on the element or by using the 'Id' extention method.");
                else SetValue(name, value);
            }
        }

        StyleCollection m_Style;

        /// <summary>
        /// Style collection associated with the attributes collection.
        /// </summary>
        public StyleCollection Style { get { if (m_Style == null)m_Style = new StyleCollection(); return m_Style; } set { m_Style = value; } }

        /// <summary>
        /// The number of attributes in the collection.
        /// </summary>
        public int Count { get { return (Dic != null ? Dic.Count : 0) + (m_Style != null && Style.HasValue ? 1 : 0); } }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members

        /// <summary>
        /// Returns the styles enuerate
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            IEnumerable<string> keys = Dic == null ? new string[0] as IEnumerable<string> : Dic.Keys;

            if (m_Style != null && m_Style.HasValue)
                keys = keys.Concat(new string[1] { "style" });

            return keys.Select(k => new KeyValuePair<string, string>(k, this[k])).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Methods

        internal void SetValue(string name, string value)
        {
            if (value == null)
            {
                if (Dic == null)
                    return;
                if (Dic.ContainsKey(name))
                    Dic.Remove(name);
                return;
            }

            if (Dic == null)
                Dic = new Dictionary<string, string>();

            name = name.ToLower();
            Dic[name] = value;
        }

        internal string GetValue(string name)
        {
            if (Dic == null)
                return null;
            name = name.ToLower();
            string val;
            if (Dic.TryGetValue(name, out val))
                return val;
            return null;
        }

        /// <summary>
        /// True if the collection contains a key.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return Dic == null ? false : Dic.ContainsKey(name);
        }

        #endregion
    }
}
