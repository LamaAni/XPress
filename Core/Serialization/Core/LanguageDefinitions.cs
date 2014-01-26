using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Core
{
    /// <summary>
    /// Launguage definitions that apply to a stream. 
    /// </summary>
    /// <typeparam name="C">A collection of language definitions that apply to the current stream.</typeparam>
    public abstract class LanguageDefinitions<C>
    {
        public LanguageDefinitions()
        {
        }

        /// <summary>
        /// A collection of chars that need trimming if before or after a value. (Not an object).
        /// </summary>
        public abstract C[] TrimChars { get; }

        /// <summary>
        /// The marker that tells the current value that it is a parse object.
        /// </summary>
        public abstract C ParseObjectMarker { get; }

        /// <summary>
        /// A symbol that states that the following value is a directive.
        /// </summary>
        public abstract C DirectiveMarker { get; }

        /// <summary>
        /// The symbol to add at the begining of every onject/array and after each object/array value.
        /// </summary>
        public abstract C PrettyfyNewLine { get; }

        /// <summary>
        /// The symbol to add at the begining of every onject/array and after each object/array value.
        /// </summary>
        public abstract C PrettyfyChildSpacer { get; }

        /// <summary>
        /// The marker for begining an object.
        /// </summary>
        public abstract C BeginObject { get; }

        /// <summary>
        /// The marker for ending an object.
        /// </summary>
        public abstract C EndObject { get; }

        /// <summary>
        /// The marker for begining an array.
        /// </summary>
        public abstract C BeginArray { get; }

        /// <summary>
        /// The marker for ending an array.
        /// </summary>
        public abstract C EndArray { get; }

        /// <summary>
        /// The data value seperator, in string this is generally ','.
        /// </summary>
        public abstract C DataValueSeperator { get;  }

        /// <summary>
        /// The data pair seperator, in string this is generally ':'
        /// </summary>
        public abstract C DataPairSeperator { get;  }

        /// <summary>
        /// The begining of reading data whilst ignoring any language definitions.
        /// </summary>
        public abstract C DataMarker { get; }

        /// <summary>
        /// The marker to escape the next symbol.
        /// </summary>
        public abstract C EscapeNextSymble { get; }

    }
}
