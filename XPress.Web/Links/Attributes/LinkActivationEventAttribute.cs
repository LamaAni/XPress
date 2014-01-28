using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPress.Web.Links.Attributes
{
    /// <summary>
    /// Defines when to call the depndent scripts of the specified onbjct. (InitFunctionFile, Constructor).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class LinkActivationEventAttribute : Attribute
    {
        /// <summary>
        /// The activation event associated with the current object. 
        /// </summary>
        /// <param name="ev"></param>
        public LinkActivationEventAttribute(ActivationEvent ev)
        {
            Event = ev;
        }

        /// <summary>
        /// The event for which to activate the current type.
        /// </summary>
        public ActivationEvent Event { get; private set; }
    }
}
