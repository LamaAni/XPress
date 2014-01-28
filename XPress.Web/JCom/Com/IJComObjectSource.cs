using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JCom.Com
{
    /// <summary>
    /// Implements a jcom object source that resolves the ID->OBJECT and OBJECT->ID parameters.
    /// </summary>
    public interface IJComObjectSource
    {
        /// <summary>
        /// Returns the object id.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        ulong GetObjectId(object o);

        /// <summary>
        /// Returns the object given the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        object GetObject(uint id);
    }
}
