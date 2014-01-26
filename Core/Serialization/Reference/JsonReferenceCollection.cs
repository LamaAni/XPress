using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Reference
{
    /// <summary>
    /// Implements a refrence json collection that maps the json object to json id.
    /// </summary>
    public class JsonReferenceCollection<T>
    {
        public JsonReferenceCollection(IJsonRefrenceDataProvider<T> dataProvider)
        {
            if (dataProvider == null)
                throw new Exception("Data provider cannot be null.");
            DataProvider = dataProvider;
        }

        #region members

        /// <summary>
        /// The data provider associated with the collection.
        /// </summary>
        public IJsonRefrenceDataProvider<T> DataProvider { get; private set; }

        /// <summary>
        /// the object mapping, maps the object to the specified id
        /// </summary>
        Dictionary<object, uint> m_byObject = new Dictionary<object, uint>(new ByRefrenceEqualityComparer());

        /// <summary>
        /// The object id collection.
        /// </summary>
        public IReadOnlyDictionary<object, uint> ByObject
        {
            get { return m_byObject; }
        }

        Dictionary<uint, object> m_byId = new Dictionary<uint, object>();

        /// <summary>
        /// the object mapping, maps the id to the object.
        /// </summary>
        public IReadOnlyDictionary<uint, object> ById
        {
            get { return m_byId; }
        }

        #endregion

        #region helper classes.

        public class ByRefrenceEqualityComparer : IEqualityComparer<object>
        {
            public ByRefrenceEqualityComparer()
            {
            }

            #region IEqualityComparer<object> Members

            public new bool Equals(object x, object y)
            {
                return MarshalByRefObject.Equals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }

            #endregion
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns true if the colelctions contains the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Contains(uint id)
        {
            return HasObject(id) || HasObjectValue(id);
        }

        /// <summary>
        /// Returns true if the object is in the current collector.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual bool HasObject(object o)
        {
            return m_byObject.ContainsKey(o);
        }

        /// <summary>
        /// returns true if the Id appears in the collection.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool HasObject(uint id)
        {
            return m_byId.ContainsKey(id);
        }

        /// <summary>
        /// Returns true if the object has a value.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual bool HasObjectValue(object o)
        {
            if (!HasObject(o))
                return false;
            return HasObjectValue(GetObjectId(o));
        }

        /// <summary>
        /// Retuns the object id if there is one, or creates a new object id.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual uint GetObjectId(object o)
        {
            if (HasObject(o))
                return ByObject[o];
            uint id = DataProvider.MakeId();
            _RegisterObject(o, id);
            return id;
        }

        /// <summary>
        /// Registers the current object to the specified id.
        /// If the current object already exists, the correct id is returned.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint RegisterObject(object o, uint id)
        {
            if (!ByObject.ContainsKey(o))
            {
                _RegisterObject(o, id);
                return id;
            }
            else return ByObject[o];
        }

        private void _RegisterObject(object o, uint id)
        {
            m_byObject[o] = id;
            m_byId[id] = o;
        }

        /// <summary>
        /// Registers the object value, and adds an object id if needed. 
        /// </summary>
        /// <param name="o">The object to register the object value to</param>
        /// <param name="val">The value to register</param>
        /// <returns>The object id</returns>
        public virtual uint RegisterObjectValue(object o, IJsonValue<T> val)
        {
            uint id = GetObjectId(o);
            RegisterObjectValue(id, val);
            return id;
        }


        /// <summary>
        /// Returns the object asociated with the id.
        /// </summary>
        /// <param name="id"></param>
        public virtual object GetObject(uint id)
        {
            if (!ById.ContainsKey(id))
                throw new Exception("Object not found");
            return ById[id];
        }

        /// <summary>
        /// Removes an object's value from the values list.
        /// </summary>
        /// <param name="o"></param>
        public virtual void DeleteObjectValue(object o)
        {
            uint id = GetObjectId(o);
            DeleteObjectValue(id);
        }

        /// <summary>
        /// Clears the associated child refrences with the current.
        /// </summary>
        /// <param name="o"></param>
        public virtual void ClearChildRefrences(object o)
        {
            if (!ByObject.ContainsKey(o))
                return;
            ClearChildRefrences(GetObjectId(o));
        }

        /// <summary>
        /// Clears the associated child refrences.
        /// </summary>
        /// <param name="id">The object id</param>
        public virtual void ClearChildRefrences(uint id)
        {
            DataProvider.ChildReferences.TryRemove(id);
        }

        /// <summary>
        /// Marks a refrence bettwen a child and a parent.
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <param name="child">The child</param>
        public virtual void MarkChildRefrence(uint parent, uint child)
        {
            if (!DataProvider.ChildReferences.ContainsKey(parent))
                DataProvider.ChildReferences[parent] = new HashSet<uint>();
            DataProvider.ChildReferences[parent].TryAdd(child);
        }

        /// <summary>
        /// Marks multiple refrences.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="children"></param>
        public virtual void MarkChildRefrences(uint parent, IEnumerable<uint> children)
        {
            if (!DataProvider.ChildReferences.ContainsKey(parent))
                DataProvider.ChildReferences[parent] = new HashSet<uint>(children);
            else DataProvider.ChildReferences[parent].UnionWith(children);
        }

        /// <summary>
        /// If true the object id is anchored to the dictionary and will not be cleared in the case 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsAnchored(uint id)
        {
            return DataProvider.Anchors.Contains(id);
        }

        /// <summary>
        /// Anchors the id to the collection.
        /// </summary>
        /// <param name="id"></param>
        public void Anchor(uint id)
        {
            if (IsAnchored(id))
                return;
            
            DataProvider.Anchors.Add(id);
        }

        /// <summary>
        /// Releases the id from the anchored collection.
        /// </summary>
        /// <param name="id"></param>
        public void ReleaseAnchor(uint id)
        {
            if (!IsAnchored(id))
                return;
            DataProvider.Anchors.Remove(id);
        }

        #endregion

        #region values handling

        /// <summary>
        /// Registers the object value to the specified id.
        /// </summary>
        /// <param name="id">The id to register the value to</param>
        /// <param name="val">The value to register</param>
        public void RegisterObjectValue(uint id, IJsonValue<T> val)
        {
            DataProvider.RegisterObjectValue(id, val);
        }

        /// <summary>
        /// Returns the object data from the object id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IJsonValue<T> GetObjectValue(uint id)
        {
            return DataProvider.GetObjectValue(id);
        }

        /// <summary>
        /// Returns true if the object has a value.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool HasObjectValue(uint id)
        {
            return DataProvider.HasObjectValue(id);
        }

        /// <summary>
        /// Removes an object's value from the values list.
        /// </summary>
        /// <param name="o"></param>
        public void DeleteObjectValue(uint id)
        {
            DataProvider.DeleteObjectValue(id);
        }

        /// <summary>
        /// Returns all the object ids that exist in the collection.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public uint[] GetAllValueIds()
        {
            return DataProvider.GetAllValueIds();
        }

        #endregion
    }
}
