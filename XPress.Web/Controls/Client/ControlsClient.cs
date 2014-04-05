using XPress.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Controls.Client
{
    /// <summary>
    /// Creates an rmc client that has properties to manage remote controls.
    /// </summary>
    [XPressMemberSelection( XPressMemberSelectionType.OptIn)]
    public class ControlsClient : JavascriptClient.JClient
    {
        public ControlsClient()
            : base()
        {
            this.ClientRegisterMessage = "Controls client registered OK!";
        }

        [XPressMember("TypeDependentDefitions")]
        Html.Collections.TypeDependentCacheCollection m_typeDependentCache;

        /// <summary>
        /// A collection of type dependent cache members. Handles client side cache creation.
        /// </summary>
        public Html.Collections.TypeDependentCacheCollection TypeDependentCache
        {
            get
            {
                if (m_typeDependentCache == null)
                    m_typeDependentCache = new Html.Collections.TypeDependentCacheCollection();
                return m_typeDependentCache;
            }
        }

        public override JavascriptClient.JClientCall CreateClientCall(JavascriptClient.Request.JClientRequest request, JavascriptClient.Response.JClientResponse response)
        {
            return new ControlsClientCall(this, request, response);
        }
    }
}
