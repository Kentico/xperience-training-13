var elemMinimize;
var elemMaximize;
var elemMinimizeAll;
var elemMaximizeAll;
var elemBorder;
var originalSizeElem;
var minSizes;
var originalSizes;

function InitResizer() {
    elemMinimize = document.getElementById('imgMinimize');
    elemMaximize = document.getElementById('imgMaximize');
    elemMinimizeAll = document.getElementById('imgMinimizeAll');
    elemMaximizeAll = document.getElementById('imgMaximizeAll');
    elemBorder = document.getElementById('resizerBorder');
    originalSizeElem = document.getElementById('originalSize');

    if (window.minSize != undefined) {
        minSizes = minSize.split(';');
    }

    if (window.originalSizeElem != undefined) {
        originalSizes = originalSizeElem.value.split(';');
    }
}

function ResizerGetParent() {
    var par = window;
    var level = parentLevel;

    while (level > 0) {
        par = par.parent;
        level--;
    }

    return par;
}

function InitAllLayouts(win) {
    if (window.allLayouts == undefined) {
        allLayouts = new Array();
    }
    var ls = win.layouts;
    if (ls != undefined) {
        var i = 0;
        for (i = 0; i < ls.length; i++) {
            allLayouts[allLayouts.length] = ls[i];
        }
    }
    if (win != top) {
        InitAllLayouts(win.parent);
    }
}

function EnsureAllLayouts() {
    if (window.allLayouts == undefined) {
        InitAllLayouts(window);
    }
}

function ClosePane(layout, pane) {
    if (layout.panes[pane]) {
        layout.close(pane);
    }
}

function OpenPane(layout, pane) {
    if (layout.panes[pane]) {
        layout.open(pane);
    }
}

function GetHorizontalPaneName() {
    var rtl = (document.body.className.indexOf('RTL') >= 0);
    var paneName = rtl ? 'east' : 'west';
    return paneName;
}

function ClosePanes(win, recursive, resizeAllPanes) {
    EnsureAllLayouts();
    var ls = win.layouts;
    var q = $cmsj(document.body);
    if (ls != undefined) {
        var i = 0;
        for (i = 0; i < ls.length; i++) {
            var l = ls[i];
            if (resizeVertical || resizeAllPanes) {
                $cmsj(document.body).queue(function () {
                    ClosePane(l, 'north');
                    q.dequeue();
                });
            }
            if (!resizeVertical || resizeAllPanes) {
                $cmsj(document.body).queue(function () {
                    ClosePane(l, GetHorizontalPaneName());
                    q.dequeue();
                });
            }
        }
    }
    if (recursive && (win != top)) {
        $cmsj(document.body).queue(function () {
            ClosePanes(win.parent, true, resizeAllPanes);
            q.dequeue();
        });
    }
}

function OpenPanes(win, recursive, resizeAllPanes) {
    EnsureAllLayouts();
    var ls = win.layouts;
    var q = $cmsj(document.body);
    if (ls != undefined) {
        var i = 0;
        for (i = 0; i < ls.length; i++) {
            var l = ls[i];
            if (resizeVertical || resizeAllPanes) {
                if (l.state.north.isClosed) {
                    $cmsj(document.body).queue(function () {
                        OpenPane(l, 'north');
                        q.dequeue();
                    });
                }
            }
            if (!resizeVertical || resizeAllPanes) {
                $cmsj(document.body).queue(function () {
                    OpenPane(l, GetHorizontalPaneName());
                    q.dequeue();
                });
            }
        }
    }
    if (recursive && (win != top)) {
        $cmsj(document.body).queue(function () {
            OpenPanes(win.parent, true, resizeAllPanes);
            q.dequeue();
        });
    }
}

function Minimize() {
    ParametrizedMinimize(false, false);
}

function ParametrizedMinimize(resizeAllPanes, togglePanesRecursively) {
    if (window.parentLevel == undefined) { return; }
    var index = 0;
    ClosePanes(window, togglePanesRecursively, resizeAllPanes);
    var par = ResizerGetParent();
    // Minimize only if not minimized
    if (elemMinimize.style.display != 'none') {
        for (index = 0; index < minSizes.length; index++) {
            var minSize = minSizes[index];
            if (minSize && (minSize != '')) {
                var fs = par.document.getElementById(framesetName);
                if (fs == null) {
                    var framesets = par.document.getElementsByTagName('frameset');
                    if (framesets.length > 0) {
                        fs = framesets[0];
                    }
                }
                if (fs) {
                    if (resizeVertical) {
                        originalSizes[index] = fs.rows;
                        fs.rows = minSize;
                    }
                    else {
                        originalSizes[index] = fs.cols;
                        fs.cols = minSize;
                    }

                    elemMinimize.style.display = 'none';
                    elemMaximize.style.display = 'inline';
                    elemBorder.style.display = 'block';
                }
            }
            par = par.parent;
        }
    }
}

function Maximize() {
    ParametrizedMaximize(false, false);
}

function ParametrizedMaximize(resizeAllPanes, togglePanesRecursively) {
    if (window.parentLevel == undefined) { return; }
    var index = 0;
    OpenPanes(window, togglePanesRecursively, resizeAllPanes);
    var par = ResizerGetParent();
    // Maximize only if not maximized
    if (elemMaximize.style.display != 'none') {
        for (index = 0; index < minSizes.length; index++) {
            var originalSize = originalSizes[index];
            if (originalSize && (originalSize != '')) {
                var fs = par.document.getElementById(framesetName);
                if (fs == null) {
                    var framesets = par.document.getElementsByTagName('frameset');
                    if (framesets.length > 0) {
                        fs = framesets[0];
                    }
                }
                if (fs) {
                    if (resizeVertical) {
                        fs.rows = originalSize;
                    }
                    else {
                        if (/\%$/.test(originalSize)) {
                            originalSize = originalSize.replace(/[0-9]+,/, '*,');
                        }
                        fs.cols = originalSize;
                    }

                    elemMinimize.style.display = 'inline';
                    elemMaximize.style.display = 'none';
                    elemBorder.style.display = 'none';
                }
            }
            par = par.parent;
        }
    }
}

function MinimizeAll(wnd) {
    ParametrizedMinimizeAll(wnd, false);
}

function ParametrizedMinimizeAll(wnd, resizeOnlyOwnDirection) {
    if (wnd == null) {
        elemMinimizeAll.style.display = 'none';
        elemMaximizeAll.style.display = 'inline';

        wnd = top.window;
    }

    if (wnd.ParametrizedMinimize) {
        wnd.ParametrizedMinimize(!resizeOnlyOwnDirection, true);
    }
    else {
        for (var i = 0; i < wnd.frames.length; i++) {
            ParametrizedMinimizeAll(wnd.frames[i], resizeOnlyOwnDirection);
        }
    }
}

function MaximizeAll(wnd) {
    ParametrizedMaximizeAll(wnd, false);
}

function ParametrizedMaximizeAll(wnd, resizeOnlyOwnDirection) {
    if (wnd == null) {
        elemMinimizeAll.style.display = 'inline';
        elemMaximizeAll.style.display = 'none';

        wnd = top.window;
        window.requestWindow = window;
    }

    if (wnd.ParametrizedMaximize) {
        wnd.ParametrizedMaximize(!resizeOnlyOwnDirection, true);
    }
    else {
        for (var i = 0; i < wnd.frames.length; i++) {
            if (window.requestWindow != wnd) {
                ParametrizedMaximizeAll(wnd.frames[i], resizeOnlyOwnDirection);
            }
        }
    }
}

function GetLeftMouseButton() {
    var s = navigator.userAgent.toLowerCase() + '';
    if ((s.indexOf('gecko/') >= 0) || (s.indexOf('opera/') >= 0) || (s.indexOf('safari/') >= 0) || IsIE9()) {
        return 0;
    }
    else {
        return 1;
    }
}

var lastResizeX = 0;
var currentFrameSize = 0;

var resizingFrame = false;

function StopResizeFrame(ev) {
    if (document.originalWindow && (document.originalWindow != window)) {
        return document.originalWindow.StopResizeFrame(ev);
    }

    resizingFrame = false;
    return false;
}

function IsWebKit() {
    var s = navigator.userAgent.toLowerCase() + '';
    return (s.indexOf('applewebkit/') >= 0);
}

function IsIE() {
    var s = navigator.userAgent.toLowerCase() + '';
    return (s.indexOf('msie') >= 0);
}

function IsIE9() {
    var s = navigator.userAgent.toLowerCase() + '';
    return (s.indexOf('msie 9') >= 0 || s.indexOf('trident/5') >= 0);
}

function getMouseMoveEventRecursively(window) {
    var ev = window.event;
    if (ev == null) {
        if (window.frames && window.frames.length > 0) {
            for (var i = 0; i < window.frames.length; i++) {
                ev = getMouseMoveEventRecursively(window.frames[i]);
                if (ev != null) {
                    break;
                }
            }
        }
    }
    return ev;
}

function getMouseMoveEvent() {
    var ev = window.event;
    if (ev == null) {
        ev = parent.window.event;
    }
    if (ev == null) {
        ev = getMouseMoveEventRecursively(parent);
    }
    if (ev != null) {
        return ev;
    }
    return null;
}

function StartResizeFrame(ev) {
    if (ev == null) {
        ev = getMouseMoveEvent();
    }

    if (ev.button == GetLeftMouseButton()) {
        lastResizeX = (ev.x ? ev.x : ev.clientX);
        currentFrameSize = document.body.clientWidth;
        resizingFrame = true;
    }
    InitResizerWindows(window.parent, window);
    return false;
}

function InitResizerWindows(wnd, originalWindow) {
    try {
        if (window.location.host == originalWindow.location.host) {
            if (wnd.frames.length > 0) {
                for (var i = 0; i < wnd.frames.length; i++) {
                    InitResizerWindows(wnd.frames[i], originalWindow);
                }
            }

            wnd.document.originalWindow = originalWindow;

            wnd.document.originalmousemove = wnd.document.body.onmousemove;
            wnd.document.originalmouseup = wnd.document.body.onmouseup;

            wnd.document.body.onmouseup = StopResizeFrame;
            wnd.document.body.onmousemove = ResizeFrame;
        }
    }
    catch (ex) {
    }
}

function RestoreResizerWindows(wnd) {
    try {
        if (window.location.host == originalWindow.location.host) {
            if (wnd.frames.length > 0) {
                for (var i = 0; i < wnd.frames.length; i++) {
                    RestoreResizerWindows(wnd.frames[i]);
                }
            }

            wnd.document.body.onmouseup = wnd.document.originalmouseup;
            wnd.document.body.onmousemove = wnd.document.originalmousemove;

            wnd.document.originalWindow = null;
        }
    }
    catch (ex) {
    }
}

function ResizeFrame(ev, wnd) {
    if (ev == null) {
        ev = getMouseMoveEvent();
    }

    if (window != document.originalWindow) {
        return document.originalWindow.ResizeFrame(ev, window);
    }

    if (resizingFrame) {
        if (ev.button == GetLeftMouseButton()) {
            var rtl = (document.body.className.indexOf('RTL') >= 0);
            var newX = (ev.x && !(rtl && IsIE())) ? ev.x : ev.clientX;
            var targetWnd = ev.view ? ev.view : ev.srcElement.document.parentWindow;
            var targetDoc = targetWnd.document;
            var changeX = 0;
            var sameWindow = (targetWnd == window) || (!rtl && IsIE9());

            if (!sameWindow && rtl) {
                newX = newX - targetDoc.body.clientWidth;
            }
            if (sameWindow && !rtl) {
                changeX = newX - lastResizeX;
            } else {
                changeX = newX;
            }

            var fs = window.parent.document.getElementById(framesetName);
            if (fs) {
                if (rtl) {
                    currentFrameSize -= changeX;
                    fs.cols = fs.cols.replace(/[^,]+$/, currentFrameSize);
                } else {
                    currentFrameSize += changeX;
                    fs.cols = fs.cols.replace(/^[^,]+/, currentFrameSize);
                }
            }

            if (sameWindow) {
                lastResizeX = newX;
            }
            else {
                if (!rtl) {
                    lastResizeX += newX;
                } else {
                    lastResizeX -= newX;
                }
            }
        }
        else {
            resizingFrame = false;
        }
    }
    return null;
}

function InitFrameResizer(elem) {
    if (elem != null) {
        if (!elem.resizerInitialized) {
            elem.resizerInitialized = true;

            elem.onmousedown = StartResizeFrame;
            elem.onmouseup = StopResizeFrame;
        }
    }
}
