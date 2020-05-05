/** 
 * Module for the MassActions control which handles actions performed on more than one items selected in the UniGrid.
 * 
 * Module attributes
 * @param {string} data.callbackTargetUniqueID - ID of the control which will receive callback call after user clicks 'OK' to perform mass action
 * @param {string} data.buttonClientID - ID of the OK button. Button which opens modal dialog or redirects
 * @param {string} data.scopeDropDownClientID - ID of the dropdown determining scope of the action (all/selected)
 * @param {string} data.actionDropDownClientID - ID of the dropdown determining mass action to perform
 * @param {string} data.selectedItemsClientID - ID of the html input (typically hidden field) whose value is set to ID's of items selected in the UniGrid. Values are separated by |
 * @param {string} data.messagePlaceHolderClientID - ID of the html element where error messages will be rendered
 * @param {string} data.noItemsSelectedMessage - message which will be displayed when no items are selected and scope is set to "selected"
 * @param {string} data.noActionSelectedMessage - message which will be displayed when no action is selected
 */
cmsdefine(["CMS/WebFormCaller", "Underscore", "jQuery"], function(webFormCaller, _, $) {

    // External underscore dependency
    var MassActions = function (data) {
            this.data = data;
            this.$scope = $("#" + data.scopeDropDownClientID);
            this.$action = $("#" + data.actionDropDownClientID);
            this.$uniGridHiddenField = $("#" + data.selectedItemsClientID);
            this.$messagePlaceHolder = $("#" + data.messagePlaceHolderClientID);
            this.data = data;

            var $button = $("#" + data.buttonClientID),
                that = this;

            $button.click(function() {
                that.handleButtonClick();
                return false;
            });
        };


    MassActions.prototype.handleButtonClick = function() {
        var selectedItems = _.compact(this.$uniGridHiddenField.val().split("|")),
            selectedScope = this.$scope.val();

        if (selectedItems.length === 0 && selectedScope !== "1") {
            this.$messagePlaceHolder.text(this.data.noItemsSelectedMessage);
            this.$messagePlaceHolder.removeClass("hidden");
            return;
        } else {
            this.$messagePlaceHolder.addClass("hidden");
        }

        var selectedAction = this.$action.val();

        if (selectedAction === "##noaction##") {
            this.$messagePlaceHolder.text(this.data.noActionSelectedMessage);
            this.$messagePlaceHolder.removeClass("hidden");
            return;
        } else {
            this.$messagePlaceHolder.addClass("hidden");
        }

        this.performMassAction(selectedScope, selectedAction, selectedItems);
    };

    MassActions.prototype.performMassAction = function(selectedScope, selectedAction, selectedItems) {
        var args = JSON.stringify({
                Scope: selectedScope,
                ActionCodeName: selectedAction,
                SelectedItems: selectedItems,
            }),
            success = function(result) {
                var resultObject = JSON.parse(result),
                    url = resultObject.url;

                switch (resultObject.type) {
                case 0: // Redirect
                    document.location.replace(url);
                    break;
                case 1: // Open modal
                    modalDialog(url, 'actionDialog', '90%', '85%');
                    break;
                }
            },
            doCallbackOptions = {
                targetControlUniqueId: this.data.callbackTargetUniqueID,
                args: args,
                successCallback: success
            };

        webFormCaller.doCallback(doCallbackOptions);
    };

    return MassActions;
});