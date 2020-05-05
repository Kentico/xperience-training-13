var firstLayoutX = 0;
var firstLayoutY = 0;

var initialWidth = 0;
var initialHeight = 0;

var currentWidth = 0;
var currentHeight = 0;

var currentLayoutElem = null;
var layoutOverlayElem = null;

var layoutInfoElem = null;
var layoutInfoText = null;

var lastLayoutElem = null;
var lastResizedElem = null;

var textBoxEdit = null;
var textAreaEdit = null;
var colorEdit = null;

var propOverlay = null;
var activeEditor = null;

function IsWebKit() {
    var s = navigator.userAgent.toLowerCase() + '';
    if (s.indexOf('applewebkit/') >= 0) {
        return true;
    }
    return false;
}

function Get(elemId) {
    return document.getElementById(elemId);
}

function UpdateLayoutInfo(el) {
    if (el != null) {
        var je = $cmsj(el);
        var pos = je.offset();

        var x = pos.left;
        var y = pos.top;

        var w = el.offsetWidth;
        var h = el.offsetHeight;

        var inS = layoutInfoElem.style;
        inS.top = y + 'px';
        inS.left = x + 'px';

        inS.width = (w - 2) + 'px';
        inS.height = (h - 2) + 'px';

        var inT = layoutInfoText;
        inT.innerHTML = (w == initialWidth ? w : '<strong>' + w + '</strong>') + ' x ' + (h == initialHeight ? h : '<strong>' + h + '</strong>');

        inT.style.top = (y + h / 2 - inT.offsetHeight / 2) + 'px';
        inT.style.left = (x + w / 2 - inT.offsetWidth / 2) + 'px';
    }
}

function IsLeftButton(ev) {
    var bt = ev.button;
    var s = navigator.userAgent.toLowerCase() + '';
    var b = $cmsj.browser;
    if (b.msie) {
        return (bt == 0) || (bt == 1);
    }
    else if (b.mozilla || b.opera || b.webkit) {
        return (bt == 0);
    }
    else {
        return (bt == 1);
    }
}

function InitHorizontalResizer(ev, elem, clientId, elementId, propertyName, inverted, infoId, callback) {
    if (ev == null) ev = window.event;

    if (IsLeftButton(ev)) {
        firstLayoutX = ev.clientX;
        firstLayoutY = ev.clientY;

        currentLayoutElem = elem;

        var elId = elementId;
        if (elId.indexOf('_') < 0) {
            elId = clientId + '_' + elId;
        }
        var el = Get(elId);

        var inId = infoId;
        if (inId == null) {
            inId = elId;
        }
        else if ((inId != '#') && (inId.indexOf('_') < 0)) {
            inId = clientId + '_' + inId;
        }

        var e = elem;
        if (e.horzProperty == null) {
            e.horzProperty = propertyName;
            e.horzClientId = clientId;
            e.horzElementId = elId;
            e.horzInverted = inverted;
            e.horzInfoId = inId;
            e.horzCallback = callback;
        }

        currentWidth = initialWidth = el.offsetWidth;
        currentHeight = initialHeight = el.offsetHeight;

        var infoElem = Get(inId);
        EnsureOverlay('e-resize', infoElem);

        InitReset(elem);

        lastResizedElem = el;
        lastLayoutElem = elem;

        el.vertProperty = null;
        el.horzProperty = propertyName;
        el.horzClientId = clientId;

        el.notResize = true;
    }
}

function InitVerticalResizer(ev, elem, clientId, elementId, propertyName, infoId, callback) {
    if (ev == null) ev = window.event;

    if (IsLeftButton(ev)) {
        firstLayoutX = ev.clientX;
        firstLayoutY = ev.clientY;

        currentLayoutElem = elem;

        var elId = elementId;
        var el = Get(elId);
        if (el == null) {
            elId = clientId + '_' + elId;
            el = Get(elId);
        }

        var inId = infoId;
        if (inId == null) {
            inId = elId;
        }
        else if ((inId != '#') && (inId.indexOf('_') < 0)) {
            inId = clientId + '_' + inId;
        }

        var e = elem;
        if (e.vertProperty == null) {
            e.vertProperty = propertyName;
            e.vertClientId = clientId;
            e.vertElementId = elId;
            e.vertInfoId = inId;
            e.vertCallback = callback;
        }

        currentWidth = initialWidth = el.offsetWidth;
        currentHeight = initialHeight = el.offsetHeight;

        var infoElem = Get(inId);
        EnsureOverlay('n-resize', infoElem);

        InitReset(elem);

        lastResizedElem = el;
        lastLayoutElem = elem;

        el.vertProperty = propertyName;
        el.vertClientId = clientId;
        el.horzProperty = null;

        el.notResize = true;
    }
}

function InitReset(e) {
    if (e.ondblclick != ResetProperties) {
        e.ondblclick = ResetProperties;
    }
}

function ResetProperties(ev) {
    if (lastResizedElem != null) {
        elem = lastResizedElem;
        if (elem.vertProperty != null) {
            SetWebPartProperty(elem.vertClientId, elem.vertProperty, '');
            elem.style.height = 'auto';
        }

        if (elem.horzProperty != null) {
            SetWebPartProperty(elem.horzClientId, elem.horzProperty, '');
            elem.style.width = 'auto';
        }
    }

    return false;
}

function LayoutMouseMove(ev) {
    if (ev == null) ev = window.event;

    var el = currentLayoutElem;
    if (el != null) {
        if (el.horzProperty != null) {
            var x = ev.clientX;
            var dx = firstLayoutX - x;
            if (el.horzInverted) {
                dx = -dx;
            }
            var elem = Get(el.horzElementId);
            currentWidth = initialWidth - dx;
            if (elem.originalWidth != null) elem.originalWidth = currentWidth;
            elem.style.width = currentWidth + 'px';

            if (el.horzInfoId != '#') {
                var infoElem = Get(el.horzInfoId);
                UpdateLayoutInfo(infoElem);
            }
        }

        if (el.vertProperty != null) {
            var y = ev.clientY;
            var dy = firstLayoutY - y;
            var elem = Get(el.vertElementId);
            currentHeight = initialHeight - dy;
            if (elem.originalHeight != null) elem.originalHeight = currentHeight;
            elem.style.height = currentHeight + 'px';

            if (el.vertInfoId != '#') {
                var infoElem = Get(el.vertInfoId);
                UpdateLayoutInfo(infoElem);
            }
        }

        if (el.resized) {
            el.resized(el);
        }
    }

    ev.returnValue = false;
    return false;
}

function LayoutMouseUp(ev) {
    if (currentLayoutElem != null) {
        var elem = currentLayoutElem;

        if ((elem.vertProperty != null) && (currentHeight != initialHeight)) {
            var h = currentHeight + 'px';
            SetWebPartProperty(elem.vertClientId, elem.vertProperty, h);
            if (elem.vertCallback) {
                elem.vertCallback(h);
            }
            Get(elem.vertElementId).notResize = null;
        }

        if ((elem.horzProperty != null) && (currentWidth != initialWidth)) {
            var w = currentWidth + 'px';
            SetWebPartProperty(elem.horzClientId, elem.horzProperty, w);
            if (elem.horzCallback) {
                elem.horzCallback(w);
            }
            Get(elem.horzElementId).notResize = null;
        }
    }

    currentLayoutElem = null;
    firstLayoutX = 0;
    firstLayoutY = 0;
    initialWidth = 0;
    initialHeight = 0;

    HideOverlay();

    return true;
}

function EnsureOverlay(cursor, el) {
    var inE = layoutInfoElem;
    if (inE == null) {
        inE = document.createElement('div');
        inE.className = 'LayoutInfoOverlay';
        inE.innerHTML = '&nbsp;';
        layoutInfoElem = inE;

        document.body.appendChild(inE);

        var inT = document.createElement('div');
        inT.className = 'LayoutInfoText';
        inT.innerHTML = '&nbsp;';
        layoutInfoText = inT;

        document.body.appendChild(inT);
    }

    var ovE = layoutOverlayElem;
    if (ovE == null) {
        ovE = document.createElement('div');
        ovE.className = 'LayoutOverlay';
        ovE.innerHTML = '&nbsp;';

        ovE.onmousemove = LayoutMouseMove;
        ovE.onmouseup = LayoutMouseUp;
        layoutOverlayElem = ovE;

        document.body.appendChild(ovE);
    }

    ovE.style.display = 'block';
    ovE.style.cursor = cursor;

    if (el != null) {
        inE.style.display = 'block';
        layoutInfoText.style.display = 'block';
    }

    UpdateLayoutInfo(el);
}

function HideOverlay() {
    layoutOverlayElem.style.display = 'none';
    if (layoutInfoElem != null) {
        layoutInfoElem.style.display = 'none';
        layoutInfoText.style.display = 'none';
    }
}

function SetCookie(cookieName, cookieValue, days) {
    var today = new Date();
    var expire = new Date();
    if ((days == null) || (days == 0)) {
        days = 1;
    }

    expire.setTime(today.getTime() + 3600000 * 24 * days);
    document.cookie = cookieName + "=" + escape(cookieValue) + ";expires=" + expire.toGMTString();
}

function ChangeLayoutItem(clientId, offset, instanceGuid, containerId) {
    var id = clientId + "_current";
    var curElem = Get(id);
    var items = parseInt(Get(clientId + "_items").value);

    var cur = parseInt(curElem.value);
    if ((cur >= 1) && (cur <= items) && (offset != 0)) {
        Get(clientId + "_item" + cur).style.display = 'none';
    }
    cur += offset;
    if (cur < 1) { cur = 1; }
    if (cur > items) { cur = items; }
    if ((cur >= 1) && (cur <= items)) {
        var el = Get(clientId + "_item" + cur);
        if (offset != 0) {
            el.style.display = 'block';
        }
    }
    curElem.value = cur;

    if (containerId != null) {
        var cont = Get(containerId);
        if (cont != null) {
            cont.currentStep = cur;
        }
    }

    if (instanceGuid != null) {
        SetCookie("CMSEd" + instanceGuid + "Current", cur, 1);
    }
}

function PreviousLayoutItem(clientId, containerId) {
    ChangeLayoutItem(clientId, -1, null, containerId);
}

function NextLayoutItem(clientId, containerId) {
    ChangeLayoutItem(clientId, 1, null, containerId);
}

function InitAutoResize(clientId, cname, adjust) {
    $cmsj(document).ready(function () {
        AutoResize(clientId, cname, adjust);
        setInterval("AutoResize('" + clientId + "', '" + cname + "', " + adjust + ")", 500);
    });
}

function CollectLocations(par, loc, cname, level) {
    var found = false;

    level -= 1;
    if (level == 0) {
        return false;
    }

    var child = par.firstChild;
    if (child == null) {
        return false;
    }

    while (child != null) {
        var c = child.className;
        var cmatch = ((c != null) && ((c == cname) || (c.startsWith(cname + ' '))));

        if (((cname == null) || cmatch) && !child.notUseForResize) {
            CollectLocations(child, loc, null, level);

            var jc = $cmsj(child);
            var ploc = jc.offset();

            if (ploc.top != null) {
                ploc.top += child.offsetHeight;
                if (ploc.top > loc.top) loc.top = ploc.top;
            }

            if ((ploc.left != null) && cmatch && (child.firstChild != null)) {
                var w = jc.children("div").width();
                ploc.left += w;
                if (ploc.left > loc.left) {
                    loc.left = ploc.left;
                }
            }

            found = true;
        }

        child = child.nextSibling;
    }

    return found;
}

function EqualHeight(groupClass) {
    if (currentLayoutElem == null) {
        $cmsj('.' + groupClass).equalHeight();
    }
}

function InitEqualHeight(groupClass) {
    setInterval('EqualHeight("' + groupClass + '")', 500);
}

function SetAllHeight(groupClass, h) {
    $cmsj('.' + groupClass).css('height', h);
}

function IsHidden(e) {
    var n = e;
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
}

function AutoResize(clientId, cname, adjust) {
    var par = Get(clientId);
    if ((par == null) || par.notResize) {
        return;
    }

    if (IsHidden(par)) {
        if (!par.wasHidden) {
            par.wasHidden = true;
            par.origHTML = par.innerHTML;
        }
        return;
    }
    // Check if the code had changed, if not do not resize
    if ((par.origHTML != null) && (par.origHTML == par.innerHTML)) {
        if (par.wasHidden) {
            par.wasHidden = false;
        }
        else {
            return;
        }
    }

    par.isResizing = true;

    try {
        var ploc = $cmsj(par).offset();
        var mloc = { left: ploc.left, top: ploc.top };

        if (!CollectLocations(par, mloc, cname, 3) && !adjust) {
            return;
        }

        var s = par.style;
        var marg = 5;

        var newwidth = (mloc.left - ploc.left) + marg;
        var newheight = (mloc.top - ploc.top) + marg;

        var cont = true;

        if (adjust) {
            if ((par.adjustedHeight == null) && (par.adjustedWidth == null)) {
                var p = par.parentNode;
                while (p != null) {
                    var ps = p.style;
                    var jp = $cmsj(p);
                    if (p.tagName == "FORM") {
                        if (p.offsetHeight < document.documentElement.offsetHeight) {
                            newheight = par.offsetHeight + (document.documentElement.offsetHeight - p.offsetHeight) - 1;
                        }
                        par.adjustedHeight = newheight;
                        break;
                    }

                    // Do not take in account the Web part toolbar wrapper tags
                    var isWPTWrapper = (p.className.match(/WPT/) != null);
                    if (isWPTWrapper) {
                        p = p.parentNode;
                        continue;
                    }

                    jp.addClass("default_width_check");

                    var nw = jp.width();
                    var nh = jp.height();

                    if ((nw != 1) || (nh != 1)) {
                        jp.removeClass("default_width_check");


                        if (p.offsetHeight > newheight) {
                            par.adjustedHeight = newheight = p.offsetHeight;
                        }
                        if (p.offsetWidth > newwidth) {
                            par.adjustedWidth = newwidth = p.offsetWidth;
                        }
                        break;
                    }

                    jp.removeClass("default_width_check");

                    p = p.parentNode;
                }
            }
            else {
                if ((par.adjustedWidth != null) && (newwidth < par.adjustedWidth)) {
                    newwidth = par.adjustedWidth;
                }
                if ((par.adjustedHeight != null) && (newheight < par.adjustedHeight)) {
                    newheight = par.adjustedHeight;
                }
            }
        }

        if (s.width != (newwidth + 'px')) {
            if (par.originalWidth == null) par.originalWidth = par.offsetWidth;
            if (newwidth < par.originalWidth) newwidth = par.originalWidth - 2;
            if (s.width != (newwidth + 'px')) {
                s.width = newwidth + 'px';
            }
        }

        if (s.height != (newheight + 'px')) {
            if (par.originalHeight == null) par.originalHeight = par.offsetHeight;
            if (newheight < par.originalHeight) newheight = par.originalHeight - 2;
            if (s.height != (newheight + 'px')) {
                s.height = newheight + 'px';
            }
        }
        s.overflow = 'hidden';
    }
    finally {
        par.isResizing = null;
    }
}

function htmlEncode(value) {
    return $cmsj('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $cmsj('<div/>').html(value).text();
}

function SetHandleClass(id) {
    var e = Get(id);
    e.className = 'WebPartHandle ' + e.className;
}