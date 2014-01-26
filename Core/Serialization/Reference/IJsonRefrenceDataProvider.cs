using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Reference
{
    /// <summary>
    /// Provides an interface for creating a json data provider that gets/sets the json values by id.
    /// </summary>
    public interface IJsonRefrenceDataProvider<T>
    {
        /// <summary>
        /// Makes a new id for a new object.
        /// </summary>
        uint MakeId();

        /// <summary>
        /// The source serializer.
        /// </summary>
        IJsonSerializer<T> Serializer { get; }

        /// <summary>
        /// The type binder associated with the serialization.
        /// </summary>
        SerializationTypeBinder<T> Binder { get; }

        /// <summary>
        /// Relations map between the objects.
        /// </summary>
        Dictionary<uint, HashSet<uint>> ChildReferences { get; }

        /// <summary>
        /// A collection of ids to that are anchored, and will not be removed by cleaning the garbage.
        /// </summary>
        HashSet<uint> Anchors { get; }

        /// <summary>
        /// Registers the object value to the specified id.
        /// </summary>
        /// <param name="id">The id to register the value to</param>
        /// <param name="val">The value to register</param>
        void RegisterObjectValue(uint id, IJsonValue<T> val);

        /// <summary>
        /// Returns the object data from the object id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IJsonValue<T> GetObjectValue(uint id);

        /// <summary>
        /// Returns true if the object has a value.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        bool HasObjectValue(uint id);

        /// <summary>
        /// Removes an object's value from the values list.
        /// </summary>
        /// <param name="o"></param>
        void DeleteObjectValue(uint id);

        /// <summary>
        /// Returns all the object ids that exist in the collection.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        uint[] GetAllValueIds();

        /// <summary>
        /// Updates the data source for the data provider.
        /// </summary>
        /// <param name="definitions"></param>
        void UpdateSource(bool isPretty = false);

        /// <summary>
        /// Returns the provider data as source.
        /// </summary>
        /// <returns>The data as source.</returns>
        T GetSource();
    }
}
