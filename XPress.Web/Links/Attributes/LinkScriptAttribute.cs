using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPress.Web.Links.Attributes
{
    /// <summary>
    /// Links a script to the Html Element type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
    public class LinkScriptAttribute : LinkAttribute
    {
        // This is a positional argument
        public LinkScriptAttribute(string url, LinkOrigin origin = LinkOrigin.File)
            : base(url, origin)
        {
            Type = LinkType.Script;
        }
    }
}
