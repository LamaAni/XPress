using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XPress.Strings
{
    public static class HashExtentions
    {
        public static string CreateMD5HashId(this string val, Encoding enc = null)
        {
            enc = enc == null ? Encoding.ASCII : enc;
            return Encoding.ASCII.GetString(System.Security.Cryptography.MD5.Create().ComputeHash(enc.GetBytes(val)));
        }

        public static string Escape(this string val)
        {
            return Microsoft.JScript.GlobalObject.escape(val);
        }

        public static string Unescape(this string val)
        {
            return Microsoft.JScript.GlobalObject.unescape(val);
        }

        static string __matchReplaceEscapeCodes(Match m)
        {
            switch (m.Value[0])
            {
                case '\b': return "\\b";
                case '\f': return "\\f";
                case '\n': return "\\n";
                case '\r': return "\\r";
                case '\t': return "\\t";
                case '\v': return "\\v";
                case '\\': return "\\\\";
                case '\'': return "\\'";
                case '\"': return "\\\"";
            }
            return m.Value;
        }

        /// <summary>
        /// Escapes the string to a json format to allow for string.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string EscapeForJson(this string val, bool addApostrophe = false)
        {
            return Regex.Replace(val, "(\x08|\x0c|\x0a|\x0d|\x09|\"" + (addApostrophe ? "|'" : "") + "|\\\\)", __matchReplaceEscapeCodes);
        }

        /// <summary>
        /// Escapes the string to allow it to be represented in a single line in js. Escapes the
        /// charectes (by putting \ before them) of ",', and newLine.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string EscapeForJS(this string val, bool addApostrophe = false)
        {
            return Regex.Replace(val, "(\"|\\n|\\r" + (addApostrophe ? "|'" : "") + "|\\\\)", __matchReplaceEscapeCodes);
            //return val;
        }


        static string __unescapeForJsMatch(Match m)
        {
            if (m.Value.Length > 1)
                switch (m.Value[1])
                {
                    case 'n': return "\b";
                    case 'r': return "\b";
                    case '\\': return "\\";
                    case '\'': return "'";
                    case '"': return "\"";
                }
            return m.Value;
        }

        static string __unescapeForJsonMatch(Match m)
        {
            if (m.Value.Length > 1)
                switch (m.Value[1])
                {
                    case 'b': return "\b";
                    case 'f': return "\b";
                    case 'n': return "\b";
                    case 'r': return "\b";
                    case 't': return "\b";
                    case 'v': return "\b";
                    case '\\': return "\\";
                    case '\'': return "'";
                    case '"': return "\"";
                }
            return m.Value;
        }

        /// <summary>
        /// Unescapes a string that was escaped to be represented in a single js line.
        /// Unescapes the charecteds (by removing \ before them) of ",', and newLine.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string UnEscapeFromJsString(this string val)
        {
            return Regex.Replace(val, "\\\\.", __unescapeForJsMatch);
        }


        /// <summary>
        /// Unescapes a string that was escaped to be represented in a single js line.
        /// Unescapes the charecteds (by removing \ before them) of ",', and newLine.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string UnEscapeFromJsonString(this string val)
        {
            return Regex.Replace(val, "\\\\.", __unescapeForJsonMatch);
        }

        /// <summary>
        /// Escapes the string to allow it to appear inside an attribute. (Assumed with ").
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string EscapeForHtmlAttribute(this string val)
        {
            val = val.Replace("\n", "&#10;");
            val = val.Replace("\"", "&quot;");
            return val;
        }

        public static string EscapeForDelimiter(this string val, string delimiter)
        {
            val = val.Replace(delimiter, "\\" + delimiter);
            return val;
        }

        public static string UnescapeForDelimiter(this string val, string delimiter)
        {
            val = val.Replace("\\" + delimiter, delimiter);
            return val;
        }

        public static string[] SplitByDelimiter(this string val, string delimiter)
        {
            return Regex.Split(val, "(?<!\\\\)" + delimiter);
        }
    }
}

