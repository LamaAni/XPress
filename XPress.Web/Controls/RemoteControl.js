// global implementation file for remote control core.
$.extend($.XPress, {
    Controls: {
        Init: function () {
            this.Init = null;
            delete this.Init;

            // initialzing...
            $.XPress.Commands.Translators.Set("RMCUpdate", function (cmnd) {
                $.XPress.Controls.UpdateControl(cmnd);
            });
        },
        UpdateControl: function (cmnd) {
            var o = $.FromId(cmnd.Id);
            if (o == null)
                return;
            o.replaceWith($(cmnd.Html));
            if ($.Vebrose)
                console.log("Updated remote control '" + cmnd.Id + "'.", cmnd);
        },
    },
});

$.XPress.Controls.Init();
