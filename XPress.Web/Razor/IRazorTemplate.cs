using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XPress.Web.Razor
{
    /// <summary>
    /// The basic interface for a razor page that is applied on any specific page. The IRazor page allows for the generation
    /// </summary>
    public interface IRazorTemplate
    {
        /// <summary>
        /// Writes the object to the templates's output stream.
        /// </summary>
        /// <param name="o"></param>
        void Write(object o);

        /// <summary>
        /// Writes a string literal into the templates output stream.
        /// </summary>
        /// <param name="literal"></param>
        void WriteLiteral(string literal);

        /// <summary>
        /// Overriden by the razor system.
        /// </summary>
        /// <param name="context"></param>
        void Execute();

        /// <summary>
        /// Called to process the request.
        /// </summary>
        void ProcessRequest(HttpContext context);
    }
}
