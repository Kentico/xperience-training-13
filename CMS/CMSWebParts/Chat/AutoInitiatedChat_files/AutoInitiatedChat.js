function AutoInitiatedChatWP(opt) {

    var options = {
        wpGUID: "",
        clientID: "",
        contentID: "",
        trans: "",
        windowURL: "",
        lblErrorID: "",
        pnlErrorID: "",
        guid:0,
        messages:"",
        initiatorName:""
    };
    options = $cmsj.extend(options, opt);

    var templateName = "autoInitiatedChatTemplate_" + options.clientID,
        envelope = $cmsj("#" + options.clientID),
        content = $cmsj("#" + options.contentID),
        lblError = $cmsj("#" + options.lblErrorID),
        pnlError = $cmsj("#" + options.pnlErrorID),
        proxy = null,
        that = this,
        cookieName = "chat_autoinitchat_displayed_" + options.wpGUID;

    if ((typeof (window.chat) !== 'undefined') && (chat != null) && (chat.IChatService != null)) {
        proxy = chat.IChatService;
    }
    else {
        return;
    }

    // Build jQuery template
    $cmsj.template(templateName, options.trans);

    envelope.hide();
    pnlError.hide();
    setTimeout(RenderWebpart, options.delay);

    this.AcceptInitiatedChat = function () {
        CallWebService("CreateSupportChatRoomManual", function (room) {
            var win = window.open(options.windowURL + "?windowroomid=" + room.ChatRoomID + "&popupSettingsId=" + options.guid, room.ChatRoomID, "width=600,height=800,location=0,scrollbars=1,resizable=1");
            if (win != null) {
                testWindow(win, 600, 800);  
                that.Clear();
                SetCookie();
            }
            else {
                alert(ChatSettings.PopupWindowErrorMsg);
            }
        }, Error, [options.messages]);
    };


    this.RejectInitiatedChat = function () {
        that.Clear();
        SetCookie();
    };


    this.Clear = function() {
        ShowHide(false, function () {
            lblError.empty();
            pnlError.hide();
        });
    };


    function testWindow(win, width, height) {
        if (win == null || typeof (win) == 'undefined') {
            alert(ChatSettings.PopupWindowErrorMsg);
        }
        else {
            win.focus();
            if ($cmsj.browser.opera || $cmsj.browser.webkit) {
                setTimeout(function () {
                    if (!win.closed && (win.innerHeight != height) && (win.innerWidth != width)) {
                        alert(ChatSettings.PopupWindowErrorMsg);
                    }
                }, 1000);
            }
        }
    }


    function SetCookie() {
        // Do not create cookies when Essential cookies are not allowed
        if (ChatSettings.CurrentCookieLevel < 0) {
            return;
        }

        // set cookie, expiration == 60 min
        var exp = new Date();
        exp.setTime(exp.getTime() + 60 * 60 * 1000);
        $cmsj.cookie(cookieName, true, { expires: exp })
    }


    function CheckCookie() {
        if ($cmsj.cookie(cookieName)) {
            return true;
        }
        return false;
    }


    function ShowHide(show, fn) {
        if (show) {
            envelope.slideDown(1000, fn);
        }
        else {
            envelope.slideUp(1000, fn);
        }
    }

    function Error(err) {
        var message;
        if (err.StatusMessage) {
            message = err.StatusMessage;
        } else if (err.get_message) {
            message = err.get_message();
        } else {
            message = err;
        }
        lblError.text(message);
        pnlError.show();
    }


    function RenderWebpart() {              
        if (CheckCookie() || (window.InitiatedChatManager && window.InitiatedChatManager.IsVisible())) {
            return;
        }

        CallWebService("GetSupportEngineersOnlineCount", function (response) {
            if (response != null) {
                var messages = $cmsj("<div></div>");
                var msgs = [];
                for (var i = 0; i < options.messages.length; i++) {
                    msgs.push({ Text: options.messages[i], MessagesTemplate: true });
                }

                $cmsj.tmpl(templateName, msgs).prependTo(messages)

                var chatInfo = {
                    InitiatorName: options.initiatorName,
                    Accept: "window.AutoInitiatedChatManager.AcceptInitiatedChat(); return false;",
                    Reject: "window.AutoInitiatedChatManager.RejectInitiatedChat(); return false;",
                    Messages: messages.html()
                };
                $cmsj.tmpl(templateName, chatInfo).prependTo(content);
                ShowHide(true);
            }
        }, null, null);
    };


    function CallWebService(fn, fnOk, fnErr, args) {
        if (proxy == null) {
            return;
        }

        if (!args) {
            args = [];
        }

        args.push(function (response) {
            // 0 is OK
            if (response && (response.StatusCode == 0)) {
                if (fnOk != null) {
                    if (response.Data) {
                        fnOk(response.Data);
                    } else {
                        fnOk();
                    }
                }
            } else {
                if (fnErr) {
                    fnErr(response);
                }
            }
        });
        args.push(fnErr);
        args.push(null);
        proxy[fn].apply(proxy, args);
    }

};


function InitAutoInitiatedChat(opt) {
    if ((typeof(window.AutoInitiatedChatManager) === 'undefined') || (window.AutoInitiatedChatManager == null)){
        window.AutoInitiatedChatManager = new AutoInitiatedChatWP(opt);
    }
}
