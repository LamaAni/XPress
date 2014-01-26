using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Core
{
    /// <summary>
    /// Implements a single data value converter.
    /// </summary>
    public class RawValueConverter<T>
    {
        /// <summary>
        /// Implement a data value converter collection
        /// </summary>
        /// <param name="canConvertRaw">If true, this converter can convert raw values into objects</param>
        /// <param name="convertToObject">The convert to object function</param>
        public RawValueConverter(Func<T, bool> canConvertRaw, Func<T, object> convertToObject)
        {
            CanConvertRawValue = canConvertRaw;
            ConvertToObject = convertToObject;
        }

        /// <summary>
        /// Checks if the element data is a match.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Func<T, bool> CanConvertRawValue { get; private set; }

        /// <summary>
        /// Converts the object data into a value.
        /// </summary>
        public Func<T, object> ConvertToObject { get; private set; }
    }
}
