using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JCom.Map
{
    /// <summary>
    /// Information abount a specific jcom member according to the specified member type.
    /// </summary>
    public class JComMemberInfo 
    {
        public JComMemberInfo(MemberInfo mi)
        {
            MappedMember = mi;
            Map();
        }

        #region static

        static ConcurrentDictionary<MemberInfo, JComMemberInfo> m_byMemberInfo = new ConcurrentDictionary<MemberInfo, JComMemberInfo>();

        /// <summary>
        /// Returns the JCom member info associated with the specified member.
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public static JComMemberInfo Get(MemberInfo mi)
        {
            if (!m_byMemberInfo.ContainsKey(mi))
                m_byMemberInfo.TryAdd(mi, new JComMemberInfo(mi));
            return m_byMemberInfo[mi];
        }

        #endregion

        #region members

        /// <summary>
        /// The member that is mapped by this info.
        /// </summary>
        public MemberInfo MappedMember { get; private set; }

        /// <summary>
        /// The client side member attribute associated with the specified member.
        /// </summary>
        public Attributes.ClientSideAttribute MemberAttribute { get; private set; }

        /// <summary>
        /// If true the current is a property/field, else the current is a method.
        /// </summary>
        public bool IsDataMember { get; private set; }

        /// <summary>
        /// If true the current is a property.
        /// </summary>
        public bool IsProperty { get; private set; }

        /// <summary>
        /// The name of the member
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// If the current can write (Data member).
        /// </summary>
        public bool CanWrite { get; private set; }

        /// <summary>
        /// if the current can read (Data member).
        /// </summary>
        public bool CanRead { get; private set; }

        /// <summary>
        /// The value type of the member.
        /// </summary>
        public Type ValueType { get; private set; }

        /// <summary>
        /// If true the current member can be updated (or called) in an asynchronized fashion.
        /// </summary>
        public bool CanBeAsynchromized { get; private set; }

        #endregion

        #region methods

        void Map()
        {
            // mapping the basic properties of the member.
            IsDataMember = !(MappedMember is MethodInfo);
            IsProperty = MappedMember is PropertyInfo;
            MemberAttribute = Attribute.GetCustomAttribute(MappedMember, typeof(Attributes.ClientSideAttribute)) as Attributes.ClientSideAttribute;

            Name = MemberAttribute.Name == null ? MappedMember.Name : MemberAttribute.Name;
            CanWrite = true;
            CanRead = true;

            if (IsDataMember)
            {
                if (IsProperty)
                {
                    PropertyInfo pi = MappedMember as PropertyInfo;
                    CanRead = pi.CanRead;
                    CanWrite = pi.CanWrite;
                    ValueType = pi.PropertyType;
                }
                else ValueType = (MappedMember as FieldInfo).FieldType;
            }

            CanBeAsynchromized = MemberAttribute.Synced != null ? MemberAttribute.Synced.Value : this.IsDataMember || (MappedMember as MethodInfo).ReturnType == typeof(void);
        }

        #endregion
    }
}
