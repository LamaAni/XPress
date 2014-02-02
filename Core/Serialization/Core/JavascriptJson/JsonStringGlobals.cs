using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Strings;

namespace XPress.Serialization.Core.JavscriptJson
{
    /// <summary>
    /// A static class that holds the default definitions for json string rmc.
    /// </summary>
    public static class JsonStringGlobals
    {

        static JsonDefinitions<string> m_ReaderDef;
        static JsonStringLanguageDefinitions m_stringLanguage = new JsonStringLanguageDefinitions();

        public static JsonStringLanguageDefinitions StringLanguage
        {
            get { return JsonStringGlobals.m_stringLanguage; }
        }

        public static JsonDefinitions<string> JsonDefintions
        {
            get
            {
                if (JsonStringGlobals.m_ReaderDef == null)
                {
                    JsonStringGlobals.m_ReaderDef = PrepareDefaultStringReaderDefinitions();
                }
                return JsonStringGlobals.m_ReaderDef;
            }
        }

        public static JsonDefinitions<string> PrepareDefaultStringReaderDefinitions()
        {
            Dictionary<Type, Func<string, object>> numericConvertors = new Dictionary<Type, Func<string, object>>();
            numericConvertors[typeof(double)] = (s) => Convert.ToDouble(s);
            numericConvertors[typeof(Int16)] = (s) => Convert.ToInt16(s);
            numericConvertors[typeof(Int32)] = (s) => Convert.ToInt32(s);
            numericConvertors[typeof(Int64)] = (s) => Convert.ToInt64(s);
            numericConvertors[typeof(Decimal)] = (s) => Convert.ToDecimal(s);
            numericConvertors[typeof(float)] = (s) => Convert.ToSingle(s);
            numericConvertors[typeof(UInt16)] = (s) => Convert.ToUInt16(s);
            numericConvertors[typeof(UInt32)] = (s) => Convert.ToUInt32(s);
            numericConvertors[typeof(UInt64)] = (s) => Convert.ToUInt64(s);

            Func<Type, string, object> rawToNumber = (t, s) =>
            {
                if (!numericConvertors.ContainsKey(t))
                    throw new Exception("Cannot find convertor for string value to type " + t);
                return numericConvertors[t](s);
            };

            JsonDefinitions<string> def = null;
            JsonInlineValueConverter<string, JsonDirective<string>> directiveConverter = new JsonInlineValueConverter<string, JsonDirective<string>>(
                "directive",
                (s) => s[0] == StringLanguage.DirectiveMarker,
                (d) => d.Data == null ? d.Directive : d.Directive + "#" + def.GetValue(d.Data),
                (d) =>
                {
                    string[] args = d.Split('#');
                    if (args.Length == 0)
                        throw new Exception("A directive must have at least an id.");
                    return new JsonDirective<string>(args[0], args.Length > 1 ? def.GetObjectFromRawValue(args[1]) : null);
                });

            JsonInlineValueConverter<string, JsonObjectPhrase<string>> parseObjectConverter = new JsonInlineValueConverter<string, JsonObjectPhrase<string>>("parseobject",
                (s) => s[0] == StringLanguage.ParseObjectMarker,
                (po) =>
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(StringLanguage.ParseObjectMarker);
                    builder.Append(def.GetValue(po.Identity));
                    foreach (object word in po.Words)
                    {
                        builder.Append(StringLanguage.ParseObjectMarker);
                        builder.Append(def.GetValue(word));
                    }
                    return builder.ToString();
                },
                (s) =>
                {
                    string[] args = s.Substring(1).Split(StringLanguage.ParseObjectMarker);
                    return new JsonObjectPhrase<string>(def.GetObjectFromRawValue(args[0]), args.Skip(1).Select(a => def.GetObjectFromRawValue(a)).ToArray());
                });

            def = new JsonDefinitions<string>(
                (d) => d.Source.Substring(d.StartIndex, d.EndIndex - d.StartIndex), s =>
                {
                    return s;
                }, "null", directiveConverter, parseObjectConverter);

            def.AddConverter<bool>(typeof(bool), "boolean", (s) => s == "true" || s == "false", (b) => b ? "true" : "false", (s) => s == "true");
            def.AddConverter<DBNull>(typeof(DBNull), "null", (s) => s.Length == 0 || s == "null", (o) => "", (s) => null);
            def.AddConverter<object>(new Type[] { typeof(Int16), typeof(Int32), typeof(Int64), typeof(double), typeof(float), typeof(Decimal), typeof(UInt16), typeof(UInt32), typeof(UInt64) },
                "numeric", s => { double val; return double.TryParse(s, out val); }, (n) => n.ToString(), (s) => new JsonNumber<string>(rawToNumber, s));


            //def.AddRawToObjectConverter(new RawValueConverter<string>((s) => s == "null", (s) => null));
            //def.AddRawToObjectConverter(new RawValueConverter<string>((s) => s.Length > 0 && s == "true" || s == "false", (s) => s == "true" ? true : false));
            //def.AddRawToObjectConverter(new RawValueConverter<string>((s) => s.Length > 0 && s.All(c => char.IsDigit(c)), (s) => new Documents.JsonNumber<string>(rawToNumber, s)));

            //// parsing the json directive to data.
            //def.AddObjectToRawConverter(new TypedObjectValueConverter<string>((o) =>
            //{
            //    // returning the string as str.
            //    return '"' + ((string)o).EscapeForJS() + '"';
            //}, new Type[] { typeof(string) }));

            //def.AddObjectToRawConverter(new TypedObjectValueConverter<string>((o) =>
            //{
            //    return o.ToString();
            //}, new Type[] { typeof(Int16), typeof(Int32), typeof(Int64), typeof(double), typeof(float), typeof(Decimal), typeof(UInt16), typeof(UInt32), typeof(UInt64) }));

            //def.AddObjectToRawConverter(new TypedObjectValueConverter<string>((o) =>
            //{
            //    return (bool)o ? "true" : "false";
            //}, typeof(bool)));

            //def.AddObjectToRawConverter(new TypedObjectValueConverter<string>((o) =>
            //{
            //    return ((JsonNumber<string>)o).Value.ToString();
            //}, typeof(JsonNumber<string>)));

            // adding the simple string converter. (Should be always last).
            def.AddConverter(typeof(string), "string", (s) => s.Length > 0 && s[0] == '"', (s) => "\"" + s.EscapeForJS() + "\"", (s) =>
            {
                if (s.Length < 3)
                    return "";

                //s = s.Substring(1, s.Length - 2).Replace("\\\"", "\""); // back to string representation.
                return s.Substring(1, s.Length - 2).UnEscapeFromJs();
            });

            return def;
        }
    }
}
