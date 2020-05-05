var IsCMSDesk = true;
var IsFullScreen = false;
var EditorElem = null;

function PreviewEditorFullScreen(isFullScreen, elemsrc, paneContentID) {
    var elem = $cmsj(elemsrc);

    // Store values to global for redirect
    IsFullScreen = isFullScreen;
    EditorElem = elem;
    var parent = $cmsj("#" + paneContentID);

    // Hide parent's scrollbars
    parent.css({ "overflow": "hidden" });

    if (!isFullScreen) {
        elem.css('width', '100%');
        elem.css('height', 'auto');
        elem.css('top', 'auto');
        elem.css('left', 'auto');

        // Show paren't scrollbars
        parent.css({ "overflow": "auto" });
    }
    else {
        var offset = 0;
        // fullScreenOffset defines additional offset - depends on content control (f.e. when displayed page title)
        if (typeof (fullScreenOffset) != 'undefined') {
            offset = fullScreenOffset;
        }

        var header = $cmsj('.PreviewMenu');
        var basicHeader = header.height() - 1;
        elem.height(parent.height() - basicHeader - offset);
        elem.offset({ top: parent.offset().top + basicHeader + offset, left: parent.offset().left });
        elem.width(parent.width());
    }
}

function LayoutResized(pane, $Pane, paneState) {
    if (IsFullScreen) {
        CM_FullScreen(IsFullScreen, EditorElem);
    }

    setBodyPosition();
}


function InitPreview(body, startWithFullScreen) {
    $cmsj("#" + body).show();

    if (startWithFullScreen && !IsFullScreen && (CM_instances != null)) {
        CM_instances[0].toolbar.fitWindow();
    }
    setBodyPosition();
}

function setBodyPosition() {
    $cmsj(".PreviewBody").css('top', $cmsj(".PreviewMenu").height());    
}