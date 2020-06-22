var JsPlumbMultichoiceNode = $class({

    Extends: JsPlumbConditionNode,

    constructor: function (readOnly, graph, definition) {
        JsPlumbConditionNode.call(this, readOnly, graph, definition);

        this.defaultSourcePointDefinition = [{ Type: "switchCase" }, { Type: "switchCase" }, { Type: "switchDefault"}];
        this.defaultCaseContent = "New condition";
    },


    /*
    *   Returns node HTML representation.
    */
    getDefaultHtmlElement: function () {
        return $cmsj(`<div class="Node${this.nodeStyle}" id="${this.htmlId}">
                        ${this.getNameHtmlRepresentation()}
                        ${this.getContentHtmlRepresentation()}
                    </div>`);
    },


    /*
    *   Returns add icon HTML representation.
    */
    getAddIconHtmlRepresentation: function () {
        if (this.readOnly)
            return '';
        return `<i id="${this.htmlId}_add" class="button cms-icon-50 icon-plus" title="${this.getAddCaseTooltip()}" aria-hidden="true"></i>`;
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
                    ${this.getDeleteIconHtmlRepresentation(this.getDeleteCaseTooltip())}
                    ${this.getEditIconHtmlRepresentation(this.getEditCaseTooltip())}
                </div>`;
    },


    /*
    *   Returns node icon HTML representation.
    */
    getNodeIconClass: function () {
        if (this.isFirstWinStepType())
            return 'icon-choice-single-scheme';
        return 'icon-choice-multi-scheme';
    },


    /*
    *   Returns add case tooltip.
    */
    getAddCaseTooltip: function () {
        return this.graph.getReadOnlyResourceString("AddCaseTooltip");
    },


    /*
    *   Returns delete case tooltip.
    */
    getDeleteCaseTooltip: function () {
        return this.graph.getReadOnlyResourceString("DeleteCaseTooltip");
    },


    /*
    *   Returns edit case tooltip.
    */
    getEditCaseTooltip: function () {
        return this.graph.getReadOnlyResourceString("EditCaseTooltip");
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
    *   Returns recalculated dynamic anchors of case source point.
    */
    getCaseSourcePointAnchors: function (switchCaseId) {
        var anchorTopOffset = this.getAnchorRelativeTopOffset(switchCaseId);

        if (this.graph.isMarketingAutomationWorkflow()) {
            return this.myJsPlumb.makeAnchor(1, anchorTopOffset, 1, 0);
        } else {
            var anchors = [
                [1, anchorTopOffset, 1, 0],
                [0, anchorTopOffset, -1, 0]
            ];
            return this.myJsPlumb.makeDynamicAnchor(anchors);
        }
    },


    /*
    *   Overrides default behavior.
    */
    setTimeout: function () {
        if (!this.isFirstWinStepType()) {
            this.hasTimeout = true;
            this.setTimeoutProperties();
        }
    },


    /*
    *   Returns true if the current step type is 'First win'.
    */
    isFirstWinStepType: function () {
        return this.definition.TypeName === 'MultichoiceFirstWin';
    }
});
