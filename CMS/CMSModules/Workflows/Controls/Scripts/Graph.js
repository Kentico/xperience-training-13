/*
*   Helper function to define classes.
*/
var $class = function (def) {
    //handles situations, where no constructor id in class
    var constructor = function () { };
    if (def.hasOwnProperty('constructor')) {
        constructor = def.constructor;
    }

    //splits class initialization to parts
    for (var name in $class.Initializers) {
        $class.Initializers[name].call(constructor, def[name], def);
    }

    return constructor;
};


/*
*   Helper class initializing classes.
*/
$class.Initializers = {
    //handles inheritances
    Extends: function (parent) {
        if (parent) {
            var F = function () { };
            this._superClass = F.prototype = parent.prototype;
            this.prototype = new F;
        }
    },
    //handles mix ins
    Mixins: function (mixins, def) {
        this.mixin = function (mixin) {
            for (var key in mixin) {
                if (key in $class.Initializers) continue;
                this.prototype[key] = mixin[key];
            }
            this.prototype.constructor = this;
        };
        var objects = [def].concat(mixins || []);
        for (var i = 0, l = objects.length; i < l; i++) {
            this.mixin(objects[i]);
        }
    }
};

/*
*   Class handling whole graph.
*/
var JsPlumbGraph = $class({

    constructor: function (readOnly, service, parentId, jsObjectName, workflowType, propertiesId, autosaveId) {
        this.jsObjectName = jsObjectName;
        this.workflowType = workflowType;
        var parent = $cmsj("#" + parentId)[0];

        this.readOnly = readOnly;

        this.myJsPlumb = jsPlumb.getInstance({ ParentElement: parent });
        this.myJsPlumb.setRenderMode(readOnly ? jsPlumb.SVG : jsPlumb.CANVAS);

        if (service) {
            this.serviceHandler = new GraphSavingServiceRequest(service, this);
            this.dataHandler = new GraphDataRetrievalHandler(service);
        }

        // Default settings
        if (this.isMarketingAutomationWorkflow()) {
            this.myJsPlumb.Defaults.Connector = ["Bezier", { curviness: 80 }];
            this.myJsPlumb.Defaults.EndpointStyle = { fillStyle: "#E0E0E0" };
            this.myJsPlumb.Defaults.HoverPaintStyle = { lineWidth: 3, strokeStyle: '#696663' };
            this.myJsPlumb.Defaults.SelectedConnectionPaintStyle = { lineWidth: 3, strokeStyle: '#696663' };
        } else {
            this.myJsPlumb.Defaults.Connector = ["Flowchart", { stub: 15 }];
            this.myJsPlumb.Defaults.EndpointStyle = { fillStyle: "#1172ae" };
            this.myJsPlumb.Defaults.HoverPaintStyle = { lineWidth: 4, strokeStyle: '#262524' };
            this.myJsPlumb.Defaults.SelectedConnectionPaintStyle = { lineWidth: 4, strokeStyle: '#262524' };
        }

        this.myJsPlumb.Defaults.PaintStyle = { lineWidth: 2, strokeStyle: '#bdbbbb' };
        this.myJsPlumb.Defaults.Anchor = "Center";
        this.myJsPlumb.Defaults.Overlays = [["PlainArrow", { location: 0.7, width: 14, length: 8 }]];
        this.myJsPlumb.Defaults.AppendElementsToBody = false;
        this.myJsPlumb.Defaults.DropOptions = { tolerance: 'touch' };

        this.nodes = new Object();
        this.nodeDataRetrieved = {};

        this.sourcePointTranslation = ["standard", "switchCase", "switchDefault", "timeout"];

        this.setHtmlElement(parent);

        if (this.isMarketingAutomationWorkflow()) {
            this.propertiesHandler = new GraphPropertiesHandler(this, propertiesId);
            this.autosaveId = autosaveId;
        }

        this.snapToGrid = $cmsj.cookie('CMSUniGraph') !== "false";
        this.gridWidth = 10;
        this.gridHeight = 10;
        this.gridSize = [this.gridWidth, this.gridHeight];


        this.nodePadding = 50;

        $cmsj(window).bind('unload', function (graph) {
            return function () {
                graph.myJsPlumb.unload();
                graph.myJsPlumb.unload();
                graph.myJsPlumb.unload();
                graph.myJsPlumb.unload();
                graph.myJsPlumb.unload();
                jsPlumb.unload();
                jsPlumb.unload();
                jsPlumb.unload();
                jsPlumb.unload();
                jsPlumb.unload();
            }
        }(this));

        $cmsj.tools.tooltip.addEffect(
            "graphDelay",
            function (done, e) {
                this.getTip().delay(200).fadeIn(200);
                done();
            },
            function (done, e) {
                this.getTip().clearQueue();
                this.getTip().hide();
            }
        );

        $cmsj.tools.tooltip.addEffect(
            "condition",
            function (done, e) {
                if (isRTL) {
                    this.getConf().offset[1] = -this.getTip().width();
                } else {
                    this.getConf().offset[1] = this.getTip().width();
                }
                this.getTip().delay(200).fadeIn(200);
                done();
            },
            function (done, e) {
                this.getTip().clearQueue();
                this.getTip().hide();
            }
        );

        this.tooltipConfiguration = {
            onBeforeShow: graphControlHandler.tooltipStopEventIfDragging,
            effect: "graphDelay",
            position: "top right",
            tipClass: "tooltip in"
        };
    },


    /*
    *   Toggles snapping to grid.
    */
    toggleSnapToGrid: function () {
        this.snapToGrid = !this.snapToGrid;
        for (var key in this.nodes) {
            this.nodes[key].setSnapToGridByGraph();
        }
        $cmsj.cookie('CMSUniGraph', this.snapToGrid, { expires: 365, path: '/' });
    },


    roundWidthToGrid: function (x) {
        x = this.snapToGrid ? parseInt(x / this.gridWidth) * this.gridWidth : parseInt(x);
        return x <= 0 ? 1 : x;
    },


    roundHeightToGrid: function (y) {
        y = this.snapToGrid ? parseInt(y / this.gridHeight) * this.gridHeight : parseInt(y);
        return y <= 0 ? 1 : y;
    },


    /*
    *   Removes selected item.
    */
    removeSelectedItem: function () {
        var deselectItem = graphSelectHandler.getDeselectItemHandler(this);

        if (this.selectedItem == null) {
            alert(this.serviceHandler.resourceStrings["NoItemSelected"]);
            return;
        }

        if (this.selectedItem.removeReattachHelper) {
            this.removeConnectionFromDatabase(this.selectedItem);
        } else if (this.selectedItem.hasClass('Node')) {
            this.removeNodeFromDatabase(this.selectedItem.data("nodeObject"));
        }
        deselectItem();
    },


    /*
    *   Duplicates selected node.
    */
    duplicateSelectedNode: function () {
        if (this.selectedItem == null || !this.selectedItem[0]) {
            alert(this.serviceHandler.resourceStrings["NoStepSelected"]);
            return;
        }

        if (this.selectedItem.hasClass('Node')) {
            this.duplicateNodeInDatabase(this.selectedItem.data("nodeObject"));
        }
    },


    /*
    *   Removes connection from graph and database.
    */
    removeConnectionFromDatabase: function (connection) {
        var cont = confirm(this.serviceHandler.resourceStrings["ConnectionDeleteConfirmation"]);
        if (cont) {
            this.serviceHandler.removeConnection(connection);
        }
    },


    /*
    *   Remove nodes from graph and database.
    */
    removeNodeFromDatabase: function (node) {
        if (!node.definition.IsDeletable) {
            alert(this.serviceHandler.resourceStrings["NondeletableNode"]);
            return;
        }

        var confirmationResourceString = node.definition.Type === 5 ? "NoteDeleteConfirmation" : "NodeDeleteConfirmation";
        var cont = confirm(this.serviceHandler.resourceStrings[confirmationResourceString]);
        if (cont) {
            this.serviceHandler.removeNode(node.id);
        }
    },


    /*
    *   Duplicates node in graph and database.
    */
    duplicateNodeInDatabase: function (node) {
        if (!node.definition.IsDeletable) {
            alert(this.serviceHandler.resourceStrings["NonduplicatableNode"]);
            return;
        }

        var offset = 50;

        var x = node.nodeJQ[0].offsetLeft + offset;
        var y = node.nodeJQ[0].offsetTop + offset;

        while (this.hasCollision(x, y)) {
            x += offset;
            y += offset;
        }

        this.serviceHandler.duplicateNode(node.id, x, y);
    },


    /*
    *   Returns true if there is a node with the specified position.
    */
    hasCollision: function (x, y) {
        for (var key in this.nodes) {
            var currentNode = this.nodes[key];
            if (currentNode.nodeJQ[0].offsetLeft === x &&
                currentNode.nodeJQ[0].offsetTop === y) {
                return true;
            }
        }
        return false;
    },


    /*
    *   Ensures correct resource strings in read only mode.
    */
    getReadOnlyResourceString: function (resource, defaultResource) {
        if (this.readOnly) {
            return '';
        }
        var str = this.getResourceString(resource);
        if (str) {
            return str;
        }
        return this.getResourceString(defaultResource);
    },


    /*
    *   Returns default resource string.
    */
    getResourceString: function (resource) {
        return this.resourceStrings[resource];
    },


    /*
    *   Refreshes node and side panel if needed.
    */
    refreshNode: function (id, refreshSidePanel) {
        this.serviceHandler.refreshNode(id, refreshSidePanel);
        this.graphJQ.parents(".GraphContainer").focus();
    },


    /*
    *   Method for setting limits on source points counts.
    */
    setSourcePointsLimits: function (limits) {
        this.sourcePointsLimits = limits;
    },


    /*
    *   Sets ID of HTML parental element.
    */
    setHtmlElement: function (parent) {
        this.graphJQ = $cmsj(parent);
        this.graphJQ.data('graphObject', this);
        this.containerJQ = this.graphJQ.parents(".GraphContainer");

        this.myJsPlumb.Defaults.ParentElement = parent;
        this.myJsPlumb.Defaults.DragOptions = { containment: this.graphJQ.selector };
        this.graphJQ.parents("body").on("mouseover", ".tooltip", graphControlHandler.tooltipStopEventIfDragging);

        if (!this.readOnly) {
            this.setSelectHandlers();
            this.setControlHandlers();
            this.setPropertiesHandlers();
            this.setDataHandlers();
        }
    },


    /*
    *   Sets handlers for selecting items.
    */
    setSelectHandlers: function () {
        var actionableNodesSelector = ".Node:not(.Note)";
        var connectionsSelector = "._jsPlumb_endpoint";

        if (!this.isMarketingAutomationWorkflow()) {
            this.graphJQ.on("dragstart", actionableNodesSelector, graphSelectHandler.getNodeSelectHandler(this));
        }

        this.graphJQ.on("click", actionableNodesSelector, graphSelectHandler.getNodeSelectHandler(this));
        this.graphJQ.on("click", connectionsSelector, graphSelectHandler.getEndpointSelectHandler(this));
        this.graphJQ.on("dragstart", connectionsSelector, graphSelectHandler.getEndpointSelectHandler(this));
        this.graphJQ.on("dropover", actionableNodesSelector, graphSelectHandler.highlightNode);
        this.graphJQ.on("dropout", actionableNodesSelector, graphSelectHandler.removeHighlight);
        this.graphJQ.on("drop", actionableNodesSelector, graphSelectHandler.removeHighlight);
        this.graphJQ.on("dropover", connectionsSelector, graphSelectHandler.highlightEndpoint);
        this.graphJQ.on("dropout", connectionsSelector, graphSelectHandler.removeHighlight);
        this.graphJQ.on("drop", connectionsSelector, graphSelectHandler.removeHighlight);
        this.myJsPlumb.bind("click", graphSelectHandler.getConnectionSelectHandler(this));
        this.disableTextSelection();
    },


    /*
    *   Disables selection of all text in graph.
    */
    disableTextSelection: function () {
        this.graphJQ[0].onselectstart = function () { return false; };
    },


    /*
    *   Enables selection of all text in graph.
    */
    enableTextSelection: function () {
        this.graphJQ[0].onselectstart = function () { };
    },


    /*
    *   Sets handlers for editing graph.
    */
    setControlHandlers: function () {
        this.graphJQ.on("click", "i.icon-plus", graphControlHandler.getAddSwitchCaseHandler(this));
        this.graphJQ.on("dblclick", "i.icon-plus, div.Node, div.Case", function (e) { e.stopPropagation(); });
        this.graphJQ.on("click", ".Node.Note", function (e) { e.stopPropagation(); });

        this.graphJQ.on("click", ".Cases i.icon-edit", graphControlHandler.editSwitchCaseHandler(this));
        this.graphJQ.on("click", ".Cases i.icon-bin", graphControlHandler.getRemoveSwitchCaseHandler(this));
        this.graphJQ.on("dragstart", "div.Node", graphControlHandler.getStartDraggingHandler(this));
        this.graphJQ.on("dragstop", "div.Node", graphControlHandler.getSetNodePositionHandler(this));
        this.graphJQ.on("dragstop", "div.Node", graphControlHandler.getStopDraggingHandler(this.myJsPlumb));
        this.myJsPlumb.bind("jsPlumbConnection", graphControlHandler.getAttachConnectionHandler(this));

        if (this.isMarketingAutomationWorkflow()) {
            this.graphJQ.on("click", ".content-header i.icon-bin", graphControlHandler.getAutomationRemoveNodeHandler(this));
            this.graphJQ.on("click", ".content-header i.icon-doc-copy", graphControlHandler.getDuplicateNodeHandler(this));
            this.graphJQ.on("click", ".inner", graphControlHandler.getShowTextboxInputHandler(this));
        } else {
            this.graphJQ.on("click", ".Name i.icon-bin", graphControlHandler.getRemoveNodeHandler(this));
            this.graphJQ.on("click", ".Name i.icon-edit", graphControlHandler.editNodeHandler(this));
            this.graphJQ.on("dblclick", "div.Node", graphControlHandler.editNodeHandler(this));
            this.graphJQ.on("dblclick", ".Editable", graphControlHandler.getShowTextboxInputHandler(this));
            this.graphJQ.on("dblclick", ".Case", graphControlHandler.editSwitchCaseHandler(this));
        }
    },


    /*
    *   Sets handlers for properties panel.
    */
    setPropertiesHandlers: function () {
        if (this.isMarketingAutomationWorkflow() && this.propertiesHandler) {
            this.graphJQ.on("mousedown", "div.Node", this.propertiesHandler.startDraggingHandler(this));
            this.graphJQ.on("mouseup", "div.Node", this.propertiesHandler.stopDraggingHandler(this));
        }
    },


    /*
    *   Sets handlers for data loading.
    */
    setDataHandlers: function () {
        if (this.isMarketingAutomationWorkflow() && this.dataHandler) {
            this.graphJQ.on("click", ".nodeDataIconPlaceholder", this.dataHandler.toggleData(this));
            this.graphJQ.on("click", ".nodeDataPlaceholder .refresh-button", this.dataHandler.refreshData(this));
        }
    },


    /*
    *   Repaints node by given definition.
    */
    repaintNode: function (definition) {
        definition = this.definitionTranslation(definition);
        this.nodes[definition.ID].repaint(definition);
    },


    /*
    *   Prints whole graph.
    */
    printGraph: function (definitions) {
        this.myJsPlumb.jsPlumbFillCanvases = false;
        if (definitions.ID !== undefined) {
            this.ID = definitions.ID;
        }

        if (!this.readOnly) {
            this.serviceHandler.resourceStrings = definitions.ServiceResourceStrings;
            this.dataHandler.resourceStrings = definitions.ServiceResourceStrings;
        }

        this.resourceStrings = definitions.GraphResourceStrings;
        this.addresses = definitions.Addresses;
        this.createNodes(definitions.Nodes);
        this.myJsPlumb.jsPlumbFillCanvases = true;
        for (var key in this.nodes) {
            this.nodes[key].repaintAllEndpoints();
        }

        //endpoints needs to be painted before creating connections
        this.createConnections(definitions.Connections);
    },


    /*
    *   Determines whether the workflow is used in the marketing automation.
    */
    isMarketingAutomationWorkflow: function () {
        return this.workflowType === 3;
    },


    /*
    *   Returns true when given click target is inside the data retrieval placeholders.
    */
    isDataRetrievalPlaceholder: function (clickTarget) {
        var targetObj = $cmsj(clickTarget);
        return targetObj.parents("div.nodeDataPlaceholder").length || targetObj.hasClass("nodeDataPlaceholder") ||
            targetObj.parents("div.nodeDataIconPlaceholder").length || targetObj.hasClass("nodeDataIconPlaceholder");
    },


    /*
    *   Returns node by given id.
    */
    getNode: function (id) {
        return this.nodes[id];
    },


    /*
    *   Adds nodes.
    */
    createNodes: function (definitions) {
        for (var i = 0; i < definitions.length; i++) {
            var nodeJQ = $cmsj("#" + this.graphJQ.attr('id') + "_" + definitions[i].ID);
            if (nodeJQ.length > 0) {
                nodeJQ.data("nodeObject").repaint(definitions[i]);
            } else {
                this.createNode(definitions[i]);
            }
        }
    },


    /*
    *   Adds single node.
    */
    createNode: function (definition) {
        definition = this.definitionTranslation(definition);

        var newNode = this.createNodeObject(definition);
        if (this.sourcePointsLimits) {
            newNode.setSourcePointsLimits(this.sourcePointsLimits.Min[definition.Type], this.sourcePointsLimits.Max[definition.Type]);
        }
        this.initNode(newNode);
        newNode.printNode();
        this.nodes[definition.ID] = newNode;

        if (this.isMarketingAutomationWorkflow()) {
            this.initNodeZIndex(newNode);
            newNode.syncZIndexOfNodeEndpoints();
        }

        return newNode;
    },


    /*
    *   Method rewriting int representation of nodes to more user friendly string.
    */
    definitionTranslation: function (definition) {
        if (definition.SourcePoints) {
            for (var i = 0; i < definition.SourcePoints.length; i++) {
                definition.SourcePoints[i].Type = this.sourcePointTranslation[definition.SourcePoints[i].Type];
            }
        }

        if (definition.Position.X < 0 && definition.Position.Y < 0) {
            // (Advanced workflow) position the graph to the left center of the canvas
            var initialXoffset = 0;
            var initialYoffset = this.graphJQ.height() / 2 + 210;

            // (Marketing automation) position the graph to the center top of the canvas
            if (this.isMarketingAutomationWorkflow()) {
                initialXoffset = 2000;
                initialYoffset = this.getNodesCount() * 180 + 40;
            }

            definition.Position.X = (this.getNodesCount() + 1) * 301 - this.graphJQ.offset().left + initialXoffset;
            definition.Position.Y = -this.graphJQ.offset().top + initialYoffset;

            if (definition.Position.X > this.graphJQ.width() - 200) {
                definition.Position.X = this.graphJQ.width() - 200;
            }

            definition.Position.X = this.roundWidthToGrid(definition.Position.X);
            definition.Position.Y = this.roundHeightToGrid(definition.Position.Y);
            if (this.serviceHandler) {
                this.serviceHandler.setNodePosition(definition.ID, definition.Position.X, definition.Position.Y);
            }
        }
        return definition;
    },


    /*
    *   Returns number of nodes.
    */
    getNodesCount: function () {
        var size = 0, key;
        for (key in this.nodes) {
            if (this.nodes[key]) size++;
        }
        return size;
    },


    /*
    *   Create new instance of node.
    */
    createNodeObject: function (definition) {
        switch (definition.Type) {
            case 0: return new JsPlumbStandardNode(this.readOnly, this, definition);
            case 1: return new JsPlumbActionNode(this.readOnly, this, definition);
            case 2: return new JsPlumbConditionNode(this.readOnly, this, definition);
            case 3: return new JsPlumbMultichoiceNode(this.readOnly, this, definition);
            case 4: return new JsPlumbUserChoiceNode(this.readOnly, this, definition);
            case 5: return new JsPlumbNoteNode(this.readOnly, this, definition);
        }
        return;
    },


    /*
    *   Initializes mandatory parameters of node.
    */
    initNode: function (node) {
        node.setJsPlumbInstance(this.myJsPlumb);
    },


    /*
    *   Initializes z-index of the node.
    */
    initNodeZIndex: function (node) {
        if ($cmsj.isEmptyObject(this.nodes)) {
            return;
        }

        // Gets the largest z-index from all nodes currently printed
        var max = Math.max.apply(null, $cmsj.map(this.nodes, function(node) {
                // Z-index of the first node is 21
                return $cmsj(node.nodeJQ)[0].style.zIndex || 20;
            })
        );

        $cmsj(node.nodeJQ).css("z-index", max + 1);
    },


    /*
    *   Creates multiple connections.
    */
    createConnections: function (definitions) {
        if (definitions) {
            for (var i = 0; i < definitions.length; i++) {
                this.createConnection(definitions[i]);
            }
        }
    },


    /*
    *   Creates single connection.
    *   For purposes of creating new node, repacking of the definitions is needed.
    */
    createConnection: function (definition) {
        var targetNode = this.nodes[definition.TargetNodeID];
        var sourceNode = this.nodes[definition.SourceNodeID];
        var connJQ = this.graphJQ.children(".conn_" + definition.ID);
        if (connJQ.length > 0) {
            graphSelectHandler.getDeselectItemHandler(this)();
            connJQ.data("jsPlumbObject").removeWithAllProperties();
        }
        if (sourceNode && targetNode && targetNode.definition.HasTargetPoint) {
            sourceNode.createConnection({ sourcePointId: definition.SourcePointID, targetNode: targetNode, ID: definition.ID });
        }
    },


    /*
    *   Removes given node.
    */
    removeNode: function (id) {
        var node = this.nodes[id];
        this.myJsPlumb.deleteEndpointsOnElement(node.htmlId);
        node.nodeJQ.remove();
        delete this.nodeDataRetrieved[id];
        delete node.id;
        $cmsj(".tooltip:visible").hide();
    },


    /*
    *   Calculates ideal initial view point of graph
    */
    calculateInitialView: function () {
        var position = this.getNearestNodePosition();
        var maxW = position.X + this.containerJQ.width() - this.nodePadding * 2;
        var minH = position.Y - this.containerJQ.height() + this.nodePadding * 2;
        for (var i in this.nodes) {
            if (position.Y > this.nodes[i].position.Y && this.nodes[i].position.X < maxW && this.nodes[i].position.Y > minH) {
                position.Y = this.nodes[i].position.Y;
            }
        }
        return position;
    },


    /*
    *   Returns coordinates of nearest node.
    */
    getNearestNodePosition: function () {
        var position = { X: this.graphJQ.width(), Y: this.graphJQ.height() };
        for (var i in this.nodes) {
            if (position.X > this.nodes[i].position.X) {
                position = this.nodes[i].position;
            }
        }
        return position;
    },


    /*
    *   Returns point on center of ideal initial view.
    */
    getCenterOfInitialView: function () {
        var position = this.calculateInitialView();
        if (position) {
            return {
                left: position.X - this.nodePadding + this.containerJQ.width() / 2,
                top: position.Y - this.nodePadding + this.containerJQ.height() / 2
            };
        }

        return { top: this.graphJQ.height() / 2, left: this.graphJQ.width() / 2 };
    },


    /*
    *   Refreshes step properties side panel.
    */
    refreshProperties: function () {
        if (this.propertiesHandler) {
            this.propertiesHandler.refreshProperties();
        }
    },


    /*
    *   Refreshes the data placeholder of the provided node.
    */
    refreshNodeDataPlaceholder: function (nodeId) {
        if (this.isMarketingAutomationWorkflow() && this.dataHandler) {
            this.dataHandler.loadData(nodeId);
        }
    },


    /*
    *   Sets the z-index CSS property of all endpoints to the z-index value of the node to which they belong.
    */
    syncZIndexOfAllEndpoints: function () {
        for (var key in this.nodes) {
            this.nodes[key].syncZIndexOfNodeEndpoints();
        }
    },


    /*
    *   Updates the header of properties panel according to the name of the provided node.
    */
    updatePropertiesHeader: function (nodeId) {
        if (this.propertiesHandler) {
            var nodeName = this.nodes[nodeId].name;

            // node name is HTML encoded, get the raw value to prevent double encoding
            var nodeNameText = graphControlHandler.decodeHtmlEntities(nodeName);

            this.propertiesHandler.updatePropertiesHeader(nodeNameText);
        }
    },


    /*
    *   Returns true if node with provided id is currently selected.
    */
    isNodeSelected: function (nodeId) {
        var selectedItemJQ = $cmsj(this.selectedItem);
        return selectedItemJQ &&
            selectedItemJQ.hasClass('Node') &&
            selectedItemJQ.data("nodeObject").id === nodeId;
    },


    /*
    *   Shows the autosave message.
    */
    showAutosave: function () {
        var autosaveContainer = $cmsj(`#${this.autosaveId} > div`);
        var autosaveAlert = autosaveContainer.find("div.alert");

        // To re-display the same autosave message, the CSS styles which hide the placeholder must be unset
        if (autosaveAlert.is(":hidden")) {
            autosaveAlert.show();
            autosaveContainer.removeClass("hide");
        }

        if (typeof(CMSHandleLabel) === 'function') {
            // Add fadeout duration and offset to the message
            CMSHandleLabel(autosaveAlert.attr("id"), "", 3000, false, {
                posOffsetX: 20,
                posOffsetY: 20
            });
        }
    },

    /*
     * Makes given nodes emphasized.
     * @param {int} current - identifier of current step for contact, following parameters are previous step identifiers.
     */
    emphasizeNodes: function (current) {
        if (this.isMarketingAutomationWorkflow() && this.readOnly) {
            var node = this.nodes[current];
            if (node) {
                node.nodeJQ.addClass("visited-current");
                node.isEmphasized = true;
            }

            for (var i = 1; i < arguments.length; i++) {
                node = this.nodes[arguments[i]]
                if (node) {
                    node.nodeJQ.addClass("visited-past");
                }
            }
        }
    },

    /*
     * Shows contact given contact data in node with given identifier.
     * @param {int} current
     * @param {html string} data
     */
    showContactData: function (current, data) {
        if (this.isMarketingAutomationWorkflow() && this.readOnly) {
            var node = this.nodes[current];
            if (node && node.isEmphasized) {
                node.nodeJQ.append(data);
                node.setPlaceholderPosition();
                this.initNodeZIndex(node);
                node.syncZIndexOfNodeEndpoints();

                var nodeIcon = $cmsj('#nodeDataIcon_' + current);
                nodeIcon.click(function () {
                    var id = this.getAttribute("data-id");
                    $cmsj('#nodeData_' + id).toggle();
                });
            }
        }
    }
});


/*
*   Class handling the graph wrapper.
*/
var JsPlumbGraphWrapper = $class({

    constructor: function (parentId, droppableScope, preselectedNode, graph, isResizable) {
        this.id = parentId;
        this.droppableScope = droppableScope;
        this.containerJQ = $cmsj("#" + parentId);
        this.preselectedNode = preselectedNode;
        this.wrapperPadding = 70;
        this.graph = graph;
        this.isResizable = isResizable;

        this.wrapContainer();
        this.wrapGraph();
        this.createLegend();
        this.setHandlers();
    },


    /*
    *   Creates legend of graph.
    */
    createLegend: function () {
        var legendJQ = $cmsj("<div id='" + this.id + "Legend' class='GraphLegend'>");
        this.containerJQ.append(legendJQ);
        legendJQ = $cmsj("#" + this.id + "Legend");
        legendJQ.append("<div><div class='ConnectorExample Manual'></div><span>" + this.getManualConnectionTypeName() + "</span></div>");
        legendJQ.append("<div><div class='ConnectorExample Automatic'></div><span>" + this.getAutomaticConnectionTypeName() + "</span></div>");
        legendJQ.on("selectstart", "*", function () { return false; });
    },


    /*
    *   Returns resource string of name of manual connection type.
    */
    getManualConnectionTypeName: function () {
        return this.graph.getResourceString("ManualConnectionType");
    },


    /*
    *   Returns resource string of name automatic connection type.
    */
    getAutomaticConnectionTypeName: function () {
        return this.graph.getResourceString("AutomaticConnectionType");
    },


    /*
    *   Wraps graph view in another div, so UniGraph doesn't break surrounding elements.
    */
    wrapContainer: function () {
        var wrapperJQ = $cmsj("<div id='" + this.id + "UniGraphControl' class='UniGraph'>");
        this.containerJQ.wrap(wrapperJQ);
        this.containerJQ = $cmsj("#" + this.containerJQ.attr("id"));

        wrapperJQ = $cmsj("#" + wrapperJQ.attr("id"));

        if (!this.isResizable.height) {
            wrapperJQ.css("height", this.containerJQ.height());
        }

        if (!this.isResizable.width) {
            wrapperJQ.css("width", this.containerJQ.width());
        }
    },


    /*
    *   Sets handlers for wrapper movement.
    */
    setHandlers: function () {
        this.setNonEditingHandlers();
        if (!this.graph.readOnly) {
            this.setEditingHandlers();
        }
    },


    /*
    *   Sets handlers used for non-editing actions.
    */
    setNonEditingHandlers: function () {
        this.graph.graphJQ.draggable({ containment: 'parent', distance: 10 });
        this.graph.graphJQ.bind("mousewheel", graphControlHandler.getWrapperMouseWheelHandler(this));
        this.graph.graphJQ.bind("dragstart", graphControlHandler.getStartDraggingHandler(this.graph));
        this.graph.graphJQ.bind("dragstop", graphControlHandler.getStopDraggingHandler(this.graph.myJsPlumb));

        $cmsj(window).resize(graphControlHandler.getResizeWrapperHandler(this));
        $cmsj(document).ready(graphControlHandler.getResizeWrapperHandler(this));
    },


    /*
    *   Sets handlers thrown from wrapper needed for editing graph.
    */
    setEditingHandlers: function () {
        this.containerJQ.droppable({
            scope: this.droppableScope, drop: graphControlHandler.getCreateNodeHandler(this.graph), tolerance: "pointer",
            activate: function () { $cmsj(".UniGraph").addClass("Move"); },
            deactivate: function () { $cmsj(".UniGraph").removeClass("Move"); }
        });
        this.containerJQ.mousedown(function () { if (typeof (this.focus) == 'function') this.focus(); });
        this.containerJQ.click(graphSelectHandler.getDeselectItemHandler(this.graph));
        this.containerJQ.keydown(graphControlHandler.getKeyDownHandler(this.graph));
        this.wrapperJQ.mouseleave(graphSelectHandler.getRemoveHoverFromAllConnectionsHandler());
    },


    /*
    *   Method used for wrapping graph in elements for limiting view.
    */
    wrapGraph: function () {
        if (!this.wrapperJQ) {
            this.createWrapper();
        }
        this.setWrapperDimensions();
        this.setDefaultView();
    },


    /*
    *   Creates wrapper around graph.
    */
    createWrapper: function () {
        var wrapperJQ = $cmsj("<div id='" + this.id + "wrapper' class='GraphWrapper'>");
        this.graph.graphJQ.wrap(wrapperJQ);
        this.graph.graphJQ = $cmsj("#" + this.graph.graphJQ.attr("id"));
        this.wrapperJQ = $cmsj("#" + wrapperJQ.attr('id'));
    },


    /*
    *   Resizes graph wrapper to maximum available size.
    */
    setWrapperDimensions: function () {
        var graphWidth = this.graph.graphJQ.outerWidth();
        var graphHeight = this.graph.graphJQ.outerHeight();
        var containerWidth = this.containerJQ.width();
        var containerHeight = this.containerJQ.height();

        containerWidth = (graphWidth * 2) - containerWidth;
        containerHeight = (graphHeight * 2) - containerHeight;

        this.wrapperJQ.css("position", "relative");
        this.wrapperJQ.css("width", containerWidth);
        this.wrapperJQ.css("height", containerHeight);
        this.wrapperJQ.css("left", -containerWidth + graphWidth);
        this.wrapperJQ.css("top", -containerHeight + graphHeight);
    },


    /*
    *   Sets default point of view on graph.
    */
    setDefaultView: function () {
        if (this.graph.nodes[this.preselectedNode]) {
            this.setViewToNode(this.graph.nodes[this.preselectedNode]);
        } else {
            var position = this.graph.getCenterOfInitialView();
            this.setViewCenterTo(position);
        }
    },


    /*
    *   Sets size of container (most upper) element of graph.
    */
    setContainerDimensions: function () {
        //this needs to be here twice - first for setting, second for occasions when the first time changed elements in window.
        this.setContainerDimensionsHelper();
        this.setContainerDimensionsHelper();
    },


    /*
    *   Helper method used to change size of graph view.
    */
    setContainerDimensionsHelper: function () {
        var parentContainerJQ = this.containerJQ.parent();
        var origParentHeight;

        if (this.isResizable.height) {
            origParentHeight = $cmsj(window).height() - parentContainerJQ.offset().top - this.containerJQ.outerHeight() + this.containerJQ.height();
        } else {
            origParentHeight = parentContainerJQ.height();
        }

        var origParentWidth = parentContainerJQ.width() - this.containerJQ.outerWidth() + this.containerJQ.width();

        if (this.isResizable.height)
            this.containerJQ.css("height", origParentHeight);
        if (this.isResizable.width)
            this.containerJQ.css("width", origParentWidth);

        parentContainerJQ.css("height", origParentHeight);
    },


    /*
    *   Gets nearest top corner of view of graph.
    */
    getViewPosition: function () {
        var graphPosition = this.graph.graphJQ.position();
        return {
            left: graphPosition.left,
            top: graphPosition.top
        };
    },


    /*
    *   Sets center of view to given point.
    */
    setViewCenterTo: function (position) {
        position.left = this.wrapperJQ.width() / 2 - position.left;
        position.top = this.wrapperJQ.height() / 2 - position.top;
        this.setViewTo(position);
    },


    /*
    *   Sets left top corner of view to given point.
    */
    setViewTo: function (position) {
        position = this.ensureViewPosition(position);
        this.graph.graphJQ.css("left", position.left);
        this.graph.graphJQ.css("top", position.top);
    },


    /*
    *   Moves graph by one step.
    */
    moveViewBy: function (deltaX, deltaY) {
        var position = this.getViewPosition();
        deltaY *= 10;
        deltaX *= 10;
        position.top += deltaY;
        position.left += deltaX;
        position = this.ensureViewPosition(position);
        this.setViewTo(position);
    },


    /*
    *   Method correcting boundaries of graph position.
    */
    ensureViewPosition: function (position) {
        var graphJQ = this.graph.graphJQ;

        if (position.top < 0)
            position.top = 0;
        else if (position.top + graphJQ.outerHeight() > this.wrapperJQ.height())
            position.top = this.wrapperJQ.height() - graphJQ.outerHeight();

        if (position.left < 0)
            position.left = 0;
        else if (position.left + graphJQ.outerWidth() > this.wrapperJQ.width())
            position.left = this.wrapperJQ.width() - graphJQ.outerWidth();

        return position;
    },


    /*
    *   Sets center of view to given node.
    */
    setViewToNode: function (node) {
        var nodeJQ = node.nodeJQ;

        var position = {
            left: nodeJQ.position().left + nodeJQ.outerWidth(true) / 2,
            top: nodeJQ.position().top + nodeJQ.outerHeight(true) / 2
        };

        this.setViewCenterTo(position);

        if (this.graph.readOnly) {
            nodeJQ.addClass('Selected');
        } else {
            nodeJQ.click();
        }
    }
});