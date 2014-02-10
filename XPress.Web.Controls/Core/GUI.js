// Core gui for controls collection.

// Adding some functions to the core collection.
$.extend($.fn, {
    OnContextOut: function (f) {
        var parent = this.first();
        if (f == null)
            parent.off("focusout.oncontext");
        else parent.on("focusout.oncontext", function (e) {
            // checking for child has focus.
            if ($(e.relatedTarget).parents().add(e.relatedTarget).any(function (el) { return el == parent[0]; })) {
                // nothing to do.
                return;
            }
            f(e);
        });
    },
    OnContextIn: function (f) {
        var parent = this.first();
        if (f == null)
            parent.off("focusin.losecontext");
        else parent.on("focusin.losecontext", function (e) {
            // checking for child has focus.
            if ($(e.relatedTarget).parents().add(e.relatedTarget).any(function (el) { return el == parent[0]; })) {
                // nothing to do.
                return;
            }
            f(e);
        });
    },

});