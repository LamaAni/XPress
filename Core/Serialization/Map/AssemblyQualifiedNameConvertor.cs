using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Map
{
    /// <summary>
    /// Implements a basic type convertor that allows for conversion to and from type names.
    /// </summary>
    public class AssemblyQualifiedNameConvertor : ITypeConverter
    {
        #region static

        static AssemblyQualifiedNameConvertor()
        {
            _global = new AssemblyQualifiedNameConvertor();
        }

        static AssemblyQualifiedNameConvertor _global;

        /// <summary>
        /// Global implementation.
        /// </summary>
        public static AssemblyQualifiedNameConvertor Global
        {
            get { return AssemblyQualifiedNameConvertor._global; }
        }

        #endregion

        #region members

        System.Collections.Concurrent.ConcurrentDictionary<string, Type> m_typeByName = new System.Collections.Concurrent.ConcurrentDictionary<string, Type>();
        System.Collections.Concurrent.ConcurrentDictionary<Type, string> m_nameByType = new System.Collections.Concurrent.ConcurrentDictionary<Type, string>();

        #endregion

        #region ITypeConverter Members

        public bool CanConvert(Type t)
        {
            return true;
        }

        public bool CanConvert(string identity)
        {
            return true;
        }

        public Type ToType(string identity)
        {
            Type t;
            if (!m_typeByName.ContainsKey(identity))
            {
                t = Type.GetType(identity);
                if (t == null)
                    throw new Exception("Cannot find type " + identity);
                m_nameByType[t] = identity;
                m_typeByName[identity] = t;
            }
            else t = m_typeByName[identity];

            return t;
        }

        public string ToIdentitiy(Type t)
        {
            string identity;
            if (!m_nameByType.ContainsKey(t))
            {
                identity = t.AssemblyQualifiedName;
                if(identity.StartsWith("ASP."))
                {
                    // assume compiler generated assembly page, take only full name.
                    identity = t.FullName;
                }
                if (t == null)
                    throw new Exception("Cannot find type " + identity);
                m_nameByType[t] = identity;
                m_typeByName[identity] = t;
            }
            else identity = m_nameByType[t];
            return identity;
        }

        #endregion
    }
}
