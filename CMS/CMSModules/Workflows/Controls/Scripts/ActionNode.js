
var JsPlumbActionNode = $class({

    Extends: JsPlumbStandardNode,

    constructor: function (readOnly, graph, definition) {
        JsPlumbStandardNode.call(this, readOnly, graph, definition);

        this.sourcePointTemplates = {
            standard: this.standardSourcePointTemplate
        };

        this.connectionTemplate = null;
    },


    /*
    *   Returns correct HTML representation of footer.
    */
    getFooterHtmlRepresentation: function () {
        return '';
    },


    /*
    *   Disables default functionality - actions does not support timeout.
    */
    setTimeout: function () { }
});