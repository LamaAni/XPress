using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Generator;
using System.Web.Razor.Tokenizer.Symbols;

namespace XPress.Web.Razor.Code
{
    /// <summary>
    /// Generates attribute code for class.
    /// </summary>
    public class XPressAttribCodeGenerator : SpanCodeGenerator
    {
        public XPressAttribCodeGenerator()
            :base()
        {
        }

        IEnumerator<ISymbol> symbolsEnum;

        bool MoveToSymbol(string symbol)
        {
            bool hasSymbol=false;
            while (symbolsEnum.Current != null)
            {
                if(symbolsEnum.Current.Content == symbol)
                {
                    hasSymbol = true;
                    break;
                }
                symbolsEnum.MoveNext();
            }
            return hasSymbol;
        }

        bool MoveToSymbolType(CSharpSymbolType type)
        {
            bool hasSymbol = false;
            while (symbolsEnum.Current != null)
            {
                CSharpSymbol symbol=symbolsEnum.Current as CSharpSymbol;
                if (symbol != null && symbol.Type == type)
                {
                    hasSymbol = true;
                    break;
                }
                symbolsEnum.MoveNext();
            }
            return hasSymbol;
        }

        string MoveAndCollectTillNextArgument(CSharpSymbolType seperator, CSharpSymbolType ender)
        {
            Stack<CSharpSymbol> openers = new Stack<CSharpSymbol>();
            StringWriter wr = new StringWriter();
            while(symbolsEnum.MoveNext())
            {
                CSharpSymbol sym=symbolsEnum.Current as CSharpSymbol;
                if(sym==null)
                {
                    wr.Write(symbolsEnum.Current.Content);
                    continue;
                }

                bool isOpener = sym.Type == CSharpSymbolType.LeftParenthesis || sym.Type == CSharpSymbolType.LeftBrace || sym.Type == CSharpSymbolType.LeftBracket;
                bool isCloser = sym.Type == CSharpSymbolType.RightParenthesis || sym.Type == CSharpSymbolType.RightBrace || sym.Type == CSharpSymbolType.RightBracket;
                bool isStopAttrib = sym.Type == ender || sym.Type ==seperator;

                if(openers.Count==0 && isStopAttrib)
                {
                    break;
                }
                else if(isOpener)
                {
                    openers.Push(sym);
                }
                else if(isCloser)
                {
                    if (openers.Count == 0 || openers.Peek().Type != sym.Type)
                        throw new Exception("Unbalanced brace when writing attributes.");
                    openers.Pop();
                }
                wr.Write(sym.Content);
            }
            return wr.ToString();
        }

        IEnumerable<CodeAttributeDeclaration> FindDeclerations(CodeGeneratorContext context)
        {
            List<CodeAttributeDeclaration> declerations = new List<CodeAttributeDeclaration>();
            if (MoveToSymbolType(CSharpSymbolType.LeftBracket))
                while (true)
                {
                    // reading the attrib type.
                    CodeAttributeDeclaration dec = new CodeAttributeDeclaration();
                    declerations.Add(dec);
                    MoveToSymbolType(CSharpSymbolType.Identifier);
                    if (symbolsEnum.Current == null)
                    {
                        System.Diagnostics.Trace.WriteLine("Attrib parser: Cannot parse the attribute symbols correctly, syntax error.");
                        return new CodeAttributeDeclaration[0];
                    }
                    CSharpSymbol identifier = symbolsEnum.Current as CSharpSymbol;
                    dec.Name = identifier.Content;
                    symbolsEnum.MoveNext();
                    CSharpSymbol enderOrBrace = symbolsEnum.Current as CSharpSymbol;
                    if (enderOrBrace.Type == CSharpSymbolType.RightBrace)
                        continue;
                    if (enderOrBrace.Type != CSharpSymbolType.LeftParenthesis)
                    {
                        System.Diagnostics.Trace.WriteLine("Attrib parser: Cannot parse the attribute symbols correctly, syntax error.");
                        return new CodeAttributeDeclaration[0];
                    }
                    // finding attributes.
                    ;
                    // reading all type attributes.
                    while (true)
                    {
                        string attr = MoveAndCollectTillNextArgument(CSharpSymbolType.Comma, CSharpSymbolType.RightParenthesis);
                        if (attr == "")
                            break;
                        dec.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression(attr)));
                        if ((symbolsEnum.Current as CSharpSymbol).Type == CSharpSymbolType.RightParenthesis)
                            break;
                    }
                    symbolsEnum.MoveNext();
                    CSharpSymbol lastSym = symbolsEnum.Current as CSharpSymbol;
                    if (lastSym.Type == CSharpSymbolType.RightBracket || lastSym.Type == CSharpSymbolType.Comma)
                    {
                        symbolsEnum.MoveNext();
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("Attrib parser: Cannot parse the attribute symbols correctly, syntax error.");
                        return new CodeAttributeDeclaration[0];
                    }

                    if(lastSym.Type == CSharpSymbolType.RightBracket)
                    {
                        if (!MoveToSymbolType(CSharpSymbolType.LeftBracket))
                            break;
                    }
                }

            // finding the first left brace, of appropriate type.
            return declerations;
        }

        public override void GenerateCode(System.Web.Razor.Parser.SyntaxTree.Span target, CodeGeneratorContext context)
        {
            // parsing the attributes
            symbolsEnum = target.Symbols.GetEnumerator();
            symbolsEnum.MoveNext();
            // scanning values till..
            context.GeneratedClass.CustomAttributes.AddRange(FindDeclerations(context).ToArray());

            //context.GeneratedClass.Comments.Add(new CodeCommentStatement("asdasd\ndfgdfg"));
            //base.GenerateCode(target, context);
        }
    }
}
