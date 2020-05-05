/**
* Modifies frame 'src' attribute and adds administration domain into it.
*/
cmsdefine(["CMS/UrlHelper", "CMS/CurrentUrlHelper", "CMS.Builder/Constants", "CMS/MessageService"], function (urlHelper, currentUrlHelper, constants, msgService) {

  // Module constructor
  var Module = function (serverData) {
    serverData = serverData || {};

    if (serverData.frameId && serverData.frameSrc && serverData.applicationPath && serverData.mixedContentMessage) {
      var frame = document.getElementById(serverData.frameId);

      if (frame) {
        var url = serverData.frameSrc;
        var applicationPath = serverData.applicationPath;
        var blankPage = "about:blank";

        if (urlHelper.isUrlSecure(currentUrlHelper.getCurrentUrl()) && !urlHelper.isUrlSecure(url)) {
            msgService.showWarning(serverData.mixedContentMessage, true);
            frame.setAttribute("src", blankPage);
            return;
        }

        if (!url.startsWith('/')) {
            url = urlHelper.addParameterToUrl(
                url,
                constants.ADMINISTRATION_DOMAIN_PARAMETER_NAME,
                urlHelper.getHostWithScheme(currentUrlHelper.getCurrentUrl())) + applicationPath;
        }
        frame.setAttribute("src", url);
      }
    }
  };

  return Module;
});