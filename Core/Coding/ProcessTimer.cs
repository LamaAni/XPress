using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Coding;

namespace XPress.Coding
{
    public class ProcessTimer : XPress.Coding.CodeTimer, XPress.Serialization.IJsonSerializable
    {
        public ProcessTimer()
        {
        }

        #region members

        Dictionary<string, CodeTimer> m_innerTimers = new Dictionary<string, CodeTimer>();

        /// <summary>
        /// Internal timers by the region of the timer block.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public new CodeTimer this[string region]
        {
            get
            {
                if (!m_innerTimers.ContainsKey(region))
                {
                    m_innerTimers[region] = new CodeTimer();
                }
                return m_innerTimers[region];
            }
        }

        #endregion

        #region print members

        public Dictionary<string, Dictionary<string, DateTime>> Inner { get; private set; }

        #endregion

        #region IJsonSerializable Members

        public object ToSerializationObject()
        {
            Dictionary<string, object> dic = TimerToDocument(this);

            if (this.m_innerTimers.Count > 0)
            {
                m_innerTimers.ForEach(kvp =>
                {
                    dic[kvp.Key] = TimerToDocument(kvp.Value);
                });
            }

            return dic;
        }


        private static Dictionary<string,object> TimerToDocument(CodeTimer timer)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            // writing internals
            timer.ForEach(kvp =>
            {
                dic[kvp.Key] = kvp.Value;
            });
            return dic;
        }

        public void FromSerialziationObject(object o)
        {
            throw new Exception("Process timer can only be serialized and not read.");
        }

        #endregion
    }
}
