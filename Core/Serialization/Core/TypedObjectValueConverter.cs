using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Core
{
    /// <summary>
    /// A typed object converter, to convert C# objects into json.
    /// </summary>
    /// <typeparam name="T">The conversion target.</typeparam>
    public class TypedObjectValueConverter<T>
    {
        public TypedObjectValueConverter(Func<object, T> convert, Type conversionType)
            : this(convert, new Type[1] { conversionType })
        {
        }

        public TypedObjectValueConverter(Func<object, T> convert, Type[] conversionTypes)
        {
            ConvertToRaw = convert;
            ConversionTypes = conversionTypes;
        }

        /// <summary>
        /// A collection of types that can be converted using this converter
        /// </summary>
        public Type[] ConversionTypes { get; private set; }

        /// <summary>
        /// The function to do the conversion.
        /// </summary>
        public Func<object,T> ConvertToRaw { get; private set; }
    }
}
