var CMS = CMS || {};

CMS.Personas = CMS.Personas || {};

/**
* Personas module with logic used in PersonaContactListExtender class.
*/
CMS.Personas.PersonaContactsList = (function () {

    var module = {},
        contactDetailDialogBaseUrl = null; // actual dialog url is set from the server side code, so proper url including hash is generated using UIContextHelper

    /**
    * Shows pop-up dialog with contact details.
    *
    * @param {int} contactId ID of contact which details should be displayed.
    */
    module.showContactDetails = function (contactId) {
        if (contactDetailDialogBaseUrl == null) {
            // Do nothing if method 'setContactDetailDialogBaseUrl' wasn't called before showing contact details
            return;
        }

        modalDialog(contactDetailDialogBaseUrl + "&objectid=" + contactId, "ContactDetail", "95%", "95%");
    };


    /**
    * Sets the base url of the dialog showing contact detail. This method has to be called before showing any contact detail.
    *
    * @param {string} dialogBaseUrl Base url of the dialog page. ObjectID query string parameter will be appended to it.
    */
    module.setContactDetailDialogBaseUrl = function (dialogBaseUrl) {
        contactDetailDialogBaseUrl = dialogBaseUrl;
    };

    return module;
})();


/**
* Calls for postback of current page.
*
* This method is called within Close button click event in pop-up dialog. Since this is part of user control in another module,
* name of method cannot be modified. Thus the module pattern cannot be used and method has to be defined in global scope.
*/
function Refresh() {
    __doPostBack("", "");
}