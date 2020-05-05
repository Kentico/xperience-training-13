/**
 * Module for updating widget content after properties change.
 */
cmsdefine(['jQuery', 'CMS.Newsletter/WidgetSelection', 'CMS.Newsletter/WidgetValidationService'], function ($, widgetSelection, widgetValidationService) {

    var handlers, $propertiesFrame;

    /**
     * Updates the widget's content if the properties were saved successfully.
     */
    function updateWidget() {
        var cms = $propertiesFrame[0].contentWindow.CMS;
        if (typeof cms !== 'undefined') {
            var saveStatusFieldSelector = cms.Application.widgetProperties.saveStatusFieldSelector;
            var $saveStatus = $propertiesFrame.contents().find(saveStatusFieldSelector);

            if ($saveStatus.val() === '1' && typeof handlers.updateWidget === 'function') {
                var widgetId = widgetSelection.getSelectedWidgetIdentifier();
                var widget = widgetSelection.getSelectedWidget();
                handlers.updateWidget(widgetId)
                        .done(function(response) {
                            widgetValidationService.validateWidget(response.Identifier, response.HasUnfilledRequiredProperty);
                            widgetValidationService.checkValidity();
                        })
                        .done(function() {
                            widgetSelection.adjustWidgetHeaderPosition(widget)
                        });
                $saveStatus.val('');
            }
        }
    }

    /**
     * Registers update widget callback raised when the widget properties iframe is loaded after a postback in a update panel
     */
    function bindPostbackEvent() {
        $propertiesFrame.get(0).contentWindow.Sys.WebForms.PageRequestManager.getInstance().add_endRequest(updateWidget);
    }

    /**
     * Module initialization.
     */
    function init(widgetHandlers) {
        handlers = $.extend({}, widgetHandlers);
        $propertiesFrame = $('#widgetPropertiesIframe');

        $propertiesFrame.load(bindPostbackEvent);
    }

    // Publish the module initialization
    return {
        init: init
    };
});