/** 
 * Module for contacting MVC authenticating endpoint.
 */

cmsdefine(["CMS/EventHub", "CMS.Builder/Constants", 'CMS.Builder/FrameLoader'], function (hub, constants, frameLoader) {

    var VirtualContextAuthenticator = function (options) {
        var authenticationFrameUrl = options.authenticationFrameUrl;
        var refreshInterval = options.refreshInterval;
        var mvcSignInToken = options.mvcSignInToken;

        loadAuthenticationFrame(authenticationFrameUrl, mvcSignInToken);

        setInterval(function () {
            // Refresh the MVC authentication. When completed, the method 'getAuthenticationFrameDataCallback' is called.
            raiseGetAuthenticationFrameDataCallback();
        }, refreshInterval * 1000);
    };


    var loadAuthenticationFrame = function (frameUrl, mvcSignInToken) {
        var frame = document.createElement('iframe');
        frame.style.display = 'none';
        frame.onload = function () {
            // Wait for the MVC frame to complete the authentication and set the authentication response cookie
            window.addEventListener('message', (event) => {
                var mvcFrameUrl = new URL(frameUrl);
                if (event.origin !== mvcFrameUrl.origin) {
                    return;
                }

                if (event.data === constants.AUTHENTICATED_MVC_FRAME_POST_MESSAGE) {
                    frame.parentNode.removeChild(frame);
                    var mvcFrameAuthenticatedEventName = frameLoader.getMvcFrameAuthenticatedEventName(frameUrl);
                    // Let the admin application know that the MVC authentication process for a specific culture has completed
                    hub.publish(mvcFrameAuthenticatedEventName);
                }
            }, { once: true });

            var urlObj = new URL(frameUrl);
            frame.contentWindow.postMessage({ key: constants.AUTHENTICATE_MVC_FRAME_POST_MESSAGE, token: mvcSignInToken }, urlObj.origin);
        };
        frame.src = frameUrl;
        document.body.appendChild(frame);
    }


    var getAuthenticationFrameDataCallback = function (callbackData) {
        // Parse the webforms callback data
        const separatorIndex = callbackData.indexOf(';');
        const frameUrl = callbackData.slice(0, separatorIndex);
        const mvcSignInToken = callbackData.slice(separatorIndex + 1);

        // Start the MVC authentication process by loading the MVC authentication iframe
        loadAuthenticationFrame(frameUrl, mvcSignInToken);
    }


    hub.subscribe(constants.ADMIN_FRAME_REQUEST_AUTHENTICATION_EVENT_NAME, function (data) {
        // Try to authenticate the MVC application for the specified culture prior switching the Pages application into this culture.
        // When completed, the method 'getAuthenticationFrameDataCallback' is called.
        raiseGetAuthenticationFrameDataCallback(data.culture);
    });


    window.CMS = window.CMS || {};
    window.CMS.VirtualContextAuthenticator = window.CMS.VirtualContextAuthenticator || {};
    window.CMS.VirtualContextAuthenticator.getAuthenticationFrameDataCallback = getAuthenticationFrameDataCallback;

    return VirtualContextAuthenticator;
});