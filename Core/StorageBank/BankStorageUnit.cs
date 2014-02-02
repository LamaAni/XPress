using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.StorageBank
{
    /// <summary>
    /// Represents a bank storable object. Defines an rmc member for all important internal values.
    /// </summary>
    public class BankStorageUnit : Coding.Collections.ICacheCollectable
    {
        /// <summary>
        /// Creates a new bank storage unit. Stores a value along with the value's parameters.
        /// </summary>
        /// <param name="isCached">If true this object can be cached in memory</param>
        /// <param name="keepInMemoryInterval">Time interval to keep the object in memory without regard if the object has any refrences to it.</param>
        /// <param name="value">The value stored in the bank unit.</param>
        public BankStorageUnit(bool isCached = true, TimeSpan keepInMemoryInterval = default(TimeSpan))
        {
            IsCached = isCached;
            KeepInMemoryInterval = keepInMemoryInterval;
        }

        #region BankStorable Members

        [XPressMember]
        /// <summary>
        /// True if the object should be kept in the cache at all. (Refrence or timed).
        /// Default is true.
        /// </summary>
        public virtual bool IsCached { get; protected set; }

        /// <summary>
        /// The time to keep the object in memory, if zero, then the object will be kept in memory as long as there is a refrence of the object by any other object.
        /// Default is zero.
        /// </summary>
        [XPressMember]
        public virtual TimeSpan KeepInMemoryInterval { get; set; }

        /// <summary>
        /// Client last access information.
        /// </summary>
        [XPressMember]
        public virtual DateTime LastAccessed { get; set; }

        /// <summary>
        /// The time (utc) this service was last loaded from serialization.
        /// </summary>
        [XPressMember]
        public virtual DateTime LastDeserialized { get; internal set; }

        /// <summary>
        /// The client id, that is composed of the page id and the client id on the page.
        /// </summary>
        [XPressMember]
        public virtual string Id { get; internal set; }

        #endregion
    }

    public class BankStorageSingleValueUnit : BankStorageUnit
    {
        public BankStorageSingleValueUnit(object value, bool isCached = true, TimeSpan keepInMemoryInterval = default(TimeSpan))
            : base(isCached, keepInMemoryInterval)
        {
            Value = value;
        }


        /// <summary>
        /// The object value of the bank storeable. This is the storage 
        /// </summary>
        [XPressMember]
        public virtual object Value { get; private set; }
    }
}
