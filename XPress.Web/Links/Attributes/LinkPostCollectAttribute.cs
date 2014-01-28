using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Links.Attributes
{
    /// <summary>
    /// Set this function to a specific link to call the post collection of scripts.
    /// The method must be of the delegate type PostCollectLinks.
    /// Allows the user to add specific links to the collection, and to manipulate the collection according to code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class LinkPostCollectAttribute : Attribute
    {
        public LinkPostCollectAttribute()
        {
        }
    }

    public delegate void PostCollectLinks();
}
