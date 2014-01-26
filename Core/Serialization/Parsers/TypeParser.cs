using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Parsers
{
    public class TypeParser<T>: JsonObjectParser<T,Type>
    {
        public TypeParser()
            : base("t")
        {

        }

        public override Documents.JsonObjectPhrase<T> ParseFromObject(Type o, SerializationContext<T> context)
        {
            return new JsonObjectPhrase<T>(Identity, context.ParseTypeToValue(o));
        }

        public override Type ObjectFromParse(Documents.JsonObjectPhrase<T> parse, SerializationContext<T> context)
        {
            if (parse.Words.Length == 0)
                throw new Exception("Type info not found when deserializing type. For phrase: " + parse);
            return context.PraseTypeFromValue(parse.Words[0]);
        }

        public class UnknownType
        {
        }
    }

}
