using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html.Linq
{
    /// <summary>
    /// Design time template extentions.
    /// </summary>
    public static class DesignTimeTemplateExtentions
    {
        
        #region container creation

        public static T Open<T>(this T c, string tagName)
             where T : HtmlTemplate
        {
            throw new NotImplementedException();
            return c;
        }

        #endregion
    }
}
