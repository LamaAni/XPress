using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Documents
{
    /// <summary>
    /// Basic json value. Can be object array value (string, number) or null.
    /// </summary>
    public interface IJsonValue<T> : IJsonRawData
    {
    }

    /// <summary>
    /// Marker element for json raw data. Allows the passing of the raw data without the need to define the serialization type.
    /// </summary>
    public interface IJsonRawData
    {

    }
}
