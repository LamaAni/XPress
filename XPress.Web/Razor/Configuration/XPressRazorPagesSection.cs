using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.WebPages.Razor.Configuration;

namespace XPress.Web.Razor.Configuration
{
    public class XPressRazorPagesSection : RazorPagesSection
    {
        public XPressRazorPagesSection()
            : base()
        {
        }

        protected override void Init()
        {
            base.Init();
        }

        protected override void InitializeDefault()
        {
            base.InitializeDefault();
            this.Namespaces.EmitClear = true;
            this.Namespaces.Add(new System.Web.Configuration.NamespaceInfo("System"));
            this.Namespaces.Add(new System.Web.Configuration.NamespaceInfo("System.Linq"));
            this.Namespaces.Add(new System.Web.Configuration.NamespaceInfo("System.Text"));
            this.Namespaces.Add(new System.Web.Configuration.NamespaceInfo("System.Collections.Generic"));
            this.Namespaces.Add(new System.Web.Configuration.NamespaceInfo("XPress.Web.Html"));
            this.Namespaces.Add(new System.Web.Configuration.NamespaceInfo("XPress.Web.Html.Linq"));
            this.Namespaces.Add(new System.Web.Configuration.NamespaceInfo("XPress.Web.Links.Attributes"));

            this.PageBaseType = "XPress.Web.Controls.Template";
        }
    }
}
