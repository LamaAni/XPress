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
    /// Returns the display rect associated with the element.
    DisplayRect: function () {
        var rect = new Object();
        var width = 0;
        var height = 0;
        var left = 0;
        var top = 0;
        if (this.length > 0) {
            width = this.outerWidth();
            height = this.outerHeight();
            if (width == null) {
                width = this.width();
                height = this.height();
            }

            pos = this.offset();
            if (pos != null) {
                left = pos.left;
                top = pos.top;
            }
        }
        return {
            top: top,
            left: left,
            width: width,
            height: height,
            bottom: top + height,
            right: left + width,
            WindowTop: function () { this.top - $(window).scrollTop() },
            WindowLeft: function () { this.left - $(window).scrollLeft() },
            // true if rect other is contained in the current rect.
            ContainedInRect: function (otherRect) {
                return this.left >= otherRect.left && this.right <= other.right && this.top >= otherRect.top && this.bottom <= other.bottom;
            },
            // true if (x,y | rect) is in the current rect;/
            IsInRect: function () {
                if (arguments.length == 0)
                    throw "You must provide a rect or a x,y position.";
                if (arguments.length == 1) {
                    var other = arguments[1];
                    return !(this.left > other.right || this.right < other.left) && !(this.top > other.bottom || this.bottom < other.top);
                }
                var x = arguments[1];
                var y = arguments[2];
                return x >= this.left && x <= this.right && y >= this.top && y <= this.bottom;
            },
            // returns the offset from another rect.
            OffsetFromRect: function (otherRect) {
                var offset = new Object();
                offset.x = 0;
                offset.y = 0;
                if (this.bottom > otherRect.bottom && this.top < otherRect.top) {
                    offset.y = 0;
                }
                if (this.bottom > otherRect.bottom)
                    offset.y = this.bottom - otherRect.bottom > 0 ? this.bottom - otherRect.bottom : 0;
                else if (this.top < otherRect.top)
                    offset.y = this.top - otherRect.top < 0 ? this.top - otherRect.top : 0;

                if (this.right > otherRect.right && this.left < otherRect.left) {
                    offset.x = 0;
                }
                if (this.right > otherRect.right)
                    offset.x = this.right - otherRect.right > 0 ? this.right - otherRect.right : 0;
                else if (this.left < otherRect.left)
                    offset.x = this.left - otherRect.left < 0 ? this.left - otherRect.left : 0;

                return offset;
            },
            Intersect: function (other) {
                if (this.left < other.left)
                    this.left = other.left;
                if (this.right > other.right)
                    if (other.right < this.left)
                        this.right = this.left; // zero it out.
                    else this.right = other.right;
                if (this.top < other.top)
                    this.top = other.top;
                if (this.bottom > other.bottom)
                    if (other.bottom < this.top)
                        this.bottom = this.top; // zero it out.
                    else this.bottom = other.bottom;

                this.width = this.right - this.left;
                this.height = this.bottom - this.top;
            },
            Equals: function (other) {
                return this.height == other.height && this.width == other.width;
            }
        };
    },
    // returns the context direction of the element.
    Direction: function () {
        var noctrol = false;
        var cur = this[0];
        if (cur != null) {
            while ($.__getStrDirection(cur) == "inherit") {
                cur = getParent(cur);
                if (cur == null) {
                    noctrol = true;
                    break;
                }
            }
        }
        else noctrol = true;

        if (noctrol)
            return "ltr";
        else return $.__getStrDirection(cur);
    },
    // true if has a vertical scroll bar.
    HasVerticalScrollBar: function () {
        return this.get(0).scrollHeight > this.height();
    },
    // true if has a horizontal scroll bar.
    HasHorizontalScrollBar: function () {
        return this.get(0).scrollWidth > this.width();
    },
    // true if has a scroll bar.
    HasScrollBar: function () {
        return this.HasHorizontalScrollBar() || this.HasVerticalScrollBar();
    },
    // returns the first parent that has a scroll bar.
    FindFirstScrollingParent: function () {
        var p = this.parent();
        if (p.length == 0)
            return $(document);
        if (p.HasScrollBar())
            return p;
        return p.FindFirstScrollingParent();
    },
    // returns all scrololing parents.
    FindScrollingParents: function (ps) {
        ps = ps == null ? [] : ps;
        var p = this.parent();
        if (p.length == 0)
            return;
        if (p.HasScrollBar())
            ps.push(p[0]);
        p.FindScrollingParents(ps);
        return $(ps);
    },
    // scroll an intem into view.
    ScrollIntoView: function (container) {
        if (this.length == 0)
            return;

        if (container == null) {
            container = this.FindFirstScrollingParent();
            if (container.length == 0)
                return; // no scrolling parent;
        }
        else if (container.jquery == null)
            container = $(container);

        var containerRect = container.DisplayRect();

        this.each(function (idx, elm) {
            var offsetFromRect = $(elm).DisplayRect().OffsetFromRect(containerRect);
            var y = offsetFromRect.y > 0 ? offsetFromRect.y + $.getScrollbarWidth() : offsetFromRect.y;
            var isRTL = getObjDir(elm) == 'rtl' && isIE;
            x = isRTL ? -offsetFromRect.x : offsetFromRect.x;
            var x = x > 0 ? x + $.getScrollbarWidth() : x;
            container[0].scrollTop = container[0].scrollTop + y;
            container[0].scrollLeft = container[0].scrollLeft + x;
        });
    },
    VisibleRect: function (rect) {
        if (rect == null)
            rect = this.DisplayRect(); // just the objects display rect.
        else if (this.css('display').indexOf('inline') == -1) {
            // parents with display inline do not effect the on the way the object is displayed. (i.e. bug?).
            rect.Intersect(this.DisplayRect());
        }

        if (this[0] == document.body)
            return rect;

        var p = this.parent();

        if (p.length > 0) {
            return p.VisibleRect(rect);
        }

        return rect;
    },
    // returns true if the current rect appears.
    IsVisible: function () {
        var vrect = this.VisibleRect();
        if (arguments.length == 0)
            return vrect.width > 0 && vrect.height > 0;
        else if (arguments.length == 1)
            return vrect.IsInRect(arguments[1]); // case a rect was sent.
        else
            return vrect.IsInRect(arguments[0], arguments[1]); // x,y
    },
    // returns false if the element or any of its parents are not displayed.
    IsDisplayed: function () {
        if (this.css('display') == 'none')
            return false;
        if (this[0] == document.body)
            return true;
        var p = this.parent();
        if (p.length == 0)
            return true;
        return p.IsDisplayed();
    },
    // returns the elemnent at a specified inner point.
    ElementFromPoint: function (x, y) {
        var rect = this.DisplayRect();
        return $.ElementFromPoint(x + rect.left, y + rect.top);
    },
});

$.extend($, {
    WindowSize: function () {
        return { height: $(window).height(), width: $(window).width() };
    },
    __getStrDirection: function (obj) {
        if (obj == null)
            return "inherit";
        if (obj.dir == "rtl" || obj.dir == "ltr")
            return obj.dir;
        if (getObjSty(obj) == null)
            return "inherit";
        if (getObjSty(obj).direction == null || getObjSty(obj).direction == "")
            return "inherit";
        return getObjSty(obj).direction;
    },
    // returns an element that appears at a specific point.
    ElementFromPoint: function (x, y) {
        if (!document.elementFromPoint) throw "Element from point is not supported in this browser. document.elementFromPoint==null."
        x += $(document).scrollLeft() > 0 ? $(document).scrollLeft() : 0;
        y += $(document).scrollTop() > 0 ? $(document).scrollTop() : 0

        return $(document.elementFromPoint(x, y));
    },
});