using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html.Rendering
{
    /// <summary>
    /// Holds html elment stream to be rendered to the client side. The stream represents the html tree.
    /// </summary>
    public class TemplateWriteContext
    {
        public TemplateWriteContext()
        {
            OpenContext = new Stack<HtmlElement>();
        }

        Stack<HtmlElement> m_openContext = new Stack<HtmlElement>();

        /// <summary>
        /// The current open context, to which element the element currently being inserted should be added.
        /// </summary>
        public Stack<HtmlElement> OpenContext
        {
            get;
            private set;
        }

        /// <summary>
        /// The last inserted literal.
        /// </summary>
        public HtmlLiteral LastLiteral { get; set; }

    }
}
