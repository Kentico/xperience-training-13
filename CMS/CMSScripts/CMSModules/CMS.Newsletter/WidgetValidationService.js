/**
 * Module for validation of the widgets.
 */
cmsdefine([
    'jQuery',
    'CMS.Newsletter/MessageService',
    'CMS/ClientLocalization'
], function ($, msgService, localization) {

    var builderFrameSelector = '#builderIframe';
    var invalidWidgetIds = [];

    /**
     * Checks if widget has filled all required properties and highlights it if not.
     * Widget with unfilled required properties is kept in collection.
     * 
     * @param {string} widgetId Widget instance identifier     
     * @param {bool} indicates whether widget has unfilled required properties
     */
    function validateWidget(widgetId, hasUnfilledRequiredProperty) {
        if (hasUnfilledRequiredProperty) {
            addInvalidWidget(widgetId);
        } else {
            removeFromInvalidWidgets(widgetId);
        }
    }
    
    /**
     * Shows or hides information message depending on validity of widgets within email.
     */
    function checkValidity() {
        if (invalidWidgetIds.length > 0) {
            msgService.showInfoRight(localization.getString('emailbuilder.error.unfilledrequiredproperties'));
        } else {
            msgService.hideInfoRight();
        }
    }

    function removeFromInvalidWidgets(widgetId) {
        var index = invalidWidgetIds.indexOf(widgetId);
        if (index > -1) {
            invalidWidgetIds.splice(index, 1);
        }

        $(builderFrameSelector).contents()
                     .find('#' + widgetId + ' > div.cms-widget-content')
                     .removeClass('cms-widget-invalid');
    }

    function addInvalidWidget(widgetId) {
        if (invalidWidgetIds.indexOf(widgetId) === -1) {
            invalidWidgetIds.push(widgetId);
        }
    }

    return {
        validateWidget: validateWidget,
        checkValidity: checkValidity
    };
});