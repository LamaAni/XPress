using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Map
{
    /// <summary>
    /// Allows for specific type identity creation.
    /// </summary>
    public interface ITypeConverter
    {
        /// <summary>
        /// True if the current converter can convert this type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool CanConvert(Type t);

        /// <summary>
        /// True if the current converter can convert this identity.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        bool CanConvert(string identity);

        /// <summary>
        /// Converts the type into a specified type map.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        Type ToType(string identity);
        /// <summary>
        /// Converts the type to a specific identity.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        string ToIdentitiy(Type t);
    }
}
