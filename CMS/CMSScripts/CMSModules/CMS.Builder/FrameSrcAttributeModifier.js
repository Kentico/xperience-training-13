/**
* Modifies frame 'src' attribute and adds administration domain into it.
*/
cmsdefine([
    "CMS/UrlHelper",
    "CMS/CurrentUrlHelper",
    "CMS.Builder/Constants",
    "CMS/MessageService",
    "CMS.Builder/FrameLoader"
], function (urlHelper, currentUrlHelper, constants, msgService, frameLoader) {

  // Module constructor
  var Module = function (serverData) {
    serverData = serverData || {};
    var frameId = serverData.frameId;

    if (frameId && serverData.frameSrc && serverData.applicationPath && serverData.mixedContentMessage) {
      var frame = document.getElementById(frameId);

      if (frame) {
        var url = serverData.frameSrc;
        var applicationPath = serverData.applicationPath;

          if (isUrlSecure(url)) {
            msgService.showWarning(serverData.mixedContentMessage, true);
            frame.setAttribute("src", "about:blank");
            return;
        }

        if (!url.startsWith('/')) {
            url = urlHelper.addParameterToUrl(
                url,
                constants.ADMINISTRATION_DOMAIN_PARAMETER_NAME,
                urlHelper.getHostWithScheme(currentUrlHelper.getCurrentUrl())) + applicationPath;
        }

        frameLoader.loadFrame(frame, url);
      }
    }
    };

    function isUrlSecure(url) {
        // We are checking current url (admin url) if it's secured (https), 
        // because we need to prevent mixed content (https and http frame on the same page).
        if (!urlHelper.isUrlSecure(currentUrlHelper.getCurrentUrl())) {
            return false;
        }

        try {
            return !urlHelper.isUrlSecure(url);
        }
        catch {
            // Relative URLs indicate an admin page to be displayed.
            return true;
        }
    }

  return Module;
});