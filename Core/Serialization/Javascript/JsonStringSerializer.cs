using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Javascript
{
    public class JsonStringSerializer : JsonSerializer<char, string>
    {
        public JsonStringSerializer()
            : base(new Core.JavscriptJson.JsonStringReader(), new Core.JavscriptJson.JsonStringWriter(), JsonStringSerialzierGlobals.Definitions)
        {
        }

        static JsonStringSerializer()
        {
            m_globalSerializer = new JsonStringSerializer();
        }

        static JsonStringSerializer m_globalSerializer;

        /// <summary>
        /// The global serialzier.
        /// </summary>
        public static JsonStringSerializer Global
        {
            get { return JsonStringSerializer.m_globalSerializer; }
        }

        public override Documents.JsonDirective<string> BinderDirective
        {
            get { return new Documents.JsonDirective<string>("contains_binder", null); }
        }

        public override byte[] ToByteArray(string value)
        {
            return UTF8Encoding.UTF8.GetBytes(value);
        }

        public override string ParseByteArray(byte[] bytes)
        {
            return UTF8Encoding.UTF8.GetString(bytes);
        }
    }
}
