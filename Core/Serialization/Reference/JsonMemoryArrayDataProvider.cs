using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Reference
{
    /// <summary>
    /// Implements a json memory array refrence collection that serializes the object values to memory and allows these values to be stored to 
    /// the form of an array of values of T. Where the first value is the collection of definitions.
    /// </summary>
    public class JsonMemoryArrayDataProvider<T> : IJsonRefrenceDataProvider<T>
    {
        /// <summary>
        /// Initialization of the memory refrence collection.
        /// </summary>
        public JsonMemoryArrayDataProvider(IJsonSerializer<T> serializer)
        {
            Serializer = serializer;
            Binder = new SerializationTypeBinder<T>();
            SourceValues = new Dictionary<uint, T>();
            JsonValues = new Dictionary<uint, IJsonValue<T>>();
            ChildReferences = new Dictionary<uint, HashSet<uint>>();
            Anchors = new HashSet<uint>();
        }

        #region collection

        /// <summary>
        /// The raw serializer.
        /// </summary>
        public IJsonSerializer<T> Serializer { get; private set; }

        /// <summary>
        /// Type binder.
        /// </summary>
        public SerializationTypeBinder<T> Binder { get; protected set; }

        /// <summary>
        /// The collection of source values.
        /// </summary>
        protected Dictionary<uint, T> SourceValues { get; set; }

        /// <summary>
        /// The collection of json values.
        /// </summary>
        protected Dictionary<uint, IJsonValue<T>> JsonValues { get; set; }

        /// <summary>
        /// Relations map between the objects.
        /// </summary>
        public Dictionary<uint, HashSet<uint>> ChildReferences { get; protected set; }

        /// <summary>
        /// A collection of ids to that are anchored, and will not be removed by cleaning the garbage.
        /// </summary>
        public HashSet<uint> Anchors { get; protected set; }

        /// <summary>
        /// The current id.
        /// </summary>
        public uint CurId { get; protected set; }

        #endregion

        #region collection methods

        public void RegisterObjectValue(uint id, Documents.IJsonValue<T> val)
        {
            SourceValues.TryRemove(id);
            JsonValues[id] = val;
        }

        public Documents.IJsonValue<T> GetObjectValue(uint id)
        {
            if (JsonValues.ContainsKey(id))
                return JsonValues[id];
            if (SourceValues.ContainsKey(id))
            {
                IJsonValue<T> val = Serializer.FromJsonRepresentation(SourceValues[id]);
                JsonValues[id] = val;
                return val;
            }

            throw new Exception("Json value not found for id " + id);
        }

        public bool HasObjectValue(uint id)
        {
            return SourceValues.ContainsKey(id) || JsonValues.ContainsKey(id);
        }

        public void DeleteObjectValue(uint id)
        {
            SourceValues.TryRemove(id);
            JsonValues.TryRemove(id);
        }

        public uint[] GetAllValueIds()
        {
            return SourceValues.Keys.Union(JsonValues.Keys).ToArray();
        }

        /// <summary>
        /// Updates the data provider source and writes all changes made to the source.
        /// (Heavy action).
        /// </summary>
        public virtual void UpdateSource(bool isPretty = false)
        {
            throw new Exception("The json memory array provider source cannot be updated. It is for memory use only, and works only with IJsonValue<T> values");
        }

        /// <summary>
        /// Returns a new id for a new object.
        /// </summary>
        /// <returns></returns>
        public uint MakeId()
        {
            uint id = CurId;
            CurId += 1;
            return id;
        }

        /// <summary>
        /// Returns the source.
        /// </summary>
        /// <returns></returns>
        public virtual T GetSource()
        {
            throw new Exception("A memory array dose not have a source.");
        }

        #endregion
    }

}
