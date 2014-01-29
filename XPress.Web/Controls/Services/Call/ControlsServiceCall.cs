using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XPress.Web.Controls;
using XPress.Web.Controls.Linq;
using XPress.Web.Controls.Rendering;
using XPress.Web.Controls.Services.Call;

namespace XPress.Web.Controls.Services.Call
{
    /// <summary>
    /// Handels the information and sequence of a service all that creates the response for a specific
    /// page.
    /// </summary>
    public class ControlsServiceCall
    {
        ///// <summary>
        ///// Creates a service call.
        ///// </summary>
        ///// <param name="service">The service associated with the call.</param>
        //public ControlsServiceCall(Services.ControlsService service, XPressClientResponse response, XPressClientRequest request, HttpContext context,  bool isPageInit)
        //{
        //    Service = service;
        //    Response = response;
        //    Request = request;
        //    PendingRequests = new Dictionary<int, XPressClientRequest>();
        //    PendingRequests[request.Id] = request;
        //    LinkCollector = new Web.Links.LinkCollector();
        //    IsPageInit = isPageInit;
        //}

        //#region members

        ///// <summary>
        ///// The call http context.
        ///// </summary>
        //public HttpContext Context { get; private set; }

        ///// <summary>
        ///// The service called.
        ///// </summary>
        //public ControlsService Service { get; private set; }

        ///// <summary>
        ///// The request sent to the service.
        ///// </summary>
        //public XPressClientRequest Request { get; private set; }

        ///// <summary>
        ///// The response the service has.
        ///// </summary>
        //public XPressClientResponse Response { get; private set; }

        ///// <summary>
        ///// The link collector associated with the current call.
        ///// </summary>
        //public XPress.Web.Links.LinkCollector LinkCollector { get; private set; }

        ///// <summary>
        ///// True if the current call is a page init.
        ///// </summary>
        //public bool IsPageInit { get; internal set; }

        ///// <summary>
        ///// A collection of the currently pending requests. (execpt the executing requests).
        ///// </summary>
        //public Dictionary<int, XPressClientRequest> PendingRequests { get; private set; }

        ///// <summary>
        ///// if true then currently executing request.
        ///// </summary>
        //public bool IsExecutingRequest { get; private set; }

        //#endregion

        //#region processing request commands.

        ///// <summary>
        ///// Called to process the remote call to the server.
        ///// </summary>
        //public void ProcessRequest()
        //{
        //    IsExecutingRequest = true;
        //    // the parents that are updating.
        //    IQuery parents;

        //    if (!IsPageInit)
        //    {
        //        // processing all calls from the client.
        //        // getting all the controls that require update.
        //        ExecuteCommands();
        //        Response.Timer["request processing"].Mark("Command exec.");

        //        // loading all updating controls.
        //        Query updating = new Query(Service.Info.MarkedForUpdate.Select(uid => Service.GetControl(uid)));
        //        Response.Timer["request processing"].Mark("Load updating controls");

        //        // invoke the updated by events.
        //        CallChangedByEvents(updating);
        //        Response.Timer["request processing"].Mark("Changed by events");

        //        // Finding all common parents.
        //        // from this point on updating controls will not be added to the range.
        //        parents = FindCommonRoots(updating.ToArray());
        //        Response.Timer["request processing"].Mark("Find common parents");

        //        // clearing updating parents, to allow similar request to be invoked.
        //        Service.Info.MarkedForUpdate.Clear();

        //        // deserializing all the appropriate child nodes to allow for updating.
        //        parents.Invoke((c) => true, true, Events.EventBubbleDirection.ToChildren); // this would propergate through all of the appropriate child nodes. (thus deserializing them).
        //        Response.Timer["request processing"].Mark("Deserialize child nodes.");
        //    }
        //    // calling the load event on the root control element.
        //    else
        //    {
        //        // registering the new service and getting the new service state.
        //        ITemplate template = Service.GetRootControl() as ITemplate;

        //        // calling the template to init.
        //        template.InitTemplate(Context, Service, this);
        //        Response.Timer["request processing"].Mark("Init template");

        //        // marking for update (just for good measure).
        //        template.UpdateClient();

        //        parents = template;
        //        parents.Invoke(this, null, Events.EventDefaults.PageLoad, true, Events.EventBubbleDirection.ToChildren);
        //        Response.Timer["request processing"].Mark("Page load");
        //        Service.Info.MarkedForUpdate.Clear();
        //    }

        //    // Loading all the specific script calles to be loaded onto the client.

        //    // Collecting new scripts if needed.
        //    CollectLinks(parents);
        //    Response.Timer["request processing"].Mark("Links collection");

        //    // collecting the specific type defenitions that will then be added to the client.
        //    CollectTypeDefinitions(parents);
        //    Response.Timer["request processing"].Mark("Types collection");
            
        //    // invoking the pre render event.
        //    parents.Invoke(this, null, Web.Controls.Events.EventDefaults.PreRender, true, Web.Controls.Events.EventBubbleDirection.ToChildren);
        //    Response.Timer["request processing"].Mark("Pre render invoke");

        //    HashSet<ulong> needActivationOnUpdate;
        //    if (!IsPageInit)
        //    {
        //        // adding the update commands to be sent to the client.
        //        needActivationOnUpdate =new HashSet<ulong>();
        //        parents.GetLinqEnumrable().ForEach(p =>
        //        {
        //            ResponseWriter wr = new ResponseWriter();
        //            p.Render(wr);
        //            Response.Invokes.Add(new Commands.ResponseUpdateCommand(p.NodeInfo.UnqiueId, wr));
        //            needActivationOnUpdate.UnionWith(wr.NeedActivationOnUpdate);
        //        });
        //    }
        //    else
        //    {
        //        ITemplate template = parents.GetLinqEnumrable().First() as ITemplate;

        //        // finalizing the template (writing extra data).
        //        template.FinalizeTemplate(Context, Service, this);
        //        Response.Timer["request processing"].Mark("Finalize template");

        //        // writing the page html.
        //        ResponseWriter writer = new ResponseWriter();
        //        template.Render(writer);
        //        Response.PageResponse = writer.InternalWriter.ToString();
        //        Response.Timer["request processing"].Mark("Render the template html");
        //        needActivationOnUpdate = writer.NeedActivationOnUpdate;
        //    }

        //    // registering activation command (js command).
        //    if (needActivationOnUpdate.Count > 0)
        //    {
        //        StringBuilder acCmnd = new StringBuilder();
        //        acCmnd.Append("$$($.FromId([");
        //        acCmnd.Append(string.Join(",", needActivationOnUpdate.Select(i => i.ToString())));
        //        acCmnd.Append("]));");
        //        Response.Invokes.Add(new Commands.ResponseJsCommand(acCmnd.ToString()));
        //    }

        //    // adding other scripts to be executed if nessary.
        //    IsExecutingRequest = false;
        //}


        ///// <summary>
        ///// Returns the common updating roots for the collection of controls.
        ///// </summary>
        ///// <returns></returns>
        //private IQuery FindCommonRoots(IEnumerable<IControl> ctrls)
        //{
        //    // finding all sure roots.
        //    HashSet<IControl> withNoParents = new HashSet<IControl>(ctrls.Where(c => !c.NodeInfo.HasParentDeserialized || c.NodeInfo.Parent == null));

        //    // finding all new possible roots. 
        //    IEnumerable<IControl> withParents = ctrls.Where(c => c.NodeInfo.HasParentDeserialized && c.NodeInfo.Parent!=null).Where(c =>
        //    {
        //        // checing if the parent needs to be updated, if not then this control needs updating.
        //        IControl cur = c.Parent();
        //        while (cur != null)
        //        {
        //            if (withNoParents.Contains(cur))
        //                return false;
        //            if (!cur.NodeInfo.HasParentDeserialized)
        //                return true; // the parent has not been deserilaized and therefore cannot be an updaitng parent. this control needs to be updated.
        //            cur = cur.Parent();
        //        }

        //        return true; // no other updater found.
        //    });

        //    return withNoParents.Concat(withParents).ToQuery();
        //}

        //#endregion

        //#region Command processing

        //void ExecuteCommands()
        //{
        //    this.Request.Commands.ForEach(cmnd =>
        //    {
        //        cmnd.Execute(this);
        //    });
        //}

        //#endregion

        //#region event commands

        //void CallChangedByEvents(IQuery q)
        //{
        //    XPress.Web.Controls.Events.EventInfo e = new Events.EventInfo(Events.EventDefaults.ChangedByEvents);

        //    q.GetLinqEnumrable().ForEach((c) =>
        //    {
        //        if (c.NodeInfo.Changed)
        //        {
        //            c.Events.Trigger(this, null, e);
        //        }
        //    });
        //}

        //#endregion

        //#region collection of client side objects.

        //private void CollectTypeDefinitions(IQuery parents)
        //{
        //    // creating the type identification.
        //    List<Client.XPressClientType> needLoadingToClient = new List<Client.XPressClientType>();
        //    parents.Invoke(c =>
        //    {
        //        Type t = c.GetType();
        //        if (!Service.IsClientTypeLoaded(t))
        //        {
        //            Service.MarkClientTypeAsLoaded(t);
        //            Client.XPressClientType ct = Client.XPressClientType.Get(t);
        //            if (ct.RequiresClientTypeDefenition)
        //            {
        //                needLoadingToClient.Add(ct);
        //            }
        //        }
        //        return true;
        //    }, true, Events.EventBubbleDirection.ToChildren);

        //    // adding the type commands to the response.
        //    needLoadingToClient.ForEach(ct => Response.PreInvokes.Add(new Commands.ResponseTypeDefCommand(ct)));
        //}

        //void CollectLinks(IQuery q)
        //{
        //    q.Invoke((c) =>
        //    {
        //        // collecting the associated links.
        //        LinkCollector.LinkControl(c);
        //        return true;
        //    }, true, Events.EventBubbleDirection.ToChildren);

        //    IEnumerable<string> needLinkUpdating = this.LinkCollector.Links
        //        .Where(l => !Service.Info.LoadedLinks.Contains(l) && XPress.Web.Links.Bank.LinkBank.Global.Contains(l))
        //        .ToArray();

        //    this.Service.Info.LoadedLinks.UnionWith(needLinkUpdating); // updated the links collection.
        //    // creating links.
        //    // adding the non script links to the response.
        //    if (needLinkUpdating.Count() > 0)
        //    {
        //        IEnumerable<XPress.Web.Links.Bank.LinkInfo> links = needLinkUpdating.Select(l =>XPress.Web.Links.Bank.LinkBank.Global.GetLinkInfo(l))
        //            .Where(l => !IsPageInit || (l.Link.LoadType != Web.Links.LinkLoadType.Head && l.Link.RequriesValidationBeforeCall))
        //            .ToArray();

        //        // loading css links.
        //        links.Where(l => !l.Link.RequriesValidationBeforeCall).ForEach((l) =>
        //        {
        //            // creating the link command.
        //            Response.PreInvokes.Add(new Commands.ResponseLinkCommand(l, Context));
        //        });

        //        links.Where(l => l.Link.RequriesValidationBeforeCall).ForEach(l =>
        //        {
        //            Response.PendInvokes.Add(new Commands.ResponseLinkCommand(l, Context));
        //        });
        //    }

        //}

        //#endregion
    }
}
