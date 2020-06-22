var JsPlumbConditionNode = $class({

    Extends: JsPlumbAbstractNode,

    constructor: function (readOnly, graph, definition) {
        JsPlumbAbstractNode.call(this, readOnly, graph, definition);

        this.everySideFakeAnchors = [
            [0.1, 0, 0, -1],
            [0.3, 0, 0, -1],
            [0.5, 0, 0, -1],
            [0.7, 0, 0, -1],
            [0.9, 0, 0, -1]
        ];

        //Fake endpoint definitions are used instead of actual target point
        //You can specify better anchors and prevents changing position of selected connection
        this.fakeEndpointDefinition = {
            dynamicAnchors: this.graph.isMarketingAutomationWorkflow() ? ["CenterTarget"] : this.everySideFakeAnchors,
            isSource: false,
            isTarget: true,
            reattach: true,
            endpoint: ["Blank"]
        };

        // Redefines inherited templates
        this.switchCaseSourcePointTemplate = {
            dynamicAnchors: this.graph.isMarketingAutomationWorkflow() ? ["CaseSource"] : ["RightMiddle", "LeftMiddle"],
            endpoint: this.graph.isMarketingAutomationWorkflow()
                ? ["Dot", { radius: 8, paintStyle: { fillStyle: "#2CB14E", icon: "case" }}]
                : ["Image", { url: this.graph.addresses["ImagesUrl"] + "/endpoint_case.png"}],
            endpointSelected: ["Image", { url: this.graph.addresses["ImagesUrl"] + "/endpoint_case_selected.png"}],
            tooltip: this.getCaseTooltip(),
            reattachHelperTooltip: graph.getReadOnlyResourceString("ReattachHelperTooltip"),
            isSource: true,
            isTarget: true,
            reattach: true,
            maxConnections: 1,
            dragOptions: { disabled: this.readOnly },
            connectorStyle: { lineWidth: 2, strokeStyle: this.graph.isMarketingAutomationWorkflow() ? "#2CB14E" : "#bdbbbb" }
        };

        this.switchDefaultSourcePointTemplate = {
            anchor: this.graph.isMarketingAutomationWorkflow() ? "CenterSource" : "BottomCenter",
            endpoint: this.graph.isMarketingAutomationWorkflow()
                ? ["Dot", { radius: 8, paintStyle: { fillStyle: "#EF5350", icon: "else" }}]
                : ["Image", { url: this.graph.addresses["ImagesUrl"] + "/endpoint_else.png"}],
            endpointSelected: ["Image", { url: this.graph.addresses["ImagesUrl"] + "/endpoint_case_selected.png"}],
            tooltip: graph.getReadOnlyResourceString(this.definition.TypeName + "Default", "SourcepointSwitchDefaultTooltip"),
            reattachHelperTooltip: graph.getReadOnlyResourceString("ReattachHelperTooltip"),
            isSource: true,
            isTarget: true,
            reattach: true,
            maxConnections: 1,
            dragOptions: { disabled: this.readOnly },
            connectorStyle: { lineWidth: 2, strokeStyle: this.graph.isMarketingAutomationWorkflow() ? "#EF5350" : "#bdbbbb" }
        };

        this.sourcePointTemplates = {
            switchDefault: this.switchDefaultSourcePointTemplate,
            switchCase: this.switchCaseSourcePointTemplate
        };

        this.targetPointTemplate = {
            anchor: this.graph.isMarketingAutomationWorkflow() ? "CenterTarget" : "Center",
            endpoint: this.graph.isMarketingAutomationWorkflow()
                ? ["Dot", { radius: 7, paintStyle: { fillStyle: "#E0E0E0", strokeStyle: "#989898" }}]
                : ["Blank"],
            isSource: false,
            isTarget: true,
            reattach: true,
            maxConnections: -1,
            fakeEndpointDefinition: this.fakeEndpointDefinition
        };

        this.defaultSourcePointDefinition = [{ Type: "switchCase" }, { Type: "switchDefault"}];
    },


    /*
    *   Returns case tooltip.
    */
    getCaseTooltip: function () {
        return this.graph.getReadOnlyResourceString(this.definition.TypeName + "Case", "SourcepointSwitchCaseTooltip");
    },


    /*
    *   Creates case row of switch node.
    */
    addConditionRow: function (sourcePoint) {
        sourcePoint.Label = this.getCleanedString(sourcePoint.Label);
        sourcePoint.HtmlID = this.getHtmlRowId(sourcePoint.ID);
        $cmsj('#' + this.htmlId + ' div.Cases').append(this.getCaseRowTemplate(sourcePoint));
        $cmsj('#' + sourcePoint.HtmlID).data('id', sourcePoint.ID);
        this.createConditionRowTooltip(sourcePoint);
    },


    /*
    *   Creates custom tooltip for condition.
    */
    createConditionRowTooltip: function (sourcePoint) {
        if (!sourcePoint.Tooltip || sourcePoint.Tooltip.length === 0) {
            return '';
        }

        var tooltipId = `${sourcePoint.HtmlID}tooltip`;
        $cmsj('#' + tooltipId).remove();
        var tooltip = $cmsj(`<div id="${tooltipId}" class="tooltip in CustomTooltip">${sourcePoint.Tooltip}</div>`);
        this.graph.graphJQ.parents("body").append(tooltip);
        var tooltipSettings = this.getConditionTooltipSettings(tooltipId);
        $cmsj("#" + sourcePoint.HtmlID).find(".Editable").tooltip(tooltipSettings).data("tooltip").show().hide();

        return;
    },


    /*
    *   Creates condition tooltip settings.
    */
    getConditionTooltipSettings: function (tooltipId) {
        var tooltipSettings = this.myJsPlumb.extend({}, this.graph.tooltipConfiguration);
        tooltipSettings.tip = '#' + tooltipId;
        if (isRTL) {
            tooltipSettings.position = "top right";
        } else {
            tooltipSettings.position = "top left";
        }
        tooltipSettings.effect = "condition";
        return tooltipSettings;
    },


    /*
    *   Returns new case row HTML representation.
    */
    getCaseRowTemplate: function (sourcePoint) {
        if (!sourcePoint.Label) {
            sourcePoint.Label = this.defaultCaseContent;
        }
        return `<div id="${sourcePoint.HtmlID}" class="box gradient Case">
                    <div class="inner ${this.getLocalizedClass(sourcePoint.IsLabelLocalized)}"><span class="Editable">${sourcePoint.Label}</span></div>
                </div>`;
    },


    /*
    *   Returns node icon HTML representation.
    */
    getNodeIconClass: function () {
        return 'icon-diamond';
    },


    /*
    *   Returns node icon HTML representation.
    */
    getNodeIconHtmlRepresentation: function () {
        var tooltip = this.graph.getResourceString(this.definition.TypeName + "Tooltip");
        return `<i class="${this.getNodeIconClass()}" aria-hidden="true"${tooltip ? ` title=${tooltip}` : ` `}></i>`;
    },


    /*
    *   Returns id of html row element.
    */
    getHtmlRowId: function (id) {
        return this.htmlId + id;
    },


    /*
    *   Returns correct HTML representation of content.
    */
    getContentHtmlRepresentation: function () {
        var inner = "";

        // Check whether the graph workflow is of type Automation - this type has a different HTML representation
        if (this.graph.isMarketingAutomationWorkflow()) {
            inner = `<div class="content-header">
                        ${this.getDeleteIconHtmlRepresentation(this.graph.getReadOnlyResourceString("DeleteNodeTooltip"))}
                        ${this.getDuplicateIconHtmlRepresentation(this.graph.getReadOnlyResourceString("DuplicateNodeTooltip"))}
                        ${this.getAddIconHtmlRepresentation(this.graph.getReadOnlyResourceString("AddCaseTooltip"))}
                        <div class="text ${this.getLocalizedClass(this.definition.IsNameLocalized)}">
                            <span class="caption">${this.name}</span>
                        </div>
                     </div>
                    <div class="Cases"></div>`;
        } else {
            inner = `${this.getCleanedString(this.content)}<div class="Cases"></div>`;
        }

        return `<div class="content gray">
                    ${inner}
                    ${this.getFooterHtmlRepresentation()}
                </div>`;
    },


    /*
    *   Redefinition of inherited method.
    *   Recounts anchors positions of case source points
    */
    resizeSourcePoint: function (sourcePoint, id) {
        if (sourcePoint.sourcePointType === "switchCase") {
            var anchor = this.getCaseSourcePointAnchors(id);
            sourcePoint.anchor = anchor;
        }
    },


    /*
    *   Returns default switch case source point.
    */
    getCaseSourcePointAnchors: function (switchCaseId) {
        if (this.graph.isMarketingAutomationWorkflow()) {
            return this.myJsPlumb.makeAnchor(0.76, 1, 0, 1);
        } else {
            var anchorTopOffset = this.getAnchorRelativeTopOffset(switchCaseId);
            var anchors = [
                [1, anchorTopOffset, 1, 0],
                [0, anchorTopOffset, -1, 0]
            ];
            return this.myJsPlumb.makeDynamicAnchor(anchors);
        }
    },


    /*
    *   Returns calculated offset of case source point from top of node.
    */
    getAnchorRelativeTopOffset: function (switchCaseId) {
        var rowJQ = $cmsj("#" + this.getHtmlRowId(switchCaseId));
        var topOffset = rowJQ.offset().top - this.nodeJQ.offset().top;
        topOffset += (rowJQ.outerHeight() / 2);
        topOffset /= this.nodeJQ.outerHeight();

        return topOffset;
    },


    /*
    *   Returns query string of source point.
    */
    getSourcepointQueryString: function (sourcePointId) {
        if (sourcePointId) {
            return "&sourcepointGuid=" + sourcePointId;
        }
        return "";
    },


    /*
    *   Removes case row.
    */
    removeCaseRow: function (caseId) {
        $cmsj("#" + this.getHtmlRowId(caseId)).remove();
        $cmsj("#" + this.getHtmlRowId(caseId) + "tooltip").remove();
        this.removeSourcePoint(caseId);
    }
});
