using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html.Linq
{

    /// <summary>
    /// Helper class to allow the inner speration of code regions.
    /// </summary>
    public abstract class QueryExtender<T>
        where T : IQuery
    {
        public QueryExtender(T q)
        {
            Query = q;
        }

        public T Query { get; private set; }
    }
}
