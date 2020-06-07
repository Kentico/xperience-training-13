/*
*   Abstract class inherited by every node.
*/
var JsPlumbAbstractNode = $class({

    // Basic initialization
    constructor: function (readOnly, graph, definition) {
        // ID from DB corresponds with id of HTMLDivElement
        this.definition = definition;
        this.id = definition.ID;
        this.readOnly = readOnly;
        graph.nodeDataRetrieved[this.id] = false;
        this.hasError = false;

        this.hasTimeout = false;
        this.timeoutDescription = null;
        this.name = "New node";
        this.content = null;
        this.setParent(graph);
        this.myJsPlumb = graph.myJsPlumb;

        this.sourcePoints = {};
        this.targetPoint = null;

        // Basic templates set
        this.sourcePointTemplates = new Object();
        this.targetPointTemplate = {};
        this.connectionTemplate = null;

        this.defaultSourcePointDefinition = [{ Type: "standard" }];
        this.sourcePointsMax = 1000;
        this.sourcePointsMin = 0;

        this.isEmphasized = false;
    },


    /*
    *   Prints node on specified point.
    */
    printNode: function () {
        this.setProperties(this.definition);
        this.appendToParent();

        this.targetPoint = this.addEndpoint(this.targetPointTemplate);
        if (this.definition.HasTargetPoint) {
            this.targetPoint.initDropTarget($cmsj("#" + this.htmlId));
        }

        this.addSourcePoints(this.definition.SourcePoints);
        this.setPosition(this.definition.Position);
        this.setPlaceholderPosition();
        this.repaintAllEndpoints();

        this.setRedeability();
        this.ensureTooltips();
    },


    /*
    *   Prepares node to be static or editable
    */
    setRedeability: function () {
        if (!this.readOnly) {
            //add ", grid: [10, 10]" to second parameter to snap to grid
            this.myJsPlumb.draggable(this.htmlId, { containment: 'parent', stack: '.Node', cancel: 'div.nodeDataPlaceholder', stop: this.removeFixedSize });
            this.setSnapToGridByGraph();
        } else {
            this.myJsPlumb.toggleDraggable(this.htmlId);
        }
    },


    /*
    *   Enables/disables snapping to grid.
    */
    setSnapToGridByGraph: function () {
        if (this.graph.snapToGrid) {
            this.nodeJQ.draggable("option", "grid", this.graph.gridSize);
        } else {
            this.nodeJQ.draggable("option", "grid", false);
        }
    },


    /*
    *   Removes width and height inline style properties.
    */
    removeFixedSize: function (_event, ui) {
        ui.helper.css({ "width": "", "height": "" });
    },


    /*
    *   Creates HTML element and appends it to parent.
    */
    appendToParent: function () {
        this.nodeJQ = this.getDefaultHtmlElement();
        this.nodeJQ.data('nodeObject', this);
        this.parent.append(this.nodeJQ);
    },


    /*
    *   Sets node to position.
    */
    setPosition: function (position) {
        this.position = this.getEnsuredNodePosition(position);
        this.nodeJQ.css("top", this.position.Y);
        this.nodeJQ.css("left", this.position.X);
    },


    /*
    *   Method correcting node position.
    */
    getEnsuredNodePosition: function (position) {

        var newPosition = {
            Y: position.Y || position.y || position.top || 0,
            X: position.X || position.x || position.left || 0
        };
        var top = parseInt(this.parent.css("padding-top"));
        var left = parseInt(this.parent.css("padding-left"));
        var right = this.parent.outerWidth() - parseInt(this.parent.css("padding-right"));
        var bottom = this.parent.outerHeight() - parseInt(this.parent.css("padding-bottom"));

        if (newPosition.Y < top)
            newPosition.Y = top;
        else if (newPosition.Y + this.nodeJQ.outerHeight() > bottom)
            newPosition.Y = bottom - this.nodeJQ.outerHeight();

        if (newPosition.X < left)
            newPosition.X = left;
        else if (newPosition.X + this.nodeJQ.outerWidth() > right)
            newPosition.X = right - this.nodeJQ.outerWidth();

        return newPosition;
    },


    /*
    *   Returns node HTML representation.
    */
    getDefaultHtmlElement: function () {
        return $cmsj(`<div class="Node ${this.nodeStyle}" id="${this.parent.attr('id')}_${this.id}">
                        ${this.getNameHtmlRepresentation()}
                        ${this.getContentHtmlRepresentation()}
                        ${this.getIconHtmlRepresentation()}
                        ${this.getDataRetrievalHtmlRepresentation()}
                    </div>`);
    },


    /*
    *   Returns correct HTML representation of name.
    */
    getNameHtmlRepresentation: function () {
        var inner = '';

        // Check whether the graph workflow is of type Automation - this type has a different HTML representation
        if (this.graph.isMarketingAutomationWorkflow()) {
            var icon = this.getNodeThumbnail();
            inner = `${icon ? icon : '<div class="cms-icon-container"><i class="cms-icon-180 icon-question-circle" aria-hidden="true"></i></div>'}`;
        } else {
            inner = `${this.getNodeIconHtmlRepresentation()}
                    <div class="text ${this.getLocalizedClass(this.definition.IsNameLocalized)}"><span class="Editable">${this.name}</span></div>
                    ${this.getDeleteIconHtmlRepresentation(this.graph.getReadOnlyResourceString("DeleteNodeTooltip"))}
                    ${this.getEditIconHtmlRepresentation(this.graph.getReadOnlyResourceString("EditNodeTooltip"))}
                    ${this.getAddIconHtmlRepresentation(this.graph.getReadOnlyResourceString("AddCaseTooltip"))}`;
        }

        return `<div class="header Name ${this.nodeStyle}">
                    ${inner}
                    <div class="clear"></div>
                </div>`;
    },


    /*
    *   Returns localized class if given true.
    */
    getLocalizedClass: function (isLocalized) {
        return isLocalized ? " Localized" : "";
    },


    /*
    *   Returns node icon HTML representation.
    */
    getNodeIconHtmlRepresentation: function () {
        return '';
    },

    /*
    *   Returns node icon HTML representation.
    */
    getNodeIconClass: function () {
        return '';
    },

    /*
    *   Returns HTML representation for icon loading automation data.
    */
    getIconHtmlRepresentation: function () {
        if (this.graph.isMarketingAutomationWorkflow() && this.hasError && !this.readOnly) {
            return `<div class="nodeErrorIconPlaceholder"><i aria-hidden="true" class="cms-icon-80 icon-exclamation-triangle"></i></div>`;
        } else if (this.graph.isMarketingAutomationWorkflow() && this.RetrievesData && !this.readOnly) {
            return `<div class="nodeDataIconPlaceholder" id="nodeDataIcon_${this.id}" data-id="${this.id}"><i aria-hidden="true" class="cms-icon-150 icon-i-circle"></i></div>`;
        } else {
            return '';
        }
    },

    /*
    *   Returns HTML representation for div loading automation data.
    */
    getDataRetrievalHtmlRepresentation: function () {
        if (this.graph.isMarketingAutomationWorkflow() && this.RetrievesData && !this.readOnly) {
            return `<div id="nodeData_${this.id}" class="nodeDataPlaceholder hidden">
                        <div class="refresh-button" data-id="${this.id}">
                            <i aria-hidden="true" class="button cms-icon-50 icon-rotate-double-right" title="${this.graph.getReadOnlyResourceString("RefreshDataPlaceholderTooltip")}"></i>
                        </div>
                        <div class="dataPlaceholderContent"></div>
                    </div>`;
        } else {
            return '';
        }
    },

    /*
    *   Returns node thumbnail HTML representation.
    */
    getNodeThumbnail: function () {
        if (this.thumbnailImageUrl) {
            return `<div class="icon" style="background-image: url('${this.thumbnailImageUrl}');"></div>`;
        } else if (this.thumbnailClass) {
            return `<div class="cms-icon-container"><i class="cms-icon-${this.graph.isMarketingAutomationWorkflow() ? '180' : '150'} ${this.thumbnailClass}" aria-hidden="true" ></i></div>`;
        }

        return null;
    },

    /*
    *   Returns add icon HTML representation.
    */
    getAddIconHtmlRepresentation: function () {
        return '';
    },


    /*
    *   Returns edit node icon HTML representation.
    */
    getEditIconHtmlRepresentation: function (title) {
        if (this.readOnly)
            return '';
        return `<i class="button cms-icon-50 icon-edit" title="${title}" aria-hidden="true"></i>`;
    },


    /*
    *   Returns delete node icon HTML representation.
    */
    getDeleteIconHtmlRepresentation: function (title) {
        if (this.readOnly || !this.definition.IsDeletable)
            return '';
        return `<i class="button cms-icon-50 icon-bin" title="${title}" aria-hidden="true"></i>`;
    },


    /*
    *   Returns duplicate node icon HTML representation.
    */
    getDuplicateIconHtmlRepresentation: function (title) {
        if (this.readOnly || !this.definition.IsDeletable || !this.graph.isMarketingAutomationWorkflow())
            return '';
        return `<i class="button cms-icon-50 icon-doc-copy" title="${title}" aria-hidden="true"></i>`;
    },


    /*
    *   Returns correct HTML representation of content.
    */
    getContentHtmlRepresentation: function () {
        var inner = '';

        // Check whether the graph workflow is of type Automation - this type has a different HTML representation
        if (this.graph.isMarketingAutomationWorkflow()) {
            inner = `<div class="content-header">
                        ${this.getDeleteIconHtmlRepresentation(this.graph.getReadOnlyResourceString("DeleteNodeTooltip"))}
                        ${this.getDuplicateIconHtmlRepresentation(this.graph.getReadOnlyResourceString("DuplicateNodeTooltip"))}
                        <div class="text ${this.getLocalizedClass(this.definition.IsNameLocalized)}">
                            <span class="caption">${this.name}</span>
                        </div>
                    </div>
                    ${this.getDescriptionHtml()}`;
        } else {
            var icon = this.getNodeThumbnail();
            inner = `${icon ? icon : this.getDescriptionHtml()}`;
        }

        return `<div class="content">${inner}${this.getFooterHtmlRepresentation()}</div>`;
    },


    /*
    *   Returns the HTML representation of node description.
    */
    getDescriptionHtml: function () {
        if (this.graph.isMarketingAutomationWorkflow()) {
            return this.content ? `<div class="main">${this.content}</div>` : '';
        }

        return `<div class="main">${this.getCleanedString(this.content)}</div>`;
    },


    /*
    *   Returns cleans string from html tags.
    */
    getCleanedString: function (string) {
        if (!string)
            return '';
        return $cmsj("<span/>").text(string).html();
    },


    /*
    *   Returns correct HTML representation of footer.
    */
    getFooterHtmlRepresentation: function () {
        if (this.hasTimeout) {
            if (this.graph.isMarketingAutomationWorkflow()) {
                return `<div class="footer">
                            <span class="timeout" title="${this.graph.getResourceString("TimeoutIconTooltip")}">${this.graph.getResourceString("Timeout")}:</span>
                            <span>${this.timeoutDescription}</span>
                        </div>`;
            } else {
                return `<div class="hr"></div>
                        <i class="cms-icon-80 icon-clock" title="${this.graph.getResourceString("TimeoutIconTooltip")}" aria-hidden="true"></i>`;
            }
        }
        return '';
    },


    /*
    *   Adds source points.
    */
    addSourcePoints: function (definition) {
        if (definition.length < this.sourcePointsMin) {
            definition = this.defaultSourcePointDefinition;
        }
        for (var key in definition) {
            var sourcePointDef = definition[key];
            if (this.sourcePointCanBeAdded())
                this.addSourcePoint(sourcePointDef);
        }
    },


    /*
    *   Adds single source point.
    */
    addSourcePoint: function (sourcePointDef) {
        var dbID = sourcePointDef.ID;
        if (!sourcePointDef.ID) {
            sourcePointDef.ID = this.getSourcePointsCount();
        }
        if (sourcePointDef.Type === "switchCase") {
            this.addConditionRow(sourcePointDef);
        }
        this.sourcePoints[sourcePointDef.ID] = this.addEndpoint(this.sourcePointTemplates[sourcePointDef.Type], sourcePointDef);
        this.sourcePoints[sourcePointDef.ID].sourcePointType = sourcePointDef.Type;
        this.sourcePoints[sourcePointDef.ID].ID = dbID;
        return this.sourcePoints[sourcePointDef.ID];
    },


    /*
    *   Creates case row of switch node.
    */
    addConditionRow: function () {
    },


    /*
    *   Sets source points count limitations.
    */
    setSourcePointsLimits: function (min, max) {
        this.sourcePointsMax = max;
        this.sourcePointsMin = min;
    },


    /*
    *   Returns newly added endpoint.
    */
    addEndpoint: function (template, sourcePointDef) {
        var primaryConfiguration = {
            node: this,
            connectorStyle: template.connectorStyle || this.connectionTemplate,
            tooltip: this.getSourcepointTooltip(sourcePointDef)
        };

        return this.myJsPlumb.addEndpoint(this.htmlId, primaryConfiguration, template);
    },


    /*
    *   Gets tooltip that should be used instead of default one.
    *   Return undefined to use default.
    *   Return null to no tooltip.
    */
    getSourcepointTooltip: function (sourcePointDef) {
        return undefined;
    },


    /*
    *   Repaints all default endpoints.
    */
    repaintAllEndpoints: function () {
        this.resizeTargetPoint();
        this.targetPoint.repaint();
        for (var key in this.sourcePoints) {
            var sourcePoint = this.sourcePoints[key];
            this.resizeSourcePoint(sourcePoint, key);
            sourcePoint.repaint();
        }
        if (this.fakeEndpointDefinition) {
            this.myJsPlumb.repaint(this.nodeJQ);
        }

        if (this.graph.isMarketingAutomationWorkflow()) {
            this.syncZIndexOfNodeEndpoints();
        }
    },


    /*
    *   Sets the z-index CSS property of all node's endpoints to the z-index of the node.
    */
    syncZIndexOfNodeEndpoints: function () {
        var nodeZIndex = this.nodeJQ.css("z-index");

        $cmsj(this.targetPoint.canvas).css("z-index", nodeZIndex);

        for (var key in this.sourcePoints) {
            $cmsj(this.sourcePoints[key].canvas).css("z-index", nodeZIndex);
        }
    },


    /*
    *   Method to be redefined in case of target point sizing depends on node size.
    */
    resizeTargetPoint: function () {
    },


    /*
    *   Method to be redefined in case of source point sizing depends on node size.
    */
    resizeSourcePoint: function (sourcePoint, id) {
    },


    /*
    *   Method to be redefined in case of node which should have switch cases in rows.
    */
    removeCaseRow: function (caseId) {
    },


    /*
    *   Sets JsPlumb instance.
    */
    setJsPlumbInstance: function (jsPlumbInstance) {
        this.myJsPlumb = jsPlumbInstance;
    },


    /*
    *   Sets ID of parental HTML element.
    */
    setParent: function (graph) {
        this.graph = graph;
        this.parent = graph.graphJQ;
        this.htmlId = this.parent.attr('id') + '_' + this.id;
    },


    /*
    *   Fills node with specified data.
    */
    setProperties: function (definition) {
        this.name = this.getCleanedString(definition.Name);
        this.content = definition.Content;
        this.hasError = definition.HasError;
        this.nodeStyle = " " + definition.CssClass + " " + this.definition.TypeName;
        if (definition.HasTimeout) {
            this.timeoutDescription = this.getCleanedString(definition.TimeoutDescription);
            this.setTimeout();
        } else {
            this.removeTimeout();
        }
        if (definition.ThumbnailImageUrl)
            this.thumbnailImageUrl = definition.ThumbnailImageUrl;
        if (definition.ThumbnailClass)
            this.thumbnailClass = definition.ThumbnailClass;
        if (definition.RetrievesData)
            this.RetrievesData = definition.RetrievesData;
    },


    /*
    *   Sets node to be have timeout properties
    */
    setTimeout: function () {
        if (!this.hasTimeout) {
            this.hasTimeout = true;
            this.sourcePointsMax++;
            this.sourcePointsMin++;
            this.setTimeoutProperties();
        }
    },


    /*
    *   To be overridden - for additional timeout settings
    */
    setTimeoutProperties: function () {
    },


    /*
    *   Removes timeout properties of node.
    */
    removeTimeout: function () {
        if (this.hasTimeout) {
            this.hasTimeout = false;
            this.timeoutDescription = null;
            this.sourcePointsMax--;
            this.sourcePointsMin--;
            this.removeTimeoutProperties();
        }
    },


    /*
    *   To be overridden - for additional timeout settings
    */
    removeTimeoutProperties: function () {
    },


    /*
    *   If there is only one source point, we know which one we should pick.
    *   Used in case that user drops connection to drop point instead of source point.
    */
    getWildcardSourcePoint: function () {
        if (this.getSourcePointsCount() === 1) {
            for (var name in this.sourcePoints) {
                if (this.sourcePoints[name])
                    return this.sourcePoints[name];
            }
        }
        return null;
    },


    /*
    *   Method returning number of source points of this node
    */
    getSourcePointsCount: function () {
        var size = 0, key;
        for (key in this.sourcePoints) {
            if (this.sourcePoints[key]) size++;
        }
        return size;
    },


    /*
    *   Creates new connection by handed definition.
    */
    createConnection: function (definition) {
        var sourcePoint = this.sourcePoints[definition.sourcePointId];
        if (sourcePoint) {
            var connection = this.myJsPlumb.connect({ source: sourcePoint, target: definition.targetNode.targetPoint });
            if (connection) {
                connection.ID = definition.ID;
                $cmsj(connection.canvas).addClass("conn_" + connection.ID);
                if (!this.readOnly) {
                    this.bindEventsToConnection(connection);
                }
            }
        }
    },


    /*
    *   Binds events on given connection.
    */
    bindEventsToConnection: function (connection) {
        connection.bind("mousedown", function (conn, e) { e.stopImmediatePropagation(); });
        connection.bind("mousemove", graphSelectHandler.getConnectionDraggingHandler(this.graph));
        $cmsj(connection.canvas).bind("dragstart", graphControlHandler.stopEvent);
    },


    /*
    *   Repaints node by given definition.
    */
    repaint: function (definition) {
        this.definition = definition;
        this.setProperties(definition);
        this.repaintNode();
        if (definition.SourcePoints.length > 0) {
            this.setNewSourcePoints(definition.SourcePoints);
        }
        if (definition.Position.x !== -1 || definition.Position.y !== -1) {
            this.setPosition(definition.Position);
        }
        this.repaintAllEndpoints();
        this.ensureTooltips();
        this.setPlaceholderPosition();
    },


    /*
    *   Repaints HTML of node.
    */
    repaintNode: function () {
        this.nodeJQ.addClass(this.nodeStyle);
        this.repaintName();
        this.repaintContent();
        this.repaintError();
    },


    /*
    *   Repaints name of node.
    */
    repaintName: function () {
        this.nodeJQ.children(".header").replaceWith(this.getNameHtmlRepresentation());
    },


    /*
    *   Repaints content of node.
    */
    repaintContent: function () {
        this.nodeJQ.children(".content").replaceWith(this.getContentHtmlRepresentation());
    },


    /*
    *   Repaints content of error message placeholder.
    */
    repaintError: function () {
        this.nodeJQ.children(".nodeErrorIconPlaceholder").replaceWith(this.getIconHtmlRepresentation());
    },

    /*
    *   Sets new source points collection given in definition.
    */
    setNewSourcePoints: function (sourcePoints) {
        var connections = this.removeAllSourcePoints();
        this.addSourcePoints(sourcePoints);
        this.createConnections(connections);
        this.toggleSourcePointsClass("Front");
    },


    /*
    *   Removes outdated source points by given new collection of nodes.
    */
    removeAllSourcePoints: function () {
        var connectionDefinitions = new Array();
        for (var key in this.sourcePoints) {
            var connection = this.getConnectionDefinitionFrom(key);
            if (connection) {
                connectionDefinitions.push(connection);
            }

            var type = this.sourcePoints[key].sourcePointType;
            if (type === "switchCase") {
                this.removeCaseRow(key);
            } else {
                this.removeSourcePoint(key);
            }
        }
        return connectionDefinitions;
    },


    /*
    *   Removes source point from node.
    */
    removeSourcePoint: function (id) {
        this.myJsPlumb.deleteEndpoint(this.sourcePoints[id]);
        delete this.sourcePoints[id];
    },


    /*
    *   Returns definition of connection of given source point.
    */
    getConnectionDefinitionFrom: function (sourcePointId) {
        var connection, target;
        var sourcePoint = this.getSourcePoint(sourcePointId);

        if (sourcePoint) {
            connection = sourcePoint.connections[0];
        }
        if (connection) {
            target = connection.target.data("nodeObject");
        }
        if (target && connection) {
            return { sourcePointId: sourcePointId, targetNode: target, ID: connection.ID };
        }
        return undefined;
    },


    /*
    *   Creates connections based on given array of definitions.
    */
    createConnections: function (definitions) {
        for (var key in definitions) {
            this.createConnection(definitions[key]);
        }
    },


    /*
    *   Returns source point by given ID or if there is only single source point, than it returns him.
    */
    getSourcePoint: function (id) {
        if (this.getSourcePointsCount() === 1) {
            for (var key in this.sourcePoints) {
                return this.sourcePoints[key];
            }
        }
        return this.sourcePoints[id];
    },


    /*
    *   Checks whether source point can be removed.
    */
    sourcePointCanBeRemoved: function () {
        return this.getSourcePointsCount() > this.sourcePointsMin;
    },


    /*
    *   Checks whether source point can be removed.
    */
    sourcePointCanBeAdded: function () {
        return this.getSourcePointsCount() < this.sourcePointsMax;
    },


    /*
    *   Ensures node to be in front of others.
    */
    ensureFront: function () {
        this.toggleClass("Front", true);

        if (this.graph.isMarketingAutomationWorkflow()) {
            this.syncZIndexOfNodeEndpoints();
        }
    },


    /*
    *   Ensures node not to be in front of others.
    */
    ensureNotFront: function () {
        this.toggleClass("Front", false);

        if (this.graph.isMarketingAutomationWorkflow()) {
            this.syncZIndexOfNodeEndpoints();
        }
    },


    /*
    *   Used for bringing endpoints and connections to front and back.
    */
    toggleClass: function (className, addOrRemove) {
        this.nodeJQ.toggleClass(className, addOrRemove);
        this.targetPoint.toggleClass(className, addOrRemove);
        this.toggleSourcePointsClass(className, addOrRemove);
    },


    /*
    *   Used for bringing source points and connections to front and back.
    */
    toggleSourcePointsClass: function (className, addOrRemove) {
        for (var key in this.sourcePoints) {
            this.sourcePoints[key].toggleClass(className, addOrRemove);
        }
    },


    /*
    *   Ensures tooltips for this node.
    */
    ensureTooltips: function () {
        this.nodeJQ.find("[title]:not(.Case)").find("[title]:not(.nodeErrorIconPlaceholder)").tooltip(this.graph.tooltipConfiguration);
        this.ensureEndpointTooltip(this.targetPoint);
        for (var key in this.sourcePoints) {
            this.ensureEndpointTooltip(this.sourcePoints[key]);
        }
    },


    /*
    *   Ensures tooltips for given endpoint.
    */
    ensureEndpointTooltip: function (endpoint) {
        var canvasJQ = $cmsj(endpoint.canvas);
        if (canvasJQ.is("[title]")) {
            canvasJQ.tooltip(this.graph.tooltipConfiguration);
        }
    },


    /*
    *   Returns maximum source point count message.
    */
    getMaxSourcePointCountError: function () {
        return this.graph.getReadOnlyResourceString("MaxCaseSourcePointCountError");
    },


    /*
    *   Returns minimum source point count message.
    */
    getMinSourcePointCountError: function () {
        return this.graph.getReadOnlyResourceString("MinCaseSourcePointCountError");
    },


    /*
    *   Returns case delete confirmation message.
    */
    getCaseDeleteConfirmation: function () {
        return this.graph.getReadOnlyResourceString("CaseDeleteConfirmation");
    },


    /*
    *   Open modal dialog.
    */
    openEditModalDialog: function (sourcePointId) {
        var url;
        if (sourcePointId === undefined) {
            url = this.graph.addresses["EditNodeDialog"];
        } else {
            url = this.graph.addresses["EditCaseDialog"];
        }
        if (url) {
            url += "&objectid=" + this.id + "&graph=" + this.graph.jsObjectName + this.getSourcepointQueryString(sourcePointId);
            modalDialog(url, "Edit", "90%", "85%");
        }
    },


    /*
    *   Returns query string of source point.
    */
    getSourcepointQueryString: function () {
        return "";
    },


    /*
    * Sets coordinates for data placeholder.
    */
    setPlaceholderPosition: function () {
        if (this.graph.isMarketingAutomationWorkflow()
            && ((this.RetrievesData && !this.readOnly) || (this.isEmphasized && this.readOnly))) {

            var placeholder = $cmsj(`#nodeData_${this.id}`);
            var top = this.nodeJQ.height() - 20;
            var left = this.nodeJQ.width() + 35;

            placeholder.css({ top: `${top}px`, left: `${left}px` });
        }
    }
});
