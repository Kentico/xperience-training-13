/** 
 * Module for contacting MVC authenticating endpoint.
 */

cmsdefine(["CMS/EventHub", "CMS.Builder/Constants", 'CMS.Builder/FrameLoader'], function (hub, constants, frameLoader) {

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
            var mvcFrameAuthenticatedEventName = frameLoader.getMvcFrameAuthenticatedEventName(frameUrl);
            hub.publish(mvcFrameAuthenticatedEventName);
        };
        frame.src = frameUrl;
        document.body.appendChild(frame);
    }


    hub.subscribe(constants.ADMIN_FRAME_REQUEST_AUTHENTICATION_EVENT_NAME, function (data) {
        raiseGetAuthenticationFrameUrlCallback(data.culture);
    });


    window.CMS = window.CMS || {};
    window.CMS.VirtualContextAuthenticator = window.CMS.VirtualContextAuthenticator || {};
    window.CMS.VirtualContextAuthenticator.loadAuthenticationFrame = loadAuthenticationFrame;

    return VirtualContextAuthenticator;
});