using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Parsers
{
    public class DateTimeParser<T>: JsonObjectParser<T,DateTime>
    {
        public DateTimeParser()
            : base("dt")
        {
        }

        public static DateTime Epoc = new DateTime(1970, 1, 1);

        public override Documents.JsonObjectPhrase<T> ParseFromObject(DateTime o, SerializationContext<T> context)
        {
            return new Documents.JsonObjectPhrase<T>(Identity, Math.Round((o - Epoc).TotalMilliseconds));
        }

        public override DateTime ObjectFromParse(Documents.JsonObjectPhrase<T> parse, SerializationContext<T> context)
        {
            return Epoc.AddMilliseconds((parse.Words[0] as JsonNumber<T>).As<Double>());
        }
    }
}
