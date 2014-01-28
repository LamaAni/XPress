using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPress.Web.Links.Attributes
{
    /// <summary>
    /// Links a css file to the page rendering. The Theme type determines which theme the current css file applies to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
    public class LinkCssAttribute : LinkAttribute
    {
        public LinkCssAttribute(string url)
            :this(url, typeof(Themes.Theme))
        {
        }

        public LinkCssAttribute(string url, Type themeType, LinkOrigin origin = LinkOrigin.File)
            : base(url, origin)
        {
            Type = LinkType.Css;
            ThemeType = themeType;
        }

        public Type ThemeType { get; private set; }
    }

}
