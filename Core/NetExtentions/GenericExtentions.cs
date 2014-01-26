using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class GenericExtentions
    {
        public static void Sort<T, TKey>(this List<T> list, Func<T, TKey> sorter)
        {
            IEnumerable<T> tobesorted=list.ToArray();
            list.Clear();
            list.AddRange(tobesorted.OrderBy<T, TKey>(sorter));
        }
    }
}
