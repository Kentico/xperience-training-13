function ChatSupportRequest(opt) {
    InicializeChatManager();

    var content = $cmsj("#" + opt.clientID);
    var proxy = ChatManager.WebService;
    if (proxy == null) {
        content.hide();
        return;
    }

    var requestTemplateB = "requestTemplateB_" + opt.clientID;
    $cmsj.template(requestTemplateB, opt.trans);

    CheckAvailability(null);

    // Define overlay for inform dialog
    ChatManager.DialogsHelper.SetDialogOverlay(opt.pnlInformDialog);

    // Set event handling for closing inform dialog
    $cmsj(opt.btnInformDialogClose).click(function () {
        ChatManager.DialogsHelper.CloseDialog(opt.pnlInformDialog);
        return false;
    });

    content.click(function () {
        // Before opening popup check support chat.
        CheckAvailability(function (online) {
            if (online) {
                // Open live support chat window.
                proxy.CreateSupportChatRoom(function (response) {
                    if (response.StatusCode == 0) {
                        var room = response.Data;
                        var win = window.open(opt.onlineUrl + "?windowroomid=" + room.ChatRoomID + "&popupSettingsId=" + opt.guid, room.ChatRoomID, "width=600,height=800,location=0,scrollbars=1,resizable=1");
                        if (win != null) {
                            testWindow(win);
                        }
                        else {
                            alert(ChatManager.Settings.PopupWindowErrorMsg);
                        }
                    } else {
                        alert(response.StatusMessage);
                    }
                }, function (err) {
                    alert(err.get_message());
                }, null);
            }
            else {
                // Open offline support request form.
                if (opt.mailEnabled) {
                    var win = window.open(ChatManager.Settings.SupportMailDialogURL, "OfflineSupportForm", ChatManager.Settings.SupportMailDialogSettings);
                    testWindow(win);
                }
                else {
                    ChatManager.DialogsHelper.DisplayDialog(opt.pnlInformDialog);
                }
            }
        });
    });


    function testWindow(win) {
        if (win == null || win === 'undefined') {
            alert(ChatManager.Settings.PopupWindowErrorMsg);
        }
        else {
            win.focus();
            if ($cmsj.browser.opera || $cmsj.browser.webkit) {
                setTimeout(function () {
                    if (!win.closed && (win.innerHeight <= 0) && (win.innerWidth <= 0)) {
                        alert(ChatManager.Settings.PopupWindowErrorMsg);
                    }
                }, 1000);
            }
        }
    }

    function CheckAvailability(callback) {
        proxy.GetSupportEngineersOnlineCount(function (response) {
            if (response.StatusCode == 0) {
                var online = (response.Data > 0);
                var data = {
                    LiveSupport: online,
                    EmailEnabled: opt.mailEnabled
                }

                content.empty();
                $cmsj.tmpl(requestTemplateB, data).appendTo(content);

                if (callback) {
                    callback(online);
                }
            } else {
                alert(response.StatusMessage);
            }
        }, function (err) {
            if (typeof (console) !== "undefined") {
                console.log(err.get_message());
            }
            setTimeout(CheckAvailability, 5000);
        }, null);
    }
}