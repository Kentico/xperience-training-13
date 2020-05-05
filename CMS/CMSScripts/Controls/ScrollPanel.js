// ScrollPanel.js

var scrollPanels = new Array();
var scrollResizeTimer = null;
var scrollGlobalPosition = 0;

// Initialize the scroll panel variables
function scrollPanelInit(scrollContainer, forceReload) {

    if (!(scrollContainer in scrollPanels)) {
        return;
    }

    var scrollPanel = scrollPanels[scrollContainer];
    var isVerticalLayout = scrollPanel.IsVerticalLayout;
    var isRTL = scrollPanel.IsRTL;

    if (forceReload) {
        // Renew the scroll panel variables
        scrollPanel.Container = null;
        scrollPanel.ScrollAreaContainer = null;
        scrollPanel.LastItem = null;
        scrollPanel.ItemWidth = null;
        scrollPanel.ItemHeight = null;
    }

    // Load the scroll panel element variables
    if (scrollPanel.Container === null) {
        scrollPanel.Container = $cmsj('#' + scrollContainer);
    }

    // Do not proceed when the scroll panel is hidden
    if (!scrollPanel.Container.is(':visible')) {
        return;
    }

    if (scrollPanel.ScrollAreaContainer === null) {
        scrollPanel.ScrollAreaContainer = $cmsj('#' + scrollPanel.InnerContainerId + ':first', scrollPanel.Container);
    }

    if (scrollPanel.SeparatorItem === null) {
        if (scrollPanel.ItemSeparatorClass.length > 0) {
            scrollPanel.SeparatorItem = $cmsj('.' + scrollPanel.ItemSeparatorClass + ':first', scrollPanel.ScrollAreaContainer);
        }
    }

    if (scrollPanel.LastItem === null) {

        // Setup the last item variable (and its related variables) when the default step is not defined
        if (scrollPanel.ItemClass.length > 0) {
            var items = $cmsj('.' + scrollPanel.ItemClass, scrollPanel.Container);
            var itemsVisible = items;
            if (items.first().parent().is(":visible")) {
                // Filter out invisible items (Web part toolbar)
                itemsVisible = items.filter(":visible");
            }
            var lastItem = itemsVisible.last();
            scrollPanel.LastItem = lastItem;
            if (!spIsAbsuluteOrFixed(scrollPanel.Container.parent())) {
                scrollPanel.ScrollAreaContainer.height(scrollPanel.Container.outerHeight());
            }

            if (scrollPanel.ItemWidth === null) {
                // Calculate the ItemWidht according to the rendered items if their class is defined
                var separatorWidth = 0;
                if (scrollPanel.SeparatorItem != null) {
                    separatorWidth = scrollPanel.SeparatorItem.outerWidth();
                }

                scrollPanel.ItemWidth = lastItem.outerWidth() + spTryParseInt(lastItem.css("margin-left"), 0) + spTryParseInt(lastItem.css("margin-right"), 0) + separatorWidth;
            }

            if (scrollPanel.ItemHeight === null) {
                // Calculate the ItemHeight according to the rendered items if their class is defined
                var separatorHeight = 0;
                if (scrollPanel.SeparatorItem != null) {
                    separatorHeight = scrollPanel.SeparatorItem.outerHeight();
                }

                scrollPanel.ItemHeight = lastItem.outerHeight() + spTryParseInt(lastItem.css("margin-top"), 0) + spTryParseInt(lastItem.css("margin-bottom"), 0) + separatorHeight;
            }

            // Expand the scroll area div in order to accommodate all the rendered web part items and ensure its scrolling capabilities
            if (scrollPanel.AdjustScrollAreaSize && !spIsAbsuluteOrFixed(scrollPanel.Container.parent())) {
                if (!isVerticalLayout) {
                    if ((items.length * scrollPanel.ItemWidth) > scrollPanel.ScrollStep) {
                        scrollPanel.ScrollAreaContainer.width((scrollPanel.ItemWidth * items.length) + scrollPanel.Container.outerWidth());
                    }
                }
                else {
                    if ((items.length * scrollPanel.ItemHeight) > scrollPanel.ScrollStep) {
                        scrollPanel.ScrollAreaContainer.height((scrollPanel.ItemHeight * items.length) + scrollPanel.Container.outerHeight());
                    }
                }
            }
        }
        else {
            // Calculate the ItemWidth/Height as a total width/height of the rendered content of the scroll panel (i.e. <UL> -> <LI> structure without css class)
            if ((scrollPanel.ItemWidth === null) || (scrollPanel.ItemHeight === null)) {
                scrollPanel.ItemWidth = scrollPanel.ScrollAreaContainer.outerWidth();
                scrollPanel.ItemHeight = scrollPanel.ScrollAreaContainer.outerHeight();
            }
        }

    }

    // Check whether to slide to the beginning (top/left) when the displayed area is out of boundaries
    if ((scrollPanel.ItemClass.length > 0)) {
        if (!isVerticalLayout) {
            var lastItemLeft = 0;
            if (scrollPanel.LastItem.length > 0) {
                lastItemLeft = scrollPanel.LastItem.position().left;
            }

            if ((scrollPanel.LastItem.length == 0)
			|| (!isRTL && (lastItemLeft < 0) && (lastItemLeft + scrollPanel.LastItem.width() < 0))
			|| (isRTL && (lastItemLeft > scrollPanel.Container.position().left + scrollPanel.Container.width()))) {
                var bodyElem = $cmsj('body');
                if (bodyElem.hasClass('Chrome') && isRTL) {
                    scrollPanel.Container.scrollLeft(scrollPanel.ScrollAreaContainer.outerWidth());
                }
                else {
                    scrollPanel.Container.scrollLeft(0);
                }
                spScrollBack(scrollContainer);
            }
        }
        else {
            if ((scrollPanel.LastItem.length == 0) || (scrollPanel.LastItem.position().top < 0)) {
                scrollPanel.Container.scrollTop(0);
                spScrollBack(scrollContainer);
            }
        }
    }

    // Setup the scroll step and set the forward button visibility
    spSetupForwardScroll(scrollContainer, forceReload);

    // Display/Hide the back scroller
    var backScrollerVisible = false;
    if (!isVerticalLayout) {
        if (!isRTL) {
            backScrollerVisible = (scrollPanel.Container.scrollLeft() > 0);
        }
        else {
            // RTL
            var originalScrollLeft = scrollPanel.Container.scrollLeft();

            // Display the back scroller if it is possible to scroll back by 1px
            var scrollOnePixelForward = (scrollPanel.ForwardScrollStep > 0) ? 1 : -1;
            scrollPanel.Container.scrollLeft(originalScrollLeft - scrollOnePixelForward);
            backScrollerVisible = (scrollPanel.Container.scrollLeft() != originalScrollLeft);

            // Restore the original scroll position
            scrollPanel.Container.scrollLeft(originalScrollLeft);
        }
    }
    else {
        backScrollerVisible = (scrollPanel.Container.scrollTop() > 0);
    }

    if (backScrollerVisible) {
        scrollPanel.BackwardScroller.fadeIn();
    }
    else {
        scrollPanel.BackwardScroller.fadeOut();
    }

    if (forceReload) {
        // Append mouse wheel event on menu
        scrollPanel.Container.mousewheel(function (event, delta) {
            if (delta > 0) { spScrollBack(scrollContainer); }
            else { spScrollForward(scrollContainer); }
            return false;
        });

        // Disable selection of the scroll buttons
        scrollPanel.ForwardScroller.spDisableSelection();
        scrollPanel.BackwardScroller.spDisableSelection();
    }
}

// Setup the scroll step and set the forward button visibility
function spSetupForwardScroll(scrollContainer, forceReload) {
    var scrollPanel = scrollPanels[scrollContainer];
    var isVerticalLayout = scrollPanel.IsVerticalLayout;
    var isRTL = scrollPanel.IsRTL;
    var forwardScrollerFadeIn = (scrollPanel.ItemClass.length == 0);

    var lastItem = scrollPanel.LastItem;
    if (lastItem == null) {
        lastItem = scrollPanel.ScrollAreaContainer.children().last();
    }

    var lastItemVisible = true;
    // Set the lastItemVisible variable only when the whole scroll panel is visible
    if (lastItem.parent().is(":visible")) {
        lastItemVisible = lastItem.is(':visible');
    }

    // Display the last item in order to indicate whether the forward scroll button should be displayed
    if (!lastItemVisible) {
        lastItem.show();
    }

    var containerPosition = scrollPanel.Container.position();
    var containerLeft = containerPosition.left;
    var containerTop = containerPosition.top;
    var docScroll = { x: 0, y: 0 };

    var containerParent = scrollPanel.Container.parent();
    // Get scroll container position for absolute/fixed context menus
    if (spIsAbsuluteOrFixed(containerParent)) {
        var parentContainerPosition = containerParent.offset();
        var parentContainerBorderLeft = parseInt(containerParent.css('border-left-width'), 10);
        var parentContainerBorderTop = parseInt(containerParent.css('border-top-width'), 10);
        containerLeft = parentContainerPosition.left + parentContainerBorderLeft;
        containerTop = parentContainerPosition.top + parentContainerBorderTop;

        // Take in account document scrolling for absolute positioning
        docScroll.x = document.body.scrollLeft || document.documentElement.scrollLeft;
        docScroll.y = document.body.scrollTop || document.documentElement.scrollTop;
    }

    // Decide whether the forward scroller should be displayed
    if (lastItem.length > 0) {
        if (!isVerticalLayout) {
            if (!isRTL) {
                forwardScrollerFadeIn = (lastItem.position().left + lastItem.width() - docScroll.x) > (containerLeft + scrollPanel.Container.width() - spTryParseInt(scrollPanel.ScrollAreaContainer.css('margin-right'), 0));
            }
            else {
                forwardScrollerFadeIn = (lastItem.position().left - docScroll.x) < (containerLeft + spTryParseInt(scrollPanel.ScrollAreaContainer.css('margin-left'), 0));
            }
        }
        else {
            var lastItemMargin = spTryParseInt(lastItem.css('margin-top'), 10);
            forwardScrollerFadeIn = (lastItem.offset().top + scrollPanel.ItemHeight - docScroll.y) > (containerTop + scrollPanel.Container.height() + lastItemMargin - spTryParseInt(scrollPanel.ScrollAreaContainer.css('margin-bottom'), 0));
        }
    }

    // Hide the last item if it was originally hidden
    if (!lastItemVisible) {
        lastItem.hide();
    }

    if (forceReload) {
        scrollPanel.ForwardScrollStep = scrollPanel.ScrollStep;

        // Revert the scroll step for RTL
        if (!scrollPanel.IsVerticalLayout && scrollPanel.IsRTL) {
            // ScrollLeft() in RTL - different behavior for different browsers:
            //                             |  FF  |  IE  |  Chrome/Safari  |
            //                  No scroll: |  0   |  0   |  scrollAreaWidth (example: 4000)
            // After 300px forward scroll: | -300 |  300 |  scrollAreaWidth - 300 (example: 3700)

            // Revert the forward scroll step
            scrollPanel.ForwardScrollStep = (-1) * scrollPanel.ScrollStep;
            var bodyElem = $cmsj('body');

            // Keep the positive step size for IE8/9 bug in RTL
            if (bodyElem.hasClass('IE8') || bodyElem.hasClass('IE9')) {
                scrollPanel.ForwardScrollStep = scrollPanel.ScrollStep;
            }
        }
    }

    if (scrollPanel.Container.is(':visible')) {
        // Display/Hide the forward scroller
        if (forwardScrollerFadeIn) {
            // Set the flag indicating whether the forward scroll is enabled
            scrollPanel.ForwardScrollEnabled = true;
            scrollPanel.ForwardScroller.fadeIn();
        }
        else {
            // Set the flag indicating whether the forward scroll is enabled
            scrollPanel.ForwardScrollEnabled = false;
            scrollPanel.ForwardScroller.fadeOut();
        }
    }
}

// Scroll back by predefined step
function spScrollBack(scrollContainer) {
    var scrollPanel = scrollPanels[scrollContainer];

    if (!scrollPanel.IsVerticalLayout) {
        if (scrollPanel.Container.scrollLeft() != 0) {
            var scroll = scrollPanel.Container.scrollLeft() - scrollPanel.ForwardScrollStep;
            scrollGlobalPosition = scroll;
            scrollPanel.Container.stop().animate({ scrollLeft: scroll }, 300, function () { scrollPanelInit(scrollContainer, false) });
        }
    }
    else {
        if (scrollPanel.Container.scrollTop() != 0) {
            var scroll = scrollPanel.Container.scrollTop() - scrollPanel.ForwardScrollStep;
            scrollGlobalPosition = scroll;
            scrollPanel.Container.stop().animate({ scrollTop: scroll }, 300, function () { scrollPanelInit(scrollContainer, false) });
        }
    }
}

// Scroll to the stored position manually
function spScrollSetPosition(scrollContainer) {
    window.setTimeout(function () {
        var scrollPanel = scrollPanels[scrollContainer];
        if (!scrollPanel.IsVerticalLayout) {
            var bodyElement = $cmsj('body');
            if (bodyElement.hasClass('Chrome') && scrollPanel.IsRTL) {
                var width = scrollPanel.ScrollAreaContainer.outerWidth();
                scrollPanel.Container.scrollLeft(width - scrollPanel.ForwardScrollStep);
            }
            else {
                scrollPanel.Container.scrollLeft(scrollGlobalPosition);
            }
        }
        else {
            scrollPanel.Container.scrollTop(scrollGlobalPosition);
        }

        scrollPanelInit(scrollContainer, false);
    }, 1);
}


// Scroll forward by predefined step
function spScrollForward(scrollContainer) {
    var scrollPanel = scrollPanels[scrollContainer];
    // Enable.disable forward scrolling according to the visibility of the last item
    spSetupForwardScroll(scrollContainer, false);

    if (!scrollPanel.IsVerticalLayout) {
        if (scrollPanel.ForwardScrollEnabled) {
            var scroll = scrollPanel.Container.scrollLeft() + scrollPanel.ForwardScrollStep;
            scrollGlobalPosition = scroll;
            scrollPanel.Container.stop().animate({ scrollLeft: scroll }, 300, function () { scrollPanelInit(scrollContainer, false) });
        }
    }
    else {
        if (scrollPanel.ForwardScrollEnabled) {
            var scroll = scrollPanel.Container.scrollTop() + scrollPanel.ForwardScrollStep;
            scrollGlobalPosition = scroll;
            scrollPanel.Container.stop().animate({ scrollTop: scroll }, 300, function () { scrollPanelInit(scrollContainer, false) });
        }
    }

    // Raise the external OnForwardScroll event
    if (scrollPanel.OnForwardScroll != null) {
        scrollPanel.OnForwardScroll(scrollPanel);
    }
}

// Convert the input string to int. Use defaultValue when the input string is not a number.
function spTryParseInt(str, defaultValue) {
    var retValue = defaultValue;
    if ((typeof (str) != 'undefined') && (str != null) && (str.length > 0)) {
        retValue = parseInt(str, 10);
        if (isNaN(retValue)) {
            retValue = defaultValue;
        }
    }

    return retValue;
}

// Disable selection extender
$cmsj.fn.extend({
    spDisableSelection: function () {
        return this.each(function () {
            this.onselectstart = function () { return false; };
            this.unselectable = "on";
            $cmsj(this)
                .css('user-select', 'none')
                .css('-o-user-select', 'none')
                .css('-moz-user-select', 'none')
                .css('-khtml-user-select', 'none')
                .css('-webkit-user-select', 'none');
        });
    }
});

// Reload all the scrollable panels
$cmsj(window).resize(function () {
    clearTimeout(scrollResizeTimer);
    scrollResizeTimer = setTimeout(function () { spRefreshScrollButtons(); }, 200);
});

// Refresh scroll panel button positions and set container height
function spRefreshScrollButtons() {

    for (var panel in scrollPanels) {
        var scrollPanel = scrollPanels[panel];

        // Proceed only real scroll panels (omit other jQuery system objects)
        if (typeof (scrollPanel.ScrollStep) === 'undefined') {
            continue;
        }

        if (scrollPanel.IsVerticalLayout && window.CKEDITOR) {
            // Refresh scroll panel button positions due to CKToolbar on the page

            // Get forward scroll position
            var forwardScrollerPosition = scrollPanel.ForwardScroller.position().top;
            if (!scrollPanel.ForwardScroller.is(":visible")) {
                scrollPanel.ForwardScroller.show();
                forwardScrollerPosition = scrollPanel.ForwardScroller.position().top;
                scrollPanel.ForwardScroller.hide();
            }

            // Set the container height according to the displayed area (context menus)
            if (!spIsAbsuluteOrFixed(scrollPanel.Container.parent())) {
                scrollPanel.Container.height(forwardScrollerPosition + scrollPanel.ForwardScroller.outerHeight() - scrollPanel.Container.position().top);
            }

            // Move the back scroll button to the corrected top position
            scrollPanel.BackwardScroller.css("top", scrollPanel.Container.position().top);
        }

        // Reload panel properties
        scrollPanelInit(panel, true);
    }
}

// Indicates whether the given object has absolute or fixed position (is context menu)
function spIsAbsuluteOrFixed(jObj) {
    var position = jObj.css('position');
    return ((position == 'absolute') || (position == 'fixed'));
}