var JsPlumbNoteNode = $class({

    Extends: JsPlumbAbstractNode,

    constructor: function (readOnly, graph, definition) {
        JsPlumbAbstractNode.call(this, readOnly, graph, definition);

        this.targetPointTemplate = {
            endpoint: ["Blank"],
            isSource: false,
            isTarget: false,
            maxConnections: 0
        };
    },


    /*
    *   Returns node HTML representation.
    */
    getDefaultHtmlElement: function () {
        return $cmsj(`<div class="Node ${this.nodeStyle}" id="${this.parent.attr('id')}_${this.id}">
                        ${this.getContentHtmlRepresentation()}
                    </div>`);
    },


    /*
    *   Returns correct HTML representation of content.
    */
    getContentHtmlRepresentation: function () {
        return `<div class="content">
                    <div class="content-header">
                        ${this.getDeleteIconHtmlRepresentation(this.graph.getReadOnlyResourceString("DeleteNodeTooltip"))}
                        ${this.getDuplicateIconHtmlRepresentation(this.graph.getReadOnlyResourceString("DuplicateNodeTooltip"))}
                    </div>
                    ${this.getDescriptionHtml()}
                </div>`;
    },


    /*
    *   Returns the HTML representation of node description.
    */
    getDescriptionHtml: function() {
        var isNoteEmptyOrNull = !this.content || this.content.length === 0 || !this.content.trim();

        return `<div class="main">
                    <div class="inner">
                        <span class="Placeholder">
                            ${isNoteEmptyOrNull ? this.graph.getResourceString("EmptyNote") : ""}
                        </span>
                        <span class="Editable">${isNoteEmptyOrNull ? "" : graphControlHandler.ensureLineBreaks(this.content)}</span>
                    </div>
                </div>`;
    }
});
