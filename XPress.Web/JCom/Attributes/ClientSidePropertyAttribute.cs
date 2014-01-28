using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMC.Web.Attributes;

namespace XPress.Web.JCom.Attributes
{
    /// <summary>
    /// Implements an field/property that will be written to the attributes collection before
    /// the call to prerender happens. Executes only on remote controls!
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ClientSidePropertyAttribute : ClientSideAttribute
    {
        /// <summary>
        /// Markes the property or field, to be rendered as a client side attribue.
        /// </summary> 
        public ClientSidePropertyAttribute()
        {
        }

        /// <summary>
        /// Generates the JSON value of the current object. By default the object's JSON representation is generated (null value).
        /// </summary>
        public ValueGenerator Generator
        {
            get;
            set;
        }

        /// <summary>
        /// If true any time a property is updtaed on the client then the server is updated to that property.
        /// </summary>
        public bool AutoUpdate { get; set; }

        /// <summary>
        /// If true the current property is rendered to an attribute istead to the data collection.
        /// This would mean that the current propery overwites!! any attribute with that name.
        /// The property value is rendered as json!
        /// </summary>
        public bool AsAttribute { get; set; }
    }
}
