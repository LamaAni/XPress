using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Razor;
using XPress.Web.Razor.Code;

namespace XPress.Web.Razor
{
    public class XPressWebPageRazorHost : WebPageRazorHost
    {
        public XPressWebPageRazorHost(string virtualPath, string physicalPath)
            : base(virtualPath, physicalPath)
        {
            this.DefaultPageBaseClass = "XPress.Web.Controls.Template";
            this.DefaultBaseClass = "XPress.Web.Controls.Template";
            this.NamespaceImports.Clear();
        }

        public override System.Web.Razor.Parser.ParserBase DecorateCodeParser(System.Web.Razor.Parser.ParserBase incomingCodeParser)
        {
            if (incomingCodeParser is System.Web.Razor.Parser.CSharpCodeParser)
                incomingCodeParser = new Code.XPressCodeParser(incomingCodeParser.Context);
            return base.DecorateCodeParser(incomingCodeParser);
        }

        public override System.Web.Razor.Generator.RazorCodeGenerator DecorateCodeGenerator(System.Web.Razor.Generator.RazorCodeGenerator incomingCodeGenerator)
        {
            if (incomingCodeGenerator is System.Web.Razor.Generator.CSharpRazorCodeGenerator)
                incomingCodeGenerator = new XPressCodeGenerator(incomingCodeGenerator.ClassName, incomingCodeGenerator.RootNamespaceName, incomingCodeGenerator.SourceFileName, incomingCodeGenerator.Host);
            return base.DecorateCodeGenerator(incomingCodeGenerator);
        }

        public override void PostProcessGeneratedCode(System.Web.Razor.Generator.CodeGeneratorContext context)
        {
            base.PostProcessGeneratedCode(context);
        }
    }
}
