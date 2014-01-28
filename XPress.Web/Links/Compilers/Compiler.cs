using XPress.Web.Links.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Links.Compilers
{
    /// <summary>
    /// Creates a abstract compiler class to generate the code to run on the client side.
    /// </summary>
    public abstract class Compiler
    {
        public Compiler()
        {

        }

        /// <summary>
        /// Creates the link construction code for the specified type (if any.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract string CreateInitCode(LinkMapInfo lmi);
    }
}
