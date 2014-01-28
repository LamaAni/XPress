using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Generator;

namespace XPress.Web.Razor.Code
{
    public class XPressCodeGenerator : System.Web.Razor.Generator.CSharpRazorCodeGenerator
    {
        public XPressCodeGenerator(string className, string rootNamesapce, string sourceFileName, System.Web.Razor.RazorEngineHost host)
            : base(className, rootNamesapce, sourceFileName, host)
        {
            //this.
        }
    }
}
