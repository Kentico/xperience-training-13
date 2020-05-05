function ChatErrorsWP(opt) {

    var defaults = {
        errorTemplate: "",
        contentClientID: "",
        clientID: "",
        showDeleteAll: "",
        envelopeID: ""
    };
    
    var that = this;
    this.Options = defaults;
    this.ChatManager = null;
    this.Type = "ChatErrors";

    Inicialize(opt);

    var chatErrorTemplateB = "chatErrorTemplateB_" + this.Options.clientID,
        chatBtnTemplateB = "chatBtnTemplateB_" + this.Options.clientID,
        ID = 0,
        cont = $cmsj(that.Options.contentClientID),
        envelope = $cmsj(that.Options.envelopeID),
        getWebpart = "ChatManager.GetWebpart(null, '" + that.Options.clientID + "', 'ChatErrors')";
        
    // Build jQuery templates
    $cmsj.template(chatErrorTemplateB, this.Options.errorTemplate);
    $cmsj.template(chatBtnTemplateB, this.Options.showDeleteAll);
 

    this.Clear = function() {
        cont.empty();
        envelope.hide();
    };


    this.DeleteChatError = function(errorID) {
        $cmsj("#" + errorID).remove();
        if (cont == null) {
            return;
        }
        if (cont.html().search("ChatError_" + that.Options.clientID) == -1) {
            cont.empty();
            envelope.hide();
        }
    };


    this.ProcessResponse = function (response) {

        envelope.show();

        // Show button delete all
        if (that.Options.showDeleteAll.length > 0) {
            if (cont.html().search("ChatError_" + that.Options.clientID) == -1) {
                $cmsj.tmpl(chatBtnTemplateB, { "DeleteAll": getWebpart + ".Clear(); return false;" }).prependTo(that.Options.contentClientID);
            }
        }

        // Extend information about error
        var error = new Object();
        var errorID = "ChatError_" + that.Options.clientID + "_ID_" + ID++;
        error.Delete = getWebpart + ".DeleteChatError('" + errorID + "'); return false;";

        if (ChatManager.StatusCodes.NotJoinedInARoom == response.StatusCode) {
            var room = ChatManager.ChatRoomsList[response.RoomID];
            if ((room != null) && (room.DisplayName.length > 0)) {
                response.StatusMessage = room.DisplayName + ": " + response.StatusMessage;
            }
            response.StatusMessage += ". <a href=\"#\" onclick=\"ChatManager.JoinRoom('" + response.GroupID + "'," + response.RoomID + ",'', function(){" + error.Delete + "}); return false;\">" + ChatManager.Settings.NotJoinedInRoomErrLink + "</a>";
        }
        error.Message = response.StatusMessage;
        var errorDiv = $cmsj('<div id="' + errorID + '" class="ChatError"></div>');
        $cmsj.tmpl(chatErrorTemplateB, error).prependTo(errorDiv);
        cont.prepend(errorDiv);
    };
    
    
    function Inicialize(opt) {
        if (typeof (opt) != 'object') {
            opt = {};
        }
        that.Options = $cmsj.extend(that.Options, opt);
    }
    that.Clear();
}; 


// Start function
function InitErrorsWebpart(opt) {
    InicializeChatManager();
    ChatManager.RegisterWebpart(new ChatErrorsWP(opt));  
};