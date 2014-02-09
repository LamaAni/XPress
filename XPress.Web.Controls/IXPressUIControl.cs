using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Controls
{
    /// <summary>
    /// Implements a marker interface to allow all the xpress ui implementations.
    /// </summary>
    [XPress.Web.Links.Attributes.LinkScript("XPress.Web.Controls.Core.GUI.js", Links.LinkOrigin.Embedded)]
    public interface IXPressUIControl : IXPressControl
    {
    }
}
