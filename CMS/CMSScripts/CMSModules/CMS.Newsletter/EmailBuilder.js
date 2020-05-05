/**
 * Email builder initialization module.
 */
cmsdefine([
    'jQuery',
    'CMS.Newsletter/WidgetController',
    'CMS.Newsletter/WidgetDnD',
    'CMS.Newsletter/WidgetManipulator',
    'CMS.Newsletter/WidgetUpdater',
    'CMS.Newsletter/MessageService',
    'CMS/ClientLocalization',
    'CMS/Tabs',
    'CMS/UrlHelper'
], function ($, WidgetController, dragNdrop, widgetManipulator, widgetUpdater, msgService, localization, tabs, urlHelper) {

    var EmailBuilder = function (data) {
        var QUERY_PARAMETER_SELECTED_TAB_INDEX = 'selectedtabindex';
        var QUERY_PARAMETER_SAVED = 'saved';

        var queryString = window.location.search;

        var selectedTabIndex = urlHelper.getParameterString(queryString, QUERY_PARAMETER_SELECTED_TAB_INDEX, null);
        var showSavedChanges = urlHelper.getParameterString(queryString, QUERY_PARAMETER_SAVED, false);

        // Show changes were saved info message on first load of widgets
        if (showSavedChanges) {
            queryString = urlHelper.removeParameter(queryString, QUERY_PARAMETER_SAVED);

            // Remove query string value from url to prevent duplicate actions
            history.replaceState({}, '', queryString);
            msgService.showSuccess(localization.getString('general.changessaved'));
        }

        if (selectedTabIndex != null) {
            // Remove query string value from url to prevent duplicate actions
            history.replaceState({}, '', urlHelper.removeParameter(queryString, QUERY_PARAMETER_SELECTED_TAB_INDEX));
        }

        var emailBuilderData = CMS.Application.emailBuilder;

        tabs.init({
            el: '.tabs',
            tabNavigationLinks: '.tab-link',
            tabContentContainers: '.tab-content',
            selectedTabIndex: selectedTabIndex
        });

        if (!data.enableWidgetManipulation) {
            msgService.showInfo(localization.getString('emailbuilder.readonlymode'));
        }

        var ctrl = new WidgetController(data.issueId);

        // Init drag & drop widgets (bind ctrl to ensure 'this' in the prototype functions)
        dragNdrop.init({
            enableWidgetManipulation: data.enableWidgetManipulation,
            handlers: {
                insertWidget: ctrl.insertWidget.bind(ctrl),
                moveWidget: ctrl.moveWidget.bind(ctrl),
                removeWidget: ctrl.removeWidget.bind(ctrl)
            }
        });

        widgetUpdater.init({
            updateWidget: ctrl.updateWidget.bind(ctrl)
        });

        // Render initial configuration
        widgetManipulator.loadWidgets(data.emailContent, data.enableWidgetManipulation);

        // Redirect to variant after variant selection change
        $('#' + emailBuilderData.variantSelectorId).change(function () {
            var redirectUrlQueryString = urlHelper.setParameter(queryString, 'objectid', $(this).val());
            redirectUrlQueryString = urlHelper.setParameter(redirectUrlQueryString, QUERY_PARAMETER_SELECTED_TAB_INDEX, tabs.getSelectedTabIndex());

            var currentLocation = window.location.href.split('?')[0];
            window.location.replace(currentLocation + redirectUrlQueryString);
        });
    }

    return EmailBuilder;
});