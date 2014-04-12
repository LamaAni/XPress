using XPress.Web.Links.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Strings;
using XPress.Web.Links;
using XPress.Serialization;

namespace XPress.Web.Html.Rendering
{
    /// <summary>
    /// A class that represents the object activation before it is built.
    /// </summary>
    public class ActivationBuilder
    {
        public ActivationBuilder(Type t)
        {
            MappedType = t;
            LinksInfo = Mapper.GetMapInfo(t);
            JComInfo = JCom.Map.JComTypeInfo.Get(t);
            Event = LinksInfo.Activation.Event;
        }

        #region members

        public Type MappedType { get; private set; }

        /// <summary>
        /// The links info associated with the object type.
        /// </summary>
        public LinkMapInfo LinksInfo { get; private set; }

        /// <summary>
        /// The information about the jcom loading. 
        /// </summary>
        public JCom.Map.JComTypeInfo JComInfo { get; private set; }

        /// <summary>
        /// JCom map info,
        /// </summary>
        List<Core.XPressResponseCommand> m_commands;

        /// <summary>
        /// A collection of commands to be executed on activation.
        /// </summary>
        public List<Core.XPressResponseCommand> Commands
        {
            get
            {
                if (m_commands == null)
                    m_commands = new List<Core.XPressResponseCommand>();
                return m_commands;
            }
        }

        /// <summary>
        /// The activation event asssociated with the current object.
        /// </summary>
        public Links.ActivationEvent Event { get; private set; }

        /// <summary>
        /// If true the current will force activation on the object.
        /// </summary>
        public bool ForceActivation { get; set; }

        /// <summary>
        /// If true the object needs activation.
        /// </summary>
        public bool NeedsActivation { get { return ForceActivation || Commands.Count > 0 || JComInfo.RequiresClientSideDefinition; } }

        #endregion

        #region methods

        /// <summary>
        /// Writes an activation event for html element. (Under the property "_bc").
        /// </summary>
        /// <param name="element"></param>
        /// <returns>A collection of attributes that need to be added back to its original value</returns>
        public List<Tuple<string,string>> WriteElementActivation(HtmlElement element, HtmlWriter writer)
        {
            List<Tuple<string, string>> revertAttribs = null;
            // adding the activation
            if (NeedsActivation)
            {
                // need to validate the control has an id.
                if (element.Id == null)
                    element.Id = writer.ObjectSource.GetObjectId(element).ToString();

                revertAttribs = new List<Tuple<string, string>>();
                // writing the activation for the html element.
                element.Attributes["_bc"] = Commands.ToJSJson().EscapeForHtmlAttribute();
                revertAttribs.Add(new Tuple<string, string>("_bc", null));
                if (this.Event.HasFlag(Links.ActivationEvent.OnUpdate))
                {
                    string id = element.Attributes["id"];
                    if (id == null)
                        throw new Exception("Cannot initialize an HtmlElement that has no id using link activation: Links.ActivationEvent.OnUpdate.");
                    writer.InitCommands.Add(new Core.JScriptCommandResponce("$$($.FromId(\"" + id.EscapeForJS() + "\"));", Core.CommandExecutionType.Post));
                }
                else
                {
                    // replacing the "_bc" attribute.
                    // creating the replacement object for active functions.
                    Dictionary<string, string> acinfo = new Dictionary<string, string>();
                    AddIfAny(ActivationEvent.MouseClick, false, element, acinfo, revertAttribs);
                    AddIfAny(ActivationEvent.MouseDown, false, element, acinfo, revertAttribs);
                    AddIfAny(ActivationEvent.MouseUp, false, element, acinfo, revertAttribs);
                    AddIfAny(ActivationEvent.Focus, false, element, acinfo, revertAttribs);

                    bool atContext = Event.HasFlag(ActivationEvent.ActiveContext);
                    AddIfAny(ActivationEvent.MouseMove, atContext, element, acinfo, revertAttribs);
                    AddIfAny(ActivationEvent.MouseOver, atContext, element, acinfo, revertAttribs);

                    element.Attributes["_ac"] = acinfo.ToJSJson().EscapeForHtmlAttribute();
                    revertAttribs.Add(new Tuple<string, string>("_ac", null));
                }
            }

            return revertAttribs;
        }

        void AddIfAny(ActivationEvent ev, bool force, HtmlElement element, Dictionary<string,string> acinfo, List<Tuple<string, string>> revertAttribs)
        {
            if ((force || Event.HasFlag(ev)))
            {
                string eventName = "on" + ev.ToString().ToLower();
                string oldEvent = element.Attributes[eventName];
                string replacementEvent = element.Attributes.Contains(eventName) ? replacementEvent = oldEvent : null;
                acinfo[eventName] = replacementEvent;

                element.Attributes[eventName] = "$A$(this,event);";
                revertAttribs.Add(new Tuple<string, string>(eventName, oldEvent));
            }
        }

        #endregion
    }
}
