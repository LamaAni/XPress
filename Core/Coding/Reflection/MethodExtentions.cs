using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

    /// <summary>
    /// Provides static extemtion methods for member identfication.
    /// </summary>
public static class RMC__MethodExtentions
{
    #region static members

    static ConcurrentDictionary<string, MemberInfo> m_map = new ConcurrentDictionary<string, MemberInfo>();
    static ConcurrentDictionary<MemberInfo, string> m_reverseMap = new ConcurrentDictionary<MemberInfo, string>();

    static long m_activeId = 0;

    #endregion

    #region Mapping method

    public static string ToMemberId(this MemberInfo mi)
    {
        if (!m_reverseMap.ContainsKey(mi))
        {
            string id = mi.DeclaringType.Assembly.FullName + "|" + mi.DeclaringType.ToString() + "|" + mi.Name;
            m_map.TryAdd(id, mi);
            m_reverseMap.TryAdd(mi, id);
        }
        return m_reverseMap[mi];
    }

    #endregion

    #region invoking members

    public static MemberInfo InvokeAsMember(this string id, object on, IEnumerable<string> jsonMembers)
    {
        // getting the member info.
        MemberInfo m;
        if (!m_map.TryGetValue(id, out m))
            throw new Exception("Nethod not found on serverside.");

        PropertyInfo pi = m as PropertyInfo;
        MethodInfo mi = m as MethodInfo;
        FieldInfo fi = m as FieldInfo;
        if (mi != null)
            mi.InvokeAsMember(on, jsonMembers);
        if (pi != null && pi.CanWrite)
            pi.InvokeAsMember(on, jsonMembers);
        else if (fi != null)
            fi.InvokeAsMember(on, jsonMembers);
        return m;
    }

    public static MemberInfo ToMemberInfo(this string id)
    {
        MemberInfo m;
        if (!m_map.TryGetValue(id, out m))
            throw new Exception("Nethod not found on serverside.");

        return m;
    }

    public static void InvokeAsMember(this MemberInfo m, object on, IEnumerable<object> members)
    {
        MethodInfo mi = m as MethodInfo;
        if (mi != null)
            mi.InvokeAsMember(on, members);
    }

    //public static void InvokeAsMember(this PropertyInfo pi, object on, IEnumerable<string> jsonMembers)
    //{
    //    // invoking as member property. (setting the value).
    //    object val = jsonMembers.First().FromJson(pi.PropertyType);
    //    pi.SetValue(on, val, new object[0]);
    //}

    //public static void InvokeAsMember(this FieldInfo fi, object on, IEnumerable<string> jsonMembers)
    //{
    //    object val = jsonMembers.First().FromJson(fi.FieldType);
    //    fi.SetValue(on, val);
    //}

    //public static void InvokeAsMember(this MethodInfo mi, object on, IEnumerable<string> jsonMembers)
    //{
    //    List<object> members = new List<object>();
    //    int index = 0;
    //    foreach (ParameterInfo pr in mi.GetParameters())
    //    {
    //        members.Add(jsonMembers.ElementAt(index).FromJson(pr.ParameterType));
    //        index += 1;
    //    }

    //    mi.InvokeAsMember(on, members);
    //}

    public static void InvokeAsMember(this MethodInfo mi, object on, IEnumerable<object> members)
    {
        mi.Invoke(on, members.ToArray());
    }

    #endregion
}
