using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Links.Parsers.Css
{
    /// <summary>
    /// Represents a css stylesheet.
    /// </summary>
    public class CssStyleSheet : IEnumerable<CssClass>
    {
        public CssStyleSheet(IEnumerable<CssClass> classes = null)
        {
            Classes = classes == null ? new List<CssClass>() : classes.ToList();
        }

        public List<CssClass> Classes { get; private set; }

        #region IEnumerable<CssClass> Members

        public IEnumerator<CssClass> GetEnumerator()
        {
            return Classes.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region string actions

        public override string ToString()
        {
            StringBuilder builder=new StringBuilder();
            Classes.ForEach(c=>{
                builder.Append(c.Name);
                builder.Append("{");
                c.Attributes.OrderBy(a=>a.Key).ForEach(a=>{
                    builder.Append(a.Key);
                    builder.Append(":");
                    builder.Append(a.Value);
                    builder.Append(";");
                });
                builder.Append("}");
            });
            return builder.ToString();
        }

        #endregion

        #region css minification

        #endregion

        #region prefix compleation

        #endregion
    }
}
