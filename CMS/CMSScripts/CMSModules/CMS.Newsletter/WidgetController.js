/**
 * Widget controller providing widget manipulation actions.
 */
cmsdefine([
    'jQuery',
    'CMS.Newsletter/WidgetRepository',
    'CMS.Newsletter/MessageService',
    'CMS.Newsletter/WidgetManipulator',
    'CMS/ClientLocalization'
], function ($, widgetRepository, msgService, widgetManipulator, localization) {

    var WidgetController = function (issueId) {
        var ctrl = this;
        ctrl.issueId = issueId;
    };

    WidgetController.prototype.insertWidget = function (widgetId, zoneId, index) {
        return widgetRepository.insertWidget(this.issueId, widgetId, zoneId, index)
            .done(function () {
                msgService.showSuccess(localization.getString('general.changessaved'));
            })
            .fail(function () {
                msgService.showError(localization.getString('emailbuilder.generalerror'));
            });
    };

    WidgetController.prototype.moveWidget = function (widgetId, zoneTo, indexTo) {
        return widgetRepository.moveWidget(this.issueId, widgetId, zoneTo, indexTo)
            .done(function () {
                msgService.showSuccess(localization.getString('general.changessaved'));
            })
            .fail(function () {
                msgService.showError(localization.getString('emailbuilder.generalerror'));
            });
    };

    WidgetController.prototype.removeWidget = function (widgetId) {
        if (!confirm(localization.getString('emailbuilder.widgetdeleteconfirm'))) {
            return $.Deferred().reject();
        }

        return widgetRepository.removeWidget(this.issueId, widgetId)
            .done(function () {
                msgService.showSuccess(localization.getString('general.changessaved'));
            })
            .fail(function () {
                msgService.showError(localization.getString('emailbuilder.generalerror'));
            });
    };

    WidgetController.prototype.updateWidget = function (widgetId) {
        return widgetRepository.getWidgetContent(this.issueId, widgetId)
            .done(function (response) {
                widgetManipulator.replaceWidgetContent(response.Identifier, response.Html);
                msgService.showSuccess(localization.getString('general.changessaved'));
            });
    };

    return WidgetController;
});