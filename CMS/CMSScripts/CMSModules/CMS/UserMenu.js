cmsdefine(['CMS/NavigationBlocker', 'jQuery'], function (NavigationBlocker, $) {

    var UserMenu = function(serverData) {
        var that = this,
            $userMenuWrapper = $(serverData.wrapperSelector),
            $myProfileLink;

        this.navigationBlocker = new NavigationBlocker();

        if ($userMenuWrapper) {
            $myProfileLink = $(serverData.checkChangesLinksSelector, $userMenuWrapper);
        } else {
            $myProfileLink = $(serverData.checkChangesLinksSelector);
        }

        $myProfileLink.on('click', function(e) {
            that.onMyProfileClick(e);
        });
    };

    // Prevent button default behavior when there are some
    // unsaved changes, otherwise do nothing.
    UserMenu.prototype.onMyProfileClick = function(e)
    {
        if (!this.navigationBlocker.canNavigate()) {
            e.preventDefault();
        };
    }

    return UserMenu;
});