using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JavascriptClient
{
    /// <summary>
    /// The context stream associated with the client. Saves data in an XPress.Seralization.ObjectStream to be serialziaed to the client side.
    /// The JClientContext preforms partial serialization to allow diffrent values to be deserialzied at diffrent times.
    /// </summary>
    public class JClientContext
    {
        /// <summary>
        /// Construction.
        /// </summary>
        internal JClientContext(JClient client)
        {
            Client = client;
        }

        public JClient Client { get; private set; }
    }
}
