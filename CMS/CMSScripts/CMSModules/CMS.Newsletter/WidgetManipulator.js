/**
 * Module for DOM manipulation of the widgets.
 */
cmsdefine(['jQuery',
    'CMS.Newsletter/FrameLoader',
    'CMS.Newsletter/WidgetRenderer',
    'CMS.Newsletter/WidgetValidationService',
    'CMS/ClientLocalization'
], function ($, frameLoader, widgetRenderer, widgetValidationService, localization) {

    var builderFrameSelector = '#builderIframe';

    /**
     * Loads the email widgets into the builder iframe.
     * Used during initial load of the builder.
     * 
     * @param emailContent zones with widgets and its content
     * @param enableWidgetManipulation indicates whether widget manipulation should be enabled
     */
    function loadWidgets(emailContent, enableWidgetManipulation) {

        if (!emailContent) {
            return;
        }

        var $builderFrame = $(builderFrameSelector);
        // Ensure loaded builder frame and render email configuration
        frameLoader.frameLoaded($builderFrame, function () {
            emailContent.zones.forEach(function (zone) {
                $builderFrame.contents().find('.cms-zone[data-zone-id="' + zone.identifier + '"]').each(function (i, el) {
                    zone.widgets.forEach(function (widget) {
                        $(el).append(widgetRenderer.renderWidget(widget, enableWidgetManipulation));
                        widgetValidationService.validateWidget(widget.identifier, widget.hasUnfilledRequiredProperty);
                    });
                });
            });
            widgetValidationService.checkValidity();
        });
    }

    /**
     * Replaces the widget's content.
     *
     * @param {string} widgetId Widget instance identifier
     * @param {string} widgetContent Widget's new content     
     */
    function replaceWidgetContent(widgetId, widgetContent) {
        var $widgetContent = $(builderFrameSelector).contents().find('#' + widgetId + ' > div.cms-widget-content');
        $widgetContent.html(widgetContent);
    }

    return {
        loadWidgets: loadWidgets,
        replaceWidgetContent: replaceWidgetContent
    };
});