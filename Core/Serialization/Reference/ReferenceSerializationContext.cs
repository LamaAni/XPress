using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Documents;
using XPress.Serialization.Map;

namespace XPress.Serialization.Reference
{
    /// <summary>
    /// A refrence enabled serialization context where refrences are loaded as serialization values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReferenceSerializationContext<T> : SerializationContext<T>
    {
        public ReferenceSerializationContext(SerializationDefinitions<T> def, JsonReferenceCollection<T> collection)
            : base(def)
        {
            Collection = collection;
        }

        #region members

        /// <summary>
        /// The json collection associated with this context.
        /// </summary>
        public JsonReferenceCollection<T> Collection { get; private set; }

        /// <summary>
        /// The current parent we are in.
        /// </summary>
        public uint? CurParent { get; private set; }

        #endregion

        #region  overriden methods

        /// <summary>
        /// Returns the json value associated with the storage.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <param name="ignoreBinder"></param>
        /// <returns></returns>
        public override Documents.IJsonValue<T> GetJsonValue(object o, Type t, Action<IJsonValue<T>> prepareConverterObjectValue = null)
        {
            if (o != null)
            {
                SerializationTypeMap<T> tm = GetMapInfo(t);
                if (tm.IsRefrenceType)
                {
                    // the object is a refrence type that should be serialized by the reference collection.
                    uint id = this.Collection.GetObjectId(o);
                    return this.GetJsonValue(o, t, id);
                }
            }
            return base.GetJsonValue(o, t, prepareConverterObjectValue);
        }

        /// <summary>
        /// Returns the object associated with the json value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="t"></param>
        /// <param name="ignoreBinder"></param>
        /// <returns></returns>
        public override object GetObject(Documents.IJsonValue<T> val, Type t, Action<object> prepareCreateObject = null)
        {
            object o = base.GetObject(val, t, prepareCreateObject);
            RefrenceId rid = o as RefrenceId;
            if (rid != null)
            {
                return GetObject(t, rid.Value);
            }
            return o;
        }

        #endregion

        #region By id methods

        /// <summary>
        /// Returns the object associated with the json value, and assignes the id to that value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="t"></param>
        /// <param name="ignoreBinder"></param>
        /// <returns></returns>
        public object GetObject(Type t, uint id)
        {
            // checking if the object is already in the collection, no need to repeat this object.
            if(Collection.HasObject(id))
                return Collection.GetObject(id);

            if (!Collection.HasObjectValue(id))
                return null; // nothing to return, this value dose not exist.

            // deserializing the object and setting the object as the value.
            return base.GetObject(Collection.GetObjectValue(id), t, (obj) => Collection.RegisterObject(obj, id));
        }

        /// <summary>
        /// Returns the json value associated with the storage, and assigns the object with that id.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <param name="ignoreBinder"></param>
        /// <returns></returns>
        public Documents.IJsonValue<T> GetJsonValue(object o, Type t, uint id)
        {
            // rendering the object value, if needed.
            if (!Collection.HasObjectValue(id))
            {
                if (CurParent != null)
                    Collection.MarkChildRefrence(CurParent.Value, id);
                uint? lastParent = CurParent;
                CurParent = id;

                base.GetJsonValue(o, t, (val) => Collection.RegisterObjectValue(id, val));
                
                CurParent = lastParent;
            }

            // returning the object value as a refrence value.
            return base.GetJsonValue(new RefrenceId(id), typeof(RefrenceId));
        }

        #endregion
    }
}
