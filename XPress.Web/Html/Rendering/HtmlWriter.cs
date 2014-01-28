using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html.Rendering
{
    /// <summary>
    /// Responds to a page request.
    /// </summary>
    public class HtmlWriter
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="source">The object id generation source. If null then a new dummy object id generator is created. (id=0 to all objects).</param>
        public HtmlWriter(JCom.Com.IJComObjectSource source)
        {
            InternalWriter = new System.IO.StringWriter();
            ObjectSource = source == null ? new ObjectIdDummyGenerator() : source;
        }

        #region members

        public JCom.Com.IJComObjectSource ObjectSource { get; private set; }

        public System.IO.TextWriter InternalWriter { get; private set; }

        List<Core.XPressResponseCommand> m_initCommands;

        /// <summary>
        /// A collection of commands to be executed on activation.
        /// </summary>
        public List<Core.XPressResponseCommand> InitCommands
        {
            get
            {
                if (m_initCommands == null)
                    m_initCommands = new List<Core.XPressResponseCommand>();
                return m_initCommands;
            }
        }

        /// <summary>
        /// True if has any init commands.
        /// </summary>
        public bool HasInitCommands
        {
            get { return m_initCommands == null ? false : m_initCommands.Count > 0; }
        }

        #endregion

        #region Response generation

        /// <summary>
        /// Retuns true if any attributes were written.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="col"></param>
        /// <param name="ignoreKeys"></param>
        /// <returns></returns>
        public virtual bool WriteAttributes(HtmlElement element, Collections.AttributeCollection col)
        {
            bool hasAttribs = false;
            // must call the enumerator of the collection since it adds the style value.
            col.ForEach(kvp =>
            {
                if (hasAttribs)
                    Write(" ");
                hasAttribs = true;

                Write(kvp.Key);
                Write("=\"");
                Write(kvp.Value);
                Write("\"");
            });

            return hasAttribs;
        }

        /// <summary>
        /// Renders a response directly to the stream.
        /// </summary>
        /// <param name="response"></param>
        public virtual void Write(string response)
        {
            InternalWriter.Write(response);
        }

        /// <summary>
        /// Reders the start tag and attributes
        /// </summary>
        /// <param name="element"></param>
        public virtual void WriteStartTag(HtmlElement element, bool withAttributes)
        {
            WriteTagName(element);
            Write(" ");
            if (withAttributes)
                WriteAttributes(element, element.Attributes);
            Write(">");
        }

        public virtual void WriteTagName(HtmlElement element)
        {
            Write("<");
            Write(element.TagName);
            Write(" ");
        }

        /// <summary>
        /// Reders the start tag and attributes
        /// </summary>
        /// <param name="element"></param>
        public virtual void WriteEndTag(HtmlElement element)
        {
            Write("</");
            Write(element.TagName);
            Write(">");
        }

        /// <summary>
        /// Reders a single tag and attributes
        /// </summary>
        /// <param name="element"></param>
        public virtual void WriteSingleTag(HtmlElement element, bool withAttributes)
        {
            WriteTagName(element);
            Write(" ");
            if (withAttributes)
                WriteAttributes(element, element.Attributes);
            Write("/>");
        }

        #endregion

        #region string representation

        /// <summary>
        /// Returns the html, as written.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return InternalWriter.ToString();
        }

        #endregion
    }

    class ObjectIdDummyGenerator : JCom.Com.IJComObjectSource
    {
        public ObjectIdDummyGenerator()
        {
        }

        #region IJComObjectSource Members

        public ulong GetObjectId(object o)
        {
            return 0;
        }

        public object GetObject(uint id)
        {
            return 0;
        }

        #endregion
    }
}
