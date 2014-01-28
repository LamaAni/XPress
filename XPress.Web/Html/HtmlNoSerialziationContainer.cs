using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html
{
    /// <summary>
    /// Implements an html control that dose not serialize its child controls if there are any.
    /// Allows for the generation of large html texts that one dose not want to keep in server memory.
    /// </summary>
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public class HtmlNoSerialziationContainer : HtmlElement
    {
        public HtmlNoSerialziationContainer(string tagName = null)
            : base(tagName)
        {
        }

        Collections.ChildCollection m_kids = null;

        /// <summary>
        /// The collection of children. These objects will not be serialized. You can still keep 
        /// a child control in serialization if you refrence it directly within the parent.
        /// </summary>
        public override Collections.ChildCollection Children
        {
            get
            {
                if (m_kids == null)
                    m_kids = new Collections.ChildCollection(this, null);
                return m_kids;
            }
        }
    }
}
