using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Core
{
    /// <summary>
    /// Represents a source marjer for a specific source type
    /// The marking points are locations int the source array that the source can retrive the data from.
    /// </summary>
    public class ElementData<T>
    {
        public ElementData(T source, int startIndex, int endIndex, JsonDefinitions<T> defintions, bool isDirective)
        {
            Source = source;
            Definitions = defintions;
            StartIndex = startIndex;
            EndIndex = endIndex;
            IsDirective = isDirective;
        }

        /// <summary>
        /// If true the current data is empty.
        /// </summary>
        public bool IsEmpty { get { return StartIndex >= EndIndex; } }

        /// <summary>
        /// If true the current value is an element directive.
        /// </summary>
        public bool IsDirective { get; private set; }

        /// <summary>
        /// Has information on how to handle data.
        /// </summary>
        public JsonDefinitions<T> Definitions { get; private set; }

        /// <summary>
        /// The reader;
        /// </summary>
        public T Source { get; private set; }

        /// <summary>
        /// The start index.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// The end index.
        /// </summary>
        public int EndIndex { get; private set; }

        /// <summary>
        /// returns the data from the source, according to the start and end indexis.
        /// This is a long action that should be taken once.
        /// </summary>
        /// <returns>The element data</returns>
        public T GetRawData()
        {
            return Definitions.GetElementData(this);
        }

        /// <summary>
        /// Returns the string representation of the data and the data location.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + this.StartIndex + "," + this.EndIndex + ") " + GetRawData().ToString();
        }

        /// <summary>
        /// Maps the data to a value.
        /// </summary>
        /// <returns>The mapped value.</returns>
        public virtual object GetValue()
        {
            return Definitions.GetObject(this);
        }
    }
}
