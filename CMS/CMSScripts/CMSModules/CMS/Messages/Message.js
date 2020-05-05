cmsdefine([], function () {
    /**
     * Message constructor function.
     * @param  text        string  text to be shown in the message.
     * @param  description string  description for the message. Might be long text.
     * @param  type        string  type of the message. Might be one of "error", "info", "warning", "success". 
     * @param  icon        string  icon for the message shown on the left side.
     */
    var Message = function (text, description, type, icon) {
        this.text = text;
        this.description = description;
        this.descriptionVisible = false;
        this.type = type;
        this.icon = icon;
    };

    /**
     * Shows or hides description text.
     */
    Message.prototype.toggleDescription = function () {
        this.descriptionVisible = !this.descriptionVisible;
    };

    return Message;
});