/**
 * Module for displaying and hiding widget header controls.
 *
 * The init() function registers the widget highlighting event handlers and handlers for selecting and dismissing a widget.
 */
cmsdefine(['jQuery', 'CMS.Newsletter/WidgetProperties', 'CMS.Newsletter/WidgetValidationService'], function ($, widgetProperties, widgetValidationService) {

    var HEADER_HEIGHT = 40, WIDGET_BORDER_WIDTH = 1;
    var $builderContent, $zones, $selectedWidget;
    var widgetHighlighting = true;

    /**
     * Selects the widget while preventing its controls to be hidden when losing mouse focus.
     *
     * @param {Element|jQuery object} widget to select
     */
    function selectWidget(widget) {
        var $widget = $(widget),
            widgetInstanceGuid = $widget.attr('id');

        showWidgetHeader($widget);
        $widget.addClass('cms-widget-selected');

        if (isWidgetSelected()) {
            dismissWidgetSelection();
        }

        $selectedWidget = $widget;
        $selectedWidget.click(stopEventPropagation);

        widgetProperties.show(widgetInstanceGuid);
    }

    /**
     * Highlights the widget while exposing its header with controls.
     *
     * @param {Element|jQuery object} widget to display its controls
     */
    function showWidgetHeader(widget) {
        adjustWidgetHeaderPosition(widget);
        $(widget).addClass('cms-widget-highlight');
    }

    /**
     * Hides the widget's header and its controls.
     *
     * @param {Element|jQuery object} widget to hide its controls
     */
    function hideWidgetHeader(widget) {
        $(widget).removeClass('cms-widget-highlight')
                 .find('.cms-widget-header')
                 .css('margin-top', '');
    }

    /**
     * Dismisses selected widget's highlighting.
     */
    function dismissWidgetSelection() {
        if (isWidgetSelected()) {
            hideWidgetHeader($selectedWidget);
            $selectedWidget.removeClass('cms-widget-selected');
            $selectedWidget.unbind('click', stopEventPropagation);
            $selectedWidget = $builderContent.find('.cms-widget-selected');

            // Hide widget properties when no other widget was selected
            if (!$selectedWidget.length) {
                widgetProperties.hide();
            }

            widgetValidationService.checkValidity();
        }
    }

    /**
     * Moves the widget header to the bottom of the widget if there's not enough space for it above the widget.
     *
     * @param {Element|jQuery object} widget to adjust its header position
     */
    function adjustWidgetHeaderPosition(widget) {
        if (isHeaderOnBottom(widget)) {
            var $widget = $(widget);
            var headerBottomPosition = $widget.find('.cms-widget-content').height() - WIDGET_BORDER_WIDTH;

            $widget.find('.cms-widget-header')
                   .css('margin-top', headerBottomPosition);
        }
    }

    /**
     * Returns selected widget.
     *
     * @returns {boolean} The selected widget or null if no widget is currently selected
     */
    function getSelectedWidget() {
        return isWidgetSelected() ? $selectedWidget : null;
    }

    /**
     * Gets the selected widget instance identifier.
     *
     * @returns {string} The selected widget's ID
     */
    function getSelectedWidgetIdentifier() {
        return isWidgetSelected() ? $selectedWidget.attr('id') : '';
    }

    /**
     * Determines whether there is an actively selected widget.
     *
     * @returns {boolean} True if a widget is currently selected, otherwise returns false
     */
    function isWidgetSelected() {
        return $selectedWidget && ($selectedWidget.length > 0);
    }

    /**
     * Disables widget higlighting when cursor moves over a widget.
     */
    function disableWidgetHighlighting() {
        widgetHighlighting = false;
    }

    /**
     * Enables widget higlighting when cursor moves over a widget.
     */
    function enableWidgetHighlighting() {
        widgetHighlighting = true;
    }

    /**
     * Determines whether the widget's header should be located on the bottom of the widget
     *
     * @param {Element|jQuery object} widget for which the header position should be determined
     */
    function isHeaderOnBottom(widget) {
        return $(widget).offset().top < HEADER_HEIGHT;
    }

    function stopEventPropagation(event) {
        event.stopPropagation();
    }

    function init() {
        widgetProperties.init();

        $builderContent = $('#builderIframe').contents();
        $zones = $builderContent.find('.cms-zone');

        $zones
            .on('mouseenter', '.cms-widget', function () {
                if (widgetHighlighting && !isWidgetSelected()) {
                    showWidgetHeader(this);
                }
            })
            .on('mouseleave', '.cms-widget', function () {
                if (!isWidgetSelected()) {
                    hideWidgetHeader(this);
                }
            })
            .on('click', '.cms-widget', function (event) {
                event.stopPropagation();
                selectWidget(this);
            });

        $builderContent.click(dismissWidgetSelection);
    }

    // Publish the module initialization and methods for widget management
    return {
        init: init,
        showWidgetHeader: showWidgetHeader,
        isWidgetSelected: isWidgetSelected,
        hideWidgetHeader: hideWidgetHeader,
        selectWidget: selectWidget,
        dismissWidgetSelection: dismissWidgetSelection,
        getSelectedWidget: getSelectedWidget,
        getSelectedWidgetIdentifier: getSelectedWidgetIdentifier,
        disableWidgetHighlighting: disableWidgetHighlighting,
        enableWidgetHighlighting: enableWidgetHighlighting,
        adjustWidgetHeaderPosition: adjustWidgetHeaderPosition
    };
});