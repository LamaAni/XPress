using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html.Linq
{
    /// <summary>
    /// The basic query object to be used when rendering controls to the client.
    /// </summary>
    public class HtmlElementQuery : IQuery
    {
        public HtmlElementQuery(HtmlElement el)
            : this(new HtmlElement[1] { el })
        {

        }

        public HtmlElementQuery(IEnumerable<HtmlElement> els)
        {
            m_elements = els;
        }

        #region members

        IEnumerable<HtmlElement> m_elements;

        #endregion

        #region IQuery Members

        /// <summary>
        /// Returns a collection of html elements that are associated with this query.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HtmlElement> GetLinqEnumrable()
        {
            return m_elements;
        }

        #endregion
    }

}
