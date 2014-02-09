using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web;

namespace XPress.Web.Controls.Generic
{
    /// <summary>
    /// Implements a remote controlled div control that allows one to controls its internal html.
    /// </summary>
    public class Div : Controls.XPressControl
    {
        public Div(string tagName="div")
            :base(tagName)
        {

        }

        /// <summary>
        /// Gets/sets the internal html. On get, returns only there is only one internal control, and its an html literal.
        /// </summary>
        public string Html
        {
            get
            {
                if (!this.HasChildren || this.Children.Count != 1 || !(this.Children[0] is Html.HtmlLiteral))
                    throw new Exception("Cannot read internal html since internal html is not a single literal.");
                return (this.Children[0] as Html.HtmlLiteral).Html;
            }
            set
            {
                this.Children.Clear();
                this.Children.Append(new Html.HtmlLiteral(value));
            }
        }
    }
}
