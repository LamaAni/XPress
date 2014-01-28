using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPress.Web.Controls
{
    /// <summary>
    /// Represents an rmc page defenition.
    /// </summary>
    public class XPressPage : XPressTemplate
    {
        public XPressPage()
            : base(null)
        {
            this.CanBePage = true;
        }
    }
}
