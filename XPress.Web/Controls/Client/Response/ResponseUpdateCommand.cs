using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Controls.Client.Response
{
    public class ResponseUpdateCommand : Core.XPressResponseCommand
    {
        public ResponseUpdateCommand(string id, string html)
            : base("RMCUpdate", Core.CommandExecutionType.Post)
        {
            Id = id;
            Html = html;
        }

        #region members

        /// <summary>
        /// The html to update the control to. (After control was rendered).
        /// </summary>
        public string Html { get; private set; }

        /// <summary>
        /// The object on the client side of the control.
        /// </summary>
        public string Id { get; private set; }

        #endregion
    }
}
