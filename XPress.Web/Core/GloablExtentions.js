// Global functions that extend jquery.
$.extend($, {
    InvokeAsMethod: function (script, args) {
        this.___dummy___method__for__invoke = new Function(script);
        this.___dummy___method__for__invoke(args);
        this.___dummy___method__for__invoke = null;
        return;
    },
    // retruns a jquery element from its id.
    FromId: function (ids) {
        if (typeof (ids) === 'string')
            ids = [ids];
        var lst = [];
        for (var i = 0; i < ids.length; i++) {
            var o = document.getElementById(ids[i]);
            if (o == null)
                continue;
            lst.push(o)
        }
        return $(lst);
    },
    // returns the keys associated with an object.
    Keys: function (o) {
        o = o == null ? this[0] : o;
        if (o == null)
            return [];
        if (Object.keys != null)
            return Object.keys(o);
        else {
            var keys = [];
            for (var k in o) { keys.push(k); }
            return keys;
        }
    },
    // the vebrose command that allows us to determine the state of printing.
    Vebrose: false,
    ToggleVebrose: function () {
        this.Vebrose = !this.Vebrose;
        console.log("Vebrose is " + (this.Vebrose ? "on" : "off"));
    },
    //#region Json commands
    JSON: {
        DateMarker: "#\"dt\"#",
        Parser: (function () {
            return function (key, data) {
                if (typeof (data) == "string" && data.trim().indexOf($.JSON.DateMarker) == 0) {
                    var ticks = parseInt(data.substring($.JSON.DateMarker.length), 10);
                    return new Date(ticks); // assume always in utc.
                }
                return data;
            }
        })(),
        From: function (str) {
            if (str == null)
                return null;
            if ($.trim(str) == "")
                return {};
            try {
                //str = str.replace(/\s*(-*[0-9]*)/g, ":\"" + this.DateMarker + "$2\"");
                return JSON.parse(str, this.Parser);
            }
            catch (e) {
                if (document.Debug == true)
                    throw e;
                e.str = str;
                console.warn("Eror when parsing json object", e);
                return {};
            }
        },
        To: function (o) {
            try {
                return JSON.stringify(o);
            }
            catch (e) {
                if (document.Debug == true)
                    throw e;
                e.str = str;
                console.warn("Eror when parsing json object", e);
                return {};
            }
        },
    },
});

// console for application (IE support)
var console = window.console ? window.console : {
    assert: function (txt) { },
    count: function (txt) { },
    debug: function (txt) { },
    dir: function (txt) { },
    dirxml: function (txt) { },
    error: function (txt) { },
    group: function (txt) { },
    groupEnd: function (txt) { },
    info: function (txt) { },
    log: function (txt) { },
    profile: function (txt) { },
    profileEnd: function (txt) { },
    time: function (txt) { },
    timeEnd: function (txt) { },
    trace: function (txt) { },
    warn: function (txt) { }
};

function _as(o, prs) {
    if (typeof (o) == "function")
        return o(prs);
    return o;
}

$.extend($, {
    EventCollection: function (o) {
        if (o == null)
            o = {};
        var evcol = {
            _callbacks: {},
            bind: function (evname, callback) {
                if (!this._callbacks[evname]) {
                    this._callbacks[evname] = $.Callbacks();
                }
                this._callbacks[evname].add(callback);
            },
            remove: function (evname, callback) {
                if (!this._callbacks[evname]) {
                    return;
                }
                this._callbacks[evname].remove(callback);
            },
            trigger: function () {
                if (arguments.length == 0)
                    return;
                var evname = arguments[0];
                var args = Array.prototype.slice.call(arguments, 1);
                if (this._callbacks[evname]) {
                    this._callbacks[evname].fire.apply(this._callbacks[evname], args);
                }
            }
        }
        $.extend(o, evcol);
        return o;
    },
});

// Generation of a new code timer.
$.extend($, {
    CodeTimer: function () {
        var timer = {
            Started: null,
            LastMeasure: new Date(),
            Start: function () {
                this.LastMeasure = this.Started = new Date();
            },

            Mark: function (name) {
                var curd = new Date();
                if (name == null)
                    name = "mark";
                var info = { totalms: curd - this.LastMeasure, name: name };
                this.push(info);
                this.LastMeasure = curd;
            },

            ToTraceString: function () {
                var ar = new Array();
                for (var i = 0; i < this.length; i++)
                    ar.push(this[i].name + " : " + this[i].totalms);
                return ar.join("\n");
            },

            ToTraceHtml: function () {
                var ar = new Array();
                for (var i = 0; i < this.length; i++)
                    ar.push(this[i].name + " : " + this[i].totalms)
                return ar.join("<br/>");
            },

            Stop: function () {
                this.TotalTime = (this.LastMeasure.getTime() - this.Started.getTime());
                this.Started = null;
            },
        };
        timer.Start();
        return timer;
    }
});

// Added function mapping methods.
$.extend($.fn, {
    // checks if any of the elements have the condition.
    any: function (condition) {
        for(var i=0;i<this.length;i++)
        {
            if (condition(this[i]))
                return true;
        }
        return false;
    },
    // checks if all of the elements have the condition.
    all: function (condition) {
        return !this.any(function (el) {
            return !condition(el);
        });
    }
});