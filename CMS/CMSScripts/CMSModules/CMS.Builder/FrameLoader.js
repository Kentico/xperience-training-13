/**
* Loads the MVC iframe safely after the user authentication cookie is set.
*/
cmsdefine(["CMS.Builder/Constants", "CMS/EventHub"], function (constants, hub) {

    var loadFrame = function (frameElement, url) {

        var authenticationCallback = function () {
            setFrameSource(frameElement, url);
            hub.unsubscribe(constants.MVC_FRAME_AUTHENTICATED_EVENT_NAME, authenticationCallback);
        }

        hub.subscribe(constants.MVC_FRAME_AUTHENTICATED_EVENT_NAME, authenticationCallback);
    };

    var setFrameSource = function (frameElement, url) {
        frameElement.src = url;
    }

    return {
        loadFrame
    };
});