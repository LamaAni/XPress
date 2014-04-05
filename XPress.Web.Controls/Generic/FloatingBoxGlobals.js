$.extend($.XPress, {
    Floating: {
        // the collection of ids that are valid boxs.
        BoxInfos: {},
        LastBoxValidationTime: new Date(),
        Stacks: {},
        Events: {
            BeforeMoveNear: "float_beforemovenear",
        },
        GetStack: function (stackIndex) {
            if (this.Stacks[stackIndex + ""] == null) {
                this.Stacks[stackIndex + ""] = new Array();
                this.Stacks[stackIndex + ""].Index = stackIndex;
            }
            return this.Stacks[stackIndex + ""];
        },
        // Validates that the box ids that exist are valid. Cleanup procedure.
        ValidateBoxes: function () {
            if (new Date() - this.LastBoxValidationTime < 1000 * 10)
                return;
            this.LastBoxValidationTime = new Date();
            var me = this;
            _doBoxValidate = function () {
                var bids = new Array();
                for (var boxId in me.BoxInfos)
                    bids.push(boxId);
                for (var i = 0; i < bids.length; i++) {
                    if ($.FromId(bids[i]) == null) {
                        var stack = me.BoxInfos[bids[i]].Stack;
                        // remove from stack.
                        me.BoxInfos[bids[i]] = null;
                        delete me.BoxInfos[bids[i]];
                    }
                }
            }
            window.setTimeout(_doBoxValidate, 0);
        },
        // Registers a new box to the stack collector.
        RegisterBox: function (box) {
            this.ValidateBoxes();
            if (this.BoxInfos[box.id] != null)
                return;
            if (box.ZStack == null)
                throw "Cannot find box zstack value, please initialize the box or the html element is not a floating box.";
            var stack = this.GetStack(box.ZStack());
            stack.push(box.id);
            var boxInfo = {
                Stack: stack,
            }
            this.BoxInfos[box.id] = boxInfo;
        },
        // Makes the current box the top window in it's stack
        MakeTopBox: function (box) {
            this.ValidateBoxes();
            var stack = this.GetStack(box.ZStack());
            stack.splice($.inArray(box.id, stack), 1);
            stack.push(box.id);
            $(stack).each(function (idx, id) {
                var ctrl = $.FromId(id);
                if (ctrl == null)
                    return;
                $(ctrl).css("zIndex", stack.Index * 1000 + 100 + idx);
            });
        }
    },
});