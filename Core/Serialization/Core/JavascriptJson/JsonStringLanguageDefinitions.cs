using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Core.JavscriptJson
{
    public class JsonStringLanguageDefinitions : LanguageDefinitions<char>
    {
        public override char ParseObjectMarker
        {
            get { return '#'; }
        }
        public override char[] TrimChars
        {
            get { return new char[4] { ' ', '\n', '\r', '\t' }; }
        }

        public override char DirectiveMarker
        {
            get { return '^'; }
        }

        public override char PrettyfyChildSpacer
        {
            get { return '\t'; }
        }

        public override char PrettyfyNewLine
        {
            get { return '\n'; }
        }

        public override char BeginArray
        {
            get { return '['; }
        }

        public override char EndArray
        {
            get { return ']'; }
        }

        public override char BeginObject
        {
            get { return '{'; }
        }

        public override char EndObject
        {
            get { return '}'; }
        }

        public override char DataMarker
        {
            get { return '"'; }
        }

        public override char EscapeNextSymble
        {
            get { return '\\'; }
        }

        public override char DataPairSeperator
        {
            get { return ':'; }
        }

        public override char DataValueSeperator
        {
            get { return ','; }
        }
    }
}
