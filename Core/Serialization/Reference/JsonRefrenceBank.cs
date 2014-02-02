using XPress.Serialization.Core;
using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Reference
{
    /// <summary>
    /// Implements a refrence json serializer. The refrence serializer controls the refrence value
    /// </summary>
    public class JsonRefrenceBank<T>
    {
        public JsonRefrenceBank(IJsonRefrenceDataProvider<T> dataProvider)
        {
            DataProvider = dataProvider;
            Collection = new JsonReferenceCollection<T>(dataProvider);
            Context = new ReferenceSerializationContext<T>(Definitions, Collection);
            Context.Binder = dataProvider.Binder;
        }

        #region members

        /// <summary>
        /// The json serialzier associated with the bank.
        /// </summary>
        public IJsonSerializer<T> Serialzier { get { return DataProvider.Serializer; } }

        /// <summary>
        /// The data provider associated with the bank.
        /// </summary>
        public IJsonRefrenceDataProvider<T> DataProvider { get; private set; }

        /// <summary>
        /// A collection of values that apply to the json serializer, and determine the definition properties.
        /// </summary>
        public SerializationDefinitions<T> Definitions { get { return DataProvider.Serializer.Definitions; } }

        /// <summary>
        /// The context object for serialization is constant since it will always add to the same collection.
        /// </summary>
        protected ReferenceSerializationContext<T> Context { get; private set; }

        /// <summary>
        /// The collection associated with the serializer. Holds all the information about the current serialziation state.
        /// </summary>
        public JsonReferenceCollection<T> Collection { get; private set; }

        #endregion

        #region storage and loading

        /// <summary>
        /// Storage of the object into the object stream if needed.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>The object id</returns>
        public uint Store(object o, bool anchor = false)
        {
            // stores the object to the collection.
            uint id = Collection.GetObjectId(o);
            if (anchor)
                Anchor(id);
            return id;
        }

        /// <summary>
        /// Loads an object from the collection. (Deserializes the object if nesscery.
        /// </summary>
        /// <param name="id">The object id</param>
        /// <returns></returns>
        public object Load(uint id)
        {
            if (Collection.HasObject(id))
                return Collection.GetObject(id);

            // getting the value.
            return DeserializeObjectValue(id);
        }

        /// <summary>
        /// Deserializes the object value that is associated with an id
        /// </summary>
        /// <param name="id">The id of the object.</param>
        /// <returns>The deserialzied object.</returns>
        public object DeserializeObjectValue(uint id)
        {
            if (!Collection.HasObjectValue(id))
                return null;
            object o = Context.GetObject(typeof(object), id);
            return o;
        }

        /// <summary>
        /// Serializes the object value to the collection.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public IJsonValue<T> SerializeObjectValue(object o, uint? id = null)
        {
            if (id == null)
                id = Collection.GetObjectId(o);
            return Context.GetJsonValue(o, o.GetType(), id.Value);
        }

        /// <summary>
        /// Anchors the object to the collection.
        /// </summary>
        /// <param name="id">The id of the object to anchor.</param>
        public void Anchor(uint id)
        {
            Collection.Anchor(id);
        }

        /// <summary>
        /// Releases an anchor from the collection.
        /// </summary>
        /// <param name="id">The id to relaease</param>
        public void ReleaseAnchor(uint id)
        {
            Collection.ReleaseAnchor(id);
        }

        /// <summary>
        /// Returns all cached objects that are currently in memory.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetCachedObjects()
        {
            return Collection.ByObject.Keys;
        }

        #endregion

        #region state updating and cleanup

        /// <summary>
        /// Stores the current state for objects that have been loaded. (Serializes them) - heavy procedure.
        /// <param name="collectGarbage">If true, deletes any objects in memory that are not connected to any other object or not anchored.</param>
        /// </summary>
        public void Update(bool collectGarbage = true)
        {
            UpdateState();
            if (collectGarbage)
                CollectGarbage();
        }

        object _updateStateLockObject = new object();

        void UpdateState()
        {
            // must lock to deny changes while storing.
            lock (_updateStateLockObject)
            {
                // getting all the existing objects by thier keys.
                KeyValuePair<uint, object>[] curState = Collection.ById.ToArray();
                // going through all the objects that were loaded and clearing their values if exist.
                curState.ForEach(kvp =>
                {
                    Collection.DeleteObjectValue(kvp.Key);
                });

                // now rewriting all objects that have been loaded.
                curState.ForEach(kvp =>
                {
                    if (Collection.HasObjectValue(kvp.Key))
                        return;
                    SerializeObjectValue(kvp.Value, kvp.Key);
                });
            }
        }

        object _collectGarbageLockObject = new object();

        /// <summary>
        /// Clears any data garbage collected in the stream. Called automatically when needed. Call directly to collect the garbage.
        /// </summary>
        public void CollectGarbage()
        {
            lock (_collectGarbageLockObject)
            {
                // tri collor markup garbage collection (on values only - since we only have record of refrences to value objects).
                // gray - all thouse not checked (and have refrences).
                // black - checked and kept (has refrences).

                uint[] ids = Collection.GetAllValueIds();

                // child refrences.
                HashSet<uint>
                    // gray collection contains any objects that need checking, but are not to be cleared from memory.
                    gray = new HashSet<uint>(Collection.DataProvider.Anchors.Intersect(ids)),
                    // checked objects that do not need clearing from memory.
                    black = new HashSet<uint>();

                // blacken all gray elements.
                List<uint> newGrayed = new List<uint>();
                while (gray.Count > 0)
                {
                    newGrayed.Clear(); 
                    foreach (uint id in gray)
                    {
                        // no need to add this key to the black.
                        black.Add(id);

                        if (!Collection.DataProvider.ChildReferences.ContainsKey(id))
                            continue;

                        // adding child refrences to gray.
                        newGrayed.AddRange(Collection.DataProvider.ChildReferences[id]);
                    }
                    gray.ExceptWith(black); // removing all blacked ids.
                    gray.UnionWith(newGrayed.Distinct().Except(black)); // adding all the ids that are new.
                }

                // delete all the white elements.
                uint[] white = ids.Except(Collection.DataProvider.Anchors).Except(black).ToArray();
                foreach (uint id in white)
                {
                    Collection.DeleteObjectValue(id);
                }
            }
        }


        /// <summary>
        /// Writes the changes made to the source (via the data provider).
        /// </summary>
        public void WriteToSource(bool update = true, bool collectGarabge = true, bool isPretty = false)
        {
            if (update)
                Update(collectGarabge);
            DataProvider.UpdateSource(isPretty);
        }

        #endregion
    }
}
