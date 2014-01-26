using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Map
{
    public class TypeMapInfo
    {
        public TypeMapInfo(Type t)
        {
            MappedType = t;
            Map();
        }

        #region Static

        static System.Collections.Concurrent.ConcurrentDictionary<Type, TypeMapInfo> m_typeInfoDic = new ConcurrentDictionary<Type, TypeMapInfo>();

        /// <summary>
        /// Returns the type map info for a specific collection.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static TypeMapInfo GetInfo(Type t)
        {
            if (!m_typeInfoDic.ContainsKey(t))
                m_typeInfoDic.TryAdd(t, new TypeMapInfo(t));

            return m_typeInfoDic[t];
        }

        #endregion

        #region members

        /// <summary>
        /// The type that is mapped in this collection.
        /// </summary>
        public Type MappedType { get; private set; }

        /// <summary>
        /// If the object is a list, then  the element type will be defined.
        /// </summary>
        public Type ArrayElementType { get; private set; }

        /// <summary>
        /// A collection of members associated with the current type.
        /// </summary>
        List<MemberMapInfo> members = null;

        /// <summary>
        /// A collection of members associated with the current type.
        /// </summary>
        public IReadOnlyList<MemberMapInfo> Members { get { return members; } }

        /// <summary>
        /// A collection of members associated with the current type.
        /// </summary>
        MemberMapInfo[] writeableMembers = null;

        /// <summary>
        /// A collection of members that can be written.
        /// </summary>
        public MemberMapInfo[] WriteableMembers { get { return writeableMembers; } }

        Dictionary<string, MemberMapInfo> _MembersByName;

        public IReadOnlyDictionary<string, MemberMapInfo> MembersByName
        {
            get { return _MembersByName; }
        }

        /// <summary>
        /// The .net serialization constructor.
        /// </summary>
        public ConstructorInfo DeserializationConstructor { get; private set; }

        /// <summary>
        /// If the current is a refrence type.
        /// </summary>
        public bool IsRefrenceType { get { return !MappedType.IsValueType; } }

        /// <summary>
        /// True if the specified type has the attribute System.Runtime.CompilerServices.CompilerGeneratedAttribute
        /// </summary>
        public bool IsCompilerGeneratedClass { get; private set; }
        
        /// <summary>
        /// True if the assembly was compiler generated.
        /// </summary>
        public bool IsCompilerGeneratedAssembly { get; private set; }

        /// <summary>
        /// Called when the object is deserializing.
        /// </summary>
        public Action<object, StreamingContext> InvokeOnDeserialzing { get; private set; }

        /// <summary>
        /// Called when the object is deserialized.
        /// </summary>
        public Action<object, StreamingContext> InvokeOnDeserialized { get; private set; }

        /// <summary>
        /// Called when the object is serialized.
        /// </summary>
        public Action<object, StreamingContext> InvokeOnSerialized { get; private set; }

        /// <summary>
        /// Called when the object is serializing.
        /// </summary>
        public Action<object, StreamingContext> InvokeOnSerializing { get; private set; }

        /// <summary>
        /// Called when the object has doen desierliaztion
        /// </summary>
        public Action<object, object> InvokeIDeserializationCallBack { get; private set; }

        /// <summary>
        /// A collection of generic type values that apply to the mapped type, by order of the type value.
        /// </summary>
        public Type[] GenericTypeArguments { get; private set; }

        #endregion

        #region Type mapping

        /// <summary>
        /// Test if a type implements IList of T, and if so, determine T.
        /// </summary>
        public static Type GetSingleGenericInterfaceType(Type type, Type genericBase)
        {

            var interfaceTest = new Func<Type, Type>(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericBase ? i.GetGenericArguments().Single() : null);

            Type elType = interfaceTest(type);
            if (elType != null)
            {
                return elType;
            }

            elType = null;

            foreach (var i in type.GetInterfaces())
            {
                elType = interfaceTest(i);
                if (elType != null)
                {
                    return elType;
                }
            }
            
            return elType;
        }

        protected void Map()
        {
            // no duplicate naming allowed.
            TypeMapInfo baseInfo = MappedType.BaseType != typeof(object) && MappedType.BaseType != null ? TypeMapInfo.GetInfo(MappedType.BaseType) : null;

            if (MappedType.IsGenericType)
            {
                GenericTypeArguments = MappedType.GetGenericArguments().ToArray();
            }

            ArrayElementType = typeof(Array).IsAssignableFrom(MappedType) ? MappedType.GetElementType() : null;

            IsCompilerGeneratedClass = MappedType.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
            IsCompilerGeneratedAssembly = MappedType.Assembly.IsDefined(typeof(System.CodeDom.Compiler.GeneratedCodeAttribute), false);
            HashSet<string> memberNamesTaken = new HashSet<string>();

            // get the member selection attribute, associated with the current.
            Attributes.IMembersSelectionAttribute msa = null;
            if (Attribute.IsDefined(MappedType, typeof(Attributes.XPressMemberSelectionAttribute)))
                msa = Attribute.GetCustomAttribute(MappedType, typeof(Attributes.XPressMemberSelectionAttribute)) as Attributes.XPressMemberSelectionAttribute;
            else if (Attribute.IsDefined(MappedType, typeof(Attributes.XPressInheritedMemberSelectionAttribute)))
                msa = Attribute.GetCustomAttribute(MappedType, typeof(Attributes.XPressInheritedMemberSelectionAttribute)) as Attributes.XPressInheritedMemberSelectionAttribute;

            // creating defualt collections.
            IEnumerable<MemberInfo> autoSelect = new MemberInfo[0], optIn = new MemberInfo[0];
            IEnumerable<MemberInfo> allMembers = new MemberInfo[0];
            Attributes.XPressMemberSelectionType selection = msa != null ? msa.Selection :
                (MappedType.IsClass && !IsCompilerGeneratedClass ? Attributes.XPressMemberSelectionType.Properties : Attributes.XPressMemberSelectionType.Fields);

            // checing for .net serialization.
            if (MappedType.GetInterfaces().Any(i => typeof(ISerializable).IsAssignableFrom(i)))
            {
                DeserializationConstructor = MappedType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(c =>
                {
                    ParameterInfo[] prs = c.GetParameters();
                    if (prs.Length != 2)
                        return false;
                    if (prs[0].ParameterType != typeof(SerializationInfo))
                        return false;
                    if (prs[1].ParameterType != typeof(StreamingContext))
                        return false;
                    return true;
                }).FirstOrDefault();
            }

            // getting all members
            allMembers = MappedType.GetMembers(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // finding deserialization and serialization methods.
            allMembers.Where(mi => mi is MethodInfo).Cast<MethodInfo>().ForEach(mi =>
            {
                if (Attribute.GetCustomAttribute(mi, typeof(System.Runtime.Serialization.OnDeserializedAttribute)) != null)
                {
                    InvokeOnDeserialized = (o, context) => mi.Invoke(o, new object[1] { context });
                }

                if (Attribute.GetCustomAttribute(mi, typeof(System.Runtime.Serialization.OnDeserializingAttribute)) != null)
                {
                    InvokeOnDeserialzing = (o, context) => mi.Invoke(o, new object[1] { context });
                }

                if (Attribute.GetCustomAttribute(mi, typeof(System.Runtime.Serialization.OnSerializedAttribute)) != null)
                {
                    InvokeOnSerialized = (o, context) => mi.Invoke(o, new object[1] { context });
                }

                if (Attribute.GetCustomAttribute(mi, typeof(System.Runtime.Serialization.OnSerializingAttribute)) != null)
                {
                    InvokeOnSerializing = (o, context) => mi.Invoke(o, new object[1] { context });
                }

            });

            allMembers = allMembers.Where(mi => mi.DeclaringType == this.MappedType) // only the current type, do not apply to base types. that comes later.
                .Where(mi => mi is FieldInfo || mi is PropertyInfo)
                .Where(mi =>
                {
                    PropertyInfo pi = mi as PropertyInfo;
                    if (pi == null)
                        return true;
                    if (pi.GetOptionalCustomModifiers().Length > 0)
                        return false;
                    return true;
                })
                .ToArray();

            optIn = allMembers
                .Where(mi => Attribute.IsDefined(mi, typeof(Attributes.XPressMemberAttribute)) || Attribute.IsDefined(mi, typeof(System.Runtime.Serialization.DataMemberAttribute)))
                .ToArray();

            // Getting auto select members.
            if (!selection.HasFlag(Attributes.XPressMemberSelectionType.OptIn))
            {
                autoSelect = allMembers.Except(optIn);
                if (!selection.HasFlag(Attributes.XPressMemberSelectionType.Properties))
                    autoSelect = autoSelect.Where(mi => !(mi is PropertyInfo));
                if (!selection.HasFlag(Attributes.XPressMemberSelectionType.Fields))
                    autoSelect = autoSelect.Where(mi => !(mi is FieldInfo));
                if (!selection.HasFlag(Attributes.XPressMemberSelectionType.ReadOnlyProperties))
                    autoSelect = autoSelect.Where(mi =>
                    {
                        PropertyInfo pi = mi as PropertyInfo;
                        if (pi == null)
                            return true;
                        if (!pi.CanWrite)
                            return false;
                        return true;
                    });
            }

            // getting all the relevant map infos for this type.
            IEnumerable<MemberMapInfo> mapInfos =
                autoSelect.Concat(optIn).Select(mi => MemberMapInfo.Get(mi)).Where(mi => !mi.IgnoreMode.HasFlag(Attributes.XPressIgnoreMode.NeverIncluded)).ToArray();

            // adding all names.
            memberNamesTaken.UnionWith(mapInfos.Select(mi => mi.Name));

            // merging with base type, not including the members that are validated by the current.
            if (baseInfo != null)
                mapInfos = mapInfos.Concat(baseInfo.Members.Where(mi => !memberNamesTaken.Contains(mi.Name)));
            members = mapInfos.OrderBy(m => m.Order).ThenBy(m => m.Name).ToList();

            writeableMembers = members.Where(mmi => mmi.Required || !mmi.IgnoreMode.HasFlag(Attributes.XPressIgnoreMode.NeverIncluded)).ToArray();

            _MembersByName = members.ToDictionary(mmi => mmi.Name, mmi => mmi);
        }

        #endregion
    }

    public enum XPressSerialziationType { Object, Dictionary, Array };
}
