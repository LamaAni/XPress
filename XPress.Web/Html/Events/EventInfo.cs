using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Html.Events
{
    public class EventInfo
    {
        #region Constructors

        public EventInfo(EventDefaults name)
            :this(name.ToString())
        {
        }

        public EventInfo(string name)
        {
            Name = name.Trim().ToLower();
            if(Name.IsEmpty())
                throw new Exception("Cannot create an event that has no name");
            if (Name.Contains("."))
            {
                string[] names = Name.Split(new char[1] { '.' }, 2).Where(n => !n.IsEmpty()).ToArray();
                Name = names[0];
                if (names.Length > 1)
                    NameSpace = names[1];
            }
        }

        #endregion

        #region members

        /// <summary>
        /// The event name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The namespace of the event. Recongnized by a "." after the event name. Such as Click.[some namespace]
        /// </summary>
        public string NameSpace { get; set; }

        /// <summary>
        /// The full name of the event including the event namespace.
        /// </summary>
        [XPress.Serialization.Attributes.XPressIgnore]
        public string FullName { get { return Name + (!NameSpace.IsEmpty() ? "." + NameSpace : ""); } }

        #endregion
    }

    public enum BubbleDirection { 
        ToParent = 0, 
        ToChildren = 1 
    }
    /// <summary>
    /// Default events that exists in the system. 
    /// </summary>
    [Flags]
    public enum EventDefaults
    {
        /// <summary>
        /// Called when the page has been loaded (only then).
        /// </summary>
        PageLoad = 0,
        /// <summary>
        /// Called when an existing control is changed by events (Update is called).
        /// </summary>
        ChangedByEvents = 4,
        /// <summary>
        /// Called right before the render to all rendering controls.
        /// </summary>
        PreRender = 1,
        /// <summary>
        /// Called after the control parent has changed.
        /// </summary>
        ParentChanged = 2,
        /// <summary>
        /// Called when the client triggers it.
        /// </summary>
        Update=8,
    }
}
