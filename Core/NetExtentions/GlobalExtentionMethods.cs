using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using XPress.NetExtentions;

public static class GlobalExtentionMethods
{
    #region Collection extention methods

    public static IEnumerable<T> Distinct<T, TVal>(this IEnumerable<T> col, Func<T, TVal> compareValueSelector)
    {
        return col.GroupBy<T, TVal>(v => compareValueSelector(v)).Select(grp => grp.First());
    }

    public static IEnumerable<TRslt> Interlace<TRslt, TA, TB>(this IEnumerable<TA> ais, IEnumerable<TB> bis, Func<TA, TB, TRslt> func)
    {
        IEnumerator<TA> aisEnum = ais.GetEnumerator();
        IEnumerator<TB> bisEnum = bis.GetEnumerator();
        List<TRslt> rslt = new List<TRslt>();

        while (aisEnum.MoveNext() && bisEnum.MoveNext())
        {
            rslt.Add(func(aisEnum.Current, bisEnum.Current));
        }

        return rslt;
    }

    public static bool IsEmpty<T>(this IEnumerable<T> col)
    {
        IEnumerator<T> e = col.GetEnumerator();
        return !e.MoveNext();
    }

    /// <summary>
    /// Executes the command for each of the elements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="vals"></param>
    /// <param name="f"></param>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> vals, Action<T> f)
    {
        foreach (T v in vals.ToArray())
            f(v);

        return vals;
    }

    /// <summary>
    /// Executes the command for each of the elemens, has an index i.
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    /// <param name="vals">The values</param>
    /// <param name="f">A function (index, elm)</param>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> vals, Action<T, int> f)
    {
        int i = 0;
        foreach (T v in vals.ToArray())
        {
            f(v, i);
            i += 1;
        }
        return vals;
    }

    public static IEnumerable<IEnumerable<T>> ToGroupsOf<T>(this IEnumerable<T> vals, int itemsInAGroup)
    {
        int totalCount = vals.Count();
        List<List<T>> lst = new List<List<T>>();
        List<T> current = null;

        foreach (T val in vals.ToArray())
        {
            if (current == null || current.Count == itemsInAGroup)
            {
                //int curCount = totalCount - totalIndex > itemsInAGroup ? itemsInAGroup : totalCount - totalIndex;
                current = new List<T>();
                lst.Add(current);
            }
            current.Add(val);
        }

        return lst;
    }

    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> vals,
        Func<TSource, TKey> selector, Func<TKey, TKey, int> comparer)
    {
        return vals.OrderBy<TSource, TKey>(selector, new XPress.Coding.GenericComparer<TKey>(comparer));
    }

    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> vals,
        Func<TSource, TKey> selector, Func<TKey, TKey, int> comparer)
    {
        return vals.OrderByDescending(selector, new XPress.Coding.GenericComparer<TKey>(comparer));
    }

    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> vals,
        Func<TSource, TKey> selector, Func<TKey, TKey, int> comparer)
    {
        return vals.ThenBy(selector, new XPress.Coding.GenericComparer<TKey>(comparer));
    }

    public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> vals,
        Func<TSource, TKey> selector, Func<TKey, TKey, int> comparer)
    {
        return vals.ThenByDescending(selector, new XPress.Coding.GenericComparer<TKey>(comparer));
    }

    #endregion

    #region expand string writer

    public static void CloseAndDispose(this System.IO.TextWriter writer)
    {
        writer.Close();
        writer.Dispose();
    }

    public static string ToStringAndDispose(this System.IO.TextWriter writer)
    {
        writer.Flush();
        string rslt = writer.ToString();
        writer.CloseAndDispose();
        return rslt;
    }

    #endregion

    #region DateTimeExtend

    public static DateTime GetDateWithOffset(this DateTime dt, TimeSpan offset)
    {
        return (dt - offset).Date;
    }

    public static DateTime GetWeek(this DateTime dt, TimeSpan offset)
    {
        dt = dt.GetDateWithOffset(offset);
        return dt.Date.AddDays(-(int)dt.DayOfWeek);
    }

    public static DateTime GetWeek(this DateTime dt)
    {
        return GetWeek(dt, new TimeSpan());
    }

    public static DateTime GetMonth(this DateTime dt, TimeSpan offset)
    {
        dt = dt.GetDateWithOffset(offset);
        return new DateTime(dt.Year, dt.Month, 1);
    }

    public static DateTime GetMonth(this DateTime dt)
    {
        return GetMonth(dt, new TimeSpan());
    }

    public static string ToHebString(this DateTime dt)
    {
        return dt.ToString("dd/MM/yyyy HH:MM");
    }

    public static string ToHebDateString(this DateTime dt)
    {
        return dt.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// Returns the nearest quarter hour.
    /// </summary>
    /// <param name="ts"></param>
    /// <returns></returns>
    public static TimeSpan ToQuarterHour(this TimeSpan ts)
    {
        double h = Math.Round(ts.TotalHours);
        double m = ts.TotalHours - h;
        if (m < 0.25)
            m = 0;
        else if (m >= 0.25 && m < 0.5)
            m = 0.25;
        else if (m >= 0.5 && m < 0.75)
            m = 0.5;
        else if (m >= 0.75)
            m = 0.75;
        return TimeSpan.FromHours(h + m);
    }

    public static string ToHoursAndMinutesString(this TimeSpan ts)
    {
        //  return ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00");
        return Convert.ToInt32(Math.Floor(ts.TotalHours)) + ":" + ts.Minutes.ToString("00");
    }

    public static string ToDayMonthKey(this DateTime dt)
    {
        return dt.Month + "_" + dt.Day;
    }

    public static string ToJSDateMSString(this DateTime dt)
    {
        return Math.Round((dt - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
    }

    public static DateTime FromJSDateMSString(this string str)
    {
        return new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(str));
    }

    #endregion

    #region Collection additions

    public static bool ContainsAny<T>(this ISet<T> set, IEnumerable<T> vals)
    {
        foreach (T v in vals)
            if (set.Contains(v))
                return true;
        return false;
    }

    public static bool TryAdd<T>(this ICollection<T> set, T val)
    {
        if (set.Contains(val))
            return false;
        set.Add(val);
        return true;
    }

    public static bool TryRemove<T>(this ICollection<T> set, T val)
    {
        if (!set.Contains(val))
            return false;
        set.Remove(val);
        return true;
    }

    public static bool TryAdd<Key, Value>(this IDictionary<Key, Value> dic, Key k, Value v)
    {
        if (dic.ContainsKey(k))
            return false;
        dic.Add(k, v);
        return true;
    }

    public static bool TryRemove<Key, Value>(this IDictionary<Key, Value> dic, Key k)
    {
        if (!dic.ContainsKey(k))
            return false;
        dic.Remove(k);
        return true;
    }

    #endregion

    #region Dictionary methods

    public static void MergeOverLeft<TKey, TValue>(this IDictionary<TKey, TValue> t, IDictionary<TKey, TValue> toBeAdded)
    {
        foreach (TKey key in toBeAdded.Keys)
        {
            t[key] = toBeAdded[key];
        }
    }

    #endregion

    #region database extnetion methods

    public static IDbCommand CreateCommand(this IDbConnection con, string query)
    {
        IDbCommand cmnd = con.CreateCommand();
        cmnd.CommandText = query;
        return cmnd;
    }
    #endregion

    #region type extend

    public static IEnumerable<MemberInfo> FindAllMembers(this Type t)
    {
        return FindAllMembers(t, MemberTypes.All);
    }

    public static IEnumerable<MemberInfo> FindAllMembers(this Type t, MemberTypes types)
    {
        return FindAllMembers(t, types, BindingFlags.NonPublic | BindingFlags.Public);
    }

    public static IEnumerable<MemberInfo> FindAllMembers(this Type t, MemberTypes types, BindingFlags flags)
    {
        return FindAllMembers(t, types, flags, null);
    }

    static void FindAllMembersRecusrive(MemberTypes types, BindingFlags flags, Dictionary<string, MemberInfo> members, Type t, MemberFilter del, Func<MemberInfo, bool> filter)
    {
        foreach (MemberInfo mi in t.FindMembers(types, flags, del, filter))
            if (members.ContainsKey(mi.Name))
                continue;
            else members[mi.Name] = mi;
        if ((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic && t.BaseType != typeof(object))
            FindAllMembersRecusrive(types, flags, members, t.BaseType, del, filter);
    }

    static bool FindMemberSearchCriteria(MemberInfo objMemberInfo, Object objSearch)
    {
        // Compare the name of the member function with the filter criteria.
        if (objSearch == null)
            return true;
        return ((Func<MemberInfo, bool>)objSearch)(objMemberInfo);
    }

    /// <summary>
    /// Finds all members in a recusive fashion. The search is name specific, and therefore
    /// from two members of the name only the member of the top type will be selected.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="types"></param>
    /// <param name="flags"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static IEnumerable<MemberInfo> FindAllMembers(this Type t, MemberTypes types, BindingFlags flags, Func<MemberInfo, bool> filter)
    {
        Dictionary<string, MemberInfo> members = new Dictionary<string, MemberInfo>();
        FindAllMembersRecusrive(types, flags, members, t, new MemberFilter(FindMemberSearchCriteria), filter);
        return members.Values.ToArray();
    }

    #endregion

    #region string extention methods

    public static bool IsEmpty(this string str)
    {
        return str == null || str.Trim().Length == 0;
    }

    /// <summary>
    /// A logical compare method. Compares the current list using the windows numeric value compare method.
    /// </summary>
    /// <param name="me"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static int LogicalCompareTo(this string x, string y)
    {
        // simple cases
        if (x == y) // also handles null
            return 0;
        if (x == null)
            return -1;
        if (y == null)
            return +1;

        int ix = 0;
        int iy = 0;
        while (ix < x.Length && iy < y.Length)
        {
            if (Char.IsDigit(x[ix]) && Char.IsDigit(y[iy]))
            {
                // We found numbers, so grab both numbers
                int ix1 = ix++;
                int iy1 = iy++;
                while (ix < x.Length && Char.IsDigit(x[ix]))
                    ix++;
                while (iy < y.Length && Char.IsDigit(y[iy]))
                    iy++;
                int numberFromX = int.Parse(x.Substring(ix1, ix - ix1));
                int numberFromY = int.Parse(y.Substring(iy1, iy - iy1));

                int comparison = numberFromX.CompareTo(numberFromY);
                if (comparison != 0)
                    return comparison;
            }
            else
            {
                int comparison = x[ix].CompareTo(y[iy]);
                if (comparison != 0)
                    return comparison;
                ix++;
                iy++;
            }
        }

        // we should not be here with no parts left, they're equal
        System.Diagnostics.Debug.Assert(ix < x.Length || iy < y.Length);

        // we still got parts of x left, y comes first
        if (ix < x.Length)
            return +1;

        // we still got parts of y left, x comes first
        return -1;
    }

    #endregion

    #region init extention methods

    static string
        numbers = "0123456789",
        letters = "abcdefghijklmnopqrstvwxyz",
        lettersUp = letters.ToUpper(),
        codeAll = numbers + letters + lettersUp;

    static Random m_rand = new Random();

    public static string GenerateCode(this int size)
    {
        return size.GenerateCode(CodeGeneratorType.All);
    }

    public static string GenerateCode(this int size, CodeGeneratorType type)
    {
        string source;

        if (type == CodeGeneratorType.All)
        {
            source = codeAll;
        }
        else
        {
            StringBuilder sourceBuilder = new StringBuilder();
            if ((type & CodeGeneratorType.Letters) == CodeGeneratorType.Numbers)
                sourceBuilder.Append(numbers);
            if ((type & CodeGeneratorType.Letters) == CodeGeneratorType.Letters)
                sourceBuilder.Append(letters);
            if ((type & CodeGeneratorType.Letters) == CodeGeneratorType.LettersUpperCase)
                sourceBuilder.Append(lettersUp);

            source = sourceBuilder.ToString();
        }

        return size.GenerateCode(source);
    }

    public static string GenerateCode(this int size, string source)
    {
        StringBuilder code = new StringBuilder();
        int maxIndex = source.Length - 1;
        for (int i = 0; i < size; i++)
        {

            code.Append(source[Convert.ToInt32(Math.Round(m_rand.NextDouble() * maxIndex))]);
        }

        return code.ToString();
    }

    public enum CodeGeneratorType { Numbers = 1, Letters = 2, LettersUpperCase = 4, All = 16 };

    #endregion

    #region Byte/Stream extention methods

    public enum ByteSizes { Bytes, KB, MB, GB, TB };

    /// <summary>
    /// returns the size of the current array in the selected unit, in a human readable form.
    /// </summary>
    /// <param name="vals"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static double GetHRSize(this byte[] vals, ByteSizes size = ByteSizes.KB)
    {
        return GetHRSize(vals.LongLength, size);
    }

    public static double GetHRSize(this System.IO.Stream strm, ByteSizes size = ByteSizes.KB)
    {
        return GetHRSize(strm.Length, size);
    }

    static double GetHRSize(long length, ByteSizes size)
    {
        switch (size)
        {
            case ByteSizes.KB: return length * 1.0 / Math.Pow(10, 3);
            case ByteSizes.GB: return length * 1.0 / Math.Pow(10, 9);
            case ByteSizes.MB: return length * 1.0 / Math.Pow(10, 6);
            case ByteSizes.TB: return length * 1.0 / Math.Pow(10, 12);
            default: return length;
        }
    }

    #endregion
}




