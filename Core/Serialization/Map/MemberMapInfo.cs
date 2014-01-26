using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Attributes;

namespace XPress.Serialization.Map
{
    /// <summary>
    /// Holds information about the rendering of the sepecified member.
    /// </summary>
    public class MemberMapInfo
    {
        public MemberMapInfo(MemberInfo mi)
        {
            MemberInfo = mi;
            IgnoreMode = XPressIgnoreMode.IfNull;
            Map();
        }

        #region Static collection

        static MemberMapInfo()
        {
        }

        static ConcurrentDictionary<MemberInfo, MemberMapInfo> m_memberMapInfos = new ConcurrentDictionary<MemberInfo, MemberMapInfo>();

        public static MemberMapInfo Get(MemberInfo mi)
        {
            if (!m_memberMapInfos.ContainsKey(mi))
                m_memberMapInfos[mi] = new MemberMapInfo(mi);
            return m_memberMapInfos[mi];
        }

        #endregion

        #region members

        /// <summary>
        /// The associated member.
        /// </summary>
        public MemberInfo MemberInfo { get; private set; }

        /// <summary>
        /// If true then this is a peroperty associated with the member info.
        /// </summary>
        public bool IsProperty { get; private set; }

        /// <summary>
        /// If true this member (property) is read only.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// The default value associated witht he current member.
        /// </summary>
        public DefaultValueAttribute DefaultValue { get; private set; }

        /// <summary>
        /// If null dont ever ignore.
        /// </summary>
        public Attributes.XPressIgnoreMode IgnoreMode { get; private set; }

        /// <summary>
        /// If true this field is required.
        /// </summary>
        public bool Required { get; private set; }

        /// <summary>
        /// The order parameter for writing this value. Params are rendered by, order param, then name.
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// The name to associated with this value.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the property associated with the current.
        /// </summary>
        public Func<object, object> Getter { get; private set; }

        /// <summary>
        /// Sets the property associated with the current.
        /// </summary>
        public Action<object, object> Setter { get; private set; }

        /// <summary>
        /// The object type associated with the field.
        /// </summary>
        public Type MemberType { get; private set; }

        #endregion

        #region Methods

        protected void Map()
        {
            // setting the default 
            Name = MemberInfo.Name[0] != '<' ? MemberInfo.Name : MemberInfo.Name.Substring(1, MemberInfo.Name.IndexOf('>', 1) - 1);
            Order = int.MaxValue;
            Required = false;
            IgnoreMode = XPressIgnoreMode.IfNull | XPressIgnoreMode.IfDefualt;
            DefaultValue = null;
            IsProperty = MemberInfo is PropertyInfo;
            IsReadOnly = IsProperty && !(MemberInfo as PropertyInfo).CanWrite;

            MemberType = IsProperty ? (MemberInfo as PropertyInfo).PropertyType : (MemberInfo as FieldInfo).FieldType;

            // reading defualt value if any;
            if (Attribute.IsDefined(MemberInfo, typeof(DefaultValueAttribute)))
                DefaultValue = Attribute.GetCustomAttribute(MemberInfo, typeof(DefaultValueAttribute)) as DefaultValueAttribute;

            // checking for RmcMember
            if (Attribute.IsDefined(MemberInfo, typeof(Attributes.XPressMemberAttribute)))
            {
                XPressMemberAttribute atr = Attribute.GetCustomAttribute(MemberInfo, typeof(Attributes.XPressMemberAttribute)) as XPressMemberAttribute;
                IgnoreMode = atr.IgnoreMode;
                if (atr.Name != null) Name = atr.Name;
                Order = atr.Order;
                Required = atr.IsRequired;
            }
            else if (Attribute.IsDefined(MemberInfo, typeof(DataMemberAttribute)))
            {
                DataMemberAttribute atr = Attribute.GetCustomAttribute(MemberInfo, typeof(DataMemberAttribute)) as DataMemberAttribute;
                if (atr.EmitDefaultValue)
                    IgnoreMode = XPressIgnoreMode.IfDefualt;
                if (atr.Name != null) Name = atr.Name;
                Order = atr.Order;
                Required = atr.IsRequired;
            }
            else if (Attribute.IsDefined(MemberInfo, typeof(XPressIgnoreAttribute)))
            {
                XPressIgnoreAttribute ignore = Attribute.GetCustomAttribute(MemberInfo, typeof(XPressIgnoreAttribute)) as XPressIgnoreAttribute;
                IgnoreMode = ignore.IgnoreMode;
            }
            else if (Attribute.IsDefined(MemberInfo, typeof(NonSerializedAttribute)) || Attribute.IsDefined(MemberInfo, typeof(IgnoreDataMemberAttribute)))
            {
                IgnoreMode = XPressIgnoreMode.NeverIncluded;
            }

            if (IsProperty)
            {
                PropertyInfo pi = MemberInfo as PropertyInfo;
                Getter = (o) => pi.GetValue(o);
                Setter = (o, v) => pi.SetValue(o, v, new object[0]);
            }
            else
            {
                FieldInfo fi = MemberInfo as FieldInfo;
                Getter = (o) => fi.GetValue(o);
                Setter = (o, v) => fi.SetValue(o, v);
            }
        }

        #endregion


        #region string representation

        public override string ToString()
        {
            return this.MemberInfo.Name + " at " + this.MemberInfo.DeclaringType.ToString();
        }

        #endregion
    }
}
