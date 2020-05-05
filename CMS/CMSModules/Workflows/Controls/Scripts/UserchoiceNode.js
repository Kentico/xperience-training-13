var JsPlumbUserChoiceNode = $class({

    Extends: JsPlumbMultichoiceNode,

    constructor: function (readOnly, graph, definition) {
        JsPlumbMultichoiceNode.call(this, readOnly, graph, definition);

        this.defaultSourcePointDefinition = [{ Type: "switchCase" }, { Type: "switchCase"}];
        this.defaultCaseContent = "New condition";

        if (this.graph.isMarketingAutomationWorkflow()) {
                this.switchCaseSourcePointTemplate.endpoint = ["Dot", { radius: 8, paintStyle: { fillStyle: "#E0E0E0", strokeStyle: "#989898" }}];
                this.switchCaseSourcePointTemplate.connectorStyle = { lineWidth: 2, strokeStyle: '#1175AE' };
        }

        this.timeoutSourcePointTemplate = {
            anchor: this.graph.isMarketingAutomationWorkflow() ? "TimeoutSource" : "BottomCenter",
            endpoint: this.graph.isMarketingAutomationWorkflow()
                ? ["Dot", { radius: 8, paintStyle: { fillStyle: "#d4ba37", icon: "timeout" }}]
                : ["Image", { url: this.graph.addresses["ImagesUrl"] + "/endpoint_timeout.png"}],
            endpointSelected: ["Image", { url: this.graph.addresses["ImagesUrl"] + "/endpoint_standard_selected.png"}],
            tooltip: graph.getReadOnlyResourceString("SourcepointTimeoutTooltip"),
            reattachHelperTooltip: graph.getReadOnlyResourceString("ReattachHelperTooltip"),
            isSource: true,
            isTarget: true,
            reattach: true,
            maxConnections: 1,
            dragOptions: { disabled: this.readOnly },
            connectorStyle: { lineWidth: 2, strokeStyle: '#bdbbbb' }
        };

        this.sourcePointTemplates = {
            switchCase: this.switchCaseSourcePointTemplate,
            timeout: this.timeoutSourcePointTemplate
        };

        // Redefinition of templates
        this.connectionTemplate = { lineWidth: 2, strokeStyle: '#1175ae' };

        // Redefines set timeout method to default one
        this.setTimeout = JsPlumbUserChoiceNode._superClass.constructor._superClass.setTimeout;
    },


    /*
    *   Returns node icon HTML representation.
    */
    getNodeIconClass: function () {
        return 'icon-choice-user-scheme';
    },

    /*
    *   Returns add case tooltip.
    */
    getAddCaseTooltip: function () {
        return this.graph.getReadOnlyResourceString("AddChoiceTooltip");
    },


    /*
    *   Returns delete case tooltip.
    */
    getDeleteCaseTooltip: function () {
        return this.graph.getReadOnlyResourceString("DeleteChoiceTooltip");
    },


    /*
    *   Returns edit case tooltip.
    */
    getEditCaseTooltip: function () {
        return this.graph.getReadOnlyResourceString("EditChoiceTooltip");
    },


    /*
    *   Returns case tooltip.
    */
    getCaseTooltip: function () {
        return this.graph.getReadOnlyResourceString(this.definition.TypeName + "Case", "SourcepointSwitchChoiceTooltip");
    },


    /*
    *   Returns maximum source point count message.
    */
    getMaxSourcePointCountError: function () {
        return this.graph.getReadOnlyResourceString("MaxChoiceSourcePointCountError");
    },


    /*
    *   Returns minimum source point count message.
    */
    getMinSourcePointCountError: function () {
        return this.graph.getReadOnlyResourceString("MinChoiceSourcePointCountError");
    },


    /*
    *   Returns case delete confirmation message.
    */
    getCaseDeleteConfirmation: function () {
        return this.graph.getReadOnlyResourceString("ChoiceDeleteConfirmation");
    }
});
