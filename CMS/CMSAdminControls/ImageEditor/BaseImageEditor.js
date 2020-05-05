
function Close() {
    CloseDialog();
}

function RefreshTree() {
    if (window.wopener.RefreshTree) {
        window.wopener.RefreshTree();
        Refresh();
    }
}

function Refresh() {
    if (window.wopener.UpdatePage) {
        window.wopener.UpdatePage();
    }
    else {
        window.wopener.location.replace(window.wopener.location);
    }
}

function InitRefresh(clientId, fullRefresh, refreshTree, guid, action) {
    eval("if (wopener.InitRefresh_" + clientId + "){ wopener.InitRefresh_" + clientId + "('', " + fullRefresh + ", " + refreshTree + ", 'attachmentguid|" + guid + "', '" + action + "'); } else { if(" + fullRefresh + "){ FullRefresh('" + clientId + "','attachmentguid|" + guid + "'); } else { Refresh(); } }");
}

// Refresh using postback
function FullRefresh(clientId, guid) {
    eval("if (wopener.FullPageRefresh_" + clientId + "){ wopener.FullPageRefresh_" + clientId + "('" + guid + "')}else{Refresh();}");
}

// Show or hide properties 
function ShowProperties(showProperties, elementId) {
    var divProperties = document.getElementById('divProperties');
    var imageFrame = window.frames['imageFrame'];

    if ((divProperties != null) && (imageFrame != null) && (elementId != null)) {
        if (showProperties) {
            divProperties.style.display = 'block';
            imageFrame.frameElement.style.display = 'none';
            document.getElementById(elementId).value = 'true';
        }
        else {
            divProperties.style.display = 'none';
            imageFrame.frameElement.style.display = 'block';
            document.getElementById(elementId).value = 'false';
        }
    }
}

function UnloadTrigger(e) {
    if (window.wopener.EditDialogStateUpdate) {
        window.wopener.EditDialogStateUpdate('false');
    }
    if (!window.skipCloseConfirm && window.discardChangesConfirmation) {
        e = e || window.event;

        //e.cancelBubble is supported by IE - this will kill the bubbling process.
        e.cancelBubble = true;
        e.returnValue = window.discardChangesConfirmation;
        //e.stopPropagation works in Firefox.
        if (e.stopPropagation) {
            e.stopPropagation();
            e.preventDefault();
        }

        //return works for Chrome and Safari
        return window.discardChangesConfirmation;
    }
}

function UpdateTrimCoords(c) {
    txtCropX.val(c.x);
    txtCropY.val(c.y);
    txtCropWidth.val(c.w);
    txtCropHeight.val(c.h);
}

function UpdateTrim() {
    var x1 = parseInt(txtCropX.val(), 10);
    if (isNaN(x1)) { x1 = 0; }
    var y1 = parseInt(txtCropY.val(), 10);
    if (isNaN(y1)) { y1 = 0; }
    var x2 = x1 + parseInt(txtCropWidth.val(), 10);
    if (isNaN(x2)) { x2 = 0; }
    var y2 = y1 + parseInt(txtCropHeight.val(), 10);
    if (isNaN(y2)) { y2 = 0; }

    if ((frames['imageFrame'] != null) && frames['imageFrame'].updateTrim) {
        frames['imageFrame'].updateTrim(x1, y1, x2, y2);
    }
}

function OnKeyDown(e) {
    if (!window.keyPressed) {
        e = e || window.event;
        var code = e.keyCode || e.which;

        // Shift
        if (code == 16) {
            UpdateTrim();
            if (chkCropLock.checked) {
                LockAspectRatio(false);
            }
            else {
                LockAspectRatio(true);
            }
            window.keyPressed = true;
        }
        // Ctrl
        if (code == 17) {
            var width = parseInt(txtCropWidth.val(), 10);
            var height = parseInt(txtCropHeight.val(), 10);
            var max = Math.max(width, height);

            txtCropWidth.val(max);
            txtCropHeight.val(max);

            UpdateTrim();
            LockAspectRatio(true);
            window.keyPressed = true;
        }
    }
}

function OnKeyUp(e) {
    if (window.keyPressed) {
        e = e || window.event;
        var code = e.keyCode || e.which;
        // Shift
        if (code == 16) {
            UpdateTrim();
            if (chkCropLock.checked) {
                LockAspectRatio(true);
            }
            else {
                LockAspectRatio(false);
            }
            window.keyPressed = false;
        }
        // Ctrl
        if (code == 17) {
            UpdateTrim();
            if (!chkCropLock.checked) {
                LockAspectRatio(false);
            }
            window.keyPressed = false;
        }
    }
}

function LockAspectRatio(lock) {
    var width = 0, height = 0;
    if (lock) {
        width = parseInt(txtCropWidth.val(), 10);
        height = parseInt(txtCropHeight.val(), 10);
    }
    if ((frames['imageFrame'] != null) && frames['imageFrame'].lockAspectRatio) {
        frames['imageFrame'].lockAspectRatio(lock, width, height);
    }
}

function InitCrop() {
    if ((frames['imageFrame'] != null) && frames['imageFrame'].initCrop) {
        frames['imageFrame'].initCrop();
    }
    else {
        setTimeout('InitCrop()', 100);
    }
}

function DestroyCrop() {
    if ((frames['imageFrame'] != null) && frames['imageFrame'].destroyCrop) {
        frames['imageFrame'].destroyCrop();
    }
    ResetCropValues();
}

function ResetCropValues(){
    txtCropX.val('0');
    txtCropY.val('0');
    txtCropWidth.val('0');
    txtCropHeight.val('0');
    chkCropLock.checked = false;
}

function InitializeEditor() { 
    window.keyPressed = false;
    window.initializeCrop = false;
    window.txtCropX = $cmsj('input[id$=txtCropX]');
    window.txtCropY = $cmsj('input[id$=txtCropY]');
    window.txtCropWidth = $cmsj('input[id$=txtCropWidth]');
    window.txtCropHeight = $cmsj('input[id$=txtCropHeight]');
    window.chkCropLock = $cmsj('input[id$=chkCropLock]')[0];

    if (window.top.AdvancedModalDialogs) {
        window.onCloseDialog = function () {
            if (!window.skipCloseConfirm && window.discardChangesConfirmation) {
                return confirm(window.discardChangesConfirmation);
            }
            return true;
        }
    }
    else {
        window.onbeforeunload = UnloadTrigger;
    }

    $cmsj('.header-inner').click(function () {
        // Initialize only if OK button for crop is enabled
        if ($cmsj(this).hasClass('js-trim-init') && $cmsj('.js-btn-crop:enabled').length) {
            InitCrop();
            $cmsj(window)
                .keydown(function (e) { OnKeyDown(e); })
                .keyup(function (e) { OnKeyUp(e); });

            $cmsj(frames['imageFrame'].document)
                .keydown(function (e) { OnKeyDown(e); })
                .keyup(function (e) { OnKeyUp(e); });
        }
        else {
            DestroyCrop();
            // Clear events
            $cmsj(window).unbind('keydown').unbind('keyup');
            $cmsj(frames['imageFrame'].document).unbind('keydown').unbind('keyup');
        }
    });

    if ($cmsj.browser.msie) {
        $cmsj('input[id$=chkCropLock]').click(function () {
            LockAspectRatio(this.checked);
        });
    }
    else {
        $cmsj('input[id$=chkCropLock]').change(function () {
            LockAspectRatio(this.checked);
        });
    }

    $cmsj('input[id$=txtCropX], input[id$=txtCropY], input[id$=txtCropWidth], input[id$=txtCropHeight]').change(function () {
        UpdateTrim();
    });

    $cmsj('.js-btn-crop-reset').click(function () {
        LockAspectRatio(false);
        DestroyCrop();
        InitCrop();

        $cmsj('span[id$=lblCropError]').hide();

        return false;
    });

    // IE
    $cmsj('a[id$=lblRotate90Left]').click(function () { window.skipCloseConfirm = true; });
    $cmsj('a[id$=lblRotate90Right]').click(function () { window.skipCloseConfirm = true; });
    $cmsj('a[id$=lblFlipHorizontal]').click(function () { window.skipCloseConfirm = true; });
    $cmsj('a[id$=lblFlipVertical]').click(function () { window.skipCloseConfirm = true; });
    $cmsj('a[id$=lblBtnGrayscale]').click(function () { window.skipCloseConfirm = true; });
}