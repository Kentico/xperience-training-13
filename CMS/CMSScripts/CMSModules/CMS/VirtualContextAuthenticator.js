/** 
 * Module for contacting MVC authenticating endpoint.
 */

cmsdefine(["CMS/EventHub", "CMS.Builder/Constants"], function (hub, constants) {

    var VirtualContextAuthenticator = function (options) {
        var authenticationFrameUrl = options.authenticationFrameUrl;
        var refreshInterval = options.refreshInterval;

        loadAuthenticationFrame(authenticationFrameUrl);

        setInterval(function () {
            raiseGetAuthenticationFrameUrlCallback();
        }, refreshInterval * 1000);
    };


    var loadAuthenticationFrame = function (frameUrl) {
        var frame = document.createElement('iframe');
        frame.style.display = 'none';
        frame.onload = function () {
            frame.parentNode.removeChild(frame);
            hub.publish(constants.MVC_FRAME_AUTHENTICATED_EVENT_NAME);
        };
        frame.src = frameUrl;
        document.body.appendChild(frame);
    }


    hub.subscribe(constants.ADMIN_FRAME_REQUEST_AUTHENTICATION_EVENT_NAME, function () {
        raiseGetAuthenticationFrameUrlCallback();;
    });


    window.CMS = window.CMS || {};
    window.CMS.VirtualContextAuthenticator = window.CMS.VirtualContextAuthenticator || {};
    window.CMS.VirtualContextAuthenticator.loadAuthenticationFrame = loadAuthenticationFrame;

    return VirtualContextAuthenticator;
});