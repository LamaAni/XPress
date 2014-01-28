using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.JCom.Map;
using System.Collections.Concurrent;
using XPress.Web.JCom.Com.Response;
using System.Reflection;

namespace XPress.Web.JCom.Compilers
{
    /// <summary>
    /// Represents a jcom compiler that will be created for a specialized language.
    /// </summary>
    public abstract class Compiler
    {
        public Compiler()
        {
        }

        #region abstract

        /// <summary>
        /// Returns the code that is generated for a specified type T. (This would be the JCom type defenition on the client side).
        /// </summary>
        /// <param name="info">The mapping info.</param>
        /// <returns></returns>
        public abstract string CreateTypeDef(Map.JComTypeInfo info);

        /// <summary>
        /// Returns an object that represents the data associated with the object. (if any).
        /// </summary>
        /// <param name="info">Mapping info</param>
        /// <param name="o">The object to get the data for</param>
        /// <returns>A object that represent the data members for the type.</returns>
        public virtual object GetObjectData(Map.JComTypeInfo info, object o)
        {
            // checking for any members.
            if (!info.RequiresDataObject)
                return null;

            Dictionary<string, object> dic = info.DataMembers.Where(mi => mi.CanRead).ToDictionary(mi => mi.Name, mi =>
            {
                if (mi.IsProperty)
                {
                    return (mi.MappedMember as PropertyInfo).GetValue(o);
                }
                return (mi.MappedMember as FieldInfo).GetValue(o);
            });

            return dic;
        }

        #endregion
    }
}
