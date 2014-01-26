using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Core.JavscriptJson
{
    public class JsonStringWriter : JsonWriter<char, string>
    {
        public JsonStringWriter()
            : this(JsonStringGlobals.StringLanguage, JsonStringGlobals.JsonDefintions)
        {

        }

        protected JsonStringWriter(LanguageDefinitions<char> language, JsonDefinitions<string> definitions)
            : base(language, definitions)
        {
        }

        /// <summary>
        /// Creates a new builder.
        /// </summary>
        /// <returns></returns>
        public override StreamBuilder<char, string> CreateBuilder()
        {
            return new JsonStringBuilder(Language);
        }
    }

    public class JsonStringBuilder : StreamBuilder<char, string>
    {
        public JsonStringBuilder(LanguageDefinitions<char> language)
            : base(language)
        {
        }

        List<string> _col = new List<string>();

        public override string ToValue()
        {
            StringBuilder builder = new StringBuilder();
            _col.ForEach(s => builder.Append(s));
            return builder.ToString();
        }

        public override string ToPrettyValue()
        {
            // adding the values. 
            StringBuilder builder = new StringBuilder();
            int childLevel = 0;
            _col.ForEach(s =>
            {

                if (s.Length == 1 && (s[0] == Language.EndObject || s[0] == Language.EndArray))
                {
                    childLevel = AddSpacer(builder, childLevel, -1);
                }
                builder.Append(s);
                if (s.Length == 1 && (s[0] == Language.BeginObject || s[0] == Language.BeginArray || s[0] == Language.DataValueSeperator))
                {
                    childLevel = AddSpacer(builder, childLevel, s[0] == Language.DataValueSeperator ? 0 : +1);
                }
            });

            return builder.ToString();
        }

        private static int AddSpacer(StringBuilder builder, int childLevel, int add)
        {
            childLevel += add;
            if (childLevel <= 0)
                childLevel = 0;
            builder.Append("\n");
            builder.Append(new string('\t', childLevel));
            return childLevel;
        }

        public override void Append(string val)
        {
            _col.Add(val);
        }

        public override void Append(char val)
        {
            _col.Add(Convert.ToString(val));
        }

        public void Clear()
        {
            _col.Clear();
        }
    }
}
