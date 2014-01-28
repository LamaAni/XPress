using XPress.Web.JavascriptClient.Request;
using XPress.Web.JavascriptClient.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XPress.Web.JavascriptClient
{
    /// <summary>
    /// The active call object that handles communication between the client and server.
    /// </summary>
    public class JClientCall
    {
        /// <summary>
        /// Creates a new jclient call processing scenario.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public JClientCall(JClient client, JClientRequest request, JClientResponse response)
        {
            Client = client;
            Request = request;
            Response = response;
        }

        /// <summary>
        /// The client attached to the call.
        /// </summary>
        public JClient Client { get; private set; }

        /// <summary>
        /// The request sent to the service.
        /// </summary>
        public JClientRequest Request { get; private set; }

        /// <summary>
        /// The response the service has.
        /// </summary>
        public JClientResponse Response { get; private set; }

        /// <summary>
        /// Process the request from the client.
        /// </summary>
        public virtual void ProcessJsonRequest(HttpContext context)
        {
            // Executing commands.
            Request.Commands.ForEach(cmnd =>
            {
                cmnd.ExecuteCommand();
            });

            // Searching commnads that have a specific responce value and returning the responce values as a command to the client side.
            Request.Commands.Where(rq => rq.ResponseValue != null).ForEach(rq =>
            {
                Response.ResponseValues[rq.CommandId] = rq.ResponseValue;
            });
        }

        /// <summary>
        /// Called after the client object has been saved.
        /// </summary>
        /// <param name="context">The http context</param>
        /// <param name="pageRequest">If true this is a page request. Otherwise this is a json request.</param>
        public virtual void PostClientSave(HttpContext context, bool pageRequest)
        {
        }


        /// <summary>
        /// Processes the request as a json page request, and initializes the template.
        /// </summary>
        /// <param name="template">The template (page)</param>
        public virtual void ProcessPageRequest(HttpContext context, JClientTemplate template)
        {
            // call to process the page.
            template.ProcessHtmlPageRequest(context);
        }
    }
}
