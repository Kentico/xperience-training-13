function InitiatedChatWP(opt) {

    var options = {
        clientID: "",
        contentID: "",
        trans: "",
        windowURL: "",
        lblErrorID: "",
        pnlErrorID: "",
        guid:0,
        pingTick: 20000
    };
    options = $cmsj.extend(options, opt);

    var templateName = "initiatedChatTemplate_" + options.clientID,
        envelope = $cmsj("#" + options.clientID),
        content = $cmsj("#" + options.contentID),
        lblError = $cmsj("#" + options.lblErrorID),
        pnlError = $cmsj("#" + options.pnlErrorID),
        proxy = null,
        roomID = -1,
        msgLen = 0,
        timeout = null,
        lastChange = null,
        isVisible = false,
        lastRoomID = -1;

    if ((typeof (window.chat) !== 'undefined') && (chat != null) && (chat.IChatService != null)) {
        proxy = chat.IChatService;
    }
    else {
        return;
    }

    // Build jQuery template
    $cmsj.template(templateName, options.trans);

    envelope.hide();
    Clear();
    Ping();


    this.AcceptInitiatedChat = function (roomID) {
        if (lastRoomID > 0) {
            openWindow(lastRoomID);
        }
        else {
            CallWebService("AcceptChatRequest", function () {
                lastRoomID = roomID;
                openWindow(roomID);
            }, Error, [roomID]);
        }
    };


    this.RejectInitiatedChat = function (roomID) {
        CallWebService("DeclineChatRequest", Clear, Clear, [roomID]);
    };


    this.IsVisible = function () {
        return isVisible;
    };


    function openWindow(roomID) {
        var win = window.open(options.windowURL + "?windowroomid=" + roomID + "&popupSettingsId=" + options.guid, roomID, "width=600,height=800,location=0,scrollbars=1,resizable=1");
        if (win != null) {
            testWindow(win, 600, 800);
            Clear();
        }
        else {
            alert(ChatSettings.PopupWindowErrorMsg);
        }
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

    function Clear() {
        lastRoomID = -1;
        ShowHide(false, function () {
            msgLen = 0;
            lblError.empty();
            pnlError.hide();
            content.empty();
        });
    };


    function ShowHide(show, fn) {
        if (show) {
            envelope.slideDown(1000, fn);
            isVisible = true;
        }
        else {
            envelope.slideUp(1000, fn);
            isVisible = false;
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


    function Ping() {
        CallWebService("PingInitiate", function (response) {
            if (response != null) {
                ProcessResponse(response);
                lastChange = response.LastChange;
            }
            timeout = setTimeout(Ping, options.pingTick);
        }, function () {
            timeout = setTimeout(Ping, options.pingTick);
        }, [lastChange]);
    };


    function ProcessResponse(response) {
        if (response.IsRemoved && (response.RoomID == roomID)) {
            Clear();
        }
        else if ((response.RoomID != roomID) || (response.Messages != msgLen)) {
            roomID = response.RoomID;
            msgLen = response.Messages.length;
            if (msgLen > 0) {
                var msgs = [];
                for (var i = 0; i < msgLen; i++) {
                    msgs.push({
                        Text: response.Messages[i],
                        MessagesTemplate: true
                    });
                }
                response.Messages = msgs;
                RenderResponse(response);
            }
        }
    }


    function RenderResponse(response) {
        pnlError.hide();
        lblError.empty();

        if ((typeof(window.AutoInitiatedChatManager) !== 'undefined') && (window.AutoInitiatedChatManager != null)) {
            window.AutoInitiatedChatManager.Clear();            
        }
 

        var messages = $cmsj("<div></div>");
        $cmsj.tmpl(templateName, response.Messages).prependTo(messages)

        var chatInfo = {
            InitiatorName: response.InitiatorName,
            Accept: "window.InitiatedChatManager.AcceptInitiatedChat(" + response.RoomID + "); return false;",
            Reject: "window.InitiatedChatManager.RejectInitiatedChat(" + response.RoomID + "); return false;",
            Messages: messages.html()
        };
        content.empty();
        $cmsj.tmpl(templateName, chatInfo).prependTo(content);
        ShowHide(true);
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


function InitInitiatedChatManager(opt) {
    if ((typeof(window.InitiatedChatManager) === 'undefined') || (window.InitiatedChatManager == null)){
        window.InitiatedChatManager = new InitiatedChatWP(opt);
    }
}
