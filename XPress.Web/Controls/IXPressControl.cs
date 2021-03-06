﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.Links;

namespace XPress.Web.Controls
{
    /// <summary>
    /// Implements an interface for the RemoteControl. Holds linked scripts.
    /// </summary>
    [Links.Attributes.LinkScript("XPress.Web.Controls.XPressControl.js", Links.LinkOrigin.Embedded, LoadType = LinkLoadType.HeadIfPossible, LoadIndex=10)]
    public interface IXPressControl : IXPressContext
    {
        /// <summary>
        /// If true this control needs to be rendered to the client side.
        /// </summary>
        bool RequiresUpdate { get; set; }
    }

    public interface IXPressContext
    {
        /// <summary>
        /// The call context associated with the client.
        /// </summary>
        JavascriptClient.JClientCallContext CallContext { get; }
    }
}
