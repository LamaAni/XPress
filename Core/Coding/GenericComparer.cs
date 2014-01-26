using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPress.Coding
{
    public class GenericComparer<T> : IComparer<T>
    {
        public GenericComparer(Func<T, T, int> f)
        {
            if (f == null)
                throw new Exception("Cannot use an null function to compare");
            m_compare = f;
        }

        Func<T, T, int> m_compare;

        public int Compare(T x, T y)
        {
            return m_compare(x, y);
        }

    }
}
