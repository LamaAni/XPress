using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html
{
    /// <summary>
    /// An html control that inserts html into the control stream. Cannot have child controls.
    /// Diffres from HtmlLiteral - it will not be joined with other literal controls, and can be processed later.
    /// </summary>
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public class HtmlSnippet : HtmlElement
    {
        public HtmlSnippet(string html)
        {
            Html = html;
        }

        /// <summary>
        /// The html in the literal.
        /// </summary>
        [XPressMember("_html", IgnoreMode = XPressIgnoreMode.IfNull)]
        public string Html
        {
            get;
            set;
        }

        /// <summary>
        /// Html literal dose not have a childrens collection.
        /// </summary>
        public override Collections.ChildCollection Children
        {
            get
            {
                return null;
            }
        }

        public override void Render(Rendering.HtmlWriter writer)
        {
            writer.Write(Html);
        }
    }
}
