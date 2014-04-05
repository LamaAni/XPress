// internal events,
({
    $: function () {
        // registering the floating box.
        $.XPress.Floating.RegisterBox(this);
        $(this).css("display", "none").css("position", "fixed");
        if (this.HideOnLoseContext()) {
            var me = this;
            $(this).OnContextOut(function (el) {
                me.Hide();
            });
        }
    },
    // the id or quey bound to by the control, function overriden by serverside property.
    BoundQuery: function (id) { },
    // shows the float.
    Show: function (doShow) {
        if (doShow == null)
            doShow = true;
        if (doShow == false) {
            this.Hide();
            return;
        }
        $(this).css("zIndex", this.ZStack() * 1000 + 100);
        if (this.MoveWindowToTopInStackWhenShown())
            $.XPress.Floating.MakeTopBox(this);
        $(this).css("display", "block");

        bindq = $(this.BoundQuery());
        if (bindq.length > 0)
            this.MoveNear(bindq);
    },
    Hide: function (doHide) {
        if (doHide == null)
            doHide = true;
        if (doHide == false) {
            this.Show();
            return;
        }
        $(this).css("display", "none");
    },
    // Moves the control near another control.
    // top: if true then move near the top of the parent, otherwise move near the bottom.
    // left: if true then align to the left.
    MoveNear: function (ctrl, left, top) {
        // no control to move near to, stying put.
        if (ctrl == null) {
            if ($.Vebrose)
                console.warn("Null control was sent when moving near box " + this.id + ", command ignored.");
            return;
        }

        if (ctrl.jquery == null)
            ctrl = $(ctrl);

        // clearing all the css values.
        $(this).css('top', null).css('left', null);

        // attaching the object to the correct corners of the parent.
        targetRect = ctrl.DisplayRect();
        windowSize = $.getWindowSize();
        thisRect = $(this).DisplayRect();

        if (ctrl.jquery == null)
            ctrl = $(ctrl);

        // loading default left and top.
        if (top == null) {
            // checking the float height, if larger then the this rect, is to be binded to the top.
            top = targetRect.top + targetRect.height + thisRect.height > windowSize.height;
        }
        if (left == null)
            // checking for direction to determine the left
            left = !(ctrl.Direction() == "rtl");

        // triggering the on before move near, with the parent control.
        $(this).trigger($.Floating.Events.BeforeMoveNear, ctrl);

        // top
        y = targetRect.top + targetRect.height;
        if (top)
            // moving to above the control.
            y = targetRect.top - thisRect.height - 2; // 2 pixes for box shadows and such...

        // left;
        x = targetRect.left;
        if (!left && targetRect.left - (thisRect.width - targetRect.width) > 0)
            x = targetRect.left - (thisRect.width - targetRect.width);

        // window edge, we need to shift the floating box to show witing the window.
        if (x < 0)
            x = 0;
        if (x + thisRect.width > windowSize.width)
            x = windowSize.width - thisRect.width;

        $(this).css('left', x + "px");
        $(this).css('top', y + "px");
    },
    // Binds the current to another html element, and shows the float near that control when shown.
    Bind: function (q) {
        if (typeof (q) == "string") {
            // assume id or jquery.
            this.BoundQuery(q);
            return;
        }
        else if (q.jquery == null)
            q = $(q);
        this.BoundQuery(q.length == 0 ? null : "#" + q[0].id);
    },
});