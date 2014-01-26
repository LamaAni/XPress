using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Coding.Collections
{
    /// <summary>
    /// Implements a set of functions to allow an object to be cache collectable.
    /// </summary>
    public interface ICacheCollectable
    {

        /// <summary>
        /// The time to keep the object in memory, if zero, then the object will be kept in memory as long as there is a refrence of the object by any other object.
        /// Default is zero.
        /// </summary>
        TimeSpan KeepInMemoryInterval { get; set; }

        /// <summary>
        /// Client last access information.
        /// </summary>
        DateTime LastAccessed { get; set; }
    }
}
