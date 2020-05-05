// Web part toolbar element variables
var wptPanel = null;
var wptLayout = null;
var wptMenu = null;
var wptScrollContainer = null;

// Toolbar helper variables
var wptStartSelectHandler = null;
var wptEnableAutoScroll = true;
var wptAutoScrollTimer;
var wptFilterWebPartsTimer;
var wptReload = false;

// Initial position variables
var wptInitContainerScroll = { x: 0, y: 0 };
var wptInitWindowScroll = { x: 0, y: 0 };
var wptPrevWindowScroll = { x: 0, y: 0 };

// Initialize the toolbar variables
function wptInit(forceReload) {
    if (forceReload) {
        // Renew the toolbar variables
        wptPanel = null;
        wptLayout = null;
        wptScrollContainer = null;
    }

    // Load the web part toolbar element variables

    if (wptPanel === null) {
        wptPanel = $cmsj('.WPTPanel:first');
    }

    if (wptLayout === null) {
        wptLayout = $cmsj('#wptLayout:first', wptPanel);
    }

    if (wptMenu === null) {
        wptMenu = $cmsj('.WPTMenu:first', wptPanel);
    }

    if (wptScrollContainer === null) {
        wptScrollContainer = $cmsj('.WPTItemsContainer:first', wptPanel);
        wptScrollContainer.height(wptLayout.height());
    }

    // Used forced reload
    if (forceReload) {
        // Disable selection for these elements. If not disabled then when dragging web parts, these elements get selected
        wptScrollContainer.spDisableSelection();

        var tooltipClass = (!wptIsRTL) ? "WPTTT LTR" : "WPTTT RTL";

        var tooltipPosition = "center " + (!wptIsRTL ? "left" : "right");

        // Set the toolbar web part tooltip
        $cmsj('.WPTSelectorEnvelope, .WPTSelectorEnvelopeHover')
            .mouseout(function () { /* IE8 fix */var tooltipObj = $cmsj(this).data("tooltip"); if (tooltipObj != null) { tooltipObj.hide(); } })
            .tooltip({
                predelay: 500,
                effect: "fade",
                events: {
                    def: "mouseenter,mouseleave mousedown",
                    tooltip: ",mouseenter mouseleave"
                },
                tipClass: tooltipClass,
                position: tooltipPosition,
                onBeforeShow: function () {

                    // IE8 tooltip fix (fixes jQuery tools - tooltip bug)
                    if ($cmsj('body').hasClass('IE8')) {
                        return true;
                    }

                    if (wptReload) {
                        return false;
                    }
                    // Display the tooltip only for the toolbar web parts which are not being dragged
                    return (this.getTrigger().get(0).style.position != "absolute");
                }
            });
    }
}

// Highlight the web parts
function wptToggle(el, highlight) {
    el.className = (highlight ? 'WPTSelectorEnvelopeHover' : 'WPTSelectorEnvelope');
}

// Process pressed keys for the search text box
function wptProceedSpecialKeys(el, e) {
    // CR
    if (e.keyCode == 13) {
        return false;
    }
        // ESC
    else if (e.keyCode == 27) {
        $cmsj(el).val('');
        e.preventDefault();
        e.stopPropagation();
        return false;
    }

    return true;
}

// Show/Hide web parts according to the search text
function wptFilter(searchText) {
    var isFirst = null;
    var isFirstItem = null;
    searchText = searchText.toLowerCase();
    var nofilter = false;

    // Declare search method
    searchMethod = function () {
        var display = 'none';
        var nodes = $cmsj(this).find('div.WPTHandle div');
        if (nodes.length == 0) {
            if ($cmsj(this).find('.AppearElement').length == 0) {
                $cmsj(this).hide();
            }
            return;
        }

        var txtNode = nodes.first();

        if (!nofilter) {
            // Try to match "name" node content with search text
            if ((searchText.length == 0) || ((typeof txtNode !== 'undefined') && (txtNode.html().toLowerCase().indexOf(searchText) != -1))) {
                display = 'block';
                found = true;
                if (isFirst === null) {
                    isFirst = true;
                }
            }
        } else {
            display = 'none';
        }

        // Get "web part" node
        var webPart = txtNode.parent().parent();
        webPart.css('display', display);
        if (isFirst) {
            display = 'none';
            isFirst = false;
        }
    };

    wptScrollContainer.find('div.WPTCat').each(function () {
        var jThis = $cmsj(this);
        found = false;

        // Do not allow to filter web part with special flag
        if (jThis.html().indexOf('__NOFILTER__') != -1) {
            nofilter = true && (searchText.length != 0);
        }

        // Get all sibling nodes (i.e. "web part" nodes) until next category
        var nextNodes = jThis.nextUntil('div.WPTCat', 'div.WPTSelectorEnvelope');
        nextNodes.each(searchMethod);

        jThis.css('display', (found ? 'block' : 'none'));
        nofilter = false;
    });

    // Scroll back to the beginning
    wptScrollContainer.scrollTop(0);

    // Show the scroll panel if not visible already
    wptScrollContainer.show();
}

// Minimize the toolbar
function wptMinimize() {
    $cmsj('.WPTMaximized:first', wptPanel).hide();
    $cmsj('.WPTMinimized:first', wptPanel).show();
    if (window.ResizeToolbar) { ResizeToolbar(); }
    $cmsj.cookie(wptIsMinimizedCookie, true, { path: '/', expires: 31 });
    wptUpdateWindowPadding();
}

// Maximize the toolbar
function wptMaximize() {
    $cmsj('.WPTMaximized:first', wptPanel).show();
    $cmsj('.WPTMinimized:first', wptPanel).hide();
    if (window.ResizeToolbar) { ResizeToolbar(); }
    $cmsj.cookie(wptIsMinimizedCookie, null, { path: '/' });
    wptInit(true);
    wptReloadScrollPanel(true);
    wptUpdateWindowPadding();
}

// Apply page padding according to the toolbar position
function wptUpdateWindowPadding() {
    $cmsj(".WPTTableCellPadding").css("min-width", wptLayout.outerWidth());

    var padding = "padding-" + (wptIsRTL ? "left" : "right");
    $cmsj("#CMSHeaderDiv")
        .css(padding, wptLayout.outerWidth() + "px")
        .width('auto');
}

// Load the not-loaded web part images with a delay
function wptLoadWebpartImages() {
    window.setTimeout(function () {
        $cmsj('.WPTHandle img', wptPanel).each(function () {
            var jThis = $cmsj(this);
            var revAttr = jThis.attr('rev');
            if (revAttr != null) {
                jThis.attr('src', WPTImgBaseSrc + revAttr);
                jThis.attr('rev', null);
            }
        });
    }, 500);
}

// Event risen before the item is dropped onto the toolbar web part list
function wptListOnBeforeDrop(sender, e) {
    var item = e.get_droppedItem();
    // Set a flag to indicate cancelling of the dragging
    item.setAttribute('data-dragcancelled', '1');
}

// Event risen before the item is dropped onto a web part zone
function OnBeforeDropWebPart(sender, e) {
    var item = e.get_droppedItem();
    // Work only with the toolbar web parts
    if (item.getAttribute('data-dragkeepcopy') == 1) {
        $cmsj('.WPTLoaderBackground').height($cmsj(document).height());
        $cmsj('#wptLoader').show();
        window.setTimeout(function () { $cmsj('#wptLoader').hide(); }, 2000);

        // Keep the document scroll position. Will be used after refresh
        parent.ScrollTop = document.body.scrollTop || document.documentElement.scrollTop;
    }
}

// jQuery events
$cmsj(document).ready(function () {
    // Initialize the toolbar variables
    wptInit(true);
    // Apply page padding according to the toolbar position
    wptUpdateWindowPadding();
    // Use the lazy load for the rest of the web part images
    wptLoadWebpartImages();
    // Scroll to the saved position
    if ((typeof (parent.ScrollTop) != "undefined") && (parent.ScrollTop > 0)) {
        $cmsj(document).scrollTop(parent.ScrollTop);
        parent.ScrollTop = 0;
    }
});

$cmsj(window).resize(function () {
    // Proceed only after all data have been loaded

    // Initialize the toolbar variables
    wptInit(true);
    // Apply page padding
    wptUpdateWindowPadding();
});


// DragAndDrop handlers

// Event risen when start dragging and the dragged item is being copied to ensure that the copied item will be dragged (not the original)
function OnCopyDraggedItem(originalItem, copiedItem) {
    // Work only with the toolbar draggable web parts (not with the regular page web parts)
    var itemAttrKeepCopy = (originalItem.getAttribute('data-dragkeepcopy') == 1);
    if (itemAttrKeepCopy) {
        originalItem.className = "WPTSelectorEnvelope";
        // Ensure that the dragged web part will be hovered for the whole time
        copiedItem.onmouseout = function () { };
    }
}

// Event risen before the item is being dragged
function OnBeforeStartDrag(el) {
    // Work only with the toolbar draggable web parts (not with the regular page web parts)
    var itemAttrKeepCopy = (el.getAttribute('data-dragkeepcopy') == 1);
    if (itemAttrKeepCopy) {
        var docScroll = { x: 0, y: 0 };
        docScroll.x = document.body.scrollLeft || document.documentElement.scrollLeft;
        docScroll.y = document.body.scrollTop || document.documentElement.scrollTop;

        // Chrome text selection correction
        wptStartSelectHandler = document.onselectstart;
        document.onselectstart = function () { return false; };

        // Setup the scroll variables used for fixing of the fixed positioning
        wptInitWindowScroll.x = docScroll.x;
        wptInitWindowScroll.y = docScroll.y;

        wptInitContainerScroll.x = wptScrollContainer.scrollLeft();
        wptInitContainerScroll.y = wptScrollContainer.scrollTop();
        wptPrevWindowScroll.x = docScroll.x;
        wptPrevWindowScroll.y = docScroll.y;

        var location = Sys.UI.DomElement.getLocation(el);
        var containerX = wptScrollContainer.scrollLeft();
        var containerY = wptScrollContainer.scrollTop();
        var wptX = wptLayout.position().left;
        var wptY = wptLayout.position().top;

        // Fix the IE RTL scrollLeft behavior
        /*var bodyElem = $cmsj('body');
        if (bodyElem.hasClass('IE8') || bodyElem.hasClass('IE9')) {
            containerX = wptInitContainerScroll.x = (-1) * wptScrollContainer.scrollLeft();
        }*/

        // Correct the dragged element location
        if (location.x < 0) {
            location.x = 0;
        }
        if (location.y < 0) {
            location.y = 0;
        }

        location.x -= (containerX + 2 * wptX); // 2*wptX - because wpt is in fixed positioning
        location.y -= (containerY + 2 * wptY);

        Sys.UI.DomElement.setLocation(el, location.x, location.y);
    }
}

// Event risen when the method setLocation() in the DragAndDrop extender is fired
function OnSetLocation(el, point) {
    // Work only with the toolbar draggable web parts (not with the regular page web parts)
    var itemAttrKeepCopy = (el.getAttribute('data-dragkeepcopy') == 1);
    if (itemAttrKeepCopy) {

        var docScroll = { x: 0, y: 0 };
        docScroll.x = document.body.scrollLeft || document.documentElement.scrollLeft;
        docScroll.y = document.body.scrollTop || document.documentElement.scrollTop;

        // Correct the X position when autoscroll is active
        if (wptPrevWindowScroll.x == docScroll.x) {
            // Correct the element location (due to used fixed positioning)
            point.x = point.x - docScroll.x + wptInitWindowScroll.x + wptInitContainerScroll.x;
        }
        // Correct the Y position when autoscroll is active
        if (wptPrevWindowScroll.y == docScroll.y) {
            // Correct the element location (due to used fixed positioning)
            point.y = point.y - docScroll.y + wptInitWindowScroll.y + wptInitContainerScroll.y;
        }

        // Save the previous document scroll position
        wptPrevWindowScroll.x = docScroll.x;
        wptPrevWindowScroll.y = docScroll.y;
    }

    return point;
}

// Event risen when the user drops the draggable web part onto the droppable area
function OnDragEnd(el) {
    // Work only with the toolbar draggable web parts (not with the regular page web parts)
    var itemAttrKeepCopy = (el.getAttribute('data-dragkeepcopy') == 1);
    if (itemAttrKeepCopy) {
        // Restore the onmouseout event 
        el.onmouseout = function () { this.className = 'WPTSelectorEnvelope'; };
        el.className = "WPTSelectorEnvelope";
        // Restore the onselectstart event (due to a Chrome bug)
        document.onselectstart = wptStartSelectHandler;
    }
}

// Event risen when the user moves a draggable web part onto a droppable area
function OnShowDropCue(cueElem, dropItem) {
    //var jCue = $cmsj(cueElem);
    // Delete the scroll timer (which is supposed to enable scrolling)
    //window.clearTimeout(wptAutoScrollTimer);
}

// Event risen when the drop cue is being repositioned. This function fixes the toolbars "fixed" positioning together with scrolled document.
function OnRepositionDropCue(el, location) {
    if (el.getAttribute('data-dragkeepcopy') == 1) {
        var scrollLeft = document.body.scrollLeft || document.documentElement.scrollLeft;
        var scrollTop = document.body.scrollTop || document.documentElement.scrollTop;
        location.x += scrollLeft;
        location.y += scrollTop;
    }

    return location;
}

// Event risen when the user moves a draggable web part out from a droppable area
function OnHideDropCue(cueElem) {
    // Delete the scroll timer (which is supposed to enable scrolling)
    window.clearTimeout(wptAutoScrollTimer);
    // Enable scrolling in 2 seconds
    wptAutoScrollTimer = window.setTimeout(function () { wptEnableAutoScroll = true; }, 2000);
}
