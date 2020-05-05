// FloatingBehavior.js
Type.registerNamespace('CMSExtendedControls');

// Fix for the Sys.UI.DomElement.getLocation
Sys.UI.DomElement.getLocation = function (element, allowFixed, absoluteOffset, notScrolling) {
    var offsetX = 0;
    var offsetY = 0;
    var parent;
    try {
        for (parent = element; parent; parent = parent.offsetParent) {
            if (!allowFixed && (parent.style.position == 'fixed')) break;

            if (parent.offsetLeft) {
                offsetX += parent.offsetLeft;
            }
            if (parent.offsetTop) {
                offsetY += parent.offsetTop;
            }
        }
    }
    catch (ex) { }

    var loc = { x: offsetX, y: offsetY };

    if (absoluteOffset) {
        var pLoc = GetPredecessorAbsoluteOffset(element);
        loc.x -= pLoc.x;
        loc.y -= pLoc.y;
    }

    if (!notScrolling) {
        var sLoc = GetScrollOffset(element);
        if (sLoc != null) {
            loc.x -= sLoc.x;
            loc.y -= sLoc.y;
        }
    }

    return loc;
};

function GetPredecessorAbsoluteOffset(element) {
    var offsetX = 0;
    var offsetY = 0;
    var parent;

    if (element.style && (element.style.position == 'absolute')) {
        return { x: 0, y: 0 };
    }

    element = element.offsetParent;
    for (parent = element; parent; parent = parent.offsetParent) {
        var position = $cmsj(parent).css('position');
        if ((position == 'absolute') || (position == 'relative')) {
            if (parent.offsetLeft) {
                offsetX += parent.offsetLeft;
            }
            if (parent.offsetTop) {
                offsetY += parent.offsetTop;
            }
        }
    }

    return { x: offsetX, y: offsetY };
}

function GetScrollOffset(element) {
    var offsetX = 0;
    var offsetY = 0;
    var parent;

    if (element.style && (element.style.position == 'absolute')) {
        return { x: 0, y: 0 };
    }

    element = element.parentNode;
    for (parent = element; parent; parent = parent.parentNode) {
        if (typeof (parent.tagName) == 'undefined') {
            break;
        }
        var t = parent.tagName.toLowerCase();
        if ((t == 'html') || (t == 'body')) {
            break;
        }

        if (parent.scrollLeft) {
            offsetX += parent.scrollLeft;
        }
        if (parent.scrollTop) {
            offsetY += parent.scrollTop;
        }
    }

    var loc = { x: offsetX, y: offsetY };

    return loc;
}

getZonesByClassName = function (className, results) {
    var hasClassName = new RegExp("(?:^|\\s)" + className + "(?:$|\\s)");
    var allElements = document.getElementsByTagName("div");

    var element;
    for (var i = 0; (element = allElements[i]) != null; i++) {
        var elementClass = element.className;
        if (elementClass && elementClass.indexOf(className) != -1 && hasClassName.test(elementClass))
            results.push(element);
    }

    return results;
};
var webPartZones = [];
function LoadWebPartZones() {
    webPartZones = getZonesByClassName("WebPartZoneBorder", webPartZones);
    webPartZones = getZonesByClassName("WebPartZoneBorderActive", webPartZones);
};

$addHandler(window, 'load', function () { LoadWebPartZones(); });

CMSExtendedControls.FloatingBehavior = function (element) {
    CMSExtendedControls.FloatingBehavior.initializeBase(this, [element]);

    var _flowLayout;
    var _handle;
    var _location;
    var _dragStartLocation;
    var _DragHandleIDValue;
    var _canDrag;
    var _originalY;
    var _originalX;
    var _itemGroup;
    var _HighlightDropableAreas;

    var _mouseDownHandler = Function.createDelegate(this, mouseDownHandler);
    var _mouseUpHandler = Function.createDelegate(this, mouseUpHandler);
    var _mouseMoveHandler = Function.createDelegate(this, mouseMoveHandler);

    this.addClass = function (element, value) {
        if (!element.className) {
            element.className = value;
        } else {
            element.className = element.className + " " + value;
        }
    };

    this.removeClass = function (element, value) {
        var re = new RegExp("\\s*(\\b|^)" + value + "(\\b|$)");
        element.className = element.className.replace(re, '');
    };
    this.get_DragHandleID = function () { return this._DragHandleIDValue; };
    this.set_DragHandleID = function (value) { this._DragHandleIDValue = value; };
    this.add_move = function (handler) { this.get_events().addHandler('move', handler); };
    this.remove_move = function (handler) { this.get_events().removeHandler('move', handler); };
    this.get_handle = function () {
        return _handle;
    };
    this.get_movehandle = function () {
        return _movehandle;
    };
    this.set_handle = function (value) {
        if (_handle != null) {
            $removeHandler(_handle, "mousedown", _mouseDownHandler);
            $removeHandler(_handle, "mouseup", _mouseUpHandler);
            $removeHandler(_handle, "mousemove", _mouseMoveHandler);
        }

        _handle = value;
        $addHandler(_handle, "mousedown", _mouseDownHandler);
        $addHandler(_handle, "mouseup", _mouseUpHandler);
        $addHandler(_handle, "mousemove", _mouseMoveHandler);
    };
    this.get_itemGroup = function () {
        return this._itemGroup;
    };
    this.set_itemGroup = function (value) {
        this._itemGroup = value;
    };

    this.get_flowLayout = function () {
        return this._flowLayout;
    };
    this.set_flowLayout = function (value) {
        this._flowLayout = value;
    };

    this.get_location = function () {
        return _location;
    };
    this.set_location = function (value) {
        if (_location != value) {
            _location = value;
            if (this.get_isInitialized()) {

                Sys.UI.DomElement.setLocation(this.get_element(), _location.x, _location.y);
            }
            this.raisePropertyChanged('location');
        }
    };

    this.initialize = function () {

        RegisterCMSFloatingBehavior();

        CMSExtendedControls.FloatingBehavior.callBaseMethod(this, 'initialize');

        // Set the handle and initialize dragging
        this.set_handle($get(this.get_DragHandleID()));
    };

    this.dispose = function () {
        if (_handle && _mouseDownHandler) {
            $removeHandler(_handle, "mousedown", _mouseDownHandler);
        }
        _mouseDownHandler = null;
        CMSExtendedControls.FloatingBehavior.callBaseMethod(this, 'dispose');
    };

    this.checkCanDrag = function (element) {
        var undraggableTagNames = ["input", "button", "select", "textarea", "label"];
        var undraggableClassNames = ["notdraggable"];

        var tagName = element.tagName;
        var className = element.className;

        if ((tagName.toLowerCase() == "a") && (element.href != null) && (element.href.length > 0)) {
            return false;
        }
        if (Array.indexOf(undraggableTagNames, tagName.toLowerCase()) > -1) {
            return false;
        }
        if (Array.indexOf(undraggableClassNames, className.toLowerCase()) > -1) {
            return false;
        }

        return true;
    };

    this.startDrag = function () {
        var el = this.get_element();

        // Get the location before making the element absolute
        // Double negation ensures passing false value when 'window.ddNotScroll = undefined'
        _location = Sys.UI.DomElement.getLocation(el, false, true, !!window.ddNotScroll);

        setBodyHeightToContentHeight();

        // Use a copy mode if the dragged element has the attribute 'data-dragKeepCopy' set
        var itemAttrKeepCopy = (el.getAttribute('data-dragkeepcopy') == 1);

        // Create a copy of the element in copy mode
        if (window.dragKeepCopy || itemAttrKeepCopy) {
            var copyElem = el.cloneNode(true);
            el.copyElem = copyElem;
            el.parentNode.insertBefore(copyElem, el);

            if (typeof (OnCopyDraggedItem) == "function") {
                OnCopyDraggedItem(copyElem, el);
            }
        }

        // Make sure that the widget header will not break its layout when dragged
        if (el.className.indexOf("EditorWidget") != -1) {
            var jHandle = $cmsj(".WebPartHandle", el);
            if (jHandle != null) {
                jHandle.css("width", jHandle.width());
            }
        }

        el.originalMinHeight = el.style.minHeight;

        // Make the element absolute
        if ((el.firstChild == null) || (el.firstChild.style == null) || (el.firstChild.style.position != 'absolute')) {
            el.style.width = el.offsetWidth + "px";
        } else {
            el.firstChild.style.width = (el.firstChild.offsetWidth + 2) + "px";
            el.childWidthSet = true;
        }

        //el.style.height = el.offsetHeight + "px";            
        Sys.UI.DomElement.setLocation(el, _location.x, _location.y);

        if (typeof (OnBeforeStartDrag) == "function") {
            OnBeforeStartDrag(el);
        }

        _dragStartLocation = Sys.UI.DomElement.getLocation(el, false, true);

        el.wasFlow = this._flowLayout;
        el.originalLeft = el.style.left;
        el.originalTop = el.style.top;

        this.startDragDrop(el);

        // Hack for restoring position to static
        el.originalPosition = (this._flowLayout ? "relative" : "static");
        el.originalZIndex = el.style.zIndex;
        el.style.zIndex = "10000";

        if (window.tagDraggedElem && !/Dragged$/.test(el.className)) {
            el.className += 'Dragged';
        }

        if (window.useDraggedClass) {
            this.addClass(el, "DraggedWebPart");
        }

        if (this._HighlightDropableAreas) {
            for (i = 0; i < webPartZones.length; i++) {
                this.addClass(webPartZones[i], "WebPartZoneHighlight");
            }
        }

        window.isDragging = true;
    };

    function setBodyHeightToContentHeight() {
        document.body.style.height = Math.max(document.documentElement.scrollHeight, document.body.scrollHeight) + "px";
    };

    function mouseDownHandler(ev) {
        if (ev.button == 2) {
            return;
        }

        window._event = ev;

        if (!this.checkCanDrag(ev.target)) return;

        window.suppressDrag = false;
        this._canDrag = true;
        this._originalY = ev.clientY;
        this._originalX = ev.clientX;
        suppressOnClick = false;

        //ev.preventDefault();
        //this.startDrag();
    };

    function mouseUpHandler(ev) {
        window._event = ev;

        this._canDrag = false;
        suppressOnClick = false;
        ev.preventDefault();
    };

    function mouseMoveHandler(ev) {
        window._event = ev;

        if (this._canDrag && !window.suppressDrag) {
            ev.preventDefault();

            if ((Math.abs(this._originalY - ev.clientY) > 2) || (Math.abs(this._originalX - ev.clientX) > 2)) {
                this._canDrag = false;
                this.startDrag();
                suppressOnClick = true;
            }
        }
    };

    // Type get_dataType()
    this.get_dragDataType = function () { return this._itemGroup; }; // Object get_data(Context)
    this.getDragData = function (context) { return { item: this.get_element(), handle: this.get_handle() }; }; // DragMode get_dragMode()
    this.get_dragMode = function () { if (window.dragMode) { return window.dragMode; } else { return Sys.Extended.UI.DragMode.Move; } }; // void onDragStart()
    this.onDragStart = function () { }; // void onDrag()
    this.onDrag = function () { }; // void onDragEnd(Canceled)
    this.onDragEnd = function (canceled) {
        window.isDragging = false;

        if (!canceled) {
            var handler = this.get_events().getHandler('move');
            if (handler) {
                var cancelArgs = new Sys.CancelEventArgs();
                handler(this, cancelArgs);
                canceled = cancelArgs.get_cancel();
            }
        }

        if (this._HighlightDropableAreas) {
            for (i = 0; i < webPartZones.length; i++) {
                var elem = webPartZones[i];
                this.removeClass(elem, "WebPartZoneHighlight");
                elem.className = elem.className.replace('WebPartZoneBorderActive', ' WebPartZoneBorder ');
            }
        }

        var el = this.get_element();

        var st = el.style;
        if (!el.flowLayout) {
            st.left = st.top = st.minHeight = st.width = st.height = null;
        }
        if (el.childWidthSet) {
            el.firstChild.style.width = null;
            el.childWidthSet = false;
        }
        else {
            el.style.width = '';
        }
        st.zIndex = el.originalZIndex;

        // Restore the widget header style attributes after drag ends
        if (el.className.indexOf("EditorWidget") != -1) {
            var jHandle = $cmsj(".WebPartHandle", el);
            if (jHandle != null) {
                jHandle.css("width", "");
            }
        }

        if (window.useDraggedClass) {
            this.removeClass(el, "DraggedWebPart");
        }

        if (window.tagDraggedElem) {
            el.className = el.className.replace(/Dragged(\b|$)/, '');
        }

        if (el.copyElem != null) {
            var target = el.parentNode;

            target.removeChild(el);
            el.copyElem.parentNode.insertBefore(el, el.copyElem);
            el.copyElem.parentNode.removeChild(el.copyElem);
        }

        if (typeof (OnDragEnd) == "function") {
            OnDragEnd(el);
        }

        window.allowClick = new Date(new Date().getTime() + 100);
    };
    this.startDragDrop = function (dragVisual) { Sys.Extended.UI.DragDropManager.startDragDrop(this, dragVisual, null); };
    this.get_dropTargetElement = function () { return document.body; };
    this.canDrop = function (dragMode, dataType, data) { return true; };
};

var floatingBehaviorRegistered = false;

RegisterCMSFloatingBehavior = function () {
    if (!floatingBehaviorRegistered) {
        floatingBehaviorRegistered = true;
        CMSExtendedControls.FloatingBehavior.registerClass('CMSExtendedControls.FloatingBehavior', Sys.Extended.UI.BehaviorBase, Sys.Extended.UI.IDragSource, Sys.IDisposable);
    }
};


