using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Documents;
using XPress.Serialization.Attributes;
using XPress.Serialization.Map;

namespace XPress.Serialization
{
    /// <summary>
    /// A type mapping binder that creates holds type identities.
    /// </summary>
    [XPressInheritedMemberSelection(XPressMemberSelectionType.OptIn)]
    public class SerializationTypeBinder<T>
    {
        public SerializationTypeBinder()
            : base()
        {
        }

        Dictionary<uint, Type> m_idToType = new Dictionary<uint, Type>();
        Dictionary<Type, uint> m_typeToId = new Dictionary<Type, uint>();

        [XPressMember("typeinfo")]
        /// <summary>
        /// Local write value member that writes the data members and loads the data members from the dictionaries.
        /// </summary>
        private object[] typeids
        {
            get
            {
                List<object> vals = new List<object>();
                foreach (KeyValuePair<uint, Type> kvp in m_idToType)
                {
                    vals.Add(kvp.Key);
                    vals.Add(AssemblyQualifiedNameConvertor.Global.ToIdentitiy(kvp.Value));
                }
                return vals.ToArray();
            }
            set
            {
                m_idToType = new Dictionary<uint, Type>();
                m_typeToId = new Dictionary<Type, uint>();
                for (int i = 0; i < value.Length; i += 2)
                {
                    uint typeid = ((JsonNumber<T>)value[i]).As<UInt32>();
                    Type type = AssemblyQualifiedNameConvertor.Global.ToType((string)value[i + 1]);
                    m_idToType[typeid] = type;
                    m_typeToId[type] = typeid;
                }
            }
        }

        /// <summary>
        /// The number of types loaded.
        /// </summary>
        public int TypeCount { get { return m_idToType.Count; } }

        [XPressMember("cid")]
        uint curId = 0;

        /// <summary>
        /// Validates a type is registered in the binding collection.
        /// </summary>
        /// <param name="t">The type to validate</param>
        public void ValidateRegisterType(Type t)
        {
            if (m_typeToId.ContainsKey(t))
                return;

            m_typeToId[t] = curId;
            m_idToType[curId] = t;
            curId += 1;
        }

        /// <summary>
        /// Returns the type id for the specified type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual uint GetTypeId(Type t)
        {
            ValidateRegisterType(t);
            return m_typeToId[t];
        }

        /// <summary>
        /// Returns the type assigned to the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Type FromTypeId(uint id)
        {
            return m_idToType[id];
        }

        /// <summary>
        /// True if the type id is in the collection.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool HasTypeId(uint id)
        {
            return m_idToType.ContainsKey(id);
        }
    }
}
