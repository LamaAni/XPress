// The core file for XPress. Handles basic functions of the rmc mechanise.

////////////////////////////////////////
// Generating constants.
$.extend($, {
    Rmc: {
    },
});

$.extend($.fn, {
    Rmc: {
        element: this, // represents the object that returns the element.
        GetClient: function () { return null;}, // dummy function to be popluated later.
    }
})

//////////////////////////////////////////////////
// Command execution
$.extend($.Rmc, {
    ////////////////////////////////////////////////////
    // Collection of system events to bind to.
    EventIds: {
        LinkComplete: 'Client_LinkComplete',
    },
    SystemEvents: $.EventCollection(),

    ///////////////////////////////////////////////////////////////////////////
    // global links collection, allows linking files to the current executing page using js.
    Links: {
        Loaded: {},
        Register: function (id, o) {
            this.Loaded[id] = o == null ? true : o;
            $.XPress.SystemEvents.trigger($.XPress.EventIds.LinkComplete);
        },
        ExtendObject: function (el, id) {
            $.extend(el, this.Loaded[id]);
            if (el.$ != null)
                el.$();
            el.$ = null;
        },
        Load: function (link) {
            var headObject = null;
            var requiresPend = false;
            switch (link.LinkType) {
                case "Css":
                    headObject = document.createElement("link");
                    headObject.href = link.Url;
                    headObject.id = link.UniqueId;
                    headObject.rel = "stylesheet";
                    break;
                case "Script":
                    headObject = document.createElement("script");
                    headObject.type = "text/javascript";
                    headObject.src = link.Url;
                    headObject.id = link.UniqueId;
                    requiresPend = true;
                    break;
                default:
                    console.warn("Cannot translate loaded link.", link);
                    break;
            }
            if (headObject == null)
                return false;
            $("head")[0].appendChild(headObject);
            return requiresPend;
        },
    },

    /////////////////////////////////////////////////////////////
    // the command collection. Execute commands, and handles pending commands.
    // Pend commands - commands that require the system to wait until these commands finish and only then 
    // allow post commands to execute.
    // Post commands - commands that must wait until all pend commands are executed.
    // Invoke commands - commands that execute when they arrive.
    Commands: {
        // Initialziation function for the commands collection.
        $: function () {
            var me = this;
            $.XPress.SystemEvents.bind($.XPress.EventIds.LinkComplete, function () {
                me.ProcessNextPostedCommand(); // process the next command if possible.
            });
        },

        // Posted commands that must wait until all pending command validators compleate.
        PostedCommands: new Array(),

        // Pending validators.
        PendingValidators: new Array(),

        // true when we are currently validating pending commands.
        IsValidatingPending: false,

        // the current active command block;
        Active: null,

        // If true then processing a command.
        IsProcessingCommand: function () { return this.Active != null; },

        // holds the trnalstor collection that allows a command to be trnslated by the command type. Note that translators with 
        // the same name will be overriden.
        Translators: {
            _col: {
                // Link collection implementation.
                link: {
                    Invoke: function (cmnd) {
                        if ($.XPress.Links.Load(cmnd)) {
                            return function () {
                                $.XPress.Links.Loaded[cmnd.UniqueId];
                            };
                        }
                        return null;
                    }
                },
                script: {
                    Invoke: function (cmnd, contextObject) {
                        // after a script command is called from the server.
                        if (contextObject == null) {
                            var docall = new Function(cmnd.Code);
                            if (document.Debug) {
                                docall();
                            }
                            else {
                                try {
                                    docall();
                                }
                                catch (e) {
                                    console.warn(e);
                                }
                            }
                        }
                        else {
                            contextObject.__invokeFunc = new Function(cmnd.Code);
                            if (document.Debug) {
                                contextObject.__invokeFunc();
                            }
                            else {
                                try {
                                    contextObject.__invokeFunc();
                                }
                                catch (e) {
                                    console.warn(e);
                                }
                            }
                            contextObject.__invokeFunc = null;
                            delete contextObject.__invokeFunc;
                        }
                    }
                }
            },
            // Sets the translator for a specific type.
            Set: function (type, o) {
                type = type.toLowerCase();
                if (type == "link" || type == "script") {
                    throw "Cannot use 'link' or 'script' as translator type. This is are reserved for system commands";
                }
                else if (typeof (o) == "function")
                    o = {
                        Invoke: o,
                    };
                else throw "A translator must be an object or function";
                this._col[type] = o;
            },
            // returns the translator for a specific type.
            Get: function (type) {
                type = type.toLowerCase();
                return this._col[type];
            },
            // Clears the translator for a specific type.
            Delete: function (type) {
                type = type.toLowerCase();
                delete this._col[type];
            },
        },

        // called to process the command.
        Execute: function (cmnds) {
            if (cmnds == null)
                return; // nothing to do.
            // validating this is an array.
            cmnds = [].concat(cmnds);

            // executing invoke commands
            for (var i = 0; i < cmnds.length; i++)
                if (cmnds[i]._et == "Invoke")
                    this.ExecuteCommandDirect(cmnds[i]);

            // executing invoke commands
            for (var i = 0; i < cmnds.length; i++) {
                if (cmnds[i]._et == "Pend") {
                    var pendInvoke = this.ExecuteCommandDirect(cmnds[i]);
                    if (pendInvoke == null)
                        continue;
                    if (typeof (pendInvoke) == "function")
                        pendInvoke = { Invoke: pendInvoke };
                    if (typeof (pendInvoke) != "object" || pendInvoke.Invoke == null)
                        console.warn("Recived pending invoke that is not a function or an object with the funtion 'Invoke'. Pend ignored.");
                    this.PendingValidators.push(pendInvoke);
                }
                else if (cmnds[i]._et == "Post") {
                    this.PostedCommands.push(cmnds[i]);
                }
            }
            this.ProcessNextPostedCommand();
        },

        // Called to process the next command if possible.
        ProcessNextPostedCommand: function () {
            // checking the current state of the collection.
            if (this.IsProcessingCommand() || this.IsValidatingPending)
                return;

            if (this.PostedCommands.length == 0) {
                this.Active == null;
                return;
            }

            var cmnd = this.PostedCommands.pop();
            this.Active = {
                // the command data
                Cmnd: cmnd,
                // when the command started
                Timer: $.CodeTimer(),
            };

            this.ValidatePending();

            // still waiting for validators.
            if (this.PendingValidators.length > 0)
                return;

            // call to process the command
            this.ExecuteCommandDirect(cmnd);

            this.Active = null;

            // call to process the next command.
            this.ProcessNextPostedCommand();
        },

        // call to validate all pending commadns
        ValidatePending: function () {
            if(this.PendingValidators.length==0)
                return;
            this.IsValidatingPending = true;
            var returnValidators = new Array();
            // continue untill there are no more validators (some may be added while we are running).
            while (this.PendingValidators.length > 0) {
                this.PendingValidators = [];
                var validators = this.PendingValidators;
                for (var i = 0; i < validators.length; i++) {
                    var validator = validators[i];
                    if (validator.Invoke(this.Active) == true)
                        continue;
                    returnValidators.push(validator);
                }
            }
            this.PendingValidators = this.PendingValidators.concat(returnValidators);
            this.IsValidatingPending = false;
        },

        // call to process a single command. 
        ExecuteCommandDirect: function (cmnd) {
            if (typeof (cmnd) != 'object') {
                console.warn("Recived command which is not an object. Command Json not read correctly or command was not in correct format. Command ignored.", cmnd);
                return;
            }

            if (cmnd._t == null) {
                console.warn("Command type not found when executing command. Command ignored.", cmnd);
                return null;
            }

            var translator = this.Translators.Get(cmnd._t);
            if (translator != null)
                return translator.Invoke(cmnd, cmnd.ContextObject);
            else {
                console.warn("Recived command for type '" + cmnd._t + "', but no command translator was found.", cmnd);
                return null;
            }
        }
    },
    // Command to activate an html object.
    Activate: function (els, event) {
        var activated = null;

        // lopping throught the selector and activating if nesssary.
        els.each(function (idx, elm) {
            // mark for activation.
            if (elm._activated == true)
                return;
            elm._activated = true;
            
            // adding to the activated collection.
            if (activated == null)
                activated = $(elm);
            else activated.push(elm);

            var acInfo = elm.getAttribute("_ac");
            if (acInfo != null) {

                elm.setAttribute("_ac", "ok"); // mark for activated.
                var acInfo = new Function("return " + acInfo + ";"); // get the actual info.

                // mark as activated and get the activation info.
                var acInfo = acInfo();

                // deleting emties.
                for (evname in acInfo) {
                    elm.setAttribute(evname, acInfo[evname]);
                }
            }

            // reading build info commands and executing them.
            var cmnds = elm.getAttribute("_bc");
            if (cmnds != null) {
                elm.removeAttribute("_bc");
                var cmnds = $.JSON.From(cmnds);
                if (cmnds.length > 0) {
                    for (var i = 0; i < cmnds.length; i++)
                        cmnds[i].ContextObject = elm;
                    $.XPress.Commands.Execute(cmnds);
                }
            }
        });

        if (event != null && activated != null)
            activated.trigger(event.type == null ? event : event.type);
    },
});

// initializing the commands collection.
$.XPress.Commands.$();

// activation event handler
// global control activation functions.
function $$() {
    // getting the query.
    var o = arguments.length == 1 && arguments[0].jquery != null ? arguments[0] : $.apply($, arguments);
    $A$(o, null);
    return o;
}

// calls for activation.
function $A$(o, ev) {
    $.XPress.Activate(o.jquery == null ? $(o) : o, ev);
}

function $EO$(o, id) {
    $.XPress.Links.ExtendObject(o, id);
}
