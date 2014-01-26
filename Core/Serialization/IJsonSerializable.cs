using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization
{
    /// <summary>
    /// Implements an interface for serializaing json objects that allows for the objects to implement a specific serialization scheme. 
    /// In this form one can return any type of object acceptable by the json system (dictionary, lists, or sets are common - this can also be just a string).
    /// </summary>
    public interface IJsonSerializable
    {
        /// <summary>
        /// Converts the current object into a serialization object.
        /// </summary>
        /// <returns></returns>
        object ToSerializationObject();

        /// <summary>
        /// Loads the data from the serialization object.
        /// </summary>
        /// <param name="o">The serialization object.</param>
        void FromSerialziationObject(object o);
    }
}
