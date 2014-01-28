using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Links.Attributes
{
    /// <summary>
    /// Tells the link compiler to ignore all links that are generated from type T.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class LinkIgnoreSourcesFromTypeAttribute : Attribute
    {
        public LinkIgnoreSourcesFromTypeAttribute(params Type[] ts)
        {
            Types = ts;
        }

        /// <summary>
        /// The type to ignore.
        /// </summary>
        public Type[] Types { get; private set; }

        /// <summary>
        /// If true then base types are ignored also.
        /// </summary>
        public bool InludeBaseTypes { get; set; }

        /// <summary>
        /// If true all the inerfaces of the specified types will also be ignored.
        /// </summary>
        public bool IncludeInterfaces { get; set; }
    }
}
