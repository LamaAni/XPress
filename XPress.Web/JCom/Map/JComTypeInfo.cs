using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace XPress.Web.JCom.Map
{
    /// <summary>
    /// Represents a jcom object map that handles the information stored inside a specified JCom object.
    /// The object map allows for specific objects to communicate object the network via json.
    /// </summary>
    public class JComTypeInfo
    {
        public JComTypeInfo(Type t)
        {
            MappedType = t;
            Map();
        }

        #region static methods

        static ConcurrentDictionary<Type, JComTypeInfo> m_byType = new ConcurrentDictionary<Type, JComTypeInfo>();

        /// <summary>
        /// Returns the object map according to the type for each of the jcom objects.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static JComTypeInfo Get(Type t)
        {
            if (!m_byType.ContainsKey(t))
                m_byType.TryAdd(t, new JComTypeInfo(t));
            return m_byType[t];
        }

        #endregion

        #region members

        /// <summary>
        /// If true the current requires a client side implementation of the type.
        /// </summary>
        public bool RequiresClientSideDefinition { get; private set; }

        public Type MappedType { get; private set; }

        List<JComMemberInfo> m_dataMembers;

        /// <summary>
        /// The members associated with this type. All associated members a either from the current type or some parent type.
        /// </summary>
        public IReadOnlyList<JComMemberInfo> DataMembers
        {
            get { return m_dataMembers; }
        }

        List<JComMemberInfo> m_methods;

        /// <summary>
        /// All the methods.
        /// </summary>
        public IReadOnlyList<JComMemberInfo> Methods
        {
            get { return m_methods; }
        }

        Dictionary<string, JComMemberInfo> m_members;

        /// <summary>
        /// A collection of all the data members by name.
        /// </summary>
        public IReadOnlyDictionary<string, JComMemberInfo> Members
        {
            get { return m_members; }
        }

        /// <summary>
        /// The name of the type on the client side.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// True if there are any readable data members to send to the client side.
        /// </summary>
        public bool RequiresDataObject { get; private set; }

        #endregion

        #region method

        void Map()
        {
            // no duplicate naming allowed.
            JComTypeInfo baseInfo = MappedType.BaseType != typeof(object) && MappedType.BaseType != null ? JComTypeInfo.Get(MappedType.BaseType) : null;

            HashSet<string> memberNamesTaken = new HashSet<string>(); // collection of all the member names taken.

            // getting all the members
            IEnumerable<MemberInfo> members = MappedType.GetMembers(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Where(mi => mi.DeclaringType == MappedType)
                .Where(mi => Attribute.IsDefined(mi, typeof(Attributes.ClientSideAttribute)))
                .Where(mi => mi is FieldInfo || mi is PropertyInfo || mi is MethodInfo)
                .Where(mi =>
                {
                    PropertyInfo pi = mi as PropertyInfo;
                    if (pi == null)
                        return true;
                    if (pi.GetOptionalCustomModifiers().Length > 0)
                        return false;
                    return true;
                }).ToArray();

            IEnumerable<JComMemberInfo> infos = members.Select(mi => JComMemberInfo.Get(mi)).ToArray();
            memberNamesTaken.UnionWith(infos.Select(i => i.Name));

            if (baseInfo != null)
                infos = infos.Concat(baseInfo.Members.Values.Where(i => !memberNamesTaken.Contains(i.Name)));

            m_members = infos.ToDictionary(mi => mi.Name);

            // adding the members.
            m_dataMembers = infos.Where(mi => mi.IsDataMember).ToList();
            m_methods = infos.Where(mi => !mi.IsDataMember).ToList();
            Name = MappedType.ToString().Replace(".", "_");

            // Check if any client side definition is required.
            RequiresClientSideDefinition = DataMembers.Count > 0 || Methods.Count > 0;
            RequiresDataObject = DataMembers.Where(mi => mi.CanRead).Count() > 0;
        }

        #endregion
    }
}
