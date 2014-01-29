using XPress.Web.JavascriptClient;
using XPress.Web.JavascriptClient.Request;
using XPress.Web.JavascriptClient.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.Html.Linq;
using XPress.Web.Html;

namespace XPress.Web.Controls.Client
{
    public class ControlsClientCall : JClientCall
    {
        public ControlsClientCall(ControlsClient client, JClientRequest request, JClientResponse response)
            : base(client, request, response)
        {
        }

        /// <summary>
        /// The XPress Client.
        /// </summary>
        public new ControlsClient Client
        {
            get { return base.Client as ControlsClient; }
        }

        public override void ProcessPageRequest(System.Web.HttpContext context, JClientTemplate template)
        {
            base.ProcessPageRequest(context, template);

            // marking all controls that have been rendered as rendered.
            template.Invoke(c =>
            {
                if (c is IRemoteControl)
                    ((IRemoteControl)c).RequiresUpdate = false;
                return true;
            }, true, BubbleDirection.ToChildren);
        }

        public override void PostClientSave(System.Web.HttpContext context, bool pageRequest)
        {
            base.PostClientSave(context, pageRequest);
            if (pageRequest)
                return;

            // check for controls that need updating, and add the updating commands.
            HtmlElement[] allThatNeedUpdate =
                Client.Cache.GetCachedObjects().Where(o => o is IRemoteControl).Cast<IRemoteControl>().Where(rmc => rmc.RequiresUpdate).Cast<HtmlElement>().ToArray();
            
            // checking for updating parents (if any).
            HtmlElement[] requireUpdate = allThatNeedUpdate.Where(c =>
            {
                // checking any parent that has an object id.
                HtmlElement cur = c;
                while (cur != null)
                {
                    if (!cur.HasLoadedParent())
                        return true;
                    cur = cur.Parent;
                    if (cur == null)
                        return true;
                    if (!(cur.Parent is IRemoteControl))
                        continue;
                    if (((IRemoteControl)cur).RequiresUpdate)
                        return false;
                }
                return true;
            }).ToArray();

            // loading all the child controls of the controls that need loading.
            HashSet<Type> loadedTypes=new HashSet<Type>();
            (new HtmlElementQuery(requireUpdate)).Invoke(el =>
            {
                Type t= el.GetType();
                if(!loadedTypes.Contains(t))
                    loadedTypes.Add(t);
                return true;
            }, true, BubbleDirection.ToChildren);

            // Calling to update client type definitions.
            Response.Commands.AddRange(Client.TypeDependentCache.CreateTypeDependentDefinitionCommands(context, loadedTypes, true));

            // Calling the pre render.
            Dictionary<HtmlElement, Html.Rendering.HtmlWriter> ctrlToWriter = new Dictionary<HtmlElement, Html.Rendering.HtmlWriter>();
            requireUpdate.ForEach(c =>
            {
                Html.Rendering.HtmlWriter writer = new Html.Rendering.HtmlWriter(this.Client.JComClient.ObjectSource);
                ctrlToWriter[c] = writer;
                c.PreRender(writer);
            });

            // Creating the response commands to update the client.
            requireUpdate.ForEach(c =>
            {
                Html.Rendering.HtmlWriter writer = ctrlToWriter[c];
                c.Render(writer);
                if (writer.HasInitCommands)
                    this.Response.Commands.AddRange(writer.InitCommands);
                if (c.Attr("id") == null)
                    throw new Exception("Cannot update (Remote control) a control that has no id.");
                this.Response.Commands.Add(
                    new Response.ResponseUpdateCommand(c.Attr("id"), writer.ToString()));
            });

            // marking all rendered as updated.
            allThatNeedUpdate.Cast<IRemoteControl>().ForEach(c => c.RequiresUpdate = false);
            
        }
    }
}
