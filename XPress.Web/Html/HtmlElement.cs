using XPress.Web.Html.Linq;
using XPress.Web.Html.Rendering;
using XPress.Serialization.Attributes;
using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Strings;

namespace XPress.Web.Html
{
    /// <summary>
    /// Represents a basic html element to be rendered to the client.
    /// </summary>
    [XPressMemberSelection(Serialization.Attributes.XPressMemberSelectionType.OptIn)]
    public class HtmlElement : IQuery
    {
        public HtmlElement(string tagName=null)
        {
            TagName = tagName;
        }

        #region Members

        /// <summary>
        /// The client id that will be used when the object is rendered.
        /// </summary>
        public virtual string Id
        {
            get;
            set;
        }

        /// <summary>
        /// The current http context.
        /// </summary>
        public System.Web.HttpContext Context { get { return System.Web.HttpContext.Current; } }

        /// <summary>
        /// True if the elment should be rendered as a signle tag element. 
        /// </summary>
        public bool AsSingleTag { get; set; }

        /// <summary>
        /// Gets the tag name associated with the element. if null then no tag or attributes are written to the control.
        /// </summary>
        [XPressMember(IgnoreMode = XPressIgnoreMode.IfNull, Name = "_tag")]
        public virtual string TagName
        {
            get;
            set;
        }


        /// <summary>
        /// A collection of html attributes.
        /// </summary>
        public virtual Collections.AttributeCollection Attributes
        {
            get { if (m_attribs == null)m_attribs = new Collections.AttributeCollection(); return m_attribs; }
        }

        /// <summary>
        /// True if the control has any attributes.
        /// </summary>
        public virtual bool HasAttributes { get { return m_attribs == null || m_attribs.Count == 0; } }

        /// <summary>
        /// A style collection
        /// </summary>
        public Collections.StyleCollection Style
        {
            get { return Attributes.Style; }
        }

        /// <summary>
        /// Renders the child elements for this control.
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void RenderChildren(Rendering.HtmlWriter writer)
        {
            if (HasChildren)
                Children.ForEach(c => c.Render(writer));
        }

        /// <summary>
        /// Rendres the init commands to be added to the object attributes. (under the attribute "_bc");
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void CreateInitCommands(ActivationBuilder builder,Rendering.HtmlWriter writer)
        { 
            // loading the link information.
            if (builder.LinksInfo.RequiresActivation)
                // creating the initialziation command. (if any).
                builder.Commands.Add(
                    new Core.JScriptCommandResponce(Links.Compilers.JSCompiler.Global.CreateInitCode(builder.LinksInfo), Core.CommandExecutionType.Post));

            // check if need to construct the object client side defention
            if (builder.JComInfo.RequiresDataObject)
                builder.Commands.Add(new JCom.Com.Response.JComBuildObjectResponse(this, writer.ObjectSource.GetObjectId(this), 
                    false, builder.JComInfo, JCom.Compilers.Specialized.JavaScriptCompiler.Global));
        }

        /// <summary>
        /// Called before the page is rendered.
        /// </summary>
        /// <param name="writer"></param>
        public virtual void PreRender(Rendering.HtmlWriter writer)
        {
            // call to render the init commands.
            RenderInitCommands(writer);

            PreRenderChildren(writer);
        }

        /// <summary>
        /// Called when invoking the child control pre render.
        /// </summary>
        /// <param name="writer"></param>
        public virtual void PreRenderChildren(Rendering.HtmlWriter writer)
        {
            // call onto the children.
            if (HasChildren)
                Children.ForEach(c => c.PreRender(writer));
        }

        /// <summary>
        /// Renders the element content.
        /// </summary>
        public virtual void Render(Rendering.HtmlWriter writer)
        {
            // adding the link commands to 
            if (!AsSingleTag)
            {
                if (TagName != null)
                    writer.WriteStartTag(this, m_attribs != null && m_attribs.Count > 0);
                RenderChildren(writer);
                if (TagName != null)
                    writer.WriteEndTag(this);
            }
            else if (TagName != null)
            {
                writer.WriteSingleTag(this, m_attribs != null && m_attribs.Count > 0);
            }

            if (__revertAttribValues != null)
            {
                List<Tuple<string, string>> revert = __revertAttribValues;
                __revertAttribValues = null;
                revert.ForEach(t =>
                {
                    this.Attributes[t.Item1] = t.Item2;
                });
            }
        }

        /// <summary>
        /// A collection for reverting the attrib values after they were replaced by the element activation writer.
        /// </summary>
        List<Tuple<string, string>> __revertAttribValues = null;

        /// <summary>
        /// Creates the init commands to be used by the html element.
        /// </summary>
        void RenderInitCommands(Rendering.HtmlWriter writer)
        {
            ActivationBuilder builder = new ActivationBuilder(this.GetType());

            CreateInitCommands(builder, writer);

            // write the activation event.
            __revertAttribValues = builder.WriteElementActivation(this, writer);
        }

        /// <summary>
        /// A collection of child controls.
        /// </summary>
        public virtual Collections.ChildCollection Children
        {
            get
            {
                if (m_kids == null)
                {
                    IEnumerable<HtmlElement> els = null;
                    // post deserialzie in next line.
                    if (m_kidsList != null && m_kidsList.HasValue && m_kidsList.Value.Length > 0)
                        els = m_kidsList.Value;
                    m_kids = new Collections.ChildCollection(this, els);
                }
                return m_kids;
            }
        }

        /// <summary>
        /// Eveents binding for html control.
        /// </summary>
        [XPressMember("_events", IgnoreMode = XPressIgnoreMode.IfNull)]
        Collections.EventCollection m_Events;

        /// <summary>
        /// A collection of events attached to this control. Triggered at serverside. 
        /// </summary>
        public virtual Collections.EventCollection Events
        {
            get { if (m_Events == null)m_Events = new Collections.EventCollection(); return m_Events; }
        }

        #endregion

        #region IQuery Members

        /// <summary>
        /// returns the current object as linq enumrable.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HtmlElement> GetLinqEnumrable()
        {
            return new HtmlElement[1] { this };
        }

        #endregion

        #region internal members (All relevant values here can be accessed via (Linq) extention methods).

        [XPressMember("_parent", IgnoreMode = XPressIgnoreMode.IfNull)]
        PostDeserialize<HtmlElement> m_parent;
        /// <summary>
        /// The parent of the control.
        /// </summary>
        internal HtmlElement Parent
        {
            get
            {
                return m_parent == null ? null : m_parent.Value;
            }
            private set
            {
                m_parent = value == null ? null : new PostDeserialize<HtmlElement>(value);
            }
        }

        Collections.ChildCollection m_kids;
        PostDeserialize<HtmlElement[]> m_kidsList;

        /// <summary>
        /// Retruns true if the current has any children without creating (internal) child collection.
        /// </summary>
        public virtual bool HasChildren
        {
            get
            {
                return m_kids != null ? m_kids.Count > 0 : (m_kidsList != null ? m_kidsList.Value != null && m_kidsList.Value.Length > 0 : false);
            }
        }

        /// <summary>
        /// Holds the serializable property for remote controls.
        /// </summary>
        [XPressMember(Name = "_kids", IgnoreMode = XPressIgnoreMode.IfNull)]
        private PostDeserialize<HtmlElement[]> ChildrenWriterProperty
        {
            get
            {
                if (m_kids == null)
                {
                    if (m_kidsList == null || !m_kidsList.HasValue || m_kidsList.Value.Length == 0)
                        return null;
                    return m_kidsList;
                }

                return m_kids.Count == 0 ? null : new PostDeserialize<HtmlElement[]>(m_kids.ToArray());
            }
            set { m_kidsList = value; }
        }

        
        Collections.AttributeCollection m_attribs;

        /// <summary>
        /// The attrib writing collection. This allows for loading and storing the attributes.
        /// </summary>
        [XPressMember(IgnoreMode = XPressIgnoreMode.IfNull, Name = "_attr")]
        internal Collections.AttributeCollection AttribsWriterCollector
        {
            get
            {
                return m_attribs == null || m_attribs.Count == 0 ? null : m_attribs;
            }
            set { m_attribs = value; }
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Binds the parent to this control.
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        internal bool BindToParent(HtmlElement el)
        {
            if (el == this)
                throw new Exception("A control cannot be its own parent.");

            if (Parent == el)
                return false; // nothing to do, same parent.

            bool wasRemoved = false;
            if (Parent != null)
            {
                Parent.Children.Remove(this); // removing from parent.
                wasRemoved = true;
            }

            Parent = el;
            return wasRemoved;
        }

        /// <summary>
        /// returns true if the control has a loaded parent.
        /// </summary>
        /// <returns></returns>
        internal bool HasLoadedParent()
        {
            if (m_parent == null)
                return false;
            return m_parent.HasValue;
        }

        #endregion
    }
}
