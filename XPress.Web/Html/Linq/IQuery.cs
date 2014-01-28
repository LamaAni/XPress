using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html.Linq
{
    /// <summary>
    /// The most basic query script avilable for generating controls. 
    /// </summary>
    public interface IQuery
    {
        IEnumerable<HtmlElement> GetLinqEnumrable();
    }
}
