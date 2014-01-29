// Service generator for JQuery enabled javascript.
// NOTE: Only implemented as a client.

// Creating the jcom client object.
$.extend($.XPress, {
    JCOM: {
        // constructor.
        Init: function () {
            TypeDefinitions = {};
            $.XPress.Commands.Translators.Set("JCOM_Construct", function (cmnd, contextObject) {
                contextObject._data = cmnd.Data;
                contextObject._oid = cmnd.OId;
                $.XPress.JCOM.ImplementTypeDef(contextObject, cmnd.TypeId);
            });
            $.XPress.Commands.Translators.Set("JCOM_TypeDef", function (cmnd) {
                $.XPress.JCOM.RegisterTypeDef(cmnd.Name, cmnd.Code);
            });
        },
        CreateCommand: function (id, name, args) {
            return {
                OId: id,
                _args: args,
                Type: "JCOM",
                Name: name,
            };
        },
        // A representation of a serverside funtion.
        Method: function (obj, name, args, async) {
            var client = $(obj).XPress.GetClient();
            if (client == null) {
                console.warn("Cannot invoke method " + name + " since no client is defined on the page. ($(obj).GetClient()==null)");
                return;
            }

            var m_args=[];
            for(var i=0;i<args.length;i++)
                m_args.push(args[i]);
            var cmnd = this.CreateCommand(obj._oid, name, m_args);
            if(async)
            {
                client.PostCommand(cmnd);
                return null;
            }
            else return client.SendCommand(cmnd, false);
        },
        // A representation of a property on the client side.
        Property: function (obj, name, args, async, canRead, canWrite) {
            // checking for read.
            if (args.length == 0) {
                if (!canRead)
                    throw "Cannot read property " + name + " since it is write only.";
                // try and read.
                if (obj._data == null)
                    return null;
                return obj._data[name]
            }
            else if (args.length > 1) {
                throw "Not implemented: multiple parameters at property are not supported.";
            }
            if (!canWrite)
                throw "The property " + name + " is read only.";

            var client = $(obj).XPress.GetClient();
            if (client == null) {
                if ($.Vebrose)
                    console.warn("Cannot update propery " + name + " on serverside since no client is defined on the page. ($(obj).GetClient()==null)");
                return;
            }

            obj._data[name] = args[0];

            var cmnd = this.CreateCommand(obj._oid, name, [args[0]]);
            if (async) {
                client.PostCommand(cmnd);
                return null;
            }
            else return client.SendCommand(cmnd, false);
        },
        // A hash collection of type definitions.
        TypeDefinitions: {},
        // Adds a typedef to the collection of type definitions.
        RegisterTypeDef: function (tid, code) {
            var tdef = (new Function("return " + code + ";"))();
            this.TypeDefinitions["t" + tid] = tdef;
        },
        // Implements a type definition on an object.
        ImplementTypeDef: function (o, tid) {
            var tdef = this.TypeDefinitions["t" + tid];
            if (tdef == null) {
                console.warn('Cannot find requested type definition ' + tid + " to extend object. (See attached object)", o);
                return;
            }
            $.extend(o, tdef);
        },
    },
});

// initialize jcom.
$.XPress.JCOM.Init();