/** 
 * Module for the Settings key edit control which handles change of the type and show the confirmation message.
 * 
 * Module attributes
 * @param {string} parameters.saveActionClass - CSS class of the OK button. Button that shows the confirmation message and saves the form
 * @param {string} parameters.keyTypeID - ID of the dropdown that holds the key types
 * @param {string} parameters.confirmMessage - Confirmation message to show
 * @param {string} parameters.initialKeyType - Initial key type
 */
cmsdefine(['jQuery'], function ($) {

    var SettingsKeyTypeConfirmation = function (parameters) {
        var that = this;
        this.parameters = parameters;
        this.saveActionButton = $('.' + parameters.saveActionClass);
        this.keyTypeInitialValue = parameters.initialKeyType;
        this.confirmMessage = parameters.confirmMessage;
        this.clickAction = this.saveActionButton.attr('onclick');

        $(function() {
            that.checkKeyTypeDropdownValue();
        });

        $('body').on('change', '#' + parameters.keyTypeID, function () {
            that.checkKeyTypeDropdownValue();
        });
    };

    SettingsKeyTypeConfirmation.prototype.checkKeyTypeDropdownValue = function() {
        if ($('#' + this.parameters.keyTypeID).val() !== this.keyTypeInitialValue) {
            this.saveActionButton.attr('onclick', "if (!confirm('" + this.confirmMessage + "')) { return false; }" + this.clickAction);
        } else {
            this.saveActionButton.attr('onclick', this.clickAction);
        }
    };

    return SettingsKeyTypeConfirmation;
});