using XPress.Web.Html.Linq;
using XPress.Web.Html.Rendering;
using XPress.Web.Links;
using XPress.Web.Links.Attributes;
using XPress.Web.Razor;
using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XPress.Web.Html.Collections;
using XPress.Serialization;
using XPress.Strings;

namespace XPress.Web.Html
{
    /// <summary>
    /// Implements a most basic client page that can handle responses from the server and the client.
    /// This is the most basic client page that forces the download of the script associated with the remote client
    /// and allows for sending commands from the client to the server.
    /// </summary>
    [LinkScript("XPress.Web.Core.jquery.js", LinkOrigin.Embedded, LoadType = LinkLoadType.HeadIfPossible, LoadIndex = 1)]
    [LinkScript("XPress.Web.Core.GloablExtentions.js", LinkOrigin.Embedded, LoadType = LinkLoadType.HeadIfPossible, LoadIndex = 2)]
    [LinkScript("XPress.Web.Core.RmcCore.js", LinkOrigin.Embedded, LoadType = LinkLoadType.HeadIfPossible, LoadIndex = 3)]
    [LinkScript("XPress.Web.JCom.JCom.js", LinkOrigin.Embedded, LoadType = LinkLoadType.HeadIfPossible, LoadIndex = 4)]
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public class HtmlTemplate : HtmlElement, IRazorTemplate
    {
        /// <summary>
        /// Basic constructor.
        /// </summary>
        public HtmlTemplate(string tagName = null)
            : base(tagName)
        {
            // default value is false.
            CanBePage = false;
        }

        #region members

        TypeDependentCacheCollection m_TypeDependentCache;

        /// <summary>
        /// A collection of links associated with the page.
        /// </summary>
        public virtual TypeDependentCacheCollection TypeDependentCache { get { if (m_TypeDependentCache == null)m_TypeDependentCache = new TypeDependentCacheCollection(); return m_TypeDependentCache; } }

        /// <summary>
        /// The tag name of the template.
        /// </summary>
        public override string TagName
        {
            get
            {
                return base.TagName;
            }
            set
            {
                base.TagName = value;
            }
        }

        #endregion

        #region template writing

        Rendering.TemplateWriteContext m_writeContext;

        /// <summary>
        /// The current write context.
        /// </summary>
        protected Rendering.TemplateWriteContext WriteContext
        {
            get
            {
                if (m_writeContext == null)
                    m_writeContext = new Rendering.TemplateWriteContext();
                return m_writeContext;
            }
        }

        /// <summary>
        /// Returns the html elemnt that currently accepts the write commands. (Default: this). 
        /// </summary>
        /// <returns></returns>
        protected HtmlElement GetWriteToContextElement()
        {
            return m_writeContext == null || m_writeContext.OpenContext.Count == 0 ? this : m_writeContext.OpenContext.Peek();
        }

        /// <summary>
        /// Makes the current control the active context of the write.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public T Open<T>(T q)
            where T : IQuery
        {
            HtmlElement element = q.GetLinqEnumrable().First();
            if (element.Parent != GetWriteToContextElement())
                Write(element);
            m_writeContext.OpenContext.Push(element);
            return q;
        }

        /// <summary>
        /// Closes the current write context and returns to the previus if any. Else returns to this.
        /// </summary>
        /// <returns>True if the context has changed.</returns>
        public IgnoreResultWriteContext Close()
        {
            if (m_writeContext.OpenContext.Count > 0)
            {
                if (m_writeContext != null)
                    WriteContext.LastLiteral = null;
                m_writeContext.OpenContext.Pop();
            }

            return new IgnoreResultWriteContext();
        }

        #endregion

        #region Razor base functions (internal).

        /// <summary>
        /// Executes the page construction. Called once when the template is executing. Overwritten by the razor system.
        /// </summary>
        public virtual void Execute()
        {
        }

        #region obsolete context methods.

        /// <summary>
        /// Tells the object to begin a new context in the output stream.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="length"></param>
        /// <param name="isLiteral"></param>
        protected void BeginContext(int startPosition, int length, bool isLiteral)
        {
        }

        /// <summary>
        /// Creates a new context for a template in the ouput stream.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="startPosition"></param>
        /// <param name="length"></param>
        /// <param name="isLiteral"></param>
        protected void BeginContext(string virtualPath, int startPosition, int length, bool isLiteral)
        {
        }

        /// <summary>
        /// Ends the current context.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="length"></param>
        /// <param name="isLiteral"></param>
        protected void EndContext(int startPosition, int length, bool isLiteral)
        {
        }

        /// <summary>
        /// Ends the current context for the output stream.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="startPosition"></param>
        /// <param name="length"></param>
        /// <param name="isLiteral"></param>
        protected void EndContext(string virtualPath, int startPosition, int length, bool isLiteral)
        {
        }

        #endregion

        #endregion

        #region Page methods

        /// <summary>
        /// The snippet associated with the head define.
        /// </summary>
        protected HtmlSnippet HeadSnippet { get; set; }

        /// <summary>
        /// The snippet associated with the body init.
        /// </summary>
        protected HtmlSnippet InitSnippet { get; set; }

        /// <summary>
        /// True if the page head was rendered to the client side.
        /// </summary>
        bool AutoPageHeadInserted { get { return HeadSnippet != null; } }

        /// <summary>
        /// True if the page head was rendered to the client side.
        /// </summary>
        bool AutoPageInitInserted { get { return InitSnippet != null; } }

        /// <summary>
        /// Inserts the page auto generated head, should be called in the head part of the page.
        /// </summary>
        /// <returns></returns>
        protected HtmlSnippet InsertAutoPageHead()
        {
            if (HeadSnippet == null)
                HeadSnippet = new HtmlSnippet("<!-- Head auto generated code -->");
            else HeadSnippet.Html = "";
            return HeadSnippet;
        }

        /// <summary>
        /// Inserts the page head, should be called in the body part of the head.
        /// </summary>
        /// <returns></returns>
        protected HtmlSnippet InsertAutoPageInit()
        {
            if (InitSnippet == null)
                InitSnippet = new HtmlSnippet("<!-- Init auto generated code -->");
            else InitSnippet.Html = "";
            return InitSnippet;
        }

        #endregion

        #region IRazorTemplate Members

        /// <summary>
        /// Returns true if the current can be treated as a page or false if the current may only be internally loaded into another page.
        /// </summary>
        public bool CanBePage
        {
            get;
            protected set;
        }

        /// <summary>
        /// Returns true if running as page.
        /// </summary>
        public bool AsPage { get; private set; }

        /// <summary>
        /// Writes the object to the templates's output stream.
        /// </summary>
        /// <param name="o"></param>
        public virtual void Write(object o)
        {
            if (o == null || GetWriteToContextElement() == o || o is IgnoreResultWriteContext)
                return; // ignore, the object cannot write itself to itself.

            bool isHtmlElement = o is HtmlElement;
            if (!isHtmlElement && o is IQuery)
                ((IQuery)o).GetLinqEnumrable().ForEach(el => { Write(el); }); // for quries that are not html elements.
            else if (o is HtmlLiteral)
            {
                WriteLiteralObject(o as HtmlLiteral);
            }
            else if (o is HtmlElement)
            {
                HtmlElement el = o as HtmlElement;
                if (el.Parent != this)
                {
                    GetWriteToContextElement().Children.Append(el);
                    // the current is not literal therefore clear the last one.
                    if (m_writeContext != null)
                        WriteContext.LastLiteral = null;
                }
            }
            else WriteLiteral(o.ToString());
        }

        /// <summary>
        /// Writes a string literal into the templates output stream.
        /// </summary>
        /// <param name="literal"></param>
        public virtual void WriteLiteral(string literal)
        {
            if (WriteContext.LastLiteral != null)
            {
                WriteContext.LastLiteral.LiteralBuilder.Append(literal);
            }
            else
            {
                WriteLiteralObject(new HtmlLiteral(literal));
            }
        }

        /// <summary>
        /// Writes a literal object to the active context.
        /// </summary>
        /// <param name="literal"></param>
        protected virtual void WriteLiteralObject(HtmlLiteral literal)
        {
            if (WriteContext.LastLiteral != null)
            {
                WriteContext.LastLiteral.LiteralBuilder.Append(literal.Html);
            }
            else
            {
                WriteContext.LastLiteral = literal;
                GetWriteToContextElement().Children.Append(literal);
            }
        }

        #endregion

        #region Rendering
        
        /// <summary>
        /// Creates an object source to determine the object id for object generation.
        /// </summary>
        /// <returns></returns>
        protected virtual JCom.Com.IJComObjectSource CreateObjectSource()
        {
            return null;
        }

        /// <summary>
        /// Called to render the page to string.
        /// </summary>
        /// <returns></returns>
        public virtual string RenderPage(HttpContext context)
        {
            // rendering the render
            Rendering.HtmlWriter writer = new Rendering.HtmlWriter(CreateObjectSource());
            CreatePageInitTypeDependentCommands(context, writer);

            this.PreRender(writer);

            // rendering the pending commands as json command.
            StringBuilder initBuilder = new StringBuilder();
            initBuilder.Append("<script>");
            initBuilder.Append("console.log('Initialzing page...');");
#if DEBUG
            initBuilder.Append("\n");
#endif
            initBuilder.Append("$(document).ready(function(){");
#if DEBUG
            initBuilder.Append("\n");
#endif
            CreatePageInitScript(writer, initBuilder);
            CreateInitScript(initBuilder);
            initBuilder.Append("console.log('Page init completed.');");
#if DEBUG
            initBuilder.Append("\n");
#endif
            initBuilder.Append("});");
#if DEBUG
            initBuilder.Append("\n");
#endif
            initBuilder.Append("</script>");

            InitSnippet.Html += initBuilder.ToString();

            this.Render(writer);

            // adding init commands (a js command collection to be called after render).
            return writer.ToString();
        }

        private void CreatePageInitTypeDependentCommands(HttpContext context, Rendering.HtmlWriter writer)
        {

            // calling to load the missing type definitions.
            HashSet<Type> types = new HashSet<Type>();

            this.Invoke((el) =>
            {
                Type t = el.GetType();
                if (!types.Contains(t))
                    types.Add(t);
                return true;
            }, true, BubbleDirection.ToChildren);

            // adding the type commands.
            writer.InitCommands.AddRange(
                TypeDependentCache.CreateTypeDependentDefinitionCommands(context, types, true));
        }

        /// <summary>
        /// Called to pre render the page.
        /// </summary>
        /// <param name="writer"></param>
        public override void PreRender(HtmlWriter writer)
        {
            // Commands list to add to the page.
            // calling to collect all child control types and render the head scripts. Also renders the pend commmands for linking.
            if (AsPage)
            {
                StringBuilder headBuilder = new StringBuilder();
                CreatePageHeadLinks(Context, writer, headBuilder);
                CreatePageHead(headBuilder);
                HeadSnippet.Html += headBuilder.ToString();
            }
            base.PreRender(writer);
        }

        private void CreatePageInitScript(HtmlWriter writer, StringBuilder initBuilder)
        {
            if (writer.HasInitCommands)
            {
#if DEBUG
                initBuilder.Append("document.Debug=true;$.Vebrose=true;\n");
#endif
                initBuilder.Append("$.XPress.Commands.Execute($.JSON.From(\"" + (new { commands = writer.InitCommands }).ToJSJson().EscapeForJS() + "\").commands);");
#if DEBUG
                initBuilder.Append("\n");
#endif
            }
        }

        /// <summary>
        /// Creates the initialziation script downloaded to the client.
        /// </summary>
        /// <param name="builder">The command builder</param>
        protected virtual void CreateInitScript(StringBuilder initBuilder)
        {
        }


        /// <summary>
        /// Creates the head script to be downloaded to the client.
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void CreatePageHead(StringBuilder headBuilder)
        {
        }

        /// <summary>
        /// Private, to create the page head links.
        /// </summary>
        /// <param name="headBuilder"></param>
        private void CreatePageHeadLinks(HttpContext context, HtmlWriter writer, StringBuilder headBuilder)
        {
#if DEBUG
            headBuilder.Append("\n");
#endif
            // adding the head dependent links and removing thier commands from the command
            // init collection.
            Core.XPressResponseCommand[] respCmnds = writer.InitCommands.ToArray();
            writer.InitCommands.Clear();
            respCmnds.ForEach(cmnd =>
            {
                Response.LinkCommand link = cmnd as Response.LinkCommand;
                if (link != null && link.Info.Link.LoadType == LinkLoadType.HeadIfPossible)
                {
                    headBuilder.Append(link.Info.RenderAsHeadLink(context));
#if DEBUG
                    headBuilder.Append("\n");
#endif
                }
                else writer.InitCommands.Add(cmnd);
            });
        }

        #endregion

        #region IRazorTemplate page processing.

        /// <summary>
        /// Called to process the request. Override this function to create new handling for the page request.
        /// </summary>
        public virtual void ProcessRequest(HttpContext context)
        {
            AsPage = true;
#if DEBUG
            if (!this.CanBePage)
                context.Response.Headers.Add("razor-object", "template");
#else
            if (this.CanBePage == false)
                throw new Exception("Cannot render page from a template object. Please change the page type or mark the page with this.AsPage=true.");
#endif
            // calling the init
            Execute();

            if (!AutoPageHeadInserted)
                this.Write(InsertAutoPageHead());
            if (!AutoPageInitInserted)
                this.Write(InsertAutoPageInit());

            // writing the page response.
            context.Response.Write(RenderPage(context));
        }



        #endregion
    }
}
