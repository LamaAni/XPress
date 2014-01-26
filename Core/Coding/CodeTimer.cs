using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace XPress.Coding
{
    public class CodeTimer : Collections.OrderedDictionary<string,TimeSpan>
    {
        public CodeTimer()
        {
            Start();
        }

        DateTime? m_StartTime;

        public DateTime? StartTime
        {
            get { return m_StartTime; }
        }

        DateTime? m_EndTime;

        public DateTime? EndTime
        {
            get { return m_EndTime; }
        }

        DateTime m_lastMark;

        public DateTime LastMark
        {
            get { return m_lastMark; }
        }

        public TimeSpan FromLastMark
        {
            get { return DateTime.Now - LastMark; }
        }

        public TimeSpan TotalTime
        {
            get
            {
                ValidateStartTime();
                return (m_EndTime == null ? DateTime.Now : m_EndTime.Value) - m_StartTime.Value;
            }
        }

        public bool IsActive { get { return m_StartTime != null; } }

        void ValidateStartTime()
        {
            if (m_StartTime == null)
                throw new Exception("The timer must be started first. Use function .Start().");
        }

        protected virtual void StopTimer()
        {
            m_StartTime = null;
        }

        public virtual void Start()
        {
            m_StartTime = DateTime.Now;
            m_lastMark = DateTime.Now;
            m_EndTime = null;
        }

        public virtual void Reset()
        {
            Reset(true);
        }

        public virtual void Reset(bool start)
        {
            this.Clear();
            m_EndTime = null;
            m_StartTime = null;
            m_lastMark = DateTime.Now;
            if (start)
                Start();
        }

        public virtual void Stop()
        {
            m_EndTime = DateTime.Now;
        }

        public TimeSpan Mark(string key)
        {
            return Mark(key, false);
        }

        public TimeSpan Mark(string key, bool fromStart)
        {
            ValidateStartTime();
            if (fromStart)
            {
                this[key] = this.TotalTime;
            }
            else
            {
                this[key] = (DateTime.Now - m_lastMark);
                m_lastMark = DateTime.Now;
            }

            return this[key];
        }

        public TimeSpan Mark()
        {
            return Mark("now");
        }

        public string ToTraceCommand(string title)
        {
            StringWriter wr = new StringWriter();
            wr.Write("TraceDetails(unescape('" + Microsoft.JScript.GlobalObject.escape(title) + "')+");
            wr.Write("'completed in " + this.TotalTime.TotalMilliseconds.ToString("#0.0") + " [ms]'");
            wr.Write(",'");
            ToTraceHtml(wr, null);
            return wr.ToStringAndDispose();
        }

        public string ToTraceHtml(string title)
        {
            StringWriter wr = new StringWriter();
            ToTraceHtml(wr, title);
            return wr.ToStringAndDispose();
        }

        public void ToTraceHtml(StringWriter wr, string title)
        {
            if (title != null)
                wr.WriteLine("title");
            bool isFirst = true;
            foreach (string mark in this.Keys)
            {
                if (!isFirst)
                {
                    wr.Write("<br>");
                }
                else isFirst = false;
                wr.Write("&nbsp;&nbsp" + mark + " : " + this[mark].TotalMilliseconds.ToString("#0.0") + " [ms]");
            }
            wr.Write("');");
        }

        public string ToTraceString(string title)
        {
            StringWriter wr = new StringWriter();
            if (title != null)
                wr.WriteLine(title);
            foreach (string mark in this.Keys)
            {
                wr.WriteLine("  " + mark + " : " + this[mark].TotalMilliseconds.ToString("#0.0") + " [ms]");
            }
            return wr.ToStringAndDispose();
        }

        public IEnumerable<KeyValuePair<string, TimeSpan>> GetOrderedByTime()
        {
            return this.OrderBy(kvp => -kvp.Value.Ticks);
        }
    }
}
