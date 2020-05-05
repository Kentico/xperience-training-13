/**
 * Service for rendering the widget content.
 */
cmsdefine(['jQuery', 'Underscore', 'CMS/ClientLocalization'], function ($, _, localization) {

    var WIDGET_TEMPLATE =
'<div class="cms-widget" id="<%= widgetId %>">\
    <div class="cms-widget-header">\
        <% if (includeControls) { %>\
            <div class="cms-widget-controls-wrapper">\
                <div class="cms-widget-controls">\
                    <div class="cms-widget-controls-right">\
                        <button class="cms-widget-delete">\
                            <i class="icon-bin"></i>\
                        </button>\
                    </div>\
                    <div class="cms-widget-controls-left"><i class="icon-dots-vertical"></i></div>\
                </div>\
            </div>\
        <% } %>\
    </div>\
    <% if (widgetHasUnfilledRequiredProperty) { %>\
        <div class="cms-widget-content cms-widget-invalid">\
            <i class="icon-exclamation-triangle cms-icon-150 cms-invalid-exclamationmark" aria-hidden="true" title="<%= tooltipText %>"/>\
            <%= widgetContent %>\
        </div>\
    <% } else if (widgetDefinitionNotFound) { %>\
        <div class="cms-widget-content cms-widget-missing">\
            <i class="icon-exclamation-triangle cms-icon-150 cms-missing-exclamationmark" aria-hidden="true"/>\
            <span> This widget definition no longer exists! </span>\
        </div>\
    <% } else { %>\
        <div class="cms-widget-content">\
            <%= widgetContent %>\
        </div>\
    <% } %>\
 </div>';

    /**
     * Renders the given widget object.
     * 
     * @param widget resolved widget
     * @param widget.identifier widget identifier
     * @param widget.html resolved widget code
     * @param widget.hasUnfilledRequiredProperty indicates whether widget has some unfilled required properties
     *
     * @param includeControls indicates whether widget controls should be included in the result widget HTML
     *
     * @returns {string} widget html code
     */
    function renderWidget(widget, includeControls) {
        return _.template(WIDGET_TEMPLATE, {
            widgetId: widget.identifier,
            widgetContent: widget.html,
            widgetHasUnfilledRequiredProperty: widget.hasUnfilledRequiredProperty,
            widgetDefinitionNotFound: widget.widgetDefinitionNotFound,
            includeControls: includeControls,
            tooltipText : localization.getString("emailbuilder.error.unfilledrequiredproperties.tooltip")
        });
    }

    return {
        renderWidget: renderWidget
    };
});