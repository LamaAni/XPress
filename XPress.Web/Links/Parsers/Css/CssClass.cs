using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Links.Parsers.Css
{
    /// <summary>
    /// Reporesents a css class.
    /// </summary>
    public class CssClass
    {
        public CssClass()
        {
            Attributes = new List<CssClassAttribute>();
        }

        public string Name { get; set; }
        public List<CssClassAttribute> Attributes { get; private set; }
    }

    public class CssClassAttribute
    {
        public CssClassAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
