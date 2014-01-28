using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPress.Web.Links.Attributes
{
    /// <summary>
    /// Creats a link to a script.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
    public class LinkInitScriptAttribute : LinkAttribute
    {
        // This is a positional argument
        public LinkInitScriptAttribute(string url, LinkOrigin origin = LinkOrigin.File)
            : base(url, origin)
        {
            Type = LinkType.InitScriptFunction;
        }
    }
}
