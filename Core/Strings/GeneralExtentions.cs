using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Strings
{
    public static class GeneralExtentions
    {
        public static bool IsNullOrEmpty(this string val, bool trim)
        {
            return val.IsNullOrEmpty(trim ? (Func<string, string>)null : (s) => s.Trim());
        }

        public static bool IsNullOrEmpty(this string val, Func<string, string> parseBeforeCheckIfEmpty)
        {
            if (val == null)
                return true;
            if (parseBeforeCheckIfEmpty != null)
                val = parseBeforeCheckIfEmpty(val);
            return val.Length == 0;
        }
    }
}
