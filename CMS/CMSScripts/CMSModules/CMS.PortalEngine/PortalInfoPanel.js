cmsdefine(['jQuery'], function ($) {
    return function (data) {
        // Registers function checking if page is shown in administration (parent is a CMSDesk page).
        var isCmsDesk = PM_IsCMSDesk();

        function PM_IsCMSDesk() {
            try {
                return (parent != null) && parent.IsCMSDesk;
            }
            catch (e) {
            }
            return true;
        }

        // Shows ViewModeInfoPanel if page is not shown in administration and preview mode is allowed in settings.
        if (!isCmsDesk) {
            if (!data.previewModePanelAllowed) {
                // Switches view mode to live site.
                window.location.href = data.liveSiteUrl;
            }

            var panelInfo = $("#" + data.clientID);
            panelInfo.css("display", "");

            var panelInfoHeight = panelInfo.outerHeight();
            $("#CMSHeaderPad").css({ 'height': panelInfoHeight + 'px' });
        }
    }
})
