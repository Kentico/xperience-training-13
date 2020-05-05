/**
 * Email widget repository that handles basic widget manipulation within the email.
 */
cmsdefine(['jQuery', 'CMS/Application'], function ($, app) {

    var webApiURL = app.getData('applicationUrl') + 'cmsapi/EmailBuilder/';

    /**
     * Inserts the widget into the zone on the specified position.
     * 
     * @param {number} issueId IssueInfo identifier
     * @param {string} widgetId Widget identifier (widgetType)
     * @param {string} zoneId zone identifier
     * @param {number} index position within the zone
     * @returns widget containing the identifier and resolved html
     */
    function insertWidget(issueId, widgetId, zoneId, index) {
        return $.post(webApiURL + 'InsertWidget', {
            IssueIdentifier: issueId,
            WidgetTypeIdentifier: widgetId,
            ZoneIdentifier: zoneId,
            Index: index
        }).then(function(response) {
            return {
                identifier: response.Identifier,
                html: response.Html,
                hasUnfilledRequiredProperty: response.HasUnfilledRequiredProperty
            }
        });
    }

    /**
     * Moves the widget to the new position and new zone.
     * 
     * @param {number} issueId IssueInfo identifier
     * @param {string} widgetId Widget instance identifier
     * @param {string} toZoneId new zone identifier
     * @param {number} toIndex new index within the new zone
     */
    function moveWidget(issueId, widgetId, toZoneId, toIndex) {
        return $.post(webApiURL + 'MoveWidget', {
            IssueIdentifier: issueId,
            ZoneIdentifier: toZoneId,
            WidgetIdentifier: widgetId,
            Index: toIndex
        });
    }

    /**
     * Removes the widget from an email issue.
     * 
     * @param {number} issueId IssueInfo identifier
     * @param {string} widgetId Widget instance identifier
     */
    function removeWidget(issueId, widgetId) {
        return $.post(webApiURL + 'RemoveWidget', {
            IssueIdentifier: issueId,
            WidgetIdentifier: widgetId
        });
    }

    /**
     * Gets the widget's content.
     * 
     * @param {number} issueId IssueInfo identifier
     * @param {string} widgetId Widget instance identifier
     */
    function getWidgetContent(issueId, widgetId) {
        return $.get(webApiURL + 'GetWidgetContent', {
            IssueIdentifier: issueId,
            WidgetIdentifier: widgetId
        });
    }

    // Publish methods
    return {
        insertWidget: insertWidget,
        moveWidget: moveWidget,
        removeWidget: removeWidget,
        getWidgetContent: getWidgetContent
    };
});