function ChatLeaveRoom(opt) {
    var defaults = {
        groupID: 0,
        clientID: -1,
        btnChatLeaveRoom: "",
        pnlContent: "",
        redirectURL: "",
        envelopeID: ""
    };
    
    var that = this;

    this.Type = "ChatLeaveRoom";
    this.GroupManager = null;
    this.Options = defaults;

    Inicialize(opt);
    
    var envelope = $cmsj(that.Options.envelopeID),
        pnlContent = $cmsj(that.Options.pnlContent);


    function Inicialize(opt) {
        if (typeof (opt) != 'object') {
            opt = {};
        }
        $cmsj.extend(that.Options, opt);

        // Leave button - set onclick function
        $cmsj(that.Options.btnChatLeaveRoom).click(function () {
            var fn = null;
            if (that.Options.redirectURL.length > 0) {
                fn = function() { self.window.location = that.Options.redirectURL; };
            }
            else {
                fn = function() { return; };
            }
            ChatManager.LeaveRoom(that.Options.groupID, fn);
            return false;
        });
    }


    this.Clear = function() {
        if (that.GroupManager.RoomID > 0) {
            envelope.show();
        }
        else {
            envelope.hide();
        }
    }
};

// Inits chat leave room web part functionality
function InitChatLeaveRoom(opt) {
    InicializeChatManager();
    ChatManager.RegisterWebpart(new ChatLeaveRoom(opt));
};