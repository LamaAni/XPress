using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Links.Attributes
{
    /// <summary>
    /// Sets the default root location for partial like file paths. If this attribute dose note exist then one needs to give the full path where links should be found.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class LinkFilesRootUrlAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string rootUrl;

        // This is a positional argument
        public LinkFilesRootUrlAttribute(string rootUrl)
        {
            this.rootUrl = rootUrl;
        }

        public string RootUrl
        {
            get { return rootUrl; }
        }
    }
}
