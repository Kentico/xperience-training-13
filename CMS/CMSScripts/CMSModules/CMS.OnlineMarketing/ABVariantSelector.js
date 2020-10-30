/**
 * A/B test variant selector module
 */
cmsdefine(['CMS/MessageService', 'CMS.OnlineMarketing/PopUpWindow', 'CMS/EventHub'], function (msgService, popUpWindow, hub) {
    var ABVariantSelector = function (data) {
        var popupContainer = document.getElementById(data.abTestPopupContainerId);
        var popupListing = document.getElementById(data.abTestVariantListingId);
        var variantSelectorButton = document.getElementById(data.variantSelectorButtonId);
        var addNewVariantButton = document.getElementById(data.addNewVariantButtonId);
        var manageABTestButton = document.getElementById(data.manageABTestButtonId);

        var promoteWinnerInfoMessageElem = document.getElementById(data.promoteWinnerInfoMessageId);

        // Checks whether CMS content have been changed
        var isContentUnchanged = function () {
            if (window.CMSContentManager) {
                var unchanged = window.CMSContentManager.checkChanges();
                return unchanged;
            } else {
                return true;
            }
        };

        // Opens modal dialog with the overview of the given A/B test
        var openManageABtestModalDialog = function () {
            var unchanged = isContentUnchanged();
            if (unchanged) {
                modalDialog(data.manageABTestUrl, 'ManageABTest', '95%', '95%');
                return false;
            }
        };

        if (manageABTestButton) {
            manageABTestButton.addEventListener('buttonClick', openManageABtestModalDialog);
        }

        if (promoteWinnerInfoMessageElem) {
            promoteWinnerInfoMessageElem.onclick = openManageABtestModalDialog;
        }

        // Performs async request only if all changes on the page are saved or there is no tracking for unsaved changes present
        var checkChangesAndPerformCallback = function (callbackArgument) {
            var unchanged = isContentUnchanged();
            if (unchanged) {
                variantSelectorCallback(JSON.stringify(callbackArgument));
            }
        };

        // Contacts server to create A/B Test with the first variant for current node
        var createABTestWithFirstVariant = function () {
            checkChangesAndPerformCallback({ actionName: data.addNewVariantAction });
            return false;
        };

        // Contacts server to add new A/B variant for current node
        var addNewVariant = function () {
            checkChangesAndPerformCallback({ actionName: data.addNewVariantAction });
            popUpWindow.toggle(popupContainer);
            return false;
        };

        var variantSelectorButtonClicked = function () {
            if (data.buttonModeOnly) {
                createABTestWithFirstVariant();
            } else {
                popUpWindow.toggle(popupContainer);
            }

            return false;
        };

        hub.subscribe(popUpWindow.closeEventName, function () {
            popupListing.closeEditor();
        });

        if (variantSelectorButton) {
            variantSelectorButton.onclick = variantSelectorButtonClicked;
        }

        if (addNewVariantButton) {
            addNewVariantButton.addEventListener('buttonClick', addNewVariant);
        }

        window.ABVariantSelectorModule = {
            abVariantSelectorCallbackReceived: function (callbackResult) {
                var result = JSON.parse(callbackResult);
                if (result.successMessage) {
                    msgService.showSuccess(result.successMessage, false);
                }

                if (result.nodeId > 0) {
                    window.SelectNode(result.nodeId);
                }
            },

            variantChosen: function (selectedItem) {
                var parameters = { actionName: data.changeVariantAction, variantIdentifier: selectedItem.id };
                checkChangesAndPerformCallback(parameters);
                popUpWindow.toggle(popupContainer);

                return false;
            },

            renameVariant: function (selectedItem, newName) {
                var parameters = { actionName: data.renameVariantAction, variantIdentifier: selectedItem.id, name: newName };
                variantSelectorCallback(JSON.stringify(parameters));

                // If current variant is selected, then text in the main button has to be changed as well
                if (data.currentVariantGuid === selectedItem.id) {
                    variantSelectorButton.firstChild.textContent = newName;
                }

                return false;
            },

            removeVariant: function (selectedItem) {
                var parameters = { actionName: data.removeVariantAction, variantIdentifier: selectedItem.id };
                variantSelectorCallback(JSON.stringify(parameters));

                if (data.currentVariantGuid === selectedItem.id) {
                    window.CMSContentManager.changed(false);
                }

                return false;
            }
        };
    };

    return ABVariantSelector;
});
