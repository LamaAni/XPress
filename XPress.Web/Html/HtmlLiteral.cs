using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html
{
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public class HtmlLiteral: HtmlElement
    {
        public HtmlLiteral(string text)
        {
            LiteralBuilder.Append(text);
        }

        [System.Runtime.Serialization.OnSerializing]
        public void OnSerializaing(System.Runtime.Serialization.StreamingContext context)
        {
            ValidateLiteralRendered();
        }

        private void ValidateLiteralRendered()
        {
            if (m_builder != null)
            { 
                m_literal = m_builder.ToString();
                m_builder = null;
            }
        }

        StringBuilder m_builder;

        /// <summary>
        /// The string builder that creates the literal.
        /// </summary>
        public StringBuilder LiteralBuilder
        {
            get
            {
                if (m_builder == null)
                    m_builder = Html == null ? new StringBuilder() : new StringBuilder(Html);
                return m_builder;
            }
        }

        /// <summary>
        /// The html in the literal.
        /// </summary>
        public string Html
        {
            get { ValidateLiteralRendered(); return m_literal; }
            set { m_builder = null; m_literal = value; }
        }

        [XPressMember("_literal", IgnoreMode = XPressIgnoreMode.IfNull)]
        string m_literal = null;

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
            ValidateLiteralRendered();
            writer.Write(m_literal);
        }
    }
}
