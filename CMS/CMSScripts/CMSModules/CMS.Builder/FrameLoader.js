/**
* Loads the MVC iframe safely after the user authentication cookie is set.
*/
cmsdefine(["CMS.Builder/Constants", "CMS/EventHub"], function (constants, hub) {

    var getMvcFrameAuthenticatedEventName = function (url) {
        var urlObj = new URL(url);
        var host = urlObj.host;

        return constants.MVC_FRAME_AUTHENTICATED_EVENT_NAME_PREFIX + host;
    }

    var loadFrame = function (frameElement, url) {
        var mvcFrameAuthenticatedEventName = getMvcFrameAuthenticatedEventName(url);

        var authenticationCallback = function () {
            setFrameSource(frameElement, url);
            hub.unsubscribe(mvcFrameAuthenticatedEventName, authenticationCallback);
        }

        hub.subscribe(mvcFrameAuthenticatedEventName, authenticationCallback);
    };

    var setFrameSource = function (frameElement, url) {
        frameElement.src = url;
    }

    return {
        getMvcFrameAuthenticatedEventName,
        loadFrame
    };
});