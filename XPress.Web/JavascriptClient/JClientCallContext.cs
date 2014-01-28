using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JavascriptClient
{
    /// <summary>
    /// Holds information abount the current client call context
    /// </summary>
    public class JClientCallContext
    {
        /// <summary>
        /// Creates a jclient call context.
        /// </summary>
        /// <param name="client">The client</param>
        /// <param name="call">The call</param>
        public JClientCallContext(JClient client, JClientState state, JClientCall call)
        {
            State = state;
            Client = client;
            Call = call;
        }

        #region members

        /// <summary>
        /// The jclient.
        /// </summary>
        public JClient Client { get; private set; }

        /// <summary>
        /// The jclient state.
        /// </summary>
        public JClientState State { get; private set; }

        /// <summary>
        /// The jclient call.
        /// </summary>
        public JClientCall Call { get; private set; }

        #endregion

        #region static members

        [ThreadStatic]
        static JClientCallContext m_Current;

        /// <summary>
        /// The JClientCallContext that applies to this thread.
        /// [ThreadStatic]
        /// </summary>
        public static JClientCallContext Current { get { return m_Current; } internal set { m_Current = value; } }

        #endregion

        #region methods

        /// <summary>
        /// Makes the this object the current context for the current thread (JClientCallContext.Current).
        /// </summary>
        public void ApplyToRunningThread()
        {
            JClientCallContext.Current = this;
        }

        #endregion

        #region static methods

        public static void ClearCurrentContext()
        {
            JClientCallContext.Current = null;
        }

        #endregion
    }
}
