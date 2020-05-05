//<![CDATA[
// Variables used for calculating the number of displayed items
var uniFlatItems = null;
var uniFlatItemsCount = 0;
var uniFlatFirstItem = null;
// Initial body height
var initHeigh = document.body.clientHeight;
// Maximal timer calls. 10 = 2 second
var maxTimerCount = 40;
// Current timer count
var timerCounter = 0;
// Offset value
var offsetValue = 0;
// Minimal height
var minHeight = 370;

var itemSelector = null;
var selectorTree = null;
var uniFlatContent = null;
var uniFlatSearchPanel = null;
var uniFlatPager = null;
var selectorFlatDescription = null;

// After DOM ready
$cmsj(document.body).ready(initializeResize);

// Initialize resize
function initializeResize() {

    // Get items
    getItems(false);
    // Initial resize
    resizeareainternal();
    // get the number of items which can fit into the page
    uniFlatItemsCount = getItemsCount();
    // Create timer
    //setTimeout(resizeChecker, 200);
    $cmsj(window).resize(function () { resizeareainternal(); });

    $cmsj('.alert-dismissable').click(function () {
        resizearea();
    });
}


// Check whether document is ready and set correct values
function resizeChecker() {

    // Check whether page is ready => body height contains correct values
    if (document.body.clientHeight != initHeigh) {

        // Resize elements
        resizearea();

        // Set window resize handler
        $cmsj(window).resize(function () { resizeareainternal(); });
    }
    else {

        // Check whether current call is smaller than max. calls count
        if (timerCounter < maxTimerCount) {
            timerCounter++;
            setTimeout(resizeChecker, 200);
        }
        else {
            // Set window resize handler
            $cmsj(window).resize(function () { resizeareainternal(); });
            // Clear counter
            timerCounter = 0;
        }
    }
}

// Resize elements handler
function resizearea() {
    getItems(true);
    resizeareainternal();
}


// Resize elements
function resizeareainternal() {
    var selectorOffsetTop = itemSelector.offset();
    if (selectorOffsetTop == null) {
        return;
    }

    var footerHeight = $cmsj("#divFooter").outerHeight(true);

    // For flat selector without footer, short it's height to prevent scrollbar on page
    if (footerHeight == null) {
        offsetValue = 0;
    }

    // Main height
    var height = document.body.clientHeight - selectorOffsetTop.top - $cmsj("div .CopyLayoutPanel").outerHeight(true) - footerHeight - offsetValue;

    // Try decrement footer height
    var dialogFooter = $cmsj(".PageFooterLine");
    if (dialogFooter != null) {
        height = height - dialogFooter.outerHeight(true);
    }

    // Set minimal height
    if (height < minHeight) {
        height = minHeight;
    }

    // Selector container
    itemSelector.css("height", height);
    // Tree
    selectorTree.css("height", height);
    // Flat content height
    uniFlatContent.css("height", height - uniFlatSearchPanel.outerHeight(true) - selectorFlatDescription.outerHeight(true) - uniFlatPager.outerHeight(true));
}


// Ensures selector objects
function getItems(forceLoad) {
    if (forceLoad || (itemSelector == null)) {
        itemSelector = $cmsj("div .ItemSelector");
        selectorTree = $cmsj("div .SelectorTree");
        uniFlatContent = $cmsj("div .UniFlatContent");
        uniFlatItems = $cmsj("div .SelectorFlatItems");
        uniFlatFirstItem = $cmsj("div .SelectorEnvelope:first");
        uniFlatSearchPanel = $cmsj("div  .uni-flat-search");
        uniFlatPager = $cmsj("div .uniflat-pager");
        selectorFlatDescription = $cmsj("div .selector-flat-description");
    }
}

// Gets a number which represents the max count o items which can fit into the "uniFlatContent" div
function getItemsCount() {
    if ((uniFlatItems == null) || (uniFlatFirstItem == null) || (uniFlatContent == null)) {
        return 0;
    }

    var cols = uniFlatItems.width() / uniFlatFirstItem.outerWidth(true) | 0;
    var rows = uniFlatContent.height() / uniFlatFirstItem.outerHeight(true) | 0;
    return cols * rows;
}

//]]>
