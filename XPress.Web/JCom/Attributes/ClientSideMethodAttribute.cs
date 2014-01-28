using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPress.Web.JCom.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ClientSideMethodAttribute : ClientSideAttribute
    {
        /// <summary>
        /// Identifies a method that is to be used for communication using JCom.
        /// </summary>
        public ClientSideMethodAttribute()
        {
        }
    }
}
