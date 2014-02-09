({
    $: function () {
        // registering the floating box.
        $.XPress.Floating.RegisterBox(this);
        $(this).css("display", "none").css("position", "fixed");
    },
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
            $.XPress.Floating.MakeTopWindow(this);
        $(this).css("display", "block");
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
    MoveNear: function (ctrl, left, top) {

    }
});