function ChatRoomUsersWP(opt) {

    // Local variables
    var defaults = {
        chatUserTemplate: "",
        sortByStatus: false,
        oneToOneURL: "",
        contentClientID: "",
        clientID: "",
        groupID: 0,
        roomID: 0,
        GUID: "",
        pnlChatRoomUsersInvitePrompt: "",
        pnlChatRoomUsersInvite: "",
        chatOnlineUsersElem: "",
        loadingDiv: "",
        btnChatRoomUsersInvite: "",
        btnChatRoomsDeletePromptClose: "",
        pagingItems: -1,
        groupPagesBy: -1,
        pagingEnabled: false,
        pnlFilterClientID: "",
        btnFilter: "",
        txtFilter: "",
        filterEnabled: false,
        pnlPagingClientID:"",
        pnlInfo: "",
        resStrNoFound: "",
        resStrResults: "",
        filterCount: -1,
        envelopeID: "",
        inviteSearchMode: false,
        chatSearchOnlineUsersElem: "",
        inviteEnabled: false
    };

    // Public properties

    this.Options = $cmsj.extend(defaults, opt);
    this.GroupManager = null; 
    this.Type = "ChatUsers";
    this.ChatOnlineUsersElem = null;
    this.ChatOnlineUsersElemEnabled = null;

    var that = this,
        chatUserTemplateB = "chatUserTemplateB_" + that.Options.clientID,
        content = $cmsj("#" + that.Options.contentClientID),
        envelope = $cmsj("#" + that.Options.envelopeID),
        pnlInvite = $cmsj("#" + that.Options.pnlChatRoomUsersInvite),
        pnlInvitePrompt = $cmsj("#" + that.Options.pnlChatRoomUsersInvitePrompt),
        lastSearch = "",
        loadingBackup = "";  
                                     

    // Clear webpart content
    this.Clear = function() {
        content.empty();
        envelope.hide();
        pnlInvite.hide();
        that.ListPaging.Clear();
    };


    // Show or hide loading div
    this.ShowLoading = function(show) {
        if (show == true) {
            loadingBackup = content.html();
            content.html(that.Options.loadingDiv);
        }
        else {
            content.html(loadingBackup);
        }
    };
  
  
    // Process respond from server
    this.ProcessResponse = function(users) {
        envelope.show();
        if (that.Options.sortByStatus) {
            users = sortByStatus(users);
        }
        that.ListPaging.Render(users);
    };
    

    function RenderUsers(users) {
          
        // Remember reference to online users WP (for inviting users to room)
        // Run this only once
        if (that.ChatOnlineUsersElemEnabled == null) {
            that.ChatOnlineUsersElem = ChatManager.GetWebpart(null, that.Options.chatOnlineUsersElem, "OnlineUsers");
            that.ChatOnlineUsersElemEnabled = true;
        }

        var isOneToOne = that.GroupManager.RoomInfo.IsOneToOne,
            isSupport = that.GroupManager.RoomInfo.IsSupport,
            isLoggedUserAdmin = that.GroupManager.RoomInfo.IsCurrentUserAdmin,
            roomID = that.GroupManager.RoomID,
            tempDiv = $cmsj("<div></div>");
            
        // Extend informations about online users
        for (var i = 0; i < users.length; i++) {
            var usr = users[i];
            var iconsCount = 0;

            // Admin levels
            //   0 == none
            //   1 == join room right
            //   2 == room admin
            //   3 == creator

            // Add permanent kick link if the room is private and current user has permission to do it
            if (isLoggedUserAdmin) {
                // Revoke acces when room is private
                if (that.GroupManager.RoomInfo.IsPrivate && !isSupport && !isOneToOne && (usr.AdminLevel < 2) && (usr.IsChatAdmin == false)) {
                    usr.KickUserPerm = "ChatManager.KickUser(" + roomID + "," + usr.ChatUserID + ",\"" + that.Options.groupID + '\", true, "' + that.Options.clientID + '"); return false;';
                    iconsCount++;
                }
                else {
                    delete usr.KickUserPerm;
                }

                // Add "Add admin" or "Delete admin" buttons if current user is admin of this room.
                if (!isSupport && !isOneToOne && (usr.IsChatAdmin == false)) {
                    if (usr.AdminLevel == 2) {
                        usr.DeleteAdmin = "ChatManager.DeleteAdmin(" + roomID + "," + usr.ChatUserID + ",\"" + that.Options.groupID + "\",'" + that.Options.clientID + "'); return false;";
                        iconsCount++;
                        delete usr.AddAdmin;
                    }
                    else if (usr.AdminLevel <= 1) {
                        usr.AddAdmin = "ChatManager.AddAdmin(" + roomID + "," + usr.ChatUserID + ",\"" + that.Options.groupID + "\",'" + that.Options.clientID + "'); return false;";
                        iconsCount++;
                        delete usr.DeleteAdmin;
                    }
                }
                
            } // End admin functions

            // Add admin info
            usr.IsAdmin = (!isSupport && !isOneToOne && ((usr.AdminLevel >= 2) || usr.IsChatAdmin));
            usr.IsCreator = (!isSupport && !isOneToOne && (usr.AdminLevel == 3));

            if (usr.IsOnline == true) {
            	if (!isOneToOne && !isSupport) {
                    usr.OneOnOneChat = "ChatManager.InitOneToOneChat(" + usr.ChatUserID + ", \"" + that.Options.oneToOneURL + "\", \"" + that.Options.GUID + "\"); return false;";
                }

                // Kick user
                if (isLoggedUserAdmin && !isSupport && !isOneToOne && (usr.AdminLevel < 2) && (usr.IsChatAdmin == false)) {
                    usr.KickUser = "ChatManager.KickUser(" + roomID + "," + usr.ChatUserID + ",\"" + that.Options.groupID + '\", false, "' + that.Options.clientID + '"); return false;';
                    iconsCount++;
                }
                else {
                    delete usr.KickUser;
                }
                
                // Is current user?
                if (ChatManager.Login.UserID == usr.ChatUserID) {
                    usr.IsCurrentUser = true;
                    usr.IsAdmin = (!isSupport && !isOneToOne && isLoggedUserAdmin);
                }
                else {
                    usr.IsCurrentUser = false;
                }
            } // end online users

            // If current user is not admin or actually processed user is current user
            if ((!isLoggedUserAdmin) || (ChatManager.Login.UserID == usr.ChatUserID)) {
                if (usr.KickUser) {
                    delete usr.KickUser;
                }
                if (usr.KickUserPerm) {
                    delete usr.KickUserPerm;
                }
                if (usr.AddAdmin) {
                    delete usr.AddAdmin;
                }
                if (usr.DeleteAdmin) {
                    delete usr.DeleteAdmin;
                }
                iconsCount = 0;
            }

            var userTemplate = $cmsj.tmpl(chatUserTemplateB, usr);
            var userName = userTemplate.find("div.ChatRoomUserName");
            userName.css("margin-right", (16 * iconsCount) + "px");
            userTemplate.appendTo(tempDiv);
        } // end for

        content.empty();
        content.html(tempDiv.html());

        // Check user's permitions for inviting
        var roomInfo = that.GroupManager.RoomInfo;
        if (that.Options.inviteEnabled && !roomInfo.IsOneToOne && (!roomInfo.IsPrivate || isLoggedUserAdmin)) {
            pnlInvite.show();
            if (!that.ChatOnlineUsersElemEnabled && (that.ChatOnlineUsersElem != null)) {
                ChatManager.RegisterWebpart(that.ChatOnlineUsersElem);
                that.ChatOnlineUsersElemEnabled = true;
            }
        }
        else {
            pnlInvite.hide();
            if ((that.ChatOnlineUsersElemEnabled == true) && (that.ChatOnlineUsersElem != null)) {
                ChatManager.UnregisterWebpart(null, that.ChatOnlineUsersElem.Options.clientID, that.ChatOnlineUsersElem.Type);
                that.ChatOnlineUsersElemEnabled = false;
            }
        }
    };


    function sortByStatus(users) {
        if (users == null){
            return [];
        }
        var online = new Array();
        var offline = new Array();
        for (var i = 0; i < users.length; i++) {
            var usr = users[i];
            if (usr.IsOnline) {
                online.push(usr);
            }
            else {
                offline.push(usr);
            }
        }
        return online.concat(offline);
    }


    function Inicialize() {
        // Build jQuery template
        $cmsj.template(chatUserTemplateB, that.Options.chatUserTemplate);

        // Define overlay for inviting users
        ChatManager.DialogsHelper.SetDialogOverlay(pnlInvitePrompt);

        // Set event handling for opening invite prompt
        $cmsj("#" + that.Options.btnChatRoomUsersInvite).click(function() {
            if (that.Options.inviteSearchMode == false) {
                if (ChatManager.LoadUsersToInvitePrompt(that.Options.chatOnlineUsersElem) == false) {
                    return;
                }
            }
            else {
                var wp = ChatManager.GetWebpart(null, that.Options.chatSearchOnlineUsersElem, "SearchOnlineUsers");
                if (wp != null) {
                    wp.Reset();
                }
            }

            ChatManager.OpenedInvitePrompt = {
                overlay: pnlInvitePrompt,
                groupID: that.Options.groupID
            };
            ChatManager.DialogsHelper.DisplayDialog(pnlInvitePrompt);
            return false;
        });

        // Set event handling for closing invite prompt
        $cmsj("#" + that.Options.btnChatRoomsDeletePromptClose).click(function() {
            ChatManager.DialogsHelper.CloseDialog(pnlInvitePrompt);
            return false;
        });
    }


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
        
    
    Inicialize();
    that.Clear();                                     
};


// Start function
function InitChatUsersWebpart(opt) {
    InicializeChatManager();
    ChatManager.RegisterWebpart(new ChatRoomUsersWP(opt));  
};