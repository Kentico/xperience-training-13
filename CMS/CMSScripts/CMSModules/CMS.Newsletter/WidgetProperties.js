/**
 * Module for displaying and hiding widget properties.
 */
cmsdefine(['jQuery', 'CMS.Newsletter/FrameLoader'], function ($, frameLoader) {

    var $widgetPropertiesIframe, lastWidgetInstanceGuid, $widgetPropertiesWrapper, $widgetPropertiesSlidable;

    /**
     * Shows the widget properties sidebar.
     */
    function showWidgetProperties(widgetInstanceGuid) {
        if (widgetInstanceGuid !== lastWidgetInstanceGuid) {
            $widgetPropertiesIframe.contents().find("body").html('');
            $widgetPropertiesIframe.attr("src", CMS.Application.emailBuilder.widgetPropertiesIframeUrl + "&widgetinstanceguid=" + widgetInstanceGuid);

            displayLoader();
        }

        $widgetPropertiesWrapper.addClass('widget-properties-wrapper-visible');
        $widgetPropertiesSlidable.addClass('widget-properties-slidable-visible');

        lastWidgetInstanceGuid = widgetInstanceGuid;
    }

    /**
     * Hides the widget properties sidebar.
     */
    function hideWidgetProperties() {
        $widgetPropertiesSlidable
            .removeClass('widget-properties-slidable-visible')
            .one('transitionend',
                function (e) {
                    $widgetPropertiesWrapper.removeClass('widget-properties-wrapper-visible');
                });
    }

    function displayLoader() {
        var $loader = $(".widget-properties-loader");

        var loaderTimeout = setTimeout(function () {
            $loader.show();
        }, 200);

        frameLoader.frameLoaded($widgetPropertiesIframe, function () {
            clearTimeout(loaderTimeout);
            $loader.hide();
        });
    }

    function init() {
        $widgetPropertiesIframe = $('#widgetPropertiesIframe');
        $widgetPropertiesWrapper = $(".widget-properties-wrapper");
        $widgetPropertiesSlidable = $widgetPropertiesWrapper.find(".widget-properties-slidable");
    }

    return {
        init: init,
        show: showWidgetProperties,
        hide: hideWidgetProperties
    }
});