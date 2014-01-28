using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JCom.Attributes
{
    /// <summary>
    /// Servs as a base class for all members and properties to be communicated using JCom.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public abstract class ClientSideAttribute : Attribute
    {
        /// <summary>
        /// Identifies a client side member or property.
        /// </summary>
        public ClientSideAttribute()
        {
        }

        /// <summary>
        /// The name to show on the client side.
        /// If null the attached function name is used. default is null.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// If true the current member will not prefor asynced communication, but rather wait for the server to respond before continuing.
        /// </summary>
        public virtual bool? Synced { get; private set; }
    }
}
