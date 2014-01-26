using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Parsers
{
    public class MemberInfoParser<T> : JsonObjectParser<T,MemberInfo>
    {
        public MemberInfoParser()
            : base("mi")
        {
        }

        public override Documents.JsonObjectPhrase<T> ParseFromObject(MemberInfo o, SerializationContext<T> context)
        {
            // getting the member info definition.
            return new Documents.JsonObjectPhrase<T>(Identity, new object[2] { context.ParseTypeToValue(o.DeclaringType), o.Name });
        }

        public override MemberInfo ObjectFromParse(Documents.JsonObjectPhrase<T> parse, SerializationContext<T> context)
        {
            if (parse.Words.Length < 2)
                throw new Exception("Phrase " + parse + "is inconsistant with member info type.");
            Type declaring = context.PraseTypeFromValue(parse.Words[0]);
            string name = parse.Words[1] as string;
            return declaring.GetMember(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)[0];
        }
    }
}
