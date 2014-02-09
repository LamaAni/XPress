using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Attributes;
using XPress.Serialization;
using XPress.Web.JavascriptClient.Response;
using XPress.Web.Core;
using XPress.Coding;

namespace XPress.Web.JavascriptClient.Response
{
    /// <summary>
    /// Implements information for a specific service response.
    /// </summary>
    [XPress.Serialization.Attributes.XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public class JClientResponse
    {
        /// <summary>
        /// Builds a response command for the jclient.
        /// </summary>
        public JClientResponse()
        {
        }

        #region command collections to be executed on the client.


        [XPressMember("ResponseVals")]
        Dictionary<string, object> m_responseValues;

        /// <summary>
        /// Responce values to specific requests.
        /// </summary>
        public Dictionary<string, object> ResponseValues
        {
            get
            {
                if (m_responseValues == null)
                    m_responseValues = new Dictionary<string, object>();
                return m_responseValues;
            }
        }

        /// <summary>
        /// The object to respond with for the case of a synced response.
        /// </summary>
        [XPressMember("SyncedResponse")]
        [XPressIgnore(XPressIgnoreMode.IfNull)]
        public object SyncedResponse { get; internal set; }

        [XPressMember("Commands")]
        [XPressIgnore(XPressIgnoreMode.IfNull)]
        List<XPress.Web.Core.XPressResponseCommand> m_commands;

        /// <summary>
        /// A commands collection to be executed after all pending validations (added by the pre invoked) are met.
        /// </summary>
        [XPressIgnore]
        public List<XPressResponseCommand> Commands
        {
            get
            {
                if (m_commands == null)
                    m_commands = new List<XPressResponseCommand>();
                return m_commands;
            }
        }

        [XPressIgnore]
        Dictionary<string, XPressResponseCommand> m_CommandsByUniqueId = new Dictionary<string, XPressResponseCommand>();

        /// <summary>
        /// A collection of responce commands by name, that need to be added to the collection of responces.
        /// </summary>
        [XPressIgnore]
        public Dictionary<string, XPressResponseCommand> CommandsByUniqueId
        {
            get { return m_CommandsByUniqueId; }
        }

        [XPressMember("sys")]
        [XPressIgnore(XPressIgnoreMode.IfNull)]
        List<XPressResponseCommand> m_systemCommands;

        /// <summary>
        /// A commands collection to be executed after all pending validations (added by the pre invoked) are met.
        /// </summary>
        [XPressIgnore]
        internal List<XPressResponseCommand> SystemCommands
        {
            get
            {
                if (m_systemCommands == null)
                    m_systemCommands = new List<XPressResponseCommand>();
                return m_systemCommands;
            }
        }

        #endregion

        #region trace info collection.

        StringWriter m_trace = new StringWriter();

        /// <summary>
        /// Writes commands to the trace to be printed at the client console.
        /// </summary>
        public StringWriter Trace
        {
            get { return m_trace; }
        }

        /// <summary>
        /// Private trace funtion to allow the trace command to be sent to the client.
        /// </summary>
        [XPress.Serialization.Attributes.XPressMember("Trace")]
        private string TraceWriter { get { return m_trace.ToString(); } }

        [XPressMember("Timer", IgnoreMode = XPressIgnoreMode.IfNull)]
        ProcessTimer m_timer;

        /// <summary>
        /// The timer to allow timing of requests on client side. Minimal interval 1 ms.
        /// </summary>
        public ProcessTimer Timer
        {
            get
            {
                if (m_timer == null)
                    m_timer = new ProcessTimer();
                return m_timer;
            }
        }

        #endregion

        #region response writing

        /// <summary>
        /// Renders the response and returns this response as string, depending of the response type required.
        /// </summary>
        /// <returns></returns>
        public string RenderResponse()
        {
            // preparing the commands by name collection.
            this.Commands.AddRange(CommandsByUniqueId.Values);
            CommandsByUniqueId.Clear();
            return this.ToJSJson();
        }

        #endregion
    }
}
