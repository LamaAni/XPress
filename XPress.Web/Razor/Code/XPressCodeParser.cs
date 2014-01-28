using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;
using System.Web.Razor.Tokenizer.Symbols;

namespace XPress.Web.Razor.Code
{
    public partial class XPressCodeParser : CSharpCodeParser
    {
        public XPressCodeParser(ParserContext context)
            : base()
        {
            this.Context = context;
            this.MapDirectives(AttribDirective, "attrib");
            //this.MapDirectives(MapLink, "link");
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This only occurs in Release builds, where this method is empty by design")]
        [Conditional("DEBUG")]
        internal void Assert(CSharpSymbolType symbol)
        {
            Debug.Assert(At(symbol));
        }

        /// <summary>
        /// Adding the attrib directive to allow for adding attributes to the main class
        /// </summary>
        protected virtual void AttribDirective()
        {
            //// Set the block type
            //FunctionsDirective();
            //// replace the code generator.
            //Span.CodeGenerator = new RmcAttribCodeGenerator();
            if(Context.DesignTimeMode)
            {
                FunctionsDirective();
                return;
            }

            Context.CurrentBlock.Type = BlockType.Functions;

            // Verify we're on "functions" and accept
            AssertDirective("attrib");
            Block block = new Block(CurrentSymbol);
            AcceptAndMoveNext();

            AcceptWhile(IsSpacingToken(includeNewLines: true, includeComments: false));

            if (!At(CSharpSymbolType.LeftBrace))
            {
                Context.OnError(CurrentLocation,
                                "Value expected",
                                Language.GetSample(CSharpSymbolType.LeftBrace));
                CompleteBlock();
                Output(SpanKind.MetaCode);
                return;
            }
            else
            {
                Span.EditHandler.AcceptedCharacters = AcceptedCharacters.None;
            }

            // Capture start point and continue
            SourceLocation blockStart = CurrentLocation;
            AcceptAndMoveNext();

            // Output what we've seen and continue
            Output(SpanKind.MetaCode);

            AutoCompleteEditHandler editHandler = new AutoCompleteEditHandler(Language.TokenizeString);
            Span.EditHandler = editHandler;

            Balance(BalancingModes.NoErrorOnFailure, CSharpSymbolType.LeftBrace, CSharpSymbolType.RightBrace, blockStart);
            Span.CodeGenerator = new XPressAttribCodeGenerator();
            if (!At(CSharpSymbolType.RightBrace))
            {
                editHandler.AutoCompleteString = "}";
                Context.OnError(block.Start, "Expected end of block before end of file", block.Name, "}", "{");
                CompleteBlock();
                Output(SpanKind.Code);
            }
            else
            {
                Output(SpanKind.Code);
                Assert(CSharpSymbolType.RightBrace);
                Span.CodeGenerator = SpanCodeGenerator.Null;
                Span.EditHandler.AcceptedCharacters = AcceptedCharacters.None;
                AcceptAndMoveNext();
                CompleteBlock();
                Output(SpanKind.MetaCode);
            }
        }

        protected virtual void LinkDirective()
        {
            AssertDirective("link");
            AcceptAndMoveNext();
            Context.CurrentBlock.Type = BlockType.Directive;
            AcceptUntil(CSharpSymbolType.NewLine);
            string linkCommand = Span.GetContent();
            Output(SpanKind.MetaCode);
            Context.CurrentBlock.CodeGenerator = new LinkCodeGenerator(linkCommand);
        }

        private void CompleteBlock()
        {
            CompleteBlock(insertMarkerIfNecessary: true);
        }

        private void CompleteBlock(bool insertMarkerIfNecessary)
        {
            CompleteBlock(insertMarkerIfNecessary, captureWhitespaceToEndOfLine: insertMarkerIfNecessary);
        }

        private void CompleteBlock(bool insertMarkerIfNecessary, bool captureWhitespaceToEndOfLine)
        {
            if (insertMarkerIfNecessary && Context.LastAcceptedCharacters != AcceptedCharacters.Any)
            {
                AddMarkerSymbolIfNecessary();
            }

            EnsureCurrent();

            // Read whitespace, but not newlines
            // If we're not inserting a marker span, we don't need to capture whitespace
            if (!Context.WhiteSpaceIsSignificantToAncestorBlock &&
                Context.CurrentBlock.Type != BlockType.Expression &&
                captureWhitespaceToEndOfLine &&
                !Context.DesignTimeMode &&
                !IsNested)
            {
                CaptureWhitespaceAtEndOfCodeOnlyLine();
            }
            else
            {
                PutCurrentBack();
            }
        }

        private void CaptureWhitespaceAtEndOfCodeOnlyLine()
        {
            IEnumerable<CSharpSymbol> ws = ReadWhile(sym => sym.Type == CSharpSymbolType.WhiteSpace);
            if (At(CSharpSymbolType.NewLine))
            {
                Accept(ws);
                AcceptAndMoveNext();
                PutCurrentBack();
            }
            else
            {
                PutCurrentBack();
                PutBack(ws);
            }
        }
        //static char[] endLinkBlock = new char[2] { '\n', '\r' };

        //public virtual void MapLink()
        //{
        //    AssertDirective("link");
        //    // the current location.
        //    AcceptAndMoveNext();

        //    SourceLocation endModelLocation = CurrentLocation;
        //    BaseTypeDirective("The 'model' keyword must be followed by a type name on the same line.", CreateModelCodeGenerator);
        //}

        //private const string GenericTypeFormatString = "{0}<{1}>";

        //private SpanCodeGenerator CreateModelCodeGenerator(string model)
        //{
        //    return new SetLinkTypeCodeGenerator();
        //    return new SetModelTypeCodeGenerator(model, GenericTypeFormatString);
        //}
    }

    public class AttributeGenerator : System.CodeDom.CodeAttributeDeclaration
    {
        public AttributeGenerator(string attr)
            :base()
        {
            // checking for the attribute name.
        }
    }

    public class LinkCodeGenerator : BlockCodeGenerator
    {
        public LinkCodeGenerator(string cmnd)
        {
            LinkCommand = cmnd;
        }

        public string LinkCommand { get; private set; }

        public override void GenerateEndBlockCode(Block target, CodeGeneratorContext context)
        {
        }

        public override void GenerateStartBlockCode(Block target, CodeGeneratorContext context)
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return "Link:" + LinkCommand;
        }
    }

    public class SetAttributeTypeCodeGenerator : SetBaseTypeCodeGenerator
    {
        public SetAttributeTypeCodeGenerator(string type, string baseType)
            :base(baseType)
        {
            AttribType = type;
        }

        public string AttribType { get; private set; }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                AttribType,
                context.Host.DefaultBaseClass,
                baseType);
        }

        public override bool Equals(object obj)
        {
            SetAttributeTypeCodeGenerator other = obj as SetAttributeTypeCodeGenerator;
            return other != null &&
                   base.Equals(obj) &&
                   String.Equals(AttribType, other.AttribType, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return XPress.Coding.HashCodeCombiner.Start()
                .Add(base.GetHashCode())
                .Add(AttribType.GetHashCode())
                .CurrentHash;
        }

        public override string ToString()
        {
            return "Attrib:" + BaseType;
        }
    }
}
