/**
 * Drag & drop module for the email builder.
 * Module uses RubaXa sortable library to handle manipulation with widgets.
 * 
 * The init() functions enables the drag & drop functionality and provides the handlers with the specific widget events.
 */
cmsdefine(['jQuery', 'sortable', 'CMS.Newsletter/WidgetRenderer', 'CMS.Newsletter/WidgetSelection', 'CMS.Newsletter/FrameLoader', 'CMS.Newsletter/WidgetValidationService'], function ($, Sortable, widgetRenderer, widgetSelection, frameLoader, widgetValidationService) {

    var $builderContent, $zones, $scrollZones,
        widgetList,
        setData,
        handlers,
        addZoneId, insertIndex,
        emailBuilderData,
        enableWidgetManipulation;

    // Replace the widget placeholder with the actual code
    function insertWidgetMarkup(widget) {
        var $placeholder = $builderContent.find('.cms-email-widget'),
            widgetHtml = widgetRenderer.renderWidget(widget, enableWidgetManipulation);

        $placeholder.replaceWith(widgetHtml);
    }

    // Select the widget once it's inserted and resolved
    function selectInsertedWidget(response) {
        var $widgetElement = $builderContent.find('#' + response.identifier);
        widgetSelection.selectWidget($widgetElement);
        widgetSelection.enableWidgetHighlighting();
    }

    // Validate the widget once it's inserted or removed
    function validateWidget(response) {
        widgetValidationService.validateWidget(response.identifier, response.hasUnfilledRequiredProperty);
    }

    // Lazy-load scroll zones (ensures that zones are already created within the builder iframe)
    function getScrollZones() {
        if (!$scrollZones || !$scrollZones.length) {
            $scrollZones = $builderContent.find('#top_scroll_page, #bottom_scroll_page');
        }
        return $scrollZones;
    }

    function handleDragEnd(evt) {
        $zones.removeClass('cms-zone-highlight');
        $('.cms-email-widget.focus').removeClass('focus');
        $(evt.item).removeClass('cms-widget-dragging');

        // Show the header of the selected widget after dropping the widget
        if (widgetSelection.isWidgetSelected()) {
            widgetSelection.showWidgetHeader(evt.item);
        }

        // Hide the scroll zones after dropping the widget
        getScrollZones().hide();
    }


    function handleDragStart(evt) {
        $zones.addClass('cms-zone-highlight');
        $(evt.item).addClass('cms-widget-dragging');

        // Hide the widget's header so that sorting works properly
        widgetSelection.hideWidgetHeader(evt.item);

        // Disable widget highlighting to prevent double widget selection on drop
        widgetSelection.disableWidgetHighlighting();

        // Show the scroll zones when dragging widgets
        getScrollZones().show();
    }

    function onAddHandler(evt) {
        // Widget added/moved to the zone - get zone ID
        addZoneId = $(evt.to).data('zone-id');
        insertIndex = $(evt.item).index();
    }

    function onStartHandler(evt) {
        handleDragStart(evt);
        addZoneId = insertIndex = undefined;
    }

    function onFilterHandler(evt) {
        handlers
            .removeWidget(evt.item.id)
            .done(function () {
                widgetValidationService.validateWidget(evt.item.id, false);
                widgetValidationService.checkValidity();
            })
            .done(function () { // Remove the widget only if the removal was successful on the server
                $(evt.item).remove();

                // Unselect the widget if it was selected
                widgetSelection.dismissWidgetSelection();
            });
    }

    function onEndHandler(evt) {
        var insertedWidgetTypeId = $(evt.item).data('widget-id'),
            widgetInstanceId = $(evt.item).attr('id'),
            oldZoneId = $(evt.from).data('zone-id');

        handleDragEnd(evt);

        if (evt.from.id === widgetList.id) {
            if (addZoneId && insertedWidgetTypeId) {
                // Insert event
                handlers.insertWidget(insertedWidgetTypeId, addZoneId, insertIndex)
                    .done(insertWidgetMarkup)
                    .done(selectInsertedWidget)
                    .done(validateWidget);

                // Needs to return, because widget highlighting must be enabled
                // after the widget's resolved which is an async operation
                return;
            }
        } else if (oldZoneId && (addZoneId || (evt.oldIndex !== evt.newIndex))) {
            // Move event if position changed (within same zone or to the different zone)
            handlers.moveWidget(widgetInstanceId, (addZoneId || oldZoneId), evt.newIndex);
        }

        widgetSelection.enableWidgetHighlighting();
    }

    // Initialize dragNdrop in iframe and widget highlighting
    function initIframe() {

        $builderContent = $('#' + emailBuilderData.builderIframeId).contents();
        $zones = $builderContent.find('.cms-zone');

        widgetSelection.init();

        if (enableWidgetManipulation) {
            // Initialize drop zones in the email builder
            $zones.each(function (index, zone) {
                Sortable.create(zone,
                {
                    group: {
                        name: 'zone',
                        put: ['widgets', 'zone']
                    },
                    handle: '.cms-widget-header',
                    filter: '.cms-widget-delete',
                    onAdd: onAddHandler,
                    onStart: onStartHandler,
                    onEnd: onEndHandler,
                    onFilter: onFilterHandler,
                    animation: 150,
                    setData: setData
                });
            });
        }
    }

    // Initialize dragNdrop and bind RubaXa handlers
    function init(config) {
        enableWidgetManipulation = config.enableWidgetManipulation;
        emailBuilderData = CMS.Application.emailBuilder;
        var $builderFrame = $('#' + emailBuilderData.builderIframeId);
        var $widgetPropertiesFrame = $('#widgetPropertiesIframe');

        widgetList = $('#widget-listing')[0];

        // Add widget properties sidebar's close button click event handler to dismiss widget selection
        $widgetPropertiesFrame.load(function () {
            $widgetPropertiesFrame.contents().find('body').on('click', '.widget-properties-close-btn', widgetSelection.dismissWidgetSelection);
        });
        
        // Initialize handlers
        handlers = config.handlers;

        // Firefox fix - drag'n'drop
        if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1) {
            setData = function (dataTransfer, dragEl) {
                // Firefox needs to have specified the dataTransfer object explicitly
                // otherwise it opens a new browser tab when the dragged element is dropped
                dataTransfer.setData('text/html', dragEl.innerHTML);
            }
        }

        // Load builder frame
        frameLoader.frameLoaded($builderFrame, initIframe);

        if (enableWidgetManipulation) {
            // Initialize drag & drop for the widgets in the widget list
            Sortable.create(widgetList,
            {
                group: {
                    name: 'widgets',
                    pull: 'clone'
                },
                sort: false,
                setData: setData,
                onStart: onStartHandler,
                onEnd: onEndHandler
            });

            // Need to implement widget highlighting via JS because Chrome keeps the widget highlighted after dropping it into a zone 
            $(widgetList)
                .on('mouseenter', '.cms-email-widget', function () {
                    $(this).addClass('focus');
                })
                .on('mouseleave', '.cms-email-widget', function () {
                    $(this).removeClass('focus');
                });
        } else {
            $(widgetList).addClass('read-only');
        }
    }

    // Publish the module initialization
    return {
        init: init
    };
});