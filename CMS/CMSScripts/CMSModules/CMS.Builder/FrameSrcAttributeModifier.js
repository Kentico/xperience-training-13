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

        if (urlHelper.isUrlSecure(currentUrlHelper.getCurrentUrl()) && !urlHelper.isUrlSecure(url)) {
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

  return Module;
});