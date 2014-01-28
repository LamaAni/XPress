using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.Html.Events;

namespace XPress.Web.Html.Collections
{
    /// <summary>
    /// Holds a collection of events that can be invoked from an I control.
    /// </summary>
    public class EventCollection : ISerializable
    {
        public EventCollection()
        {
        }

        #region ISerializable Members

        public EventCollection(SerializationInfo info, StreamingContext context)
        {
            m_invokes = new Dictionary<string, List<Tuple<EventInfo, Action<object, EventArgs>>>>();
            object[] invokes = (object[])info.GetValue("i", typeof(object[]));
            if (invokes != null)
            {
                for (int i = 0; i < invokes.Length; i += 2)
                {
                    string key = (string)invokes[i];
                    object[] calls = (object[])invokes[i + 1];
                    List<Tuple<EventInfo, Action<object, EventArgs>>> callList =
                        new List<Tuple<EventInfo, Action<object, EventArgs>>>();
                    for (int j = 0; j < calls.Length; j += 2)
                    {
                        EventInfo evi = (EventInfo)calls[j];
                        Action<object, EventArgs> f = (Action<object, EventArgs>)calls[j + 1];
                        callList.Add(new Tuple<EventInfo, Action<object, EventArgs>>(evi, f));
                    }
                    m_invokes.Add(key, callList);
                }
            }
            m_loadEventTriggered = info.GetBoolean("let");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // reading the event collection data;
            if (m_invokes != null)
            {
                KeyValuePair<string, List<Tuple<EventInfo, Action<object, EventArgs>>>>[] valid =
                    m_invokes.Where(kvp => kvp.Value.Count > 0).ToArray();

                object[] invokes = new object[valid.Length * 2];
                valid.ForEach((kvp, i) =>
                {
                    invokes[i * 2] = kvp.Key;
                    object[] calls = new object[kvp.Value.Count * 2];
                    kvp.Value.ForEach((elm, j) =>
                    {
                        calls[j * 2] = elm.Item1;
                        calls[j * 2 + 1] = elm.Item2;
                    });
                    invokes[i * 2 + 1] = calls;
                });
                info.AddValue("i", invokes);
            }
            else info.AddValue("i", null);
            info.AddValue("let", m_loadEventTriggered);
        }

        #endregion

        #region members

        bool m_loadEventTriggered = false;

        public bool LoadEventTriggered
        {
            get { return m_loadEventTriggered; }
        }

        Dictionary<string, List<Tuple<EventInfo, Action<object, EventArgs>>>> m_invokes;

        /// <summary>
        /// A collection of all actions that are bound to the control.
        /// </summary>
        protected Dictionary<string, List<Tuple<EventInfo, Action<object, EventArgs>>>> Invokes
        {
            get { if (m_invokes == null)m_invokes = new Dictionary<string, List<Tuple<EventInfo, Action<object, EventArgs>>>>(); return m_invokes; }
        }

        public int Count
        {
            get
            {
                return Invokes.Values.Where(v => v.Count > 0).Count();
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Binds an event to the current.
        /// </summary>
        /// <param name="i">The event info</param>
        /// <param name="a">The action to take</param>
        public void Bind(EventInfo i, Action<object, EventArgs> a)
        {
            if (!Invokes.ContainsKey(i.Name))
                Invokes[i.Name] = new List<Tuple<EventInfo, Action<object, EventArgs>>>();
            Invokes[i.Name].Add(new Tuple<EventInfo, Action<object, EventArgs>>(i, a));
        }

        public void UnBind(string eventName)
        {
            EventInfo i = new EventInfo(eventName);
            UnBind(i);
        }

        /// <summary>
        /// Clears all bound events for the current name. If a namespace is provideed only events in that namespace are removed.
        /// </summary>
        /// <param name="eventName"></param>
        public void UnBind(EventInfo i)
        {
            if (!Invokes.ContainsKey(i.Name))
                return;
            // getting the evnt list that are bound to the current.
            if (i.NameSpace == null)
            {
                // unbinding all events.
                Invokes.Remove(i.Name);
            }
            else
            {
                // filtering the actions in the current namespace.
                Invokes[i.Name] = Invokes[i.Name].Where(tpl => tpl.Item1.NameSpace != i.NameSpace).ToList();
            }
        }


        /// <summary>
        /// Get the evnets that match the event name and namespace.
        /// </summary>
        /// <param name="eventName">The event to invoke.</param>
        /// <param name="triggerOncePerObject">If true this event will not be triggred more then once in the liftime of an object</param>
        /// <returns>The events list</returns>
        public IEnumerable<Tuple<EventInfo, Action<object, EventArgs>>> GetEventList(string eventName)
        {
            return GetEventList(new EventInfo(eventName));
        }

        /// <summary>
        /// Get the evnets that match the event name and namespace.
        /// </summary>
        /// <param name="eventName">The event to invoke.</param>
        /// <param name="triggerOncePerObject">If true this event will not be triggred more then once in the liftime of an object</param>
        /// <returns>The events list</returns>
        public IEnumerable<Tuple<EventInfo, Action<object, EventArgs>>> GetEventList(EventInfo i)
        {
            if (m_invokes==null || !Invokes.ContainsKey(i.Name))
                return new Tuple<EventInfo, Action<object, EventArgs>>[0];
            IEnumerable<Tuple<EventInfo, Action<object, EventArgs>>> evs = Invokes[i.Name];
            if (i.NameSpace != null)
                evs = evs.Where(tpl => tpl.Item1.NameSpace == i.NameSpace);
            return evs.ToArray();
        }

        /// <summary>
        /// Called to trigger the event.
        /// </summary>
        /// <param name="i">The event that was triggered</param>
        /// <returns>Null if no events were found. False if one of the event functions has retured false. otherwise true.</returns>
        public virtual void Trigger(object sender, EventArgs e, EventInfo i)
        {
            if (i.Name == "load")
            {
                if (LoadEventTriggered)
                    return;
                m_loadEventTriggered = true;
            }
            IEnumerable<Tuple<EventInfo, Action<object, EventArgs>>> events = GetEventList(i);
            if (events.Count() == 0)
                return;
            events.ForEach(ev => ev.Item2(sender, e));
        }

        /// <summary>
        /// Returns true if the current contains the event.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool Contains(EventInfo info)
        {
            return Contains(info.Name) && (info.NameSpace == null || Invokes[info.Name].Any(tpl => tpl.Item1.NameSpace == info.NameSpace));
        }

        /// <summary>
        /// Returns true if the current contains the event.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool Contains(string eventName)
        {
            return Invokes.ContainsKey(eventName);
        }

        #endregion
    }
}
