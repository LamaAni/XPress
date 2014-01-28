using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPress.Web.Links.Attributes
{
    /// <summary>
    /// Creates a constructor link. Multiple constructors can exist.
    /// </summary>
    public class LinkConstructorAttribute : LinkScriptAttribute
    {
        public LinkConstructorAttribute(string url, LinkOrigin origin= LinkOrigin.File)
            : base(url, origin)
        {
            Type = LinkType.Constructor;
        }
    }
}
