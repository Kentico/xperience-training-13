if (window.cmsfixpanelheight == null) {
    var cmsfixpanelheight = 0;
}

function CMSFixPosition(id, plcId) {
    var oldHeight = 0;

    // CMSFixPanel initial height if already exists
    var plc = $cmsj('#plc_' + id);
    if (plc.length > 0) {
        oldHeight = plc.outerHeight(true);
    }
    // Get header container
    var elm = $cmsj('#' + id);

    // Header container height
    cmsfixpanelheight -= oldHeight;
    var top = cmsfixpanelheight;
    // If header container already has its height, keep it - there might be other elements inside such as alerts
    if (elm.outerHeight(true) != 0) {
        cmsfixpanelheight += elm.outerHeight(true);
    }
    // Otherwise set height according to the child nodes
    else {
        cmsfixpanelheight += getMaxPossibleHeight(elm[0]);
    }
    if (plc.length > 0) {
        plc.remove();
    }
    // Set height to CMSFixPanel to indent content below header actions
    plc = $cmsj('<div></div>').attr('id', 'plc_' + id).addClass('CMSFixPanel').css('height', cmsfixpanelheight);
    if (plcId != '') {
        $cmsj('#' + plcId).prepend(plc);
    }
    else {
        elm.before(plc);
    }
    // Set styles to header container
    elm.css('position', 'fixed').css('left', 0).css('top', top).css('width', '100%').css('z-index', 20000);
}

function getMaxPossibleHeight(elem) {
    var maxPossibleHeight = 0;
    var node = elem.childNodes;
    for (var i = 0; i < node.length; i++) {
        // If node is of element type
        if (node[i].nodeType == 1) {
            // Set height according to the highest child element
            if (node[i].offsetHeight > maxPossibleHeight) {
                maxPossibleHeight = node[i].offsetHeight;
            }
        }
    }
    return maxPossibleHeight;
}