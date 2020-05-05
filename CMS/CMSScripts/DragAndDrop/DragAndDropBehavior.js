// DragAndDropBehavior.js
Type.registerNamespace('CMSExtendedControls');

CMSExtendedControls.DragAndDropBehavior = function (element) {
    RegisterCMSDragDropBehavior();

    CMSExtendedControls.DragAndDropBehavior.initializeBase(this, [element]);

    this._FlowLayout = false;
    this._DragItemClassValue = null;
    this._DragItemHandleClassValue = null;
    this._DropCueIDValue = null;
    this._ItemGroupValue = null;
    this._HighlightDropableAreasValue = null;

    this._dropCue = null;
    this._floatingBehaviors = [];
};
CMSExtendedControls.getScrollOffset = function (element, recursive) {
    if (element == null) {
        return { x: 0, y: 0 };
    }

    var left = element.scrollLeft;
    var top = element.scrollTop;

    if (recursive) {
        var parent = element.parentNode;
        while ((parent != null) && (parent.scrollLeft != null)) {
            left += parent.scrollLeft;
            top += parent.scrollTop;

            if ((parent == document.body) && (left != 0 && top != 0)) break;
            parent = parent.parentNode;
        }
    }
    return { x: left, y: top };
};
CMSExtendedControls.setLocation = function (element, point) {

    if (typeof (OnSetLocation) == "function") {
        point = OnSetLocation(element, point);
    }

    Sys.UI.DomElement.setLocation(element, point.x, point.y);
};
CMSExtendedControls.DragAndDropBehavior.prototype = {

    initialize: function () {

        // Set last active behavior
        lastDragAndDropBehavior = this;

        // Register ourselves as a drop target.
        Sys.Extended.UI.DragDropManager.registerDropTarget(this);

        /* Bug fix for AJAX function getScrollOffset */
        Sys.Extended.UI.DragDropManager._getInstance().getScrollOffset = CMSExtendedControls.getScrollOffset;

        /* Bug fix for AJAX function setLocation used for items with fixed position */
        Sys.Extended.UI.CommonToolkitScripts.setLocation = CMSExtendedControls.setLocation;

        // Initialize drag behavior after a while
        window.setTimeout(Function.createDelegate(this, this._initializeDraggableItems), 500);

        this._dropCue = $get(this.get_DropCueID());
        if (this._dropCue != null) {
            this._dropCue.style.display = 'none';
        }
    },

    dispose: function () {
        Sys.Extended.UI.DragDropManager.unregisterDropTarget(this);

        this._clearFloatingBehaviors();

        CMSExtendedControls.DragAndDropBehavior.callBaseMethod(this, 'dispose');
    },

    add_onDrop: function (handler) {
        this.get_events().addHandler("onDrop", handler);
    },

    remove_onDrop: function (handler) {
        this.get_events().removeHandler("onDrop", handler);
    },

    // onDrop property maps to onDrop event
    get_onDrop: function () {
        return this.get_events().getHandler("onDrop");
    },

    set_onDrop: function (value) {
        if (value && (0 < value.length)) {
            var func = CommonToolkitScripts.resolveFunction(value);
            if (func) {
                this.add_onDrop(func);
            } else {
                throw Error.argumentType('value', typeof (value), 'Function', 'resize handler not a function, function name, or function text.');
            }
        }
    },

    add_onBeforeDrop: function (handler) {
        this.get_events().addHandler("onBeforeDrop", handler);
    },

    remove_onBeforeDrop: function (handler) {
        this.get_events().removeHandler("onBeforeDrop", handler);
    },

    // onBeforeDrop property maps to onBeforeDrop event
    get_onBeforeDrop: function () {
        return this.get_events().getHandler("onBeforeDrop");
    },

    set_onBeforeDrop: function (value) {
        if (value && (0 < value.length)) {
            var func = CommonToolkitScripts.resolveFunction(value);
            if (func) {
                this.add_onBeforeDrop(func);
            } else {
                throw Error.argumentType('value', typeof (value), 'Function', 'resize handler not a function, function name, or function text.');
            }
        }
    },

    _raiseEvent: function (eventName, eventArgs) {
        var handler = this.get_events().getHandler(eventName);
        if (handler) {
            if (!eventArgs) eventArgs = Sys.EventArgs.Empty;
            handler(this, eventArgs);
        }
    },

    _clearFloatingBehaviors: function () {
        while (this._floatingBehaviors.length > 0) {
            var behavior = this._floatingBehaviors.pop();
            behavior.dispose();
        }
    },

    _containsClass: function (el, className) {
        if ((className == null) || (el.className == null)) return false;
        if (new RegExp('\\b' + className + '\\b').test(el.className)) return true;
        return false;
    },

    _findChildByClass: function (item, className, stopOnClassName, notRecursive) {
        // First check all immediate child items
        var child = item.firstChild;
        while (child != null) {
            if (this._containsClass(child, className)) return child;
            child = child.nextSibling;
        }

        // Not found, recursively check all child items
        if (!notRecursive) {
            child = item.firstChild;
            while (child != null) {
                if (!this._containsClass(child, stopOnClassName)) {
                    var found = this._findChildByClass(child, className, stopOnClassName);
                    if (found != null) return found;
                }
                child = child.nextSibling;
            }
        }
    },

    _matchedItem: function (child) {
        return ((child.className != null) && ((child.className == this._DragItemClassValue) || (child.className.startsWith(this._DragItemClassValue + ' '))) && (child != this._dropCue));
    },

    // Find all items with the drag item class and make each item
    // draggable        
    _initializeDraggableItems: function (dummy, el) {
        if (el == null) {
            this._clearFloatingBehaviors();

            el = this.get_element();
        }

        if (el != null) {
            var child = el.firstChild;
            while (child != null) {
                if (this._matchedItem(child)) {
                    var handle = this._findChildByClass(child, this._DragItemHandleClassValue, "NotHandle");
                    if (handle) {
                        var handleId = handle.id;
                        var behaviorId = child.id + "_WidgetFloatingBehavior";
                        child.flowLayout = this._FlowLayout;

                        // make the item draggable by adding floating behavior to it                    
                        var floatingBehavior = $create(CMSExtendedControls.FloatingBehavior,
                            { "DragHandleID": handleId, "id": behaviorId, "name": behaviorId, "itemGroup": this._ItemGroupValue, "flowLayout": this._FlowLayout }, {}, {}, child);

                        floatingBehavior._HighlightDropableAreas = this._HighlightDropableAreasValue;
                        Array.add(this._floatingBehaviors, floatingBehavior);
                    }
                }
                if (window.recursiveDragAndDrop) {
                    // Recursively initialize child elements
                    this._initializeDraggableItems(dummy, child);
                }
                child = child.nextSibling;
            }
        }
    },

    get_FlowLayout: function () {
        return this._FlowLayout;
    },

    set_FlowLayout: function (value) {
        this._FlowLayout = value;
    },

    get_DragItemClass: function () {
        return this._DragItemClassValue;
    },

    set_DragItemClass: function (value) {
        this._DragItemClassValue = value;
    },

    get_DropCueID: function () {
        return this._DropCueIDValue;
    },

    set_DropCueID: function (value) {
        this._DropCueIDValue = value;
    },

    get_DragItemHandleClass: function () {
        return this._DragItemHandleClassValue;
    },

    set_DragItemHandleClass: function (value) {
        this._DragItemHandleClassValue = value;
    },

    get_ItemGroup: function () {
        return this._ItemGroupValue;
    },

    set_ItemGroup: function (value) {
        this._ItemGroupValue = value;
    },

    get_HighlightDropableAreas: function () {
        return this._HighlightDropableAreasValue;
    },

    set_HighlightDropableAreas: function (value) {
        this._HighlightDropableAreasValue = value;
    },

    getDescriptor: function () {
        var td = CMSExtendedControls.DragAndDropBehavior.callBaseMethod(this, 'getDescriptor');
        return td;
    },

    // IDropTarget members.
    get_dropTargetElement: function () {
        return this.get_element();
    },

    drop: function (dragMode, type, data) {
        this._cancelCueTimeout();
        this._hideDropCue(data);
        this._placeItem(data);
    },

    canDrop: function (dragMode, dataType, data) {
        var dcp = this._dropCue.parentNode;
        if (this._isHidden(this._dropCue)) {
            return false;
        }

        if ((data != null) && this._isParent(dcp, data.item)) {
            return false;
        }

        if (dataType != this._ItemGroupValue) {
            return false;
        }
        else {
            return true;
        }
    },

    onDragEnterTarget: function (dragMode, type, data) {
        //this._showDropCue(data);
        this._cancelCueTimeout();
        this._showCueTimeout = window.setTimeout(Function.createDelegate(this, function () {
            this._showDropCue(data);
        }), 200);
    },

    onDragLeaveTarget: function (dragMode, type, data) {
        this._cancelCueTimeout();
        this._hideCueTimeout = window.setTimeout(Function.createDelegate(this, this._hideDropCue), 700);
        //this._hideDropCue(data);
    },

    onDragInTarget: function (dragMode, type, data) {
        //this._cancelCueTimeout();
        if (!this._repositioningInProgress) {
            this._repositioningInProgress = true;
            if (this._FlowLayout) {
                this._repositionDropCue(data);
                this._repositioningInProgress = false;
            }
            else {
                window.setTimeout(Function.createDelegate(this, function () {
                    this._repositionDropCue(data);
                    this._repositioningInProgress = false;
                }), 200);
            }
        }
    },

    _cancelCueTimeout: function () {
        if (this._hideCueTimeout != null) {
            //this._hideDropCue();
            window.clearTimeout(this._hideCueTimeout);
            this._hideCueTimeout = null;
        }
        if (this._showCueTimeout != null) {
            window.clearTimeout(this._showCueTimeout);
            this._showCueTimeout = null;
        }
    },

    _findFirstItem: function (el) {
        if (el == null) {
            el = this.get_element();
        }

        var child = el.firstChild;
        while (child != null) {
            if (this._matchedItem(child)) {
                return child;
            }
            if (window.recursiveDragAndDrop) {
                // Find recursively
                var recursive = this._findFirstItem(child);
                if (recursive != null) {
                    return recursive;
                }
            }
            child = child.nextSibling;
        }

        return null;
    },

    _collectChildren: function (par, children) {
        var child = par.firstChild;

        while (child != null) {
            if (this._matchedItem(child)) {
                children.push(child);
            }
            if (window.recursiveDragAndDrop) {
                this._collectChildren(child, children);
            }

            child = child.nextSibling;
        }
    },

    _findItemAt: function (x, y, item, el) {
        if (el == null) {
            el = this.get_element();
        }

        var children = new Array();
        this._collectChildren(el, children);

        var yOffset = (window.dragYOffset ? window.dragYOffset : 0);

        var dc = this._dropCue;
        var h = 22;
        var w = null;

        var fl = dc.style.cssFloat;
        if (fl == null) {
            fl = dc.style.styleFloat;
        }
        var right = (fl == "right");

        var best = 0;
        var bestMatch = 10000;
        var bestTop = false;

        y -= yOffset;

        for (var i = 0; i < children.length; i++) {
            var child = children[i];
            if (child == item) {
                continue;
            }

            var pos = Sys.UI.DomElement.getLocation(child, true, false, !window.ddScroll);

            var ex = pos.x;
            var ey = pos.y;

            var ey2 = ey + child.offsetHeight;
            var ex2 = ex + child.offsetWidth;

            // Closest positive Y match
            var dy = (y - ey + 10);
            if (dy >= 0) {
                var dx = (x - 0.5 * (ex + ex2));
                var topX = (right ? dx >= 0 : dx < 0);
                if (dx < 0) {
                    dx = -dx;
                }

                var match = dy + 0.00001 * dx;
                if (match < bestMatch) {
                    bestMatch = match;
                    best = i;
                    var topY = (y - (ey + ey2 * 2) / 3) > 0
                    bestTop = (dy < 20) || (topX && topY);
                }
            }
        }

        if (!window.dropAfter) {
            if ((best > 0) || (bestMatch != 10000) && bestTop) {
                do {
                    best = best + 1;
                }
                while ((children.length > best) && (children[best] == item))
            }
        }

        if (best >= children.length) {
            return null;
        }
        else {
            return children[best];
        }
    },

    _showDropCue: function (data) {
        this._repositionDropCue(data);

        var dc = this._dropCue;
        var dcs = dc.style;
        data.item.flowLayout = this._FlowLayout;

        if (this._FlowLayout) {
            dcs.position = 'absolute';

            var dcp = dc.parentNode;
            dcp.className = dcp.className.replace(/ ActiveDropArea$/g, '');
            dcp.className += " ActiveDropArea";
        }
        else {
            dcs.display = "block";
            dcs.visibility = "visible";

            var bounds = Sys.UI.DomElement.getBounds(data.item);

            var h = 22;
            var w = null;

            var fl = dcs.cssFloat;
            if (fl == null) {
                fl = dcs.styleFloat;
            }
            if (fl != null) {
                w = data.item.offsetWidth;
                h = data.item.offsetHeight;
            }

            var newheight = h.toString() + "px";
            if (dcs.height != newheight) {
                dcs.height = newheight;
            }

            var newwidth = ((w != null) ? w.toString() + 'px' : null);
            if (dcs.width != newwidth) {
                dcs.width = newwidth;
            }

            if (typeof (OnShowDropCue) == "function") {
                OnShowDropCue(this._dropCue, data.item);
            }
        }
    },

    _hideDropCue: function (data) {
        this._hideCueTimeout = null;
        var dc = this._dropCue;
        var dcs = dc.style;

        dcs.display = "none";
        dcs.visibility = "hidden";
        dcs.height = "";
        dcs.width = "";

        var dcp = dc.parentNode;
        dcp.className = dcp.className.replace(/ ActiveDropArea$/g, '');

        if (typeof (OnHideDropCue) == "function") {
            OnHideDropCue(dc);
        }
    },

    _repositionDropCue: function (data) {
        if ((window._event != null) && !this._FlowLayout) {
            var ctrlPressed = window._event.ctrlKey;
            if (!window.captureCueCtrl && ctrlPressed) {
                this._hideDropCue();
                return;
            }
        }

        var location = Sys.UI.DomElement.getLocation(data.item, false, true);

        if (typeof (OnRepositionDropCue) == "function") {
            location = OnRepositionDropCue(data.item, location);
        }

        var dc = this._dropCue;
        dc.notUseForResize = true;

        var dcs = dc.style;

        if (this._FlowLayout) {
            dcs.left = location.x + 'px';
            dcs.top = location.y + 'px';
        }
        else {
            var nearestChild = this._findItemAt(location.x, location.y, data.item);

            var dropElemAfter = window.dropAfter;

            dc.className = dc.className.replace(/Shift$/, '');
            dc.className = dc.className.replace(/Ctrl$/, '');
            dc.className = dc.className.replace(/Left$/, '');

            var par = dc.parentNode;
            var childPar = null;

            if (par != null) {
                if (nearestChild == null) {
                    var last = par.lastChild;
                    while ((last.className == null) || (last.className == 'ClearBoth')) {
                        last = last.previousSibling;
                    }
                    if (last != dc) {
                        if (last.nextSibling) {
                            par.insertBefore(dc, last.nextSibling);
                        }
                        else {
                            par.appendChild(dc);
                        }
                    }
                }
                else {
                    childPar = nearestChild.parentNode;
                    if (childPar != null) {
                        if (dropElemAfter) {
                            if (nearestChild.nextSibling != dc) {
                                par.removeChild(dc);
                                if (nearestChild.nextSibling) {
                                    childPar.insertBefore(dc, nearestChild.nextSibling);
                                }
                                else {
                                    childPar.appendChild(dc);
                                }
                            }
                        }
                        else if (nearestChild.previousSibling != dc) {
                            par.removeChild(dc);
                            childPar.insertBefore(dc, nearestChild);
                        }

                        var leftCueOffset = window.leftCueOffset;
                        if (leftCueOffset != null) {
                            var childLocation = Sys.UI.DomElement.getLocation(nearestChild, false, true);
                            var isOnLeft = false;
                            var rtl = (document.body.className.indexOf('RTL') >= 0);
                            if (rtl) {
                                isOnLeft = ((childLocation.x + nearestChild.offsetWidth - location.x - data.item.offsetWidth) < leftCueOffset);
                            }
                            else {
                                isOnLeft = ((location.x - childLocation.x) < leftCueOffset);
                            }

                            if (isOnLeft) {
                                dc.className += "Left";
                            }
                            dc.isOnLeft = isOnLeft;
                            childPar.isOnLeft = isOnLeft;
                        }
                    }
                }
            }
        }

        if (window._event != null) {
            var ctrlPressed = window._event.ctrlKey;
            data.item.notUseForResize = ctrlPressed;

            if (window.captureCueCtrl) {
                var shiftPressed = window._event.shiftKey;

                if (ctrlPressed) {
                    dc.className += "Ctrl";

                    if (window.captureCueCtrlShift) {
                        if (shiftPressed) {
                            dc.className += "Shift";
                        }
                    }
                }

                dc.ctrlPressed = ctrlPressed;
                dc.shiftPressed = shiftPressed;

                if (childPar != null) {
                    childPar.ctrlPressed = ctrlPressed;
                    childPar.shiftPressed = shiftPressed;
                }
            }
        }
    },

    _isHidden: function (e) {
        var n = e.parentNode;
        var abs = false;
        while (n.style != null) {
            var ns = n.style;
            if ((!abs && (ns.height == "0px")) || (ns.display == "none") || (ns.visibility == "hidden")) {
                return false;
            }
            if (ns.position == "absolute") {
                abs = true;
            }
            n = n.parentNode;
        }
    },

    _isParent: function (e, par) {
        var ep = e.parentNode;
        while (ep != null) {
            if (ep == par) return true;
            ep = ep.parentNode;
        }
        return false;
    },

    _placeItem: function (data) {
        var el = this.get_element();
        var par = this._dropCue.parentNode;
        if (par == null) {
            par = el;
        }

        par.notResize = true;
        while (par.isResizing) {
        }

        var e = data.item;
        var epar = e.parentNode;
        epar.notResize = true;
        while (epar.isResizing) {
        }

        var st = e.style;

        var cueClass = this._dropCue.className;
        var flow = / CueFree$/.test(cueClass)

        e.keepPosition = (flow && (e.parentNode == par));

        this._raiseBeforeDropEvent( /* Container */el, /* dropped item */e, /* position */position);

        var loc = Sys.UI.DomElement.getLocation(e, false, false, true);
        var position = 0;

        var cancelled = false;
        if (e.getAttribute('data-dragcancelled') != "1") {
            if (!e.keepPosition) {
                if (this._isParent(par, e)) {
                    alert('Cannot drop item to its own child node.');
                    return;
                }
                epar.removeChild(e);
                par.insertBefore(e, this._dropCue);
            }
        }
        else {
            cancelled = true;
        }

        e.removeAttribute('data-dragcancelled');

        if (cancelled) {
            if (e.wasFlow) {
                st.position = 'relative';
                st.left = e.originalLeft;
                st.top = e.originalTop;
                st.height = '0px';
                st.width = '';
                e.flowLayout = true;
            }
            return;
        }
        else {
            // Find the position of the dropped item
            if (!e.keepPosition) {
                var item = par.firstChild;
                while ((item != e) && (item != null)) {
                    if (this._matchedItem(item)) {
                        position++;
                    }
                    item = item.nextSibling;
                }
            }

            // Handle free layout
            el.flowX = el.flowY = null;

            if (flow) {
                if (st.position == 'absolute') {
                    var ploc = Sys.UI.DomElement.getLocation(e.parentNode, false, false, true);

                    var x = loc.x - ploc.x;
                    var y = loc.y - ploc.y;

                    if (x < 0) x = 0;
                    if (y < 0) y = 0;

                    st.position = 'relative';
                    st.left = x + 'px';
                    st.top = y + 'px';
                    st.height = '0px';

                    e.flowX = x;
                    e.flowY = y;
                }
            }
            st.width = '';

            // Change the envelope if needed
            var inner = this._findChildByClass(e, 'WebPartInner', null, true);
            if (inner != null) {
                st.position = '';
                st.height = '';
                st.width = '';
                st.float = '';

                inner.style.position = '';

                if (flow) {
                    inner.style.position = 'absolute';
                    e.originalPosition = st.position = 'relative';
                    st.height = '0px';
                }
                else if (/ CueLFloat$/.test(cueClass)) {
                    st.float = 'left';
                }
                else if (/ CueRFloat$/.test(cueClass)) {
                    st.float = 'right';
                }
            }

            this._raiseDropEvent( /* Container */el, /* dropped item */e, /* position */position);
        }

        this._hideDropCue();

        par.notResize = null;
        epar.notResize = null;
    },


    _raiseDropEvent: function (container, droppedItem, position) {
        this._raiseEvent("onDrop", new CMSExtendedControls.DropEventArgs(container, droppedItem, position));
    },

    _raiseBeforeDropEvent: function (container, droppedItem, position) {
        this._raiseEvent("onBeforeDrop", new CMSExtendedControls.DropEventArgs(container, droppedItem, position));
    }
};
CMSExtendedControls.DropEventArgs = function (container, droppedItem, position) {
    CMSExtendedControls.DropEventArgs.initializeBase(this);

    this._container = container;
    this._droppedItem = droppedItem;
    this._position = position;
};
CMSExtendedControls.DropEventArgs.prototype = {
    get_container: function () {
        return this._container;
    },
    get_droppedItem: function () {
        return this._droppedItem;
    },
    get_position: function () {
        return this._position;
    }
};
// Register the behavior
var dragDropBehaviorRegistered = false;

RegisterCMSDragDropBehavior = function () {
    if (!dragDropBehaviorRegistered) {
        dragDropBehaviorRegistered = true;
        CMSExtendedControls.DragAndDropBehavior.registerClass('CMSExtendedControls.DragAndDropBehavior', Sys.Extended.UI.BehaviorBase, Sys.Extended.UI.IDragSource, Sys.Extended.UI.IDropTarget, Sys.IDisposable);
        CMSExtendedControls.DropEventArgs.registerClass("CMSExtendedControls.DropEventArgs", Sys.EventArgs);
        RegisterCMSFloatingBehavior();
    }
};
/* Bug fix for AJAX function Sys.UI.DomElement._getCurrentStyle */
Sys.UI.DomElement._getCurrentStyle = function Sys$UI$DomElement$_getCurrentStyle(element) {
    if (element.nodeName != "#text") {
        var w = (element.ownerDocument ? element.ownerDocument : element.documentElement).defaultView;
        return ((w && (element !== w) && w.getComputedStyle) ? w.getComputedStyle(element, null) : element.style);
    }
    return null;
};
