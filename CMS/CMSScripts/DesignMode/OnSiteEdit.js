// Array containing OnSite Editing web part borders
var OEWebPartBorders = new Array();
var OEEmptyWebPartSpanTags = new Array();
var OEEmptyWebPartMenu = null;
var OEOnResizeTimer;
var OEOnMenuOutTimer;
var OEHighlightContainer = null;
var OEActiveWebPart = null;
var OEContextMenuTimer;
var OEHighlightStatusEnum = { NONE: 0, ALL: 1 };
var OEHighlightButtonStatus = OEHighlightStatusEnum.NONE;


// DOM Ready
$cmsj(document).ready(function () {
    dialogZIndex = 30000;
    OEHighlightContainer = $cmsj('<div id="OEHighlightContainer" class="cms-bootstrap"></div>');
    $cmsj(document.body).append(OEHighlightContainer);

    $cmsj('.OnSiteWebPartBegin').each(function () {
        // Proceed the web part to ensure highlighting or marking the web part as an "empty" web part
        ProceedItem(this, true);
    });

    // Assign the mouse events to the highlight menu of the web part borders
    OEHighlightContainer
        .on("mouseover", ".OnSiteHighlight", function () {
            if (OEActiveWebPart == null) {
                var jContainer = $cmsj(this).parent();
                OEActiveWebPart = jContainer;
                jContainer.show();
                if (OEHighlightButtonStatus == OEHighlightStatusEnum.NONE) {
                    jContainer.children('.OnSiteMenuTable').css('z-index', '30000');
                    // Adjust the position if outside of the screen
                    OEHighlightMenuFitIntoScreen(jContainer);
                }
            }
        })
        .on("mouseout", ".OnSiteHighlight", function () {
            var jContainer = $cmsj(this).parent();
            if (OEHighlightButtonStatus == OEHighlightStatusEnum.NONE) {
                OEActiveWebPart = null;
                jContainer.hide();
            }

            // Remove high zIndex from the child elements
            OEClearZIndex(jContainer);
        });

    // Generate empty web part context menu
    OEGenerateEmptyWebPartMenu();

    // Show/Hide context menu when mouse enters/leaves the context menu
    $cmsj('.OnSiteContextMenu')
        .bind('mouseenter', function (e) {
            clearTimeout(OEContextMenuTimer);
            $cmsj(this).show();
            OEActivateMenuButton($cmsj(this).data("buttonEl"));
        })
        .bind('mouseleave', function (e) {
            OEHideContextMenu(this);
        });

    // Hide the context menu when mouse leaves the context menu button
    $cmsj('.UIToolbar .BigButton').mouseleave(function () { OEDeactivateMenuButton(this); $cmsj('.OnSiteContextMenu').hide(); });

    // Register handler for document content change event
    if (window.CMSContentManager) {
        CMSContentManager.eventManager.on('contentChanged', OERefreshSaveButton);
    }
});

// Proceed the web part to ensure highlighting or marking the web part as an "empty" web part
function ProceedItem(startSpanEl, ensureImageLateLoad) {
    var startSpan = $cmsj(startSpanEl);
    var endSpan = startSpan.nextAll(".OnSiteWebPartEnd:first");

    // Hide the web part enclosing span tags
    startSpan.hide();
    endSpan.hide();

    var propertiesPageUrl = startSpanEl.getAttribute('data-propertiespageurl') || '';
    var editPageUrl = startSpanEl.getAttribute('data-editpageurl') || '';

    // Work only with web parts which have the edit or properties pages defined
    if ((propertiesPageUrl.length > 0) || (editPageUrl.length > 0)) {

        // Check whether the content of the web part is empty -> if it is, then add the web part to the "Empty web parts" array
        var sibling = OENextElementSibling(startSpanEl);
        var tempSibling = sibling;
        var isVisible = false;

        // Loop through all the siblings and decide whether they have any visible content
        while (!isVisible && (tempSibling != null)) {
            if (((tempSibling.offsetWidth > 0) && (tempSibling.offsetHeight > 0))
                    || OEElementVisible(tempSibling, startSpanEl)) {
                isVisible = true;
            }

            // Check 'load' and 'error' image events
            if (!!ensureImageLateLoad) {
                OEEnsureImageLateLoad(tempSibling, startSpanEl);
            }

            tempSibling = OENextElementSibling(tempSibling);
        }

        if ((sibling == null) || !isVisible) {
            // Create the "Empty web part" text only when it has not been generated yet
            var index = OEEmptyWebPartSpanTags.indexOf(startSpanEl);
            if (index == -1) {
                OEEmptyWebPartSpanTags.push(startSpanEl);

                // Re-assign the next sibling (will activate the highlight functionality for this web part)
                sibling = OENextElementSibling(startSpanEl);
            }
        }

        var webPartObj = { webPartSpanId: '', startX: 0, startY: 0, endX: 0, endY: 0 };
        webPartObj.webPartSpanId = startSpanEl.id;
        sibling = OENextElementSibling(startSpanEl);

        // Loop through all the next siblings and assign mouse actions activating/deactivating highlighting
        while (sibling != null) {

            $cmsj(sibling)
                .unbind('mouseover')
                .bind('mouseover', function (e) {
                    if (OEHighlightButtonStatus == OEHighlightStatusEnum.NONE) {
                        // Check whether the CTRL key was pressed
                        var highlightMode = OEHighlightStatusEnum.ALL;
                        if ((e != null) && e.ctrlKey) {
                            // When CTRL key was pressed, disable highlight
                            highlightMode = OEHighlightStatusEnum.NONE;
                            return;
                        }

                        OEActivateWebPartBorder(webPartObj, e, true, highlightMode);
                    }
                })
                .unbind('mouseout')
                .bind('mouseout', function (e) {
                    OEDeactivateWebPartBorder(webPartObj, e);
                });

            sibling = OENextElementSibling(sibling);
        }

        // Save the web part highlight borders only when it has not been done yet
        var index = OEWebPartBorders.indexOf(webPartObj);
        if (index == -1) {
            OEWebPartBorders.push(webPartObj);
        }
    }
}

// Remove high zIndex from the child elements
function OEClearZIndex(container) {
    if (container != null) {
        container.children().css('z-index', '');
    }
}

// Remove all created highlight borders. Only the container stays.
function OEDeleteHighlightBorders() {
    if (OEHighlightContainer != null) {
        OEHighlightContainer.children().remove('.OnSiteHighlightContainer');
    }
}

// Window resize
$cmsj(window).resize(function () {

    // It is not desired to react on the resize event on mobile devices
    if (OEIsMobile) {
        return;
    }

    OEDeleteHighlightBorders();

    window.clearTimeout(OEOnResizeTimer);
    OEOnResizeTimer = window.setTimeout(function () {
        if (OEHighlightButtonStatus != OEHighlightStatusEnum.NONE) {
            OEHighlightButtonStatus = OEHighlightStatusEnum.NONE;

            // Highlight all web parts
            OEHighlightToggle();
        }
    }, 300);
});

// Restore the original position of the highlight menu
function OEHighlightRestoreMenuPosition(menuDiv) {

    // Restore the original position (if stored)
    if (menuDiv.data('left') != null) {
        menuDiv.css('right', 'auto');
        menuDiv.css('left', menuDiv.data('left') + "px");
    }
    if (menuDiv.data('top') != null) {
        menuDiv.css('top', menuDiv.data('top') + "px");
    }
}

// Adjust the position if outside of the screen
function OEHighlightMenuFitIntoScreen(highlightContainer) {
    var menuDiv = $cmsj('.OnSiteMenuTable', highlightContainer);
    if (menuDiv.get(0) == null) {
        return;
    }

    // Restore the original position of the highlight menu
    OEHighlightRestoreMenuPosition(menuDiv);

    // Get the scroll and window dimensions variables
    var cmsHeaderHeight = $cmsj('#CMSHeaderDiv').height();
    var docScrollX = document.body.scrollLeft || document.documentElement.scrollLeft;
    var docScrollY = document.body.scrollTop || document.documentElement.scrollTop;
    var jWindow = $cmsj(window);
    var windowWidth = jWindow.width();
    var windowHeight = jWindow.height();
    var windowLeftBorder = docScrollX;
    var windowRightBorder = docScrollX + windowWidth;
    var windowTopBorder = docScrollY + cmsHeaderHeight;
    var windowBottomBorder = docScrollY + windowHeight;
    var menuLeft = menuDiv.position().left;
    var menuRight = menuLeft + menuDiv.outerWidth();
    var menuTop = menuDiv.position().top;
    var menuBottom = menuTop + menuDiv.outerHeight();

    // Store the original left position
    if (menuDiv.data('left') == null) {
        menuDiv.data('left', menuLeft);
    }

    // Store the original top position
    if (menuDiv.data('top') == null) {
        menuDiv.data('top', menuTop);
    }

    // Adjust left position of the menu
    if (menuLeft < windowLeftBorder) {
        menuDiv.css('left', windowLeftBorder + 'px');
        menuDiv.css('right', 'auto');
    }
        // Adjust right position of the menu
    else if (menuRight > windowRightBorder) {
        menuDiv.css('left', 'auto');
        menuDiv.css('right', -windowLeftBorder + 'px');
    }

    // Adjust top position of the menu
    if (menuBottom > windowBottomBorder) {
        var webPartObj = menuDiv.parent().data('webPart');
        var top = webPartObj.startY;
        if (webPartObj.startY < windowTopBorder) {
            top = windowTopBorder;
        }
        menuDiv.css('top', top + 'px');
    }
}

// Calculate web part highlight border position
function OECalculateWebPartBorders(startSpan, webPartObj) {
    var updatedWebPartObj = { webPartSpanId: webPartObj.webPartSpanId, startX: 10000, startY: 10000, endX: 0, endY: 0 };

    // Loop thought all the next siblings and update the web parts border position
    var sibling = OENextElementSibling(startSpan);
    while (sibling != null) {
        // Update the web parts border position
        updatedWebPartObj = OEUpdateDimension($cmsj(sibling), updatedWebPartObj, true);
        sibling = OENextElementSibling(sibling);
    }

    var jDocument = $cmsj(document);
    var docHeight = jDocument.height();
    var docWidth = jDocument.width();

    // Increase the highlight area by 1px in all directions
    if (updatedWebPartObj.startX > 0) {
        updatedWebPartObj.startX -= 1;
    }
    if (updatedWebPartObj.startY > 0) {
        updatedWebPartObj.startY -= 1;
    }
    if (updatedWebPartObj.endX < docWidth) {
        updatedWebPartObj.endX += 1;
    }
    if (updatedWebPartObj.endY >= docHeight) {
        // Is outside of the screen => move the bottom line up
        updatedWebPartObj.endY = (docHeight - 1);
    }

    return updatedWebPartObj;
}

// Highlights the web part borders
function OEActivateWebPartBorder(webPartObj, event, refreshPosition, highlightMode) {

    // Get current highlight container
    var highlightContainer = $cmsj('#OE_' + webPartObj.webPartSpanId, OEHighlightContainer);

    if (OEHighlightButtonStatus == OEHighlightStatusEnum.NONE) {
        // Hide all highlighting
        $cmsj('.OnSiteHighlightContainer', OEHighlightContainer).hide();
    }

    if ((highlightContainer.get(0) == null) || refreshPosition) {
        // Render the web part border
        var startSpan = $cmsj('#' + webPartObj.webPartSpanId);
        var updatedWebPartObj = OECalculateWebPartBorders(startSpan.get(0), webPartObj);

        // Do not render highlight container when the highlight rectangle has a mismatched dimension
        if ((updatedWebPartObj.startX > updatedWebPartObj.endX)
                || (updatedWebPartObj.startY > updatedWebPartObj.endY)) {
            return;
        }

        if ((highlightContainer.get(0) == null)
            || (updatedWebPartObj.startX != webPartObj.startX)
            || (updatedWebPartObj.startY != webPartObj.startY)
            || (updatedWebPartObj.endX != webPartObj.endX)
            || (updatedWebPartObj.endY != webPartObj.endY)) {

            webPartObj.webPartSpanId = updatedWebPartObj.webPartSpanId;
            webPartObj.startX = updatedWebPartObj.startX;
            webPartObj.startY = updatedWebPartObj.startY;
            webPartObj.endX = updatedWebPartObj.endX;
            webPartObj.endY = updatedWebPartObj.endY;
            highlightContainer.remove();
            highlightContainer = OEGetHighlightWebPartHtml(webPartObj);
        }
    }

    OEActiveWebPart = highlightContainer;

    if ((highlightMode != OEHighlightStatusEnum.NONE)
        || OEIsInside(webPartObj, event.pageX, event.pageY)) {
        // Display the highlight container
        highlightContainer.show();
        if (OEHighlightButtonStatus == OEHighlightStatusEnum.NONE) {
            // Adjust the position if outside of the screen
            OEHighlightMenuFitIntoScreen(highlightContainer);
            highlightContainer.children('.OnSiteMenuTable').css('z-index', '100000');
        }
    }
}

// De-actives the web part borders
function OEDeactivateWebPartBorder(webPartObj, event) {
    var highlightContainer = $cmsj('#OE_' + webPartObj.webPartSpanId, OEHighlightContainer)

    if (OEHighlightButtonStatus == OEHighlightStatusEnum.NONE) {
        if (event != null) {
            if (OEIsInside(webPartObj, event.pageX, event.pageY)) {
                return;
            }
        }

        OEActiveWebPart = null;
        highlightContainer.hide();
    }

    // Remove the high zIndex from the child elements
    OEClearZIndex(highlightContainer);
}

// Returns html containing the web part highlight border
function OEGetHighlightWebPartHtml(webPartObj) {
    var divBorder = $cmsj('<div class="OnSiteHighlight"></div>');
    var cmsHeaderHeight = $cmsj('#CMSHeaderDiv').height();
    var borderPaddingWidth = divBorder.outerWidth() - divBorder.width();
    var borderPaddingHeight = divBorder.outerHeight() - divBorder.height();

    // Get the configuration menu html
    var configHtml = OEGetWebPartConfigHtml(webPartObj.webPartSpanId, false);
    var webPartBorderContainer = null;

    if (configHtml.length > 0) {
        // Top
        var borderTop = '<div class="OnSiteHighlight" style="left: ' + webPartObj.startX + 'px; top: ' + webPartObj.startY + 'px; min-width: ' + (webPartObj.endX - webPartObj.startX - borderPaddingWidth) + 'px;"></div>';
        // Left
        var borderLeft = '<div class="OnSiteHighlight" style="left: ' + webPartObj.startX + 'px; top: ' + webPartObj.startY + 'px; height: ' + (webPartObj.endY - webPartObj.startY - borderPaddingHeight) + 'px;"></div>';
        // Right
        var borderRight = '<div class="OnSiteHighlight" style="left: ' + (webPartObj.endX - borderPaddingWidth) + 'px; top: ' + webPartObj.startY + 'px; height: ' + (webPartObj.endY - webPartObj.startY - borderPaddingHeight) + 'px;"></div>';
        // Bottom
        var borderBottom = '<div class="OnSiteHighlight" style="left: ' + webPartObj.startX + 'px; top: ' + webPartObj.endY + 'px; min-width: ' + (webPartObj.endX - webPartObj.startX - borderPaddingWidth) + 'px;"></div>';
        borderBottom += '<div class="OnSiteHighlight OnSiteMenuTable" style="left: ' + webPartObj.startX + 'px; top: ' + webPartObj.endY + 'px;">' + configHtml + '</div>';
        // Border container
        webPartBorderContainer = $cmsj('<div id="OE_' + webPartObj.webPartSpanId + '" class="OnSiteHighlightContainer ' + (!OEIsRTL ? 'LTR' : '') + '" style="left: ' + webPartObj.startX + 'px; top: ' + webPartObj.startY + 'px;">' + borderTop + borderRight + borderBottom + borderLeft + '</div>');
        webPartBorderContainer.data('webPart', webPartObj);

        $cmsj(OEHighlightContainer).append(webPartBorderContainer);
    }

    return webPartBorderContainer;
}

// Update web part highlight border dimensions (jObj is a child node of the web part)
function OEUpdateDimension(jObj, webPartObj, isFirstChildNode) {
    // Element must be visible and cannot be: "OPTION" tag (they are hidden), "BR" tag (they scroll the page when trying to get their height())
    if (jObj.is(':visible') && !jObj.is('option') && (!jObj.is('br') || isFirstChildNode)) {

        // Do not process child nodes with fixed or absolute positioning
        var cssPosition = jObj.css('position');
        if (!isFirstChildNode && ((cssPosition == 'absolute') || (cssPosition == 'fixed'))) {
            return webPartObj;
        }

        var cssClear = jObj.css('clear');
        var jObjHeight = jObj.outerHeight();
        var jObjWidth = jObj.outerWidth();
        if ((cssClear == 'both') && (OENextElementSibling(jObj.get(0)) == null) && ((jObjHeight == 0) || (jObjWidth == 0))) {
            return webPartObj;
        }

        if (!jObj.is('a') || isFirstChildNode) {
            // Start point - X axis
            var objOffset = jObj.offset();
            if (objOffset.left < webPartObj.startX) { webPartObj.startX = objOffset.left; }

            // Do not calculate top margin and padding when the first child
            var paddingTop = 0;

            // Start point - Y axis
            if ((objOffset.top + paddingTop) < webPartObj.startY) { webPartObj.startY = Math.floor(objOffset.top + paddingTop); }

            // End point
            var objEndX = objOffset.left + jObjWidth;
            if (objEndX > webPartObj.endX) { webPartObj.endX = objEndX; }
            var objEndY = objOffset.top + jObjHeight;
            if (objEndY > webPartObj.endY) { webPartObj.endY = Math.floor(objEndY); }
        }

        // Include child nodes in the dimension calculation as well
        jObj.children().each(function () {
            webPartObj = OEUpdateDimension($cmsj(this), webPartObj, false);
        });
    }

    return webPartObj;
}

// Get the next sibling (including TextNodes - which will be wrapped into the span tags)
function OENextElementSibling(el) {

    if (el == null) {
        return null;
    }

    // Get the next sibling (include ELEMENT_NODEs and TEXT_NODEs)
    do {
        el = el.nextSibling
    }
        // NodeType == 1: standard tag (<span>, <div>...)
        // NodeType == 3: TextNode
    while ((el != null)
            && (
                ((el.nodeType !== 1) && (el.nodeType !== 3))
                || ((el.nodeType === 3) && ($cmsj.trim(el.nodeValue).length === 0))
                )
            );

    // TextNode
    if ((el != null) && (el.nodeType === 3)) {
        // Wrap the TextNode into the span tag
        $cmsj(el).wrap('<span class="OnSitePlainText"></span>');
        el = el.parentNode;
    }

    // Get the next sibling (exit if the end span tag is reached)
    if ((el == null) || (el.className == 'OnSiteWebPartEnd') || (el.className == 'OnSiteWebPartBegin')) {
        return null;
    }

    return el;
}

// Indicates whether the mouse position is inside of the specified web part
function OEIsInside(webPartObj, mouseX, mouseY) {
    if ((webPartObj.endX == 0) && (webPartObj.endY == 0)) {
        return true;
    }

    return (mouseX > webPartObj.startX) && (mouseX < webPartObj.endX) && (mouseY > webPartObj.startY) && (mouseY < webPartObj.endY);
}

// Indicates whether the horizontal scroll bar is visible
function OEIsHorizontalScrollBarVisible() {
    // Expect scroll bar visible by default
    var horizontalScrollBarVisible = true;
    var scrollDocY = document.body.scrollTop || document.documentElement.scrollTop;
    if (scrollDocY == 0) {
        // No scroll -> expect no scroll bar
        horizontalScrollBarVisible = false;

        // Try to scroll by 1px down
        var jDocument = $cmsj(document);
        jDocument.scrollTop(1);
        // It is possible to scroll -> scroll bar is visible
        if (jDocument.scrollTop() > 0) {
            jDocument.scrollTop(0);
            horizontalScrollBarVisible = true;
        }
    }

    return horizontalScrollBarVisible;
}

// Highlight/De-active all borders of all web parts on the page. Ensures re-calculating of the web part borders positions
function OEHighlightToggle(event, buttonEl) {
    if (OEHighlightContainer == null) {
        return;
    }

    // Check whether the horizontal scroll bar is visible
    var scrollBarVisibleBeforeHighlight = OEIsHorizontalScrollBarVisible();

    // Restore the original position of the highlight menus
    $cmsj('.OnSiteMenuTable', OEHighlightContainer).each(function () {
        OEHighlightRestoreMenuPosition($cmsj(this));
    });

    if (OEHighlightButtonStatus == OEHighlightStatusEnum.NONE) {
        // Highlight all web parts and widgets

        // Check whether the CTRL key was pressed
        OEHighlightButtonStatus = OEHighlightStatusEnum.ALL;
        if ((event != null) && event.ctrlKey) {
            // When CTRL key was pressed, disable highlight
            OEHighlightButtonStatus = OEHighlightStatusEnum.NONE;
            return;
        }

        // Highlight the menu button
        OEActivateMenuButton(buttonEl, true);

        // Highlight all web parts
        OEActivateWebPartBorders(true);

        // Highlight all Widget borders
        borderDeactivationEnabled = false;
        $cmsj('.WebPartBorder').mouseover();
    }
    else {
        // Deactivate Highlight
        OEDeactivateMenuButton(buttonEl, true);
        $cmsj('.OnSiteHighlightContainer', OEHighlightContainer).hide();

        // No web parts highlighted
        OEHighlightButtonStatus = OEHighlightStatusEnum.NONE;

        // Hide all widget borders
        borderDeactivationEnabled = true;
        $cmsj('.WebPartBorderActive').mouseout();
    }

    // Check whether the horizontal scroll bar is visible after the web part borders were highlighted
    var scrollBarVisibleAfterHighlight = OEIsHorizontalScrollBarVisible();

    // When no scroll bar was visible at the beginning and after highlight was displayed, then re-calculate the highlight borders
    if (scrollBarVisibleBeforeHighlight != scrollBarVisibleAfterHighlight) {
        var tempDiv = null;
        if (scrollBarVisibleAfterHighlight) {
            // Insert a temporary div outside of the visible screen to ensure that the scroll bars will be visible
            tempDiv = $cmsj('<div style="position: absolute;  top: ' + ($cmsj(document).height() + 10) + 'px; width: 1px; height: 1px;"></div>');
            $cmsj(document.body).append(tempDiv);
        }

        // Delete current highlight borders (they were calculated when the horizontal scroll bar was not displayed)
        OEDeleteHighlightBorders();

        // Re-calculate the highlight borders. Now including the horizontal scroll bar as well.
        OEActivateWebPartBorders(false);

        if (tempDiv != null) {
            // Remove the temporary div
            tempDiv.remove();
        }
    }
}

// Highlight borders of all web parts 
function OEActivateWebPartBorders(refreshPositions) {
    var len = OEWebPartBorders.length;
    for (var i = 0; i < len; i++) {
        var webPartObj = OEWebPartBorders[i];
        OEActivateWebPartBorder(webPartObj, null, refreshPositions, OEHighlightButtonStatus);
    }
}

// Generate empty web part context menu
function OEGenerateEmptyWebPartMenu() {
    if (OEEmptyWebPartMenu != null) {
        return;
    }

    OEEmptyWebPartMenu = $cmsj('.OnSiteHiddenButton .UniMenuContextMenu');
    OEEmptyWebPartMenu.empty();

    var webPartOpeningDiv = '<div class="Item ItemPadding">';
    var webPartClosingDiv = '</div>';

    var len = OEEmptyWebPartSpanTags.length;

    // Display the "Empty" button when there are web parts/widgets without content
    if (len > 0) {
        $cmsj('.OnSiteHiddenButton').show();
    }
    else {
        $cmsj('.OnSiteHiddenButton').hide();
    }

    for (var i = 0; i < len; i++) {
        var webPartSpanTag = OEEmptyWebPartSpanTags[i];
        var html = webPartOpeningDiv + OEGetWebPartConfigHtml(webPartSpanTag.id, true) + webPartClosingDiv;
        OEEmptyWebPartMenu.append(html);
    }

    if (len == 0) {
        OEEmptyWebPartMenu.empty();
    }

    // Disable closing the context menu when the web part title is clicked
    OEEmptyWebPartMenu.children('.Item').each(function () {
        $cmsj(this).click(function (event) {
            event.stopImmediatePropagation();
        });
    });
}

// Ensures that an image element will be removed from the "Empty" web parts context menu and its "Empty web part" text will be removed after its content is loaded
function OEEnsureImageLateLoad(element, startSpanEl) {
    var jElement = $cmsj(element);

    if (element.tagName == "IMG") {
        // Check load, error image events
        jElement
            // Store the span element for "load, error" event handlers
            .data("startSpanEl", startSpanEl)
            // Event raised when the image is loaded
            .load(function () {
                var startSpanEl = $cmsj(this).data("startSpanEl");
                var index = OEEmptyWebPartSpanTags.indexOf(startSpanEl);
                if (index != -1) {
                    // Remove the element from empty web part array
                    OEEmptyWebPartSpanTags.splice(index, 1);
                    // Regenerate the context menu
                    OEEmptyWebPartMenu = null;
                    OEGenerateEmptyWebPartMenu();
                }
            })
            // Event raised when the image is not found
            .error(function () {
                // Check whether missing image is replaced by a browser's default image (Chrome)
                var jElement = $cmsj(this);
                var startSpanEl = $cmsj(this).data("startSpanEl");

                var elWidth = jElement.css("width");
                var elHeight = jElement.css("height");
                jElement.css("width", 'auto').css("height", 'auto');

                // Proceed the web part again to ensure highlighting or marking the web part as an "empty" web part (this solves the issue when different browsers replace/hide images which cannot be found - 404)
                ProceedItem(startSpanEl, false);

                // Restore the image width and height
                jElement.css("width", elWidth).css("height", elHeight);
            });
    }
    else {
        // Ensure that images wrapped in <div>, <a>... tags will be checked as well (this solves the issue when the image has width and height set and the image url is incorrect)
        $cmsj('img', jElement).each(function () {
            OEEnsureImageLateLoad(this, startSpanEl);
        });
    }
}

// Check whether the given element element is visible (for image elements ensures late load events)
function OEElementVisible(element, startSpanEl) {

    // Child element is visible
    if ((element.offsetWidth > 0) && (element.offsetHeight > 0)) {
        return true;
    }
        // Ensure load/error event handlers for Image tags (solves image late load issue)
    else if (element.tagName == "IMG") {
        OEEnsureImageLateLoad(element, startSpanEl);
        return false;
    }

    // Check recursively its child elements
    var length = element.childNodes.length;
    for (var i = 0; i < length; i++) {
        var childEl = element.childNodes[i];
        if (OEElementVisible(childEl, startSpanEl)) {
            return true;
        }
    }

    return false;
}

// Returns html the edit/configuration section of the web part highlight border
function OEGetWebPartConfigHtml(webPartSpanId, revertOrder) {
    var MINIMUM_EDIT_DIALOG_WIDTH = 1000;

    var spanObj = document.getElementById(webPartSpanId);
    if (spanObj == null) {
        return "";
    }

    var title = spanObj.getAttribute('data-title') || '';
    // Apply HTMLEncode on the title string
    title = $cmsj('<div/>').text(title).html();
    var propertiesPageUrl = spanObj.getAttribute('data-propertiespageurl') || '';
    var editPageUrl = spanObj.getAttribute('data-editpageurl') || '';
    var dialogWidth = spanObj.getAttribute('data-dialogwidth') || '';

    // Set the dialog width according to the child width
    if (dialogWidth.length == 0) {
        var webPartObj = $cmsj(spanObj).next();
        if (!webPartObj.hasClass("OnSitePlainText") && !webPartObj.hasClass("OnSiteWebPartEnd")) {
            dialogWidth = webPartObj.outerWidth() + 20;
        }
    }

    if (dialogWidth < MINIMUM_EDIT_DIALOG_WIDTH) {
        dialogWidth = MINIMUM_EDIT_DIALOG_WIDTH;
    }

    // Do not use a specific dialog width for mobile browsers -> modal dialogs in mobile browsers are opened in a new tab
    if (OEIsMobile) {
        dialogWidth = '100%';
    }

    var configHtml = '';

    // Render the configuration html only when the edit or properties page url are defined
    if ((propertiesPageUrl.length > 0) || (editPageUrl.length > 0)) {
        var deactivateHighlightScript = "OEDeactivateWebPartBorder({ webPartSpanId: '" + webPartSpanId + "' }, null ); ";
        var closeParentContextMenuScript = "$cmsj(this).parents('.OnSiteContextMenu').hide(); ";

        // Mobile devices work with ontouchend better than with onclick
        var onclick = (!OEIsMobile) ? "onclick" : "ontouchend";
        var editableScript = closeParentContextMenuScript + deactivateHighlightScript + "modalDialog('" + editPageUrl + "', 'editpage', '" + ((dialogWidth.length > 0) ? dialogWidth : MINIMUM_EDIT_DIALOG_WIDTH) + "', '90%')";
        var propertiesScript = closeParentContextMenuScript + deactivateHighlightScript + "modalDialog('" + propertiesPageUrl + "', 'configurewebpart', '900', '638')";
        var editHtml = '<div class="ActionIcon"  ' + onclick + '="' + editableScript + ';"><i aria-hidden="true" class="cms-icon-80 icon-edit"></i></div>';
        var propertiesHtml = '<div class="ActionIcon" ' + onclick + '="' + propertiesScript + ';"> <i aria-hidden="true" class="cms-icon-80 icon-cogwheel"></i></div>';

        configHtml = '<div class="Name"><span>' + title + '</span></div>';

        if (!!revertOrder) {
            if (propertiesPageUrl.length > 0) {
                configHtml += propertiesHtml;
            }
            if (editPageUrl.length > 0) {
                configHtml += editHtml;
            }
        }
        else {
            if (editPageUrl.length > 0) {
                configHtml += editHtml;
            }
            if (propertiesPageUrl.length > 0) {
                configHtml += propertiesHtml;
            }
        }
    }

    return configHtml;
}

// Convert the input string to int. Use defaultValue when the input string is not a number.
function OETryParseInt(str, defaultValue) {
    var retValue = defaultValue;
    if ((typeof (str) != 'undefined') && (str != null) && (str.length > 0)) {
        retValue = parseInt(str, 10);
        if (isNaN(retValue)) {
            retValue = defaultValue;
        }
    }

    return retValue;
}

// Hide the specified context menu and deactivate its corresponding menu button
function OEHideContextMenu(menuEl, noDelay) {
    var timeout = 500;
    if (!!noDelay) {
        timeout = 0;
    }

    clearTimeout(OEContextMenuTimer);
    OEContextMenuTimer = window.setTimeout(function () {
        jMenu = $cmsj(menuEl);
        OEDeactivateMenuButton(jMenu.data("buttonEl"));
        jMenu.hide();
    }, timeout);
}

// Highlighting the specified menu button
function OEActivateMenuButton(buttonEl, usePermanentCss) {
    var highlightCSSClass = "Selected";
    if (!!usePermanentCss) {
        highlightCSSClass = "SelectedPermanent";
    }

    $cmsj(buttonEl).addClass(highlightCSSClass);
}

// Deactivate highlighting the specified menu button
function OEDeactivateMenuButton(buttonEl, usePermanentCss) {
    var highlightCSSClass = "Selected";
    if (!!usePermanentCss) {
        highlightCSSClass = "SelectedPermanent";
    }

    $cmsj(buttonEl).removeClass(highlightCSSClass);
}

// Deactivate highlighting the the menu buttons (except permanent highlight buttons i.e. "Highlight webparts" button)
function OEDeactivateMenuButtons() {
    $cmsj(".UniMenuContent .Selected").each(function () {
        OEDeactivateMenuButton(this);
    })
}

// Function called when the document content has been changed or the all the document changes have been cleared
function OERefreshSaveButton(event, isModified) {
    // Enables/Disables the save button
    var btn = $cmsj(".OESaveButton");
    if (btn.length) {
        var img = btn.find("i");
        var src = img.attr("src");

        if (isModified) {
            // Enable save button
            btn
                .removeClass("BigButtonDisabled")
                .addClass("BigButton")
                .prop("title", btn.attr('data-enabledTooltip'));
            img.prop("title", btn.attr('data-enabledTooltip'));
        }
        else {
            // Disable save button
            btn
                .removeClass("BigButton")
                .addClass("BigButtonDisabled")
                .prop("title", btn.attr('data-disabledTooltip'));
            img.prop("title", btn.attr('data-disabledTooltip'));
        }
    }
}


// Display the context menu
function OEShowContextMenu(buttonEl, contextMenuName) {

    // Hide all other context menus
    clearTimeout(OEContextMenuTimer);
    $cmsj('.OnSiteContextMenu').hide();

    OEDeactivateMenuButtons();

    var jContextMenu = $cmsj('.OnSiteContextMenu[data-contextmenu="' + contextMenuName + '"]');
    jButton = $cmsj(buttonEl);
    jContextMenu.data("buttonEl", buttonEl);
    btnOffset = jButton.offset();
    jContextMenu.css('top', btnOffset.top - $cmsj(window).scrollTop() + jButton.outerHeight());
    jContextMenu.css('display', 'table');

    if (!OEIsRTL) {
        jContextMenu.css('left', btnOffset.left);
    }
    else {
        // RTL culture
        jContextMenu.css('left', btnOffset.left - jContextMenu.outerWidth() + jButton.outerWidth());
    }
}

// Refresh the page after creating a new/linked document
function RefreshTree(parentNodeId, nodeId) {
    refreshPageOnClose = true;

    if ((nodeId > 0) && (nodeId != OECurrentNodeId) && (parentNodeId != OECurrentNodeId)) {

        // Redirect to the specified document
        var reloadUrl = $cmsj.param.querystring(window.location.href, "onsitenodeid=" + nodeId);
        window.location = reloadUrl.replace('safemode=1', 'safemode=0');
    }
}

// IE8 - fix
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (obj, start) {
        for (var i = (start || 0), j = this.length; i < j; i++) {
            if (this[i] === obj) { return i; }
        }
        return -1;
    }
}

