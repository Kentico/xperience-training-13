/*
*   This class contains handlers for selecting graph elements.
*/
var GraphSelectionHandler = $class({

    /*
    *   General handler for purposes of deselecting element.
    */
    getDeselectItemHandler: function (graph) {
        return function (e) {
            var selectedItem = graph.selectedItem;
            if (selectedItem != null) {
                if (selectedItem.removeReattachHelper) {
                    selectedItem.removeReattachHelper();
                    $cmsj(selectedItem.canvas).removeClass("Selected");
                    selectedItem.endpoints.forEach(function (endpoint) {
                        endpoint.removeClass("Selected");
                        endpoint.node.ensureNotFront();
                    });
                } else if (selectedItem.hasClass('Node')) {
                    selectedItem.removeClass("Selected");

                    var node = selectedItem.data("nodeObject");
                    var placeholder = selectedItem.find(".nodeDataPlaceholder").first();

                    // If node is unselected and data placeholder is hidden, we should no longer keep the node in front.
                    if (placeholder.length === 0 || placeholder.hasClass("hidden")) {
                        node.ensureNotFront();
                    } else if (graph.isMarketingAutomationWorkflow()) {
                        node.syncZIndexOfNodeEndpoints();
                    }

                    if (e && e.target) {
                        var clickedNode = $cmsj(e.target).closest(".Node");

                        // hide properties panel if click was not on node
                        if (clickedNode.length === 0 && graph.propertiesHandler) {
                            graph.propertiesHandler.hideProperties();
                        }
                    } else if (graph.propertiesHandler) {
                        // Event is typically missing on WCF service failure callbacks, we cannot know what hapened, it is better to hide properties
                        graph.propertiesHandler.hideProperties();
                    }
                }
                graph.selectedItem = null;
            }
        };
    },


    /*
    *   Handles node selection.
    */
    getNodeSelectHandler: function (graph) {
        return function (event) {
            event.stopPropagation();

            if (graphSelectHandler.isClickOnSelectedNode(graph, event) ||
                (graph.propertiesHandler && !graph.propertiesHandler.canSelectNode(event.target))) {
                return;
            }

            if (graph.selectedItem !== event.currentTarget) {
                graphSelectHandler.getDeselectItemHandler(graph)(event);

                var nodeJQ = $cmsj(event.currentTarget);
                nodeJQ.addClass("Selected");
                graph.selectedItem = nodeJQ;

                var node = nodeJQ.data("nodeObject");
                node.ensureFront();

                if (graph.propertiesHandler) {
                    graph.propertiesHandler.showProperties(node);
                }
            }
        };
    },


    /*
    *   Handles connection selection.
    */
    getConnectionSelectHandler: function (graph) {
        var deselectItem = graphSelectHandler.getDeselectItemHandler(graph);

        return function (connection, event) {
            var selectedItem = graph.selectedItem;
            if (selectedItem !== connection) {
                deselectItem(event);
                graph.selectedItem = connection;
                connection.createReattachHelper();
                $cmsj(connection.canvas).addClass("Selected");
                connection.endpoints.forEach(function (endpoint) {
                    endpoint.addClass("Selected");
                    endpoint.node.ensureFront();
                });
            }
            if (event) {
                event.stopPropagation();
            }
        };
    },


    /*
    *   Handles endpoint selection.
    */
    getEndpointSelectHandler: function (graph) {
        var endpointShouldBeSelected = graphSelectHandler.endpointShouldBeSelected;
        var selectConnection = graphSelectHandler.getConnectionSelectHandler(graph);

        return function (event) {
            var endpoint = $cmsj(this).data('jsPlumbObject');
            if (!endpoint) {
                endpoint = $cmsj(this).parent().data('jsPlumbObject');
            }

            if (endpointShouldBeSelected(endpoint)) {
                selectConnection(endpoint.connections[0], event);
            }
        };
    },


    /*
    *   Controls whether or not should connection on endpoint be selected.
    */
    endpointShouldBeSelected: function (endpoint) {
        return endpoint.isSource && !endpoint.isReattachHelper && endpoint.connections[0];
    },


    /*
    *   Highlights node to notify that connection can be attached.
    */
    highlightNode: function (event, ui) {
        var endpoint = ui.draggable.data("jsPlumbObject");
        var node = $cmsj(this).data("nodeObject");
        if (endpoint && node !== endpoint.node && endpoint.oppositeEndpoint && node !== endpoint.oppositeEndpoint.node) { // is potential situation for highlighting
            var idx = endpoint.idx;
            if (idx === 0) {
                var sourcepoint = node.getWildcardSourcePoint();
                if (sourcepoint && !sourcepoint.isFull()) {
                    node.nodeJQ.addClass("Highlight");
                }
            } else {
                node.nodeJQ.addClass("Highlight");
            }
        }
    },


    /*
    *   Highlights endpoint to notify that connection can be attached.
    */
    highlightEndpoint: function (event, ui) {
        var endpoint = ui.draggable.data("jsPlumbObject");
        var target = $cmsj(this).data("jsPlumbObject");
        // Check the endpoint isn't already assigned or doesn't point to itself
        if (endpoint && (!target.isFull() || endpoint.idx === 1) && endpoint.oppositeEndpoint && endpoint.oppositeEndpoint.node !== target.node) {
            var idx = endpoint.idx;
            if ((idx == 1 && (!target.isSource || target.isReattachHelper) && target.node !== endpoint.node) || (idx === 0 && target.isSource)) {
                $cmsj(this).addClass("Highlight");
            }
        }
    },


    /*
    *   Removes highlight from element.
    */
    removeHighlight: function () {
        $cmsj(this).removeClass("Highlight");
    },

    /*
    *   Reacts on mousemove event and checks for atempts to drag connection.
    */
    getConnectionDraggingHandler: function (graph) {
        var selectConnection = graphSelectHandler.getConnectionSelectHandler(graph);
        return function (connector, e) {
            if (!connector.isMouseDown()) {
                return;
            }
            var targetJQ = $cmsj(connector.canvas);
            var hoverConnections = $cmsj("._jsPlumb_hover._jsPlumb_connector").not(targetJQ);
            graphSelectHandler.removeHoverFromConnections(hoverConnections);
            var conn = targetJQ.data("jsPlumbObject");
            var newEvent = $cmsj.extend(true, {}, e);
            if (graph.selectedItem !== conn) {
                selectConnection(conn);
            }
            graphControlHandler.stopEvent(e);
            newEvent.currentTarget = newEvent.target = conn.endpoints[1].canvas;

            var startDragging = graphControlHandler.getStartDraggingConnection(newEvent, connector);
            var condition = graphSelectHandler.getIsEndpointDragHandlerComplete(conn.endpoints[1]);
            graphControlHandler.waitUntil(startDragging, condition, 10);
        };
    },


    /*
    *   Checks if dragged endpoint was already painted.
    */
    getIsEndpointDragHandlerComplete: function (endpoint) {
        return function () {
            return $cmsj(endpoint.canvas).width() > 0 && (endpoint.tagname != "IMG" || endpoint.canvas.src.length > 0);
        };
    },


    /*
    *   Changes all connections back from hover state.
    */
    getRemoveHoverFromAllConnectionsHandler: function () {
        return function (e) {
            if (!e.jsPlumbProcessed && $cmsj(".ui-draggable-dragging").length === 0) {
                var hoverConnections = $cmsj("._jsPlumb_hover._jsPlumb_connector");
                graphSelectHandler.removeHoverFromConnections(hoverConnections);
            }
        };
    },


    /*
    *   Changes connections back from hover state.
    */
    removeHoverFromConnections: function (connectionsJQ) {
        connectionsJQ.each(function () {
            var connection = $cmsj(this).data("jsPlumbObject");
            connection.setHover(false);
        });
    },


    /*
     * Returns true when already selected node was clicked.
     */
    isClickOnSelectedNode: function (graph, event) {
        var nodeJQ = $cmsj(event.currentTarget);
        var itemJQ = $cmsj(graph.selectedItem);

        return itemJQ && nodeJQ && itemJQ.is(nodeJQ[0]);
    }
});


/*
*   Class handling step properties panel
*/
var GraphPropertiesHandler = $class({
    constructor: function (graph, propertiesId) {
        this.isDragging = false;
        this.graph = graph;
        this.propertiesId = propertiesId;

        if (this.graph.isMarketingAutomationWorkflow()) {
            this.propertiesFrame = $cmsj("#" + propertiesId);
            this.propertiesWrapper = $cmsj(".automation-step-properties-wrapper");
            this.propertiesSlidable = this.propertiesWrapper.find(".automation-step-properties-slidable");
            this.loader = $cmsj(".automation-step-properties-loader");

            var self = this;
            this.propertiesFrame.on("load", function () {
                graph.containerJQ.focus();
                self.hideLoader();
                self.propertiesFrame.contents().find('body').on('click', '.automation-step-properties-close-btn', graphSelectHandler.getDeselectItemHandler(graph));
            });
        }
    },

    /*
    *   Shows properties panel for given node.
    */
    showProperties: function (node) {
        if (!this.graph.isMarketingAutomationWorkflow() || this.isDragging === true) {
            return;
        }

        this.showLoader();

        var url = this.graph.addresses["Properties"];

        // properties frame of previous step may be still loading, kill it
        this.propertiesFrame.stop();
        this.propertiesFrame.attr("src", url + "?objectId=" + node.id + "&graph=" + this.graph.jsObjectName);

        if (!this.propertiesWrapper.hasClass('automation-step-properties-wrapper-visible')) {
            this.propertiesWrapper.addClass('automation-step-properties-wrapper-visible');
        }
        if (!this.propertiesSlidable.hasClass('automation-step-properties-slidable-visible')) {
            this.propertiesSlidable.addClass('automation-step-properties-slidable-visible');
        }
    },


    /*
    *   Hides properties panel.
    */
    hideProperties: function () {
        if (!this.graph.isMarketingAutomationWorkflow() || this.isDragging === true
            || !this.propertiesSlidable.hasClass('automation-step-properties-slidable-visible') || !this.propertiesWrapper.hasClass('automation-step-properties-wrapper-visible')) {
            return;
        }

        var self = this;
        self.propertiesSlidable
            .removeClass('automation-step-properties-slidable-visible')
            .one('transitionend',
                function () {
                    self.propertiesWrapper.removeClass('automation-step-properties-wrapper-visible');
                });
    },


    /*
    *   Shows loading placeholder.
    */
    showLoader: function () {
        this.loader.find("#cms-loader").show();
        this.loader.show();
    },


    /*
    *   Hides loading placeholder.
    */
    hideLoader: function () {
        this.loader.find("#cms-loader").hide();
        this.loader.hide();
    },


    /*
    *   Refreshes the opened tab in properties panel.
    */
    refreshProperties: function () {
        if (!this.graph.isMarketingAutomationWorkflow()) {
            return;
        }

        var frameWithTabs = this.propertiesFrame.contents().find('iframe').first();
        if (frameWithTabs) {
            var selectedTab = frameWithTabs.contents().find('.nav-tabs li.active').first();
            if (selectedTab) {
                selectedTab.click();
            }
        }
    },


    /*
    *   Updates the header of properties panel.
    */
    updatePropertiesHeader: function (headerText) {
        if (!this.graph.isMarketingAutomationWorkflow()) {
            return;
        }

        var headerElem = this.propertiesFrame.contents().find(".automation-step-properties-header h1 span").first();
        if (headerElem) {
            headerElem.attr("title", headerText);
            headerElem.text(headerText);
        }
    },


    /*
    *   Handler setting dragging flag.
    */
    startDraggingHandler: function (graph) {
        return function () {
            graph.propertiesHandler.isDragging = true;
        };
    },


    /*
    *   Handler resetting dragging flag.
    */
    stopDraggingHandler: function (graph) {
        return function () {
            graph.propertiesHandler.isDragging = false;
        };
    },


    /*
    *   Determines whether node can be selected.
    */
    canSelectNode: function (element) {
        var elementJQ = $cmsj(element);

        return !this.isDragging &&
            !elementJQ.hasClass("icon-i-circle") && // data placeholder
            !elementJQ.hasClass("icon-doc-copy") && // duplicate step
            !elementJQ.hasClass("icon-bin") && // delete step
            !elementJQ.hasClass("icon-plus") && // add new case
            elementJQ.closest(".Case").length === 0; //  case row including all actions
    }
});


/*
*   Handles controls of graph - mostly nodes manipulations
*/
var GraphControlHandler = $class({

    /*
    *   Returns handler for starting the connection drag.
    */
    getStartDraggingConnection: function (newEvent, connector) {
        return function () {
            if (!connector.isMouseDown()) {
                return;
            }
            var targetJQ = $cmsj(newEvent.target);
            targetJQ.draggable("option", "cursorAt", { top: targetJQ.height() / 2, left: targetJQ.width() / 2 });
            newEvent.type = "mousedown";
            newEvent.jsPlumbProcessed = true;
            connector.releaseMouse();
            targetJQ.trigger(newEvent);
            setTimeout(function (newEvent) {
                return function () {
                    var targetJQ = $cmsj(newEvent.target);
                    targetJQ.draggable("option", "cursorAt", false);
                };
            }(targetJQ), 100);
        };
    },


    /*
    *   Pooling until condition is true, than calls function.
    */
    waitUntil: function (func, getCondition, cycle) {
        if (getCondition()) {
            func();
        } else {
            setTimeout(graphControlHandler.getWaitUntil(func, getCondition, cycle), cycle);
        }
    },


    /*
    *   Closure over waitUntil function - IE workaround.
    */
    getWaitUntil: function (func, getCondition, cycle) {
        return function () {
            graphControlHandler.waitUntil(func, getCondition, cycle);
        };
    },


    /*
    *   Completelly stops event.
    */
    stopEvent: function (e) {
        if (e) {
            e.stopImmediatePropagation();
            e.preventDefault();
            e.bubbles = false;
            e.jsPlumbProcessed = true;
        }
    },


    /*
    *   Handles resize of window.
    */
    getResizeWrapperHandler: function (wrapper) {
        return function (event) {
            var position = wrapper.getViewPosition();

            position.left += wrapper.containerJQ.width();
            position.top += wrapper.containerJQ.height();

            wrapper.setContainerDimensions();
            wrapper.setWrapperDimensions();

            position.left -= wrapper.containerJQ.width();
            position.top -= wrapper.containerJQ.height();

            wrapper.setViewTo(position);
        };
    },


    /*
    *   Handles wheel event.
    */
    getWrapperMouseWheelHandler: function (wrapper) {
        return function (event, deltaY) {
            if ($cmsj(".ui-draggable-dragging").length === 0) {
                wrapper.moveViewBy(0, deltaY * 4);
                return false;
            }
            return;
        };
    },


    /*
    *   Handles click on button to add switch case.
    */
    getAddSwitchCaseHandler: function (graph) {
        var service = graph.serviceHandler;

        return function (event) {
            var nodeId = $cmsj(this).closest(".Node").data('nodeObject').id;
            var node = graph.getNode(nodeId);

            if (node.sourcePointCanBeAdded()) {
                service.addSwitchCase(nodeId);
            } else {
                alert(node.getMaxSourcePointCountError());
            }

            if (graph.isMarketingAutomationWorkflow()) {
                event.stopPropagation();
            }
        };
    },


    /*
    *   Handles click on button for removing switch case.
    */
    getRemoveSwitchCaseHandler: function (graph) {
        var service = graph.serviceHandler;

        return function (event) {
            var caseJQ = $cmsj(this).parent(".Case");
            var node = caseJQ.closest(".Node").data('nodeObject');
            var nodeId = node.id;
            var caseId = caseJQ.data('id');

            if (node.sourcePointCanBeRemoved()) {
                var cont = confirm(node.getCaseDeleteConfirmation());
                if (cont) {
                    service.removeSwitchCase(nodeId, caseId);
                }
            } else {
                alert(node.getMinSourcePointCountError());
            }

            event.stopPropagation();
        };
    },


    /*
    *   Handles keyboard input while graph wrapper has focus.
    */
    getKeyDownHandler: function (graph) {
        return function (event) {
            var keyCode = event.keyCode || event.which;

            if (!graph.myJsPlumb.currentlyDragging) {
                var dragging = $cmsj(".ui-draggable-dragging");
                if (dragging.length) {
                    dragging.remove();
                } else if (keyCode === $cmsj.ui.keyCode.DELETE) {
                    graph.removeSelectedItem();
                } else if (graph.isMarketingAutomationWorkflow()
                    && (event.ctrlKey || event.metaKey) // metaKey == Command key on macOS
                    && !event.altKey
                    && keyCode === 'D'.charCodeAt(0)) {
                    graph.duplicateSelectedNode();
                    event.preventDefault();
                }
            }
        };
    },


    /*
    *   Handler for reattaching / creating connections.
    */
    getAttachConnectionHandler: function (graph) {
        var service = graph.serviceHandler;
        var select = graphSelectHandler.getConnectionSelectHandler(graph);

        return function (value, event) {
            var connection = value.connection;
            var sourceNode = $cmsj('#' + value.sourceId).data('nodeObject');
            var targetNode = $cmsj('#' + value.targetId).data('nodeObject');

            if (connection.ID) {
                if (value.indexEdited === 0) {
                    service.editConnectionStart(connection, sourceNode.id, value.sourceEndpoint.ID);
                    sourceNode.ensureFront();
                } else {
                    service.editConnectionEnd(connection, targetNode.id);
                    targetNode.ensureFront();
                }
            } else {
                service.createConnection(sourceNode.id, targetNode.id, value.sourceEndpoint.ID, connection);
                sourceNode.bindEventsToConnection(connection);
                sourceNode.ensureFront();
                targetNode.ensureFront();
            }
            select(connection, event);
        };
    },


    /*
    *   Handler for setting position of node.
    */
    getSetNodePositionHandler: function (graph) {
        var service = graph.serviceHandler;
        var graphJQ = graph.graphJQ;

        return function (event) {
            var nodeJQ = $cmsj(event.target);
            if (!nodeJQ.hasClass("Node")) {
                nodeJQ = nodeJQ.parents(".Node");
            }
            var node = nodeJQ.data('nodeObject');
            var x = nodeJQ.offset().left - graphJQ.offset().left;
            var y = nodeJQ.offset().top - graphJQ.offset().top;

            if (node.position.X !== x || node.position.Y !== y) {
                service.setNodePosition(node.id, x, y);

                node.position.X = x;
                node.position.Y = y;
            }

            if (graph.isMarketingAutomationWorkflow()) {
                nodeJQ.toggleClass("Dragging", false);
            }

            if (graph.propertiesHandler) {
                var placeholder = nodeJQ.find(".nodeDataPlaceholder").first();

                // If node is not selected and data placeholder is hidden, we should no longer keep the node in front.
                if (!nodeJQ.is(graph.selectedItem) && (placeholder.length === 0 || placeholder.hasClass("hidden"))) {
                    node.ensureNotFront();
                }
            }
        };
    },


    /*
    *   Optimizes dragging of whole graph and its components.
    */
    getStartDraggingHandler: function (graph) {
        var jsPlumbInstance = graph.myJsPlumb;

        return function (e) {
            $cmsj(".UniGraph").addClass("Move");
            var targetJQ = $cmsj(e.target);
            var tooltipsJQ = $cmsj(".tooltip");
            tooltipsJQ.clearQueue();
            tooltipsJQ.hide();

            if (!targetJQ.hasClass("_jsPlumb_endpoint")) {
                jsPlumbInstance.currentlyDragging = true;
            }

            if (targetJQ.hasClass("Node") && graph.snapToGrid) {
                var position = targetJQ.position();
                var width = targetJQ.width() + parseInt(targetJQ.css("border-left-width"));
                var tick = targetJQ.data("uiDraggable").offset.click;

                if (isRTL) {
                    position.left += width;
                }

                tick.top += position.top - graph.roundHeightToGrid(position.top);
                tick.left += position.left - graph.roundHeightToGrid(position.left);

                if (graph.isMarketingAutomationWorkflow()) {
                    targetJQ.toggleClass("Dragging", true);

                    // When a node is dragged, the canvas elements are repainted, their z-indexes changed
                    // and thus the z-indexes of its endpoints must be synced accordingly.
                    graph.syncZIndexOfAllEndpoints();
                }
            }

            if (graph.propertiesHandler) {
                var node = targetJQ.data("nodeObject");
                if (node) {
                    node.ensureFront();
                }
            }
        };
    },


    /*
    *   Optimizes dragging of whole graph and its components.
    */
    getStopDraggingHandler: function (jsPlumbInstance) {
        return function (e, ui) {
            $cmsj(".UniGraph").removeClass("Move");
            if (!ui.helper.hasClass("_jsPlumb_endpoint")) {
                jsPlumbInstance.currentlyDragging = false;
            }
        };
    },


    /*
    *   Handler for adding node.
    */
    getCreateNodeHandler: function (graph) {
        var service = graph.serviceHandler;
        return function (event, ui) {
            if (ui.helper.is(':visible')) {
                var type = ui.helper.attr("step-type");
                var action = ui.helper.attr("step-action");
                var objectOffset = ui.helper.offset();
                var graphOffset = graph.graphJQ.offset();
                var x = objectOffset.left - graphOffset.left;
                var y = objectOffset.top - graphOffset.top;
                x = graph.roundWidthToGrid(x);
                y = graph.roundHeightToGrid(y);
                var hoverConnections = graphControlHandler.getHoverConnections(graph.graphJQ);
                var connectionIds = graphControlHandler.getConnectionsIds(hoverConnections);
                if (connectionIds.length > 0 && type !== "Note") {
                    service.createNodeOnConnections(type, action, graph.ID, x, y, connectionIds);
                } else {
                    service.createNode(type, action, graph.ID, x, y);
                }
            }
        };
    },


    /*
    *   Returns hover connections of given graph.
    */
    getHoverConnections: function (graphJQ) {
        return graphJQ.children("._jsPlumb_hover._jsPlumb_connector");
    },


    /*
    *   Returns ids of given connections.
    */
    getConnectionsIds: function (connectionsJQ) {
        var ids = [];
        for (var i = 0; i < connectionsJQ.length; i++) {
            var connection = $cmsj(connectionsJQ[i]);
            ids.push(connection.data("jsPlumbObject").ID);
        }
        return ids;
    },


    /*
    *   Handler for showing textboxes used for editing labels.
    */
    getShowTextboxInputHandler: function (graph) {
        var hideHandler = graphControlHandler.getHideTextboxInputHandler(graph);
        var labelEditHandler = graphControlHandler.getLabelEditHandler();
        return function (event) {
            var targetJQ = graph.isMarketingAutomationWorkflow() ? $cmsj(event.currentTarget) : $cmsj(event.currentTarget).parent();
            var node = targetJQ.parents(".Node").data("nodeObject");
            var isNote = node.definition.Type === 5;

            if (targetJQ.find(".LabelEdit").length > 0) {
                return;
            }

            targetJQ.find(".Placeholder").empty();

            var height = targetJQ.parent().innerHeight() - 11;
            var width = targetJQ.parent().innerWidth() - 11;
            var position = targetJQ.position();
            var textboxJQ = $cmsj(`<textarea class="LabelEdit" maxlength="450" style="width: ${width}px; height: ${height}px; top: ${position.top}px;">`);
            var text = graphControlHandler.ensureNewLines(targetJQ.find(".Editable").html().trim());
            text = graphControlHandler.decodeHtmlEntities(text);

            textboxJQ.val(text);
            targetJQ.css("color", "transparent");
            targetJQ.prepend(textboxJQ);
            textboxJQ.focus();

            textboxJQ.keydown(function(event) {
                labelEditHandler(event);
            });
            textboxJQ.mousedown(function (event) {
                event.stopPropagation();
            });

            if (graph.isMarketingAutomationWorkflow()) {
                if (isNote) {
                    // Binds to the value change (including key input or mouse paste/cut events)
                    textboxJQ.bind('input propertychange', graphControlHandler.ensureNoteResize);
                    // Initializes the textarea height
                    textboxJQ.trigger("input");
                }

                textboxJQ.click(function (event) {
                    event.stopPropagation();
                });
            } else {
                textboxJQ.dblclick(function (event) {
                    event.stopPropagation();
                });
            }
            textboxJQ.focusout(function(event) {
                hideHandler(event, !isNote || text !== textboxJQ.val());
            });

            textboxJQ.keyup();
            graph.enableTextSelection();
            event.stopPropagation();
        };
    },


    /*
    *   Ensures the textarea and the whole note is resized according to its content.
    */
    ensureNoteResize: function (event) {
        var textareaJQ = $cmsj(event.target);
        var noteJQ = textareaJQ.parents(".Note");
        var noteWithoutTextareaHeight = parseInt(noteJQ.css("height"), 10) - parseInt(textareaJQ.css("height"), 10);

        textareaJQ.css("height", "auto");
        textareaJQ.css("height", (textareaJQ.prop("scrollHeight") + 19) + "px"); // 19px is a row height, adding this additional height prevents the initial "jump"

        if (!event.isTrigger) {
            noteJQ.css("height", "auto");
            noteJQ.css("height", (textareaJQ.prop("scrollHeight") + noteWithoutTextareaHeight) + "px");
        }
    },


    /*
    *   Handles keyboard input while graph wrapper has focus.
    *   For everything but notes: Pressing ENTER saves the changes.
    *   For notes: pressing ENTER adds a new line, pressing the combination CTRL+ENTER saves the changes.
    */
    getLabelEditHandler: function () {
        return function (event) {
            var keyCode = event.keyCode || event.which;
            var isNote = $cmsj(event.target).parents(".Note").length === 1;

            if (keyCode === $cmsj.ui.keyCode.DELETE) {
                event.stopPropagation();
            } else if ((!isNote && keyCode === $cmsj.ui.keyCode.ENTER) ||
                        (isNote && (event.ctrlKey || event.metaKey) && keyCode === $cmsj.ui.keyCode.ENTER)) {
                event.target.blur();
                event.preventDefault();
            }
        };
    },


    /*
    *   Handler for hiding textboxes used for editing labels.
    */
    getHideTextboxInputHandler: function (graph) {
        this.service = graph.serviceHandler;
        return function (event, isContentChanged) {
            event.stopPropagation();
            graph.disableTextSelection();
            var inputJQ = $cmsj(event.target);
            var targetJQ = inputJQ.parent();
            var text = inputJQ.val().trim();
            var node = targetJQ.parents(".Node").data("nodeObject");
            var isNote = node.definition.Type === 5;

            if (text.length > 0 || isNote) {
                // Save the changes in database
                if (isContentChanged) {
                    graphControlHandler.saveLabelChange(targetJQ, text);
                }

                text = graphControlHandler.escapeHtml(text);

                // Display the text in <span>
                text = isNote ? graphControlHandler.ensureLineBreaks(text) : text;
                targetJQ.children(".Editable").html(text);
            }

            // Show placeholder text if the node type is 'Note' and submitted text is empty
            if (text.length === 0 && isNote) {
                targetJQ.find(".Placeholder").html(graph.getResourceString("EmptyNote"));
            }

            // Safari fix: bring focus back to graph container before removing .LabelEdit (to prevent unwanted shift to the left)
            graph.containerJQ.focus();
            targetJQ.children(".LabelEdit").remove();

            targetJQ.parent().children().each(function (id, element) {
                if (element !== targetJQ[0]) {
                    $cmsj(element).show();
                }
            });
            targetJQ.removeAttr("style");

            node.repaintAllEndpoints();

            if (event.type === "keydown" && event.keyCode === $cmsj.ui.keyCode.ENTER) {
                var container = targetJQ.parents("div.GraphContainer");
                if (typeof (container.focus) == 'function') {
                    setTimeout(function () {
                        container.focus();
                    }, 200);
                }
            }
        };
    },


    /*
    *   Saves changes of label.
    */
    saveLabelChange: function (labelJQ, value) {
        if (labelJQ.parents(".Case").length === 1) {
            graphControlHandler.saveCaseLabelChange(labelJQ, value);
        } else if (labelJQ.parents(".Note").length === 1) {
            var escapedValue = graphControlHandler.escapeHtml(value);
            graphControlHandler.saveNoteChange(labelJQ, escapedValue);
        } else if (labelJQ.parents(".Node").length === 1) {
            graphControlHandler.saveNodeLabelChange(labelJQ, value);
        }
    },


    /*
    *   Saves changes of case label.
    */
    saveCaseLabelChange: function (labelJQ, name) {
        var node = labelJQ.closest(".Node").data('nodeObject');
        var caseJQ = labelJQ.parent(".Case");
        var caseId = caseJQ.data('id');
        this.service.setSwitchCaseName(node.id, caseId, name);
    },


    /*
    *   Saves changes of node label.
    */
    saveNodeLabelChange: function (labelJQ, name) {
        var node = labelJQ.parents(".Node").data('nodeObject');
        this.service.setNodeName(node.id, name);
    },


    /*
    *   Saves changes of a note.
    */
    saveNoteChange: function (labelJQ, noteContent) {
        var node = labelJQ.parents(".Node").data('nodeObject');
        this.service.setNoteContent(node.id, noteContent);
    },


    /*
    *   Replaces all occurences of a newline "\n" with an HTML line break "<br />" in the text.
    */
    ensureLineBreaks: function (text) {
        return text.replace(/\r?\n/g, "<br />");
    },


    /*
    *   Replaces all occurences of an HTML line break "<br>"/"<br/>"/"<br />" with a newline "\n" in the text.
    */
    ensureNewLines: function (text) {
        return text.replace(/<br\s*[/]?>/gi, "\n");
    },


    /*
    *   Decodes HTML entities in the text.
    */
    decodeHtmlEntities: function (text) {
        return $cmsj('<div>').html(text).text();
    },


    /*
    *   Encodes all HTML tags in the text.
    */
    escapeHtml: function (text) {
        return $cmsj('<div>').text(text).html();
    },


    /*
    *   Handles click on button for editing switch case.
    */
    editSwitchCaseHandler: function (graph) {
        return function (event) {
            if (graph.isDataRetrievalPlaceholder(event.target)) {
                return;
            }

            var node = $cmsj(event.target).parents(".Node").data('nodeObject');
            var caseId = $cmsj(event.currentTarget).parents(".Case").addBack().filter(".Case").data('id');
            node.openEditModalDialog(caseId);
        };
    },


    /*
    *   Handler for editing node.
    */
    editNodeHandler: function (graph) {
        return function (event) {
            if (graph.isDataRetrievalPlaceholder(event.target)) {
                return;
            }

            var node = $cmsj(event.target).parents(".Node").addBack().filter(".Node").data("nodeObject");
            node.openEditModalDialog();
        };
    },


    /*
    *   Handler for deleting node.
    */
    getRemoveNodeHandler: function (graph) {
        return function (event) {
            event.currentTarget = $cmsj(event.target).parents(".Node");
            graphSelectHandler.getNodeSelectHandler(graph)(event);
            graph.removeSelectedItem();
        };
    },


    /*
    *   Handler for deleting automation node.
    */
    getAutomationRemoveNodeHandler: function (graph) {
        return function (event) {
            var node = $cmsj(event.target).parents(".Node");
            graph.removeNodeFromDatabase(node.data("nodeObject"));

            event.stopPropagation();
        };
    },


    /*
    *   Handler for duplicating node.
    */
    getDuplicateNodeHandler: function (graph) {
        return function (event) {
            var node = $cmsj(event.target).parents(".Node");
            graph.duplicateNodeInDatabase(node.data("nodeObject"));

            event.stopPropagation();
        };
    },


    /*
    *   Handler for disabling tooltip when dragging.
    */
    tooltipStopEventIfDragging: function () {
        $cmsj(".tooltip:visible").hide();
        return $cmsj(".ui-draggable-dragging").length === 0;
    },


    /*
    *   Propagates mouse move event under drag helper for selecting connections to split.
    */
    toolbarDragHandler: function (e, ui) {
        if (window.dragHelperWidth == null) {
            var helper = ui.helper;
            window.dragHelperWidth = helper.width();
            var newEvent = $cmsj.extend(true, {}, e);
            helper.hide();
            var target = document.elementFromPoint(arguments[0].clientX, arguments[0].clientY);
            newEvent.currentTarget = newEvent.target = target;
            newEvent.type = "mousemove";
            $cmsj(target).trigger(newEvent);
            helper.show();
        }
        window.dragHelperWidth = null;
    },


    /*
    *   Initializes toolbar to work with current graph.
    */
    initToolbar: function (toolbarId) {
        $cmsj("#" + toolbarId + " .BigButton").bind("drag", graphControlHandler.toolbarDragHandler);
    }
});


/*
*   This class contains handlers for saving the graph content.
*/
var GraphSavingServiceRequest = $class({

    constructor: function (service, graph) {
        this.service = service;
        this.graph = graph;
        this.graph.graphTimer = null;
        this.graph.graphWaitingCount = 0;
        this.bindServiceEvents();
    },

    bindServiceEvents: function () {
        $cmsj(window).bind('beforeunload', function (e) { window.isUnloading = true; });
    },

    showProgress: function () {
        if (window.Loader) {
            window.Loader.show();
        }
    },

    createNodeOnConnections: function (type, actionId, parentId, x, y, connectionIds) {
        this.showProgress();
        this.service.CreateNodeOnConnections(type, actionId, parentId, x, y, connectionIds, graphServiceResponseHandler.createNodeOnConnectionsHandler(this.graph), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    createNode: function (type, actionId, parentId, x, y) {
        this.showProgress();
        this.service.CreateNode(type, actionId, parentId, x, y, graphServiceResponseHandler.createNodeHandler(this.graph), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    setNodePosition: function (id, x, y) {
        this.showProgress();
        var removeObject = graphServiceResponseHandler.removeNodeHandler(this.graph, id);
        this.service.SetNodePosition(id, parseInt(x), parseInt(y), graphServiceResponseHandler.savingCheckHandler(this.graph, removeObject), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    addSwitchCase: function (id) {
        this.showProgress();
        this.service.AddSwitchCase(id, graphServiceResponseHandler.repaintNodeHandler(this.graph, id, true, false, true), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    removeSwitchCase: function (nodeId, caseId) {
        var removeObject = graphServiceResponseHandler.removeSwitchCaseHandler(this.graph, nodeId, caseId);
        var repaintObject = graphServiceResponseHandler.repaintNode(this.graph);
        this.showProgress();

        this.service.RemoveSwitchCase(nodeId, caseId, graphServiceResponseHandler.removeHandler(this.graph, removeObject, repaintObject), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    editConnectionStart: function (connection, sourceNodeId, sourcePointId, value) {
        var removeObject = graphServiceResponseHandler.removeConnectionHandler(this.graph, connection);
        this.showProgress();
        this.service.EditConnectionStart(connection.ID, sourceNodeId, sourcePointId, graphServiceResponseHandler.savingCheckHandler(this.graph, removeObject), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    editConnectionEnd: function (connection, targetNodeId) {
        var removeObject = graphServiceResponseHandler.removeConnectionHandler(this.graph, connection);
        this.showProgress();
        this.service.EditConnectionEnd(connection.ID, targetNodeId, graphServiceResponseHandler.savingCheckHandler(this.graph, removeObject), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    createConnection: function (sourceNodeId, targetNodeId, sourcePointId, connection) {
        this.showProgress();
        this.service.CreateConnection(sourceNodeId, targetNodeId, sourcePointId, graphServiceResponseHandler.setConnectionIdHandler(this.graph, connection), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    removeNode: function (id) {
        var onSuccess = function (graph) {
            var removeObjectFunc = function() {
                var deletedNode = graph.getNode(id);

                if (graph.propertiesHandler && deletedNode.nodeJQ.is(graph.selectedItem)) {
                    graph.selectedItem = null;
                    graph.propertiesHandler.hideProperties();
                }

                graph.removeNode(id);
            };

            return function (response) {
                graphServiceResponseHandler.removeHandler(graph, removeObjectFunc)(response);
            };
        };

        this.showProgress();
        this.service.RemoveNode(id, onSuccess(this.graph), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    duplicateNode: function (id, x, y) {
        this.showProgress();
        this.service.DuplicateNode(id, x, y, graphServiceResponseHandler.createNodeHandler(this.graph), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    removeConnection: function (connection) {
        var removeObject = graphServiceResponseHandler.removeConnectionHandler(this.graph, connection);
        this.showProgress();
        this.service.RemoveConnection(connection.ID, graphServiceResponseHandler.removeHandler(this.graph, removeObject), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    setNodeName: function (nodeId, name) {
        var removeObject = graphServiceResponseHandler.removeNodeHandler(this.graph, nodeId);
        this.showProgress();
        this.service.SetNodeName(nodeId, name, graphServiceResponseHandler.savingCheckHandler(this.graph, removeObject), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    setSwitchCaseName: function (nodeId, caseId, name) {
        var removeObject = graphServiceResponseHandler.removeSwitchCaseHandler(this.graph, nodeId, caseId);
        this.showProgress();

        var onSuccess = function (graph, removeObjectFunc) {
            return function (response) {
                graphServiceResponseHandler.savingCheckHandler(graph, removeObjectFunc)(response);
                if (graph.isNodeSelected(nodeId)) {
                    graph.refreshProperties();
                }
            };
        };

        this.service.SetSwitchCaseName(nodeId, caseId, name, onSuccess(this.graph, removeObject), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    setNoteContent: function (nodeId, noteContent) {
        this.showProgress();
        this.service.SetNoteContent(nodeId, noteContent, graphServiceResponseHandler.savingCheckHandler(this.graph), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    refreshNode: function (id, refreshSidePanel) {
        this.showProgress();
        this.service.GetNode(id, graphServiceResponseHandler.repaintNodeHandler(this.graph, id, refreshSidePanel, true, false), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    }
});


/*
*   Class handling loading and populating information for steps.
*/
var GraphDataRetrievalHandler = $class({
    constructor: function (service) {
        this.service = service;
        this.bindServiceEvents();
    },

    bindServiceEvents: function () {
        $cmsj(window).bind('beforeunload', function () { window.isUnloading = true; });
    },

    /*
     * Toggles visibility of data placeholder.
     */
    toggleData: function (graph) {
        return function (event) {
            if (graph.isMarketingAutomationWorkflow()) {
                var id = this.getAttribute("data-id");
                var nodeDataPlaceholder = $cmsj('#nodeData_' + id);
                var node = graph.nodes[id];

                if (nodeDataPlaceholder.hasClass("hidden")) {
                    if (!graph.nodeDataRetrieved[id]) {
                        graph.dataHandler.loadData(id);
                        graph.nodeDataRetrieved[id] = true;
                    }
                    nodeDataPlaceholder.removeClass("hidden");
                    node.ensureFront();
                } else {
                    nodeDataPlaceholder.addClass("hidden");

                    // If node is not selected and data placeholder is hidden, we should no longer keep the node in front.
                    if (!node.nodeJQ.is(graph.selectedItem)) {
                        node.ensureNotFront();
                    }
                }

                event.stopPropagation();
            }
        };
    },

    /*
     * Refreshes data placeholder.
     */
    refreshData: function (graph) {
        return function (event) {
            event.stopPropagation();
            if (graph.isMarketingAutomationWorkflow()) {
                // blink animation on refresh - useful when content does not change
                $cmsj(event.target).closest('.nodeDataPlaceholder').find('.dataPlaceholderContent').css({ opacity: 0 });

                var id = this.getAttribute("data-id");
                graph.dataHandler.loadData(id);
            }
        };
    },

    /*
     * Loads data into placeholder.
     */
    loadData: function (id) {
        this.service.GetData(id, this.loadDataHandler(id), graphServiceResponseHandler.serviceFailure(this.resourceStrings["CriticalError"]), null);
    },

    /*
     * Callback to display data inside the placeholder.
     */
    loadDataHandler: function (id) {
        return function (response) {
            $cmsj('#nodeData_' + id + ' .dataPlaceholderContent').html(response.Data).animate({ opacity: 1 }, 100);
        };
    }
});

/*
*   Class handling general errors.
*/
var GraphSavingServiceResponse = $class({

    setConnectionIdHandler: function (graph, connection) {
        var removeObject = graphServiceResponseHandler.removeConnectionHandler(graph, connection);
        return function (response) {
            if (graphServiceResponseHandler.savingCheck(graph, response, removeObject)) {
                connection.ID = response.Data;
                $cmsj(connection.canvas).addClass("conn_" + connection.ID);
            }
        };
    },

    createNodeOnConnectionsHandler: function (graph) {
        return function (response) {
            if (graphServiceResponseHandler.savingCheck(graph, response)) {
                graph.createNodes(response.Nodes);
                graph.createConnections(response.Connections);
            }
        };
    },

    createNodeHandler: function (graph) {
        return function (response) {
            if (graphServiceResponseHandler.savingCheck(graph, response)) {
                graph.createNode(response.Data);
            }
        };
    },

    repaintNodeHandler: function (graph, nodeId, refreshSidePanel, refreshDataPlaceholder, showAutosaveMessage) {
        var removeObject = graphServiceResponseHandler.removeNodeHandler(graph, nodeId);
        return function (response) {
            if (graphServiceResponseHandler.savingCheck(graph, response, removeObject, null, showAutosaveMessage) && response.Data) {
                graph.repaintNode(response.Data);
                graph.updatePropertiesHeader(nodeId);

                // Refresh the node data placeholder if it was opened
                if (refreshDataPlaceholder === true && graph.nodeDataRetrieved[nodeId] === true) {
                    graph.refreshNodeDataPlaceholder(nodeId);
                }
            }
            if (refreshSidePanel === true) {
                if (graph.isNodeSelected(nodeId)) {
                    graph.refreshProperties();
                }
            }
        };
    },

    removeHandler: function (graph, removeObject, repaintObject) {
        return function (response) {
            if (graphServiceResponseHandler.savingCheck(graph, response, removeObject, repaintObject)) {
                removeObject();
            }
        };
    },

    savingCheckHandler: function (graph, removeObject) {
        return function (response) {
            graphServiceResponseHandler.savingCheck(graph, response, removeObject);
        };
    },

    hideProgress: function () {
        if (window.Loader) {
            window.Loader.hide();
        }
    },

    savingCheck: function (graph, response, removeObject, repaintObject, showAutosaveMessage = true) {
        // Handle ScreenLock
        if (window.top.HideScreenLockWarningAndSync) {
            window.top.HideScreenLockWarningAndSync(response.ScreenLockInterval);
        }

        // Hide progress indicator
        graphServiceResponseHandler.hideProgress(graph);

        if (response.StatusCode === 200) {
            if (graph.isMarketingAutomationWorkflow() && showAutosaveMessage) {
                graph.showAutosave();
            }
            return true;
        } else if (response.StatusCode === 404 && typeof (removeObject) == "function") {
            removeObject();
        } else if (response.StatusCode === 400 && typeof (repaintObject) == "function" && response.Data) {
            repaintObject(response.Data);
        }
        alert(response.StatusMessage);
        return false;
    },

    serviceFailure: function (msg) {
        return function (response) {
            if (window.isUnloading) {
                return;
            } else if (response._statusCode === 401) {
                location.reload();
            } else {
                alert(msg);
            }
        };
    },

    removeConnectionHandler: function (graph, connection) {
        return function (event) {
            graphSelectHandler.getDeselectItemHandler(graph)(event);
            connection.removeWithAllProperties();
        };
    },

    removeNodeHandler: function (graph, nodeId) {
        return function (event) {
            graphSelectHandler.getDeselectItemHandler(graph)(event);
            graph.removeNode(nodeId);
        };
    },

    removeSwitchCaseHandler: function (graph, nodeId, caseId) {
        return function () {
            var node = graph.getNode(nodeId);
            node.removeCaseRow(caseId);
            node.repaintAllEndpoints();
            if (graph.isNodeSelected(nodeId)) {
                graph.refreshProperties();
            }
        };
    },

    repaintNode: function (graph) {
        return function (data) {
            graph.repaintNode(data);
        };
    }
});

graphServiceResponseHandler = new GraphSavingServiceResponse();
graphControlHandler = new GraphControlHandler();
graphSelectHandler = new GraphSelectionHandler();
