var CMSUniMenu = {
    FxSpeed: 200,
    MenuDelay: 700,
    SubMenuTimes: {},
    SubMenus: {},

    SelectMenuButton: function (buttonName) {
        if (typeof (SelectButton) === 'function') {
            buttonName = buttonName.replace(".", "\\.");
            SelectButton($cmsj('div[name=' + buttonName + ']').get(0));
        }
    },

    ToogleSubMenu: function (button) {
        btn = $cmsj(button), subItems = btn.find('.SubMenuItems');

        btn.parents('.ui-layout-pane').first().css({ 'overflow': 'visible', 'z-index': '10' });

        var pos = btn.position();
        var border = subItems.outerWidth() - subItems.innerWidth();

        subItems.css({
            'position': 'absolute',
            'z-index': '11',
            'top': pos.top + btn.outerHeight(),
            'min-width': btn.innerWidth() - border
        });

        var subItemsWidth = subItems.outerWidth();
        var btnWidth = btn.outerWidth();
        var position;
        if ($cmsj(document.body).hasClass('RTL') && (subItems.parents('.LTR').length == 0)) {
            position = pos.left - subItemsWidth + btnWidth;
        } else {
            position = pos.left;
        }

        if (position < 0) {
            position = pos.left;
        }
        else if (position + subItemsWidth > $cmsj(window).width()) {
            position = pos.left - subItemsWidth + btnWidth;
        }

        subItems.css({ 'left': position });

        // Save sub menu container
        CMSUniMenu.SubMenus[button.id] = subItems;

        if (subItems.is(':hidden')) {
            CMSUniMenu.ShowSubMenu(button.id);
        }
        else {
            CMSUniMenu.HideSubMenu(button.id);
        }
    },

    ShowSubMenu: function (btnId) {
        // Bind enter/leave events
        $cmsj('#' + btnId).mouseleave(function () {
            if (!CMSUniMenu.SubMenus[btnId].is(':hidden')) {
                CMSUniMenu.SubMenuTimes[btnId] = setTimeout(function () { CMSUniMenu.HideSubMenu(btnId); }, CMSUniMenu.MenuDelay);
            }
        }).mouseenter(function () {
            if (!CMSUniMenu.SubMenus[btnId].is(':hidden')) {
                clearTimeout(CMSUniMenu.SubMenuTimes[btnId]);
            }
        });

        // Show sub menu
        CMSUniMenu.SubMenus[btnId].slideDown(CMSUniMenu.FxSpeed);

        // Relocate slider buttons
        var scrollers = $cmsj('.ForwardScroller, .BackwardScroller', CMSUniMenu.SubMenus[btnId]);
        scrollers.each(function () {
            var scrollerObj = $cmsj(this);
            var scrollerWidth = scrollerObj.width();
            var parentWidth = scrollerObj.parent().width();
            scrollerObj.css("left", (parentWidth / 2) - (scrollerWidth / 2));
            scrollPanelInit($cmsj('.ContextMenuContainer', CMSUniMenu.SubMenus[btnId]).attr('id'), true);
        });

    },

    HideSubMenu: function (btnId) {
        // Unbind current events
        btn = $cmsj('#' + btnId).unbind('mouseleave').unbind('mouseenter');
        // Hide sub menu
        CMSUniMenu.SubMenus[btnId].slideUp(CMSUniMenu.FxSpeed);
    },

    ChangeButton: function (button, text, iconUrl) {
        if (button) {
            var jButton = $cmsj(button);
            if (iconUrl) {
                jButton.find('img.MenuButtonImage').attr('src', iconUrl);
            }

            if (text && (text != '')) {
                jButton.find('span.MenuButtonText').text(text);
            }
        }
    }
}