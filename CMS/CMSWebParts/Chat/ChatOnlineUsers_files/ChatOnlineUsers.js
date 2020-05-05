// ChatOnlineUsers webpart  object
function ChatOnlineUsersWP(opt) {

    // Local variables
    var defaults = {
        onlineUserTemplate: "",
        chatRoomWindowUrl: "",
        contentClientID: "",
        clientID: "",
        GUID: "",
        inviteMode: false,
        pagingItems: -1,
        groupPagesBy: -1,
        pagingEnabled: false,
        pnlFilterClientID: "",
        btnFilter: "",
        txtFilter: "",
        filterEnabled: false,
        pnlPagingClientID: "",
        pnlInfo: "",
        resStrNoFound: "",
        resStrResults: "",
        loadingDiv: "",
        filterCount: -1,
        envelopeID: "",
        resStrNoOneInviteMode:"",
        groupID: "",
        invitePanel: ""
    };
    var that = this;

    
    // Public properties
    this.Options = $cmsj.extend(defaults, opt);
    this.GroupManager = null;
    this.Type = "OnlineUsers";
    this.ListPaging = null;

    var envelope = $cmsj("#" + that.Options.envelopeID),
        content = $cmsj("#" + that.Options.contentClientID),
        invitePanel = $cmsj("#" + that.Options.invitePanel),
        loadingBackup = "",
        templateID = "chatOnlineUsersTemplate" + that.Options.clientID;

    if (that.Options.inviteMode == true) {
        that.Type = "OnlineUsersInvite";
    }

    // Build jQuery template
    $cmsj.template(templateID, "<div class=\"ChatOnlineUser\">" + that.Options.onlineUserTemplate + "</div>");
  
  
    // Process response from server
    this.ProcessResponse = function(users) {

        // if invite mode activated, users in actualy joined room are deleted from list (local copy of array)
        if (that.Options.inviteMode == true) {
            if (that.GroupManager == null) {
                that.GroupManager = ChatManager.FindGroupManager(that.Options.groupID);
            }
            var group = that.GroupManager;
            if (group != null) {
                var finalList = new Array(),
                roomUsers = group.RoomUsersList,
                roomAllowAnonym = group.RoomInfo.AllowAnonym,
                roomPrivate = group.RoomInfo.IsPrivate;
                
                for (var i = 0; i < users.length; i++) {
                    var usr = users[i];
                    if ((roomUsers[usr.ChatUserID] == null) || (!roomPrivate && !roomUsers[usr.ChatUserID].IsOnline)) {
                        if (roomAllowAnonym || !usr.IsAnonymous) {
                            finalList.push(usr);
                        }
                    }
                }
                users = finalList;
            }
        }
        
        envelope.show();
        that.ListPaging.Render(users);
    };

    
    // Clears webpart content
    this.Clear = function() {
        content.empty();
        that.ListPaging.Clear();

        if (invitePanel.length > 0) {
            invitePanel.hide();
        }

        if (envelope.length > 0) {
            envelope.hide();
        }
        // Chat mega webpart - unregister webpart if not used (mega webpart is in search mode)
        else if (that.Options.inviteMode == false) {
            that.UNREGISTER = true;
        }
    }
    

    // Show or hide loading div
    this.ShowLoading = function(show) {
        if (show) {
            loadingBackup = content.html();
            content.html(that.Options.loadingDiv);
            envelope.show();
        }
        else {
            var loading = content.find(".ChatMessagesWPLoading");
            if (loading.length > 0)
            {
                content.html(loadingBackup);
            }
        }
    };
    
    
    // render users to webpart div
    function RenderUsers(users) { 
        // Empty panel with online users
        content.empty();

        // Extend information about users and fill the data to the panel
        for (var i = 0; i < users.length; i++) {
            var usr = users[i];
            usr.IsCurrentUser = (usr.ChatUserID == $cmsj.ChatManager.Login.UserID) ? true : false;
            if (that.Options.inviteMode == true) {
                if (usr.IsCurrentUser) {
                    continue;
                }
                usr.OnClick = "ChatManager.InviteToRoom(ChatManager.OpenedInvitePrompt.groupID, " + usr.ChatUserID + "); ChatManager.DialogsHelper.CloseDialog(ChatManager.OpenedInvitePrompt.overlay); return false;";
            }
            else {
                usr.OnClick = "ChatManager.InitOneToOneChat(" + usr.ChatUserID + ", '" + that.Options.chatRoomWindowUrl + "', '" + that.Options.GUID + "'); return false;";
            }
            $cmsj.tmpl(templateID, usr).appendTo(content);
        }
        // if there is no one to invite in invite mode
        if (that.Options.inviteMode == true) {
            var list = that.ListPaging.GetList();
            if ((list == null) || (list.length == 0)) {
                content.html(that.Options.resStrNoOneInviteMode);
                invitePanel.hide();
            } 
            else {
                invitePanel.show();
            }
        }
    };
    
    
    this.ListPaging = new ListPaging(
        {
            groupID: that.Options.groupID,
            clientID: that.Options.clientID,
            wpType: that.Type,
            functionRenderList: RenderUsers,
            filterPanelID: that.Options.pnlFilterClientID,
            filterPanelInfoID: that.Options.pnlInfo,
            filterNotFoundStr: that.Options.resStrNoFound,
            filterFoundStr: that.Options.resStrResults,
            filterEnabled: that.Options.filterEnabled,
            filterShowItems: that.Options.filterCount,
            filterTextboxID: that.Options.txtFilter,
            filterButtonID: that.Options.btnFilter,
            filterProperty: "Nickname",
            pagingEnabled: that.Options.pagingEnabled,
            pagingItems: that.Options.pagingItems,
            groupPagesBy: that.Options.groupPagesBy,
            pagingContentID: that.Options.pnlPagingClientID
        });
        
    this.Clear();
}


// Inits chat online users web part functionality
function InitChatOnlineUsersWebpart(opt) {
    InicializeChatManager();

    // Create chat online users object and add it to ChatManager
    ChatManager.RegisterWebpart(new ChatOnlineUsersWP(opt));
}

