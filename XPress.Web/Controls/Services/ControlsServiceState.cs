using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Controls.Services
{
    /// <summary>
    /// Holds information about the sepecific service state
    /// </summary>
    public class ControlsServiceState
    {
        public ControlsServiceState(string pageId)
        {
            PageId = pageId;
        }

        #region static members

        [ThreadStatic]
        static ControlsServiceState m_sCurrent;

        /// <summary>
        /// The current executing service state that is attached to the  running thread.
        /// </summary>
        public static ControlsServiceState Current
        {
            get { return ControlsServiceState.m_sCurrent; }
            set { ControlsServiceState.m_sCurrent = value; }
        }

        /// <summary>
        /// The time to keep the service alive when no contact from the client is made. This parameter has an effect on memory.
        /// </summary>
        public static TimeSpan ServiceNoContactKeepAliveTime = new TimeSpan(0, 5, 0);

        #endregion

        #region members

        /// <summary>
        /// The serice id.
        /// </summary>
        public string PageId { get; private set; }

        /// <summary>
        /// If the current requires an update.
        /// </summary>
        public bool RequiresUpdate { get; internal set; }

        /// <summary>
        /// The time this service was last accessed. Determines also the time when this service becomes obsolete. Depending on the definiton
        /// of the service max no contact time (default is 5 minutes).
        /// </summary>
        public DateTime LastAccessed { get; internal set; }

        /// <summary>
        /// returns true if the current service is still alive.
        /// </summary>
        public bool IsAlive { get { return (DateTime.Now - LastAccessed) <= ServiceNoContactKeepAliveTime; } }

        #endregion
    }
}
