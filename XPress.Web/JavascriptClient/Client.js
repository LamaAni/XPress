// client extentions for jquery.
// Implements an rmc client for the remote controls system.
// adding the extnetion object for the remote control clients. Applies to all controls.
$.extend($.fn.XPress, {
    // commands to the rmc. (All controls).
    // the element associated with the object.
    element: this,
    Send: function (f) { this.element.bind($.XPress.EventIds.Send, f); },
    Recive: function (f) { this.element.bind($.XPress.EventIds.Recive, f); },
    RequestStart: function (f) { this.element.bind($.XPress.EventIds.RequestStart, f); },
    RequestComplete: function (f) { this.element.bind($.XPress.EventIds.RequestComplete, f); },
    StateChanged: function (f) { this.element.bind($.XPress.EventIds.StateChanged, f); },
    RequestBufferEmpty: function (f) { this.element.bind($.XPress.EventIds.RequestBufferEmpty, f); },
    ServiceNotFound: function (f) { this.element.bind($.XPress.EventIds.ServiceNotFound, f); },
    LinkComplete: function (f) { this.element.bind($.XPress.EventIds.LinkComplete, f); },
    Init: function (f) { this.element.bind($.XPress.EventIds.Init, f); },
    ServiceDisconnect: function (f) { this.element.bind($.XPress.EventIds.ServiceDisconnect, f); },

    // client identification.
    GetClient: function () {
        // TODO:Add multiple clients
        // Thise dose depend on the client handling?? in the serverside. But currently not supported.
        return $.XPress.GlobalJClient;
    },
});

$.extend($.XPress.EventIds, {
    Send: 'Client_Start',
    Recive: 'Client_RequestStart',
    RequestComplete: 'Client_RequestComplete',
    RequestStart: 'Client_RequestStart',
    StateChanged: 'Client_StateChanged',
    RequestBufferEmpty: 'Client_RequestBufferEmpty',
    ServiceNotFound: 'Client_NotFound',
    ServiceDisconnect: 'Client_Disconnect',
    Init: 'Client_Init'
});

// Global rmc object.
$.extend($.XPress, {
    // Client creation (for page purpuse only)
    InitJClient: function (id, options) {
        this.GlobalJClient = $.XPress.JClient(id, options);
        return this.GlobalJClient;
    },

    // the global client.
    GlobalJClient: null,
});

//#region XPress client defenition.
$.extend($.XPress, {
    // A command client for sending and reciving commands from the server.
    JClient: function (id, options, events) {
        // validating parameters.
        if (options == null)
            options = {};
        else options = typeof (options) == "string" ? $.JSON.From(options) : options;

        if (events == null)
            events = $.EventCollection();

        var client = {
            //////////////////////////////////////////////////////////////
            // Initialization
            $: function (id, options) {
                this.$ = null; //constructor can only be called once.
                this.ClientId = id;
                this.Options = $.extend(this.Options, options); // merging the non default.
                // registering events for initialization.
                var me = this;
                $(window).bind("beforeunload", function (ev) {
                    me.CloseClient();
                });

                this.RegisterClient();
                console.log("JClient initialzied.");

                this.LastClientSuccessfulCommandTime = new Date().getTime();
                this.LastClientSuccessfulConnectionTime = new Date().getTime();
            },

            /////////////////////////////////////////////////////////////
            /// members
            ClientId: null,

            // Options collection.
            Options: {
                // default options for the remote client.
                // the service url. Usualy points to the current page.
                Url: [location.protocol, '//', location.host, location.pathname].join(''),
                // The request timeout for requests sent to the serever.
                RequestTimeout: 2 * 1000,
                // The request timeout for a beat command.
                BeatTimeout:1*1000,
                // if true there is a beat timer
                HasBeat: false,
                // the maximal number of attempts to be sent.
                MaxAttempts: 100,
                // the maximal time when the client can be not connected to the server.
                MaxClientNotConnectedTime: 10 * 1000 * 100,
                // The fast hartbeat timespan after a successful command to server.
                WaitBeforeMoveToNormalHartBeat: 5 * 1000,
                // Time interval for normal hartbeat - when the window is in focus and active.
                NormalHartBeatInterval: 30 * 1000,
                // Time interval for fast hartbeat.
                FastHartBeatInterval: 1 * 1000,
            },

            //////////////////////////////////////////////
            //// MEMBERS

            // the last time the client was successfuly connected to the server.
            LastClientSuccessfulConnectionTime: 0,
            LastClientSuccessfulCommandTime: 0,

            // Marks the action time.
            MarkTime: function (cmnd, comm) {
                if (cmnd)
                    this.LastClientSuccessfulCommandTime = new Date().getTime();
                if (comm)
                    this.LastClientSuccessfulConnectionTime = new Date().getTime();
            },
            
            // Returns the valid delay time for the current context.
            GetBeatDelayTime:function(){
                if (this.LastClientSuccessfulCommandTime + this.Options.WaitBeforeMoveToNormalHartBeat > new Date().getTime())
                    return this.Options.FastHartBeatInterval;
                return this.Options.NormalHartBeatInterval;
            },

            // the time to wait for a new command buffer before sending the current command.
            WaitForCommandsTime: 100,

            // the event collection associated with the current.
            Events: events,

            // the command buffer where the commands are loaded.
            Buffer: {
                buffer: new Array(),
                LastPosted: new Date(),
                // push a new command to the bottom of the array. (or to the location of the index).
                Post: function (cmnd, idx) {
                    // validate the index.
                    if (idx != null && idx < 0 || idx > this.buffer.length)
                        idx = null;

                    // adding to buffer.
                    if (idx == null || idx < 0 || idx <= this.buffer.length)
                        this.buffer.push(cmnd);
                    else this.buffer.splice(idx, 0, cmnd);

                    // updating the last posted.
                    this.LastPosted = new Date();
                },
                // empties the buffer and returns the command list.
                FlushCommandBuffer: function () { var ar = this.buffer; this.buffer = new Array(); return ar; },
                // if true then the buffer has commands that are waiting to be sent to server.
                HasWaitingCommands: function () { return this.buffer.length > 0; },
            },

            HaltCommunication: false,

            // the last loaded buffer context.
            LastBufferContext: null,

            // true if currently sending.
            IsSending: function () { return this.LastBufferContext == null ? false : true; },

            __requestId: 0,
            // returns the next request id.
            NextRequestId: function () { var id = this.__requestId; this.__requestId += 1; return id; },


            // clled when the state of the object is changed.
            StateChanged: function () {
                this.Events.trigger($.XPress.EventIds.StateChanged);
            },

            //////////////////////////////////////////////
            //// Methods

            // ALL FIELDS MUST BE FILLD
            // creates a new pure ajax command
            CreateAjaxCommand: function (strdata, url, timeout, context, hasSession, cmndType, async) {
                var cmnd = {
                    url: url,
                    type: "POST",
                    timeout: timeout,
                    dataType: "text",
                    cache: false,
                    context: context,
                    data: strdata,
                    contentType: "text/html;charset=UTF-8",
                    async: async,
                    HasSession: hasSession,
                    CommandType: cmndType,
                    ClientId: this.ClientId,
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("jClientCmd", this.Command.CommandType);
                        xhr.setRequestHeader("jclientid", this.Command.ClientId);
                        var rmcHeader = "RespondAsJson";
                        if (!this.Command.HasSession || this.Command.CommandType == "jsonWaitForResponse" || this.Command.CommandType == "beat")
                            rmcHeader += ";NoSession";
                        xhr.setRequestHeader("_rmcrazor", rmcHeader);
                    },
                };
                cmnd.context.Command = cmnd;
                return cmnd;
            },

            // Creates a new command in the ajax form to be sent to the server.
            CreateRequestAjaxCommand: function (rq, async, cmndType) {
                return this.CreateAjaxCommand(
                    $.JSON.To(rq),
                    rq.Url,
                    _as(this.Options.RequestTimeout),
                    { Client: this, Request: rq },
                    true,
                    cmndType,
                    async);
            },

            __commandIdIndex: 0,
            // returns the next command id, to allow enumerating the id's responces.
            GetNextCommandId: function () {
                var id = this.__commandIdIndex;
                this.__commandIdIndex += 1;
                return id;
            },

            // creates a client command to be sent to the server, from the commands in the buffer.
            CreateRequestCommand: function (buffer) {
                // make sure its an array.
                buffer = [].concat(buffer);
                // go through the buffer and add the command index. 
                // this command index will allow the command to return a spcific responce for each of the commands.
                for (var i = 0; i < buffer.length; i++) {
                    buffer[i]._cmndid = this.GetNextCommandId();
                }
                return {
                    Commands: buffer,
                    TimeStamp: new Date(),
                    Id: this.NextRequestId(),
                    Url: _as(this.Options.Url),
                    Attempt: 0,
                };
            },

            __sendBufferIntervalIndex: null,

            // if true the command invoke interval is active and waiting to send the buffer.
            HasBufferInvokeInterval: function () { return this.__sendBufferIntervalIndex != null; },

            // tries to start the command interval for sending the buffer.
            BeginSendBufferInterval: function () {
                if (!this.Buffer.HasWaitingCommands())
                    return false; // no need to continue, no commands present.
                if (this.HasBufferInvokeInterval())
                    return true; // there was something in the buffer but the interval is already working.

                var client = this;
                var invokeInterval = function () {
                    return client.__InvokeBufferCheck();
                }

                if (invokeInterval())
                    __sendBufferIntervalIndex = window.setInterval(invokeInterval, 10);

                return true;
            },

            // stop the command sending interval.
            __ClearInterval: function () {
                var interval = this.__sendBufferIntervalIndex;
                this.__sendBufferIntervalIndex = null;
                window.clearInterval(interval);
                return false;
            },

            // call to check the buffer for invocation.
            __InvokeBufferCheck: function () {
                if (!this.Buffer.HasWaitingCommands()) {
                    this.__ClearInterval();
                    return false;
                }

                if (this.IsSending() || new Date().getTime() - this.Buffer.LastPosted.getTime() < this.WaitForCommandsTime) {
                    return true; // still waiting or already sending.
                }

                // send the bach to the server.
                this.SendBatch(this.Buffer.FlushCommandBuffer());

                return false;
            },

            // checks wether the current client should send the next buffer.
            CheckForWaiting: function () {
                this.BeginSendBufferInterval();
            },

            // sends the next buffer batch to the server.
            SendBatch: function (buffer) {
                var rq = this.CreateRequestCommand(buffer);
                rq.isBuffer = true;
                this.__SendRequest(rq, true);
            },

            // posts a command on the command buffer to be executed when the last request has completed.
            PostCommand: function (cmnd, idx) {
                if (cmnd == null)
                    return false;
                this.Buffer.Post(cmnd, idx);
                this.CheckForWaiting();
            },

            // sends a command directly to the server and awaits the command response to allow the process to complete.
            SendCommand: function (cmnd, async) {
                if (async == null)
                    async = false;
                this.__SendRequest(this.CreateRequestCommand([cmnd]), async);
                return async ? null : cmnd.Response;
            },

            __SendRequest: function (rq, async) {
                if (this.HaltCommunication)
                    return;
                if (async == null)
                    async = true;
                var ajaxCmnd = this.CreateRequestAjaxCommand(rq, async, "json");

                // saving the buffer context.
                if (rq.isBuffer == true) {
                    if (this.IsSending() && document.Debug) {
                        consol.error("Attempt to send a buffer to the server while another buffer is being sent.");
                        return false;
                    }
                    this.LastBufferContext = rq.context;
                }

                // binding the event commands.
                $.extend(ajaxCmnd, {
                    error: function (jqXHR, textStatus, errorThrown) {
                        // running on the context object.
                        this.Client.Error(this, jqXHR, textStatus, errorThrown);
                    },
                    success: function (data, textStatus, jqXHR) {
                        // running on the context object.
                        this.Command.SyncedResponse = this.Client.Recive(this, data, textStatus, jqXHR, this.Command.async);
                    },
                    complete: function (jqXHR, textStaus) {
                        // running on the context object.
                        // request has completed. do nothing here, the response may still be loading scripts.
                        this.Client.CallComplete(this, jqXHR, textStaus);
                    }
                });

                // Executing the ajax command.
                ajaxCmnd.context.Command = ajaxCmnd;
                $.ajax(ajaxCmnd);

                // returning sync (if there is one) response.
                return ajaxCmnd.SyncedResponse;
            },

            // Called when a call is compleated to handle the response.
            CallComplete: function (context, jqXHR, textStaus) {
                if (context.Errored) {
                    // checking for async 
                    if (context.Request.isBuffer != true) {
                        // A non buffer command cannot be resent to the server.
                        var str = "Error when sending unbuffered request to server (throgh SendCommand). Use PostCommand for recovery mechanisms to be active. Error details: " + context.textStatus;
                        console.error(str);
                        throw str;
                        return; // noting to do. just return.
                    }
                    else if (context.TimedOut != true) {
                        // A non buffer command cannot be resent to the server.
                        var str = "Error when sending buffered request to server (throgh PostCommand), attached request aborted. Error details: " + context.textStatus;
                        var rq = context.Request;
                        function f() {
                            console.error(str, rq);
                            throw str;
                        }
                        window.setTimeout(f, 0);
                        // move to next buffered command.
                    }
                    else {
                        context.Request.Attempt += 1;
                        if (context.Request.Attempt > this.Options.MaxAttempts) {
                            console.warn("Reached max attempts on request " + context.Request.Id);
                            if (document.Debug) {
                                if (context.Request.isBuffer)
                                    this.LastBufferContext = null; // clear the current buffer context if this is a buffer request.
                                this.StateChanged();
                                console.error("Request aborted", context.Request);
                            }
                        }
                        else {
                            // goto waiting for response (buffered command only).
                            context.Errored = false;
                            // resend the command.
                            this.BeginWaitForResponse(context.Command);
                            return;
                        }
                    }
                }
                
                // checking for move to next buffer command
                if (context.Request.isBuffer)
                {
                    this.CleanBufferAndContinuteToNext();
                }
            },

            CleanBufferAndContinuteToNext:function(){
                this.LastBufferContext = null; // clear the current buffer context if this is a buffer request.

                // mark the change of the state.
                this.StateChanged();

                // Checking for next commands.
                this.CheckForWaiting();
            },

            // Called on a successful response to trnaslate the data and execute the commands.
            Recive: function (context, data, textStatus, jqXHR, async) {
                // marking the times.
                this.MarkTime(true, true);
                if (this.BeatCommand != null)
                    this.BeatCommand.Reset();

                if (async == null)
                    async = true;
                // mark the context as not errored.
                context.Errored = false;

                // recive data object and convert to command.
                var me = this;
                // to allow for debug mode. (seperate send recive from the commands).
                var f = function () {
                    return me.ReciveCommand(data, context);
                }

                // check to see in what way to update the command.
                // sync or not.
                if (async) {
                    window.setTimeout(f, 0);
                    return null;
                }
                else return f();
            },

            // Recives and executes a responce command from the server.
            ReciveCommand: function (cmnd, context) {
                if (typeof (cmnd) == "string")
                    cmnd = $.JSON.From(cmnd);

                if (cmnd.IsError) {
                    console.warn(cmnd.Message);
                    if (cmnd.Errors != null) {
                        for (var i = 0; i < cmnd.Errors.length; i++) {
                            console.warn("Error: " + cmnd.Errors[i].Message, cmnd.Errors[i]);
                        }
                    }

                    return;
                }
                if ($.Vebrose)
                    console.log("Recived json responce from server", cmnd);

                // event trigger.
                this.Events.trigger($.XPress.EventIds.Recive, cmnd);

                if (cmnd.Trace != null)
                    console.log(cmnd.Trace);

                // checking for system commands.
                if (cmnd.sys != null) {
                    for (var i = 0; i < cmnd.sys.length; i++) {
                        switch (cmnd.sys[i].Command) {
                            case "Disconnect":
                                console.warn("The serverside has called to disconnect the client.");
                                this.Disconnect();
                                return null;
                                break;
                        }
                    }
                }

                // matching requests to responces if any.
                if (cmnd.ResponseVals != null && context!=null&&context.Request!=null &&context.Request.Commands!=null)
                {
                    // mathcing.
                    for (var i = 0; i < context.Request.Commands.length; i++) {
                        context.Request.Commands[i].Response = cmnd.ResponseVals[context.Request.Commands[i]._cmndid];
                    }
                }

                // call to execute te commands in the global context.
                if (cmnd.Commands != null)
                    $.XPress.Commands.Execute(cmnd.Commands);

                // returning the sync response.
                return cmnd.SyncedResponse;
            },

            // Called on an error in the request.
            Error: function (context, jqXHR, textStatus, errorThrown) {
                // check for command timedout.
                context.textStatus = textStatus;
                context.TimedOut = textStatus.toLowerCase() == "timeout";
                context.Errored = true;
                context.errorThrown = errorThrown;
            },

            // called to register the client at the serverside. This is for good order only, the client was already created at the page request.
            RegisterClient: function () {
                var response = this.SendCommand({
                    Type: "System",
                    Command: "Register",
                }, false);

                console.log("Server says: " + response);
                this.InitHartBeat();
            },

            // Closes the client.
            CloseClient: function () {
                console.log("Window unloads closing client...");
                var cmnd = this.CreateAjaxCommand("Dead", this.Options.Url, 2000, {}, false, "close", false);
                $.ajax(cmnd);
                console.log("Client closed");
            },
            // disconnect the client.
            Disconnect: function () {
                this.Events.trigger($.XPress.EventIds.ServiceDisconnect, null);
                this.HaltCommunication = true;
            },

            // Ping the server for changes if needed.
            PingForUpdates: function () {
                if (!(this.IsSending()) && this.Buffer.buffer.length == 0) {
                    this.PostCommand({
                        Type: "System",
                        Command: "RetrivePendingCommands",
                    });
                    return true;
                }
                return false;
            },

            ////////////////////////////////////////////////
            /// Beat and pending.
            StopHartBeat:function(){
                this.Options.HasBeat = false;
            },
            StartHartBeat:function(){
                this.Options.HasBeat = true;
            },
            _beatInitialized: false,
            BeatCommand: null,
            // The sending of beat commands
            InitHartBeat: function () {
                // called to execute the beat command.
                if (this._beatInitialized == true)
                    return;
                this._beatInitialized = true;
                this.BeatCommand = this.CreateAjaxCommand("hartbeat",
                    this.Options.Url,
                    _as(this.Options.BeatTimeout),
                    {
                        Client: this,
                        IsErrored: false,
                        LastIntervalIndex: null,
                    },
                    false,
                    "beat",
                    true);
                $.extend(this.BeatCommand, {
                    error: function (jqXHR, textStatus, errorThrown) {
                        // running on the context object.
                        this.IsErrored = true;
                    },
                    success: function (data, textStatus, jqXHR) {
                        this.Client.MarkTime(false, true);
                        if (data == "Dead") {
                            this.Client.Disconnect();
                            console.warn("Disconnced from server by server response. Server commanded: disconnect.");
                        }
                        else if (data == "Changed") {
                            if (this.Client.PingForUpdates())
                                console.log("Server notifed it has has changed, updating the client.");
                        }
                        else if ($.Vebrose)
                            console.log("Client Hartbeat");
                    },
                    complete: function (jqXHR, textStaus) {
                        // the beat compleated succesfuly, need to check if to send the beat.
                        if (this.Client.HaltCommunication) {
                            this.Client._beatInitialized = false;
                            return;
                        }

                        // running on the context object.
                        if (this.IsErrored) {
                            if (this.Client.LastClientSuccessfulConnectionTime + this.Client.Options.MaxClientNotConnectedTime < new Date().getTime()) {
                                this.Client.Disconnect();
                                console.warn("Beat command was overdue, client probably disconnected from server.Beat command timeout. Increase LastClientSuccessfulConnectionTime if timout is to fast.");
                            }
                            else {
                                // resend.
                                console.warn("Beat attempt to server faild, retrying..");
                                this.Command.Send(this.Client.Options.FastHartBeatInterval);
                            }
                        }
                        else {
                            var context = this;
                            this.Command.Send();
                        }
                    },
                    Send: function (time) {
                        if (time == null)
                            time = this.context.Client.GetBeatDelayTime();
                        this.context.IsErrored = false;

                        var me = this;
                        function f() {
                            // send the command.
                            me.context.LastIntervalIndex = null;
                            me.timeout = me.context.Client.Options.BeatTimeout;
                            if(me.Client.Options.HasBeat)
                                $.ajax(me);
                            else 
                            {
                                // Missed hart beat since client has Options.Beat=false, attempting to resend on next interval.
                                me.Send(time);
                            }
                        }
                        this.context.LastIntervalIndex = window.setTimeout(f, time);
                    },
                    Reset: function () {
                        if (this.context.LastIntervalIndex == null)
                            return; // already sending not in interval.
                        window.clearTimeout(this.context.LastIntervalIndex);
                        this.Send();
                    },
                });

                this.BeatCommand.context.Command = this.BeatCommand;
                if(this.Options.HasBeat)
                    this.BeatCommand.Send(Math.round(Math.random() * 200));
            },

            ///////////////////////////////////////
            // Wait for response
            // happens when the server is either doing a long action or the service was disconnected when/during the current action.
            // if the current action reached the server then the result would be updated in the next command, if that command dose not exist then one would be sent.
            BeginWaitForResponse: function (pendingCommand) {
                var cmnd = this.CreateAjaxCommand(pendingCommand.context.Request.Id + "", this.Options.Url, this.Options.RequestTimeout, { Client: this }, false, "waitforresponse", true);
                cmnd.context.PendingCommand = pendingCommand;
                cmnd.context.Command = cmnd;
                $.extend(cmnd, {
                    error: function (jqXHR, textStatus, errorThrown) {
                        // running on the context object.
                        this.TimedOut = textStatus == "timeout";
                        this.IsErrored = true;
                    },
                    success: function (data, textStatus, jqXHR) {
                        // need to check the command response.
                        var cmndLength = data.indexOf(":");
                        var hasData = true;
                        if (cmndLength < 0) {
                            cmndLength = data.length;
                            hasData = false;
                        }
                        var command = data.substring(0, cmndLength);
                        var response = hasData ? data.substring(cmndLength + 1, data.length) : null;

                        switch (command) {
                            case "Obsolete":
                                // claning the buffer from the previus command.
                                console.warn("Command that was sent to server has become obsolete and was ignored.", this.PendingCommand.context.Request);
                                if (this.Client.IsSending())
                                    this.Client.CleanBufferAndContinuteToNext();
                                break;
                            case "Resend":
                                // the current command did not reach the server. resend command.
                                $.ajax(this.PendingCommand);
                                break;
                            case "Disconnect":
                                console.warn("Waiting for response recived disconnect command.", this.PendingCommand.context.Request);
                                this.Client.Disconnect();
                                break;
                            case "IsExecuting":
                                console.log("Waiting for response: command is still executing on server side, wait and resend 'waiting for response' command.");
                                var me=this.Command;
                                function f()
                                {
                                    me.Send();
                                }
                                window.setTimeout(f, this.Client.Options.FastHartBeatInterval);
                                break;
                            case "Completed":
                                // claning the buffer from the previus command.
                                console.log("Waiting for response: command compleated on server and responded. (Response length: " + response.length + ")");
                                if (this.Client.IsSending())
                                    this.Client.CleanBufferAndContinuteToNext();

                                // call to recive the response.
                                this.Client.Recive(this.PendingCommand.context.Request, response, textStatus, jqXHR, true);
                                break;
                            default:
                                console.warn("recived unrecognized response for WaitForResponseCommand", data)
                                break;
                        }
                    },
                    complete: function (jqXHR, textStaus) {
                        if (this.IsErrored) {
                            if (!this.TimedOut) {
                                // resned by interval.
                                console.warn("Error when trying to send a waiting for response command to the server.", textStaus);
                            }
                            this.Command.Send();
                        }
                    },
                    Send: function () {
                        if (this.context.Client.HaltCommunication)
                        {
                            console.warn("Cannot send waiting for response since client is not connected. (HaltCommunication=true)");
                            return;
                        }
                        this.context.IsErrored = false;
                        console.warn("Request timed out at the server.. sending waiting for response command...");
                        $.ajax(this);
                    }
                });
                cmnd.Send();
            },
        };
        client.$(id, options);
        return client;
    },
});

console.log("RMC jquery initialized (execute in console '$.ToggleVebrose()' for full details in trace.)");
