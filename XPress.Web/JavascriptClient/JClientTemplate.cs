using XPress.Web.Links.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization;
using XPress.Strings;
using XPress.Serialization.Attributes;
using System.Web;
using XPress.Web.JavascriptClient.Response;
using XPress.Web.Razor;
using XPress.Web.Razor.Storage;

namespace XPress.Web.JavascriptClient
{
    /// <summary>
    /// Represents a html template that contains a jclient. The template handles the communication between the client and the server.
    /// </summary>
    [LinkScript("XPress.Web.JavascriptClient.Client.js", Links.LinkOrigin.Embedded, LoadType = Links.LinkLoadType.HeadIfPossible, LoadIndex = 10)]
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public class JClientTemplate : Html.HtmlTemplate
    {
        /// <summary>
        /// Constructor for JClientTemplate.
        /// </summary>
        public JClientTemplate(string tagName = null)
            : base(tagName)
        {
        }

        /// <summary>
        /// Static constructor for JClientTemplate.
        /// </summary>
        ~JClientTemplate()
        {
        }

        static JClientTemplate()
        {
            // adding command to handle errored json requests.
            Razor.XPressRazorHttpModule.ExecuteOnError.Add((errs) =>
            {
                if (HttpContext.Current.Request.GetXPressRazorRequestType() == JClientRequestType.Json && JClientCallContext.Current != null)
                {
                    // clearing the executing request id (if any).
                    JClientCallContext.Current.State.ExecutingRequestId = -1;

                    //if (JClientState.Current.IsInWaitForResponseMode)
                    JClientCallContext.Current.State.WaitingResponse = errs.ToJsonErrorString();

                    // store the client. (since it was loaded).
                    JClientCallContext.Current.Client.Store(Bank);

                    // store back the state.
                    JClientCallContext.Current.State.Store(Bank);

                    // clears the current state.
                    JClientCallContext.ClearCurrentContext();
                }
            });
        }

        #region members

        /// <summary>
        /// The client associated with the page.
        /// </summary>
        public JClient Client { get { return JClientCallContext.Current.Client; } }

        /// <summary>
        /// The current jclient call.
        /// </summary>
        public JClientCall Call { get { return JClientCallContext.Current.Call; } }

        #endregion

        #region storage

        [ThreadStatic]
        static XPressRazorCacheBanks m_Bank;

        /// <summary>
        /// The storage bank associated with the current thread. This storage bank handles loading and saving of the client state to allow handeling of server commands.
        /// [ThreadStatic]
        /// </summary>
        public static XPressRazorCacheBanks Bank
        {
            get
            {
                if (m_Bank == null)
                    return XPressRazorCacheBanks.Global;
                return m_Bank;
            }
            set
            {
                m_Bank = value;
            }
        }

        #endregion

        #region Client

        /// <summary>
        /// Creates the client that will be associated with the pate. Called when the page initializes.
        /// </summary>
        /// <returns></returns>
        public virtual JClient CreateClient()
        {
            return new JClient();
        }

        protected override JCom.Com.IJComObjectSource CreateObjectSource()
        {
            return Client.JComClient.ObjectSource;
        }

        #endregion

        #region Request processing

        /// <summary>
        /// Request processing for the JClient template page.
        /// </summary>
        /// <param name="context"></param>
        public override void ProcessRequest(System.Web.HttpContext context)
        {
            // working by request
            switch (context.Request.GetXPressRazorRequestType())
            {
                case JClientRequestType.Beat:
                    ProcessRequestAsBeat(context);
                    break;
                case JClientRequestType.Json:
                    ProcessRequestAsJson(context);
                    break;
                case JClientRequestType.WaitForResponse:
                    ProcessRequestAsWaitingForResponse(context);
                    break;
                case JClientRequestType.DestroyClient:
                    ProcessRequestAsDestroyClient(context);
                    break;
                default: ProcessRequestAsPage(context);
                    break;
            }
        }

        /// <summary>
        /// Called from the client to destroy the JClient on the server side since the page has been destroyed (or others...).
        /// </summary>
        /// <param name="context">The http context associated witht the request</param>
        protected virtual void ProcessRequestAsDestroyClient(System.Web.HttpContext context)
        {
            JClientState state;
            if (TryGetClientState(context, out state))
            {
                JClientState.DestroyClient(state, Bank);
            }
            context.Response.Clear();
            context.Response.End();
        }

        /// <summary>
        /// Tries to get the client state and if fails tries to delete the client state.
        /// </summary>
        /// <param name="clientStateId">The id of the client state.</param>
        /// <param name="state">The client state</param>
        /// <returns>true if succeeded</returns>
        public static bool TryGetClientState(HttpContext context, out JClientState state)
        {
            state = JClientState.LoadClientState(context.Request.GetJClientIdFromRequestHeders(), Bank);
            if (state == null || !state.IsAlive)
            {
                // state was not found, client disconnected from server.
                if (state != null)
                    JClientState.DestroyClient(state, Bank);
                return false;
            }
            return true;
        }

        /// <summary>
        /// processes the request as a waiting for response command sent from the client.
        /// This request happens when a command bach on the client side is timed out and now the client is waiting for the server to respond.
        /// The server may respond with commands or a resend command in case there was a communication error on the clientside, this
        /// would depend on the client side options.
        /// </summary>
        protected void ProcessRequestAsWaitingForResponse(HttpContext context)
        {
            JClientState state;
            string response = "Disconnect";
            if (TryGetClientState(context, out state))
            {
                // checking the current state of the request and the last request loaded.
                int rqid = Convert.ToInt32(context.Request.ReadAsString()); // the request id.
                if(state.LastRequestId==rqid)
                {
                    // first of all.
                    state.IsInWaitForResponseMode = true;
                    if(state.ExecutingRequestId==rqid)
                    {
                        // still executing need to delay and resned.
                        response = "IsExecuting";
                    }
                    else
                    {
                        // need to return the response.
                        response = "Completed:" + state.WaitingResponse;
                        state.IsInWaitForResponseMode = false;
                        state.WaitingResponse = null;
                    }
                }
                else
                {
                    // request has not reached the server or is obsolete.
                    response = state.LastRequestId > rqid ? "Obsolete" : "Resend";
                }

                // marking the hartbeat and storing the state.
                state.MarkHartbeat();
                state.Store(Bank);
            }

            context.Response.Clear();
            context.Response.Write(response);
        }

        /// <summary>
        /// Processes a hartbeat command from the client.
        /// </summary>
        /// <param name="context">The http context</param>
        protected void ProcessRequestAsBeat(System.Web.HttpContext context)
        {
            JClientState state;
            if (TryGetClientState(context, out state))
            {
                // marking the last hartbeat.
                context.Response.Write(state.HasPendingResponses ? Response.BeatCommandEnum.Changed.ToString() : Response.BeatCommandEnum.OK.ToString());
                state.MarkHartbeat();
                state.Store(Bank);
            }
            else context.Response.Write(Response.BeatCommandEnum.Dead.ToString());
        }

        /// <summary>
        /// Called to process the request as a page request.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void ProcessRequestAsPage(System.Web.HttpContext context)
        {
            // REquest is an emptuy object since a json command was not sent.
            Request.JClientRequest request = new Request.JClientRequest();

            // Responce (JSON).
            Response.JClientResponse response = new Response.JClientResponse();

            // Clering the states if any (there might be states associated with a previus call on this thread).
            JClientCallContext.ClearCurrentContext();

            // setting the storage bank for the current thread. (TODO: add bank handling here).
            JClientTemplate.Bank = XPressRazorCacheBanks.Global;

            // creating the client. (Also sets the JClient.Current and JClientState.Current, and stores the clients to the db).
            JClient client = JClientState.InitClient(this, Bank);
            JClientState state = client.State;

            response.Timer.Mark("Client and generation (with storage)");

            // marks the current jclient state.
            state.MarkHartbeat();

            JClientCall call = client.CreateClientCall(request, response);
            JClientCallContext callContext = new JClientCallContext(client, state, call);
            callContext.ApplyToRunningThread();

            response.Timer.Mark("Client call creation");

            // since the current response dose not differ from the basic html page,
            // there is no need to change anything, just return the client and set the options for this client.
            call.ProcessPageRequest(context, this);

            response.Timer.Mark("Page request processing");

            // Stores the client after request was made. (This is the actual client state).
            client.Store(Bank);
            response.Timer.Mark("Store client");

            // call the post client save.
            call.PostClientSave(context, false);
            response.Timer.Mark("Post client save");

            // marking the last hartbeat.
            state.MarkHartbeat();

            // updating the state.
            state.Store(Bank);

            response.Timer.Mark("Restoring the ClientState.");
        }

        /// <summary>
        /// Called to process the html page request
        /// </summary>
        internal void ProcessHtmlPageRequest(HttpContext context)
        {
            base.ProcessRequest(context);
        }

        /// <summary>
        /// Called to process the request as a json command, and respond accordingly.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void ProcessRequestAsJson(System.Web.HttpContext context)
        {
            // the response object to the jclient request.
            JClientResponse response = new JClientResponse();
            response.Timer.Start();

            // Loading the state.
            JClientState state;
            if (TryGetClientState(context, out state))
            {
                response.Timer.Mark("Read client state object");
                // loading the request.
                Request.JClientRequest request = context.Request.ReadRequestFromStream();
                response.Timer.Mark("Read request from stream");

                // Loading the client and executing commands.
                
                JClient client = JClientState.LoadClient(state, Bank, true);
                response.Timer.Mark("Loaded the client");
                // create the client call the processes the commnads.
                JClientCall call = client.CreateClientCall(request, response);

                // creating and applying the call context.
                JClientCallContext callContext = new JClientCallContext(client, state, call);
                callContext.ApplyToRunningThread();

                // the state is ok, and should be maked as accessed.
                state.MarkHartbeat();

                // for buffered requests.
                if (request.IsBufferedRequest)
                {
                    // set the currently executing request.
                    state.ExecutingRequestId = request.Id;
                    state.LastRequestId = request.Id;
                }

                // store the state before processing the call.
                state.Store(Bank);
                response.Timer.Mark("Prepare process request");

                // call to process the request.
                call.ProcessJsonRequest(context);
                response.Timer.Mark("Process request");

                // Stores the client after request was made. (This is the actual client state).
                client.Store(Bank);
                response.Timer.Mark("Store client");

                // call to process further commands after the client has been saved.
                call.PostClientSave(context, false);
                response.Timer.Mark("Post client save");

                if (request.IsBufferedRequest)
                {
                    state.ExecutingRequestId = -1;
                }

                response.Timer.Mark("JSON Request processing total", true);

                // if the state has to wait for response mode then we need to render the response to the state.
                if (request.IsBufferedRequest && state.IsInWaitForResponseMode)
                {
                    state.WaitingResponse = response.RenderResponse();
                }
                else // render the response to the client.
                {
                    context.Response.Clear();
                    context.Response.Write(response.RenderResponse());
                }

                // mark the last time the request has been loaded and store the state.
                state.MarkHartbeat();
                state.Store(Bank);

                // clearing the call context.
                JClientCallContext.ClearCurrentContext();
            }
            else
            {
                // writing error response
                response.SystemCommands.Add(new Response.JClientSystemCommand(Response.JClientSystemResponseCommandEnum.Disconnect));
                response.Timer.Mark("JSON Request processing compleated");
                context.Response.Clear();
                context.Response.Write(response.RenderResponse());
            }
        }

        /// <summary>
        /// overrides the creation of the initalization script.
        /// </summary>
        /// <param name="initBuilder"></param>
        protected override void CreateInitScript(StringBuilder initBuilder)
        {

            // creating the initialise client command.
            initBuilder.Append("var dat=$.JSON.From(\"" + new
            {
                options = this.Client.Options,
                ClientId = this.Client.State.Id, // send the state id since the state is the most basic object to obtain in any request.
            }.ToJSJson().EscapeForJS() + "\");$.XPress.InitJClient(dat.ClientId,dat.options);");
#if DEBUG
            initBuilder.Append("\n");
#endif
            // creating the basis for the init script.
            base.CreateInitScript(initBuilder);

            // adding responce commands (current, to the page execution scheme).
            initBuilder.Append("$.XPress.GlobalJClient.ReciveCommand(\"" + Call.Response.RenderResponse().EscapeForJS() + "\");");
#if DEBUG
            initBuilder.Append("\n");
#endif
        }

        #endregion
    }
}
