
/**  
 * @function callWebService   
 * @description Call webservice function and process response.  
 * @param fn Name of webservice function
 * @param fnOk Reference to function which will be called if response is successful
 * @param fnErr Reference to funciton which will be called if response is unsuccessfull   
 * @param args Array of arguments for webservice function         
 * @param obj Reference to object which called this function                                                        
*/ 
function callWebService(fn, fnOk, fnErr, args, obj) {
    if (ChatManager.WebService == null){
    	return;
    }
    
    if (!args || args == null) {
        args = [];
    }
	
	var group, roomid;
	if ((obj != null) && fn == "PingRoom"){
		group = obj.GroupID;
	    roomid = obj.RoomID;
	}
	args.push(function (response) {
	    // Response is OK or error while logging in (this error is solved in fnOk)
	    if (response.StatusCode == 0 || (fn == "Login" && ChatManager.StatusCodes.ChatUserNotFound == response.StatusCode)) {

	        // If there was service fail before, count ok response 
	        if (ChatManager.ServiceFails) {
	            ChatManager.ServiceOK++;

	            // If there are at least thrice more ok responses than error responses, let's say chat is working  
	            if ((ChatManager.ServiceOK / ChatManager.ServiceFails) > 3) {
	                ChatManager.ServiceOK = 0;
	                ChatManager.ServiceFails = 0;
	            }
	        }

	        if (fnOk != null) {
	            if (response.Data) {
	                fnOk(response.Data);
	            }
	            else {
	                fnOk();
	            }
	        }
	    }
	    else {
	        var wasLoggedIn = ChatManager.Login.IsLoggedIn  || ChatManager.Login.IsSupporter;
	        switch (response.StatusCode) {

	            // Not logged in, disable all webparts  
	            case ChatManager.StatusCodes.NotLoggedIn:
	            	
	                ChatManager.Login.SetProperties({ "IsLoggedIn": false, "IsAnonymous": true, "UserID": -1 });

	                if (wasLoggedIn){
		                if (ChatManager.IsLogoutRedirectSet()) {
		                    alert(response.StatusMessage);
		                }
		                else {
		                    ChatManager.GlobalError(response);
		                }
	                }
	                ChatManager.LoggedOut();

	                if (obj != null) {

	                    // Must UnlockPingMutex in that case.
	                    obj.UnlockPingMutex();
	                }
	                break;

	            // Not joined in room, clear room specific webparts   
	            case ChatManager.StatusCodes.NotJoinedInARoom:
	                if (obj != null) {
	                    if (roomid != obj.RoomID) {
	                        obj.UnlockPingMutex();
	                        break;
	                    }

	                    response.GroupID = group;
	                    response.RoomID = roomid;

	                    if (wasLoggedIn){
	                    	if (obj.IsLeaveRoomRedirectSituation(roomid)) {
	                        	alert(response.StatusMessage);
		                    }
		                    else {
		                        ChatManager.GlobalError(response);
		                    }
		                }

	                    // Must UnlockPingMutex in that case.
	                    obj.LeaveRoom(null, null, true, roomid);
	                    obj.UnlockPingMutex();
	                }
	                break;

	            // Kicked from room, clear room specific webparts  
	            case ChatManager.StatusCodes.KickedFromRoom:
	                if (obj != null) {

	                    if ((roomid != obj.RoomID) && (fn == "PingRoom")) {
	                        obj.UnlockPingMutex();
	                        break;
	                    }
						
						if (wasLoggedIn){
		                    // Save information about kick to cookie if Essential cookies are allowed
		                    if (ChatManager.Settings.CurrentCookieLevel >= 0) {
		                        var exp = new Date();
		                        exp.setTime(exp.getTime() + ChatManager.Settings.KickLastingInterval * 1000);
		                        $cmsj.cookie("chat_kick_roomid_" + roomid, "kicked", { expires: exp })
		                    }
	
		                    alert(response.StatusMessage);
		                }
	                    obj.LeaveRoom(null, null, true, roomid);

	                    // Must UnlockPingMutex in that case.
	                    obj.UnlockPingMutex();
	                }
	                break;

	            // Access denied  
	            case ChatManager.StatusCodes.AccessDenied:
	                if (obj != null) {

	                    if ((roomid != obj.RoomID) && (fn == "PingRoom")) {
	                        obj.UnlockPingMutex();
	                        break;
	                    }
						
						if (wasLoggedIn){
		                    // fnok, leavejoin, noCall, roomID
		                    if (obj.IsLeaveRoomRedirectSituation(roomid)) {
		                        alert(response.StatusMessage);
		                    }
		                    else {
		                        ChatManager.GlobalError(response);
		                    }
		                }
	                    obj.LeaveRoom(null, null, true, roomid);

	                    // Must UnlockPingMutex in that case.
	                    obj.UnlockPingMutex();
                        break;
	                }	                

	            // Other errors  
	            default:
	                if (ChatManager.Settings.Debug == true) {
	                    response.StatusMessage = fn + ": " + response.StatusMessage;
	                }
	                fnErr(response);
	        }
	        if (fn == "JoinRoom") {
	            obj.UnlockJoinRoomMutex();
	        }

	    }
	});
    
    // If webservice fails or network error, call GlobalError 
    args.push(function(response) {
        ChatManager.GlobalError(response, true);
        if (obj != null) {
            if (fn == "JoinRoom") {
                obj.UnlockJoinRoomMutex();
            }
            if (fn == "PingRoom") {
                obj.UnlockPingMutex();
            }
            if (fn == "Ping") {
                obj.UnlockPingMutex();
            }
        }
    });
    args.push(null);
    ChatManager.WebService[fn].apply(ChatManager.WebService, args);   
};




//  ROOM MANAGER OBJECT 
/**
 * @class Room Manager manages one group of "room specific webparts". 
 *        It can be joined in one room at one moment. Room manager is managed by Chat Manager object.
 *
 * @param chatMan Reference to ChatManager (only ChatManager can create this object)
 * @param grID ID of group which will be managed by this Room Manager                      
*/
function ChatGroupManager(chatMan, grID) {

    // PUBLIC PROPERTIES
    
    this.GroupID = grID;

    this.ChatManager = chatMan;

    this.Tick = (ChatSettings.RoomPingInterval) ? (ChatSettings.RoomPingInterval * 1000) : 5000;
    
    if (this.Tick < 1000){
    	this.Tick = 1000;
    } 
    else if (this.Tick > 30000){
    	this.Tick = 30000;
    }

    this.RoomID = -1;

    this.MsgCount = ChatSettings.FirstLoadMessagesCount;
     
    this.RoomInfo = null;
    
    this.WebpartsUsers = [];

    this.WebpartsMessages = [];

    this.WebpartsSenders = [];

    this.WebpartsRooms = [];

    this.WebpartsLeaveRooms = [];

    this.WebpartsRoomName = [];
    
    // ID of room which have to be joined after chat starts (get from webparts properties)
    this.RoomIDToJoin = -1;
     
    this.RoomIDToJoinCookie = -1;

	this.LeaveRoomURL = "";
	
	
    // LOCAL VARIABLES

    var that = this,
    
    msgLastChange = null,
    
    usrLastChange = null,
    
    // Reference to timeout which call next ping
    autoPingInterval = null,
    
    // Mutex for synchronization - indicates that ping was called and now waiting for response
    isCallingPing = false,
    isJoiningRoom = false,
    isSendingMessage = false;
    
    
    this.RoomUsersList = new Array();
    this.RoomMessagesList = new Array(); 


    // PUBLIC METHODS

    this.AddWebpart = function(obj, type) {

        // Register RM to webpart for two-way communication
        obj.GroupManager = this;

        switch (type) {
            case "ChatMessages":
                this.WebpartsMessages.push(obj);
                if (obj.Options.count >= 0) {
		            this.MsgCount = obj.Options.count;
		        }
		        if (msgLastChange == null){
		            msgLastChange = 0;
		        }
                break;
            case "ChatUsers":
                this.WebpartsUsers.push(obj);
                if (usrLastChange == null){
		        	usrLastChange = 0;
		        }
                break;
            case "ChatSender":
                this.WebpartsSenders.push(obj);
                if (usrLastChange == null){
		        	usrLastChange = 0;
		        }
                break;
            case "ChatLeaveRoom":
                this.WebpartsLeaveRooms.push(obj);
                if (that.LeaveRoomURL.length == 0){
                	that.LeaveRoomURL = obj.Options.redirectURL;
                }
                break;
            case "ChatRooms":
                this.WebpartsRooms.push(obj);
                break;
            case "ChatRoomName":
                this.WebpartsRoomName.push(obj);
                break;

            default:
            	if (ChatManager.Settings.Debug == true){
                	errorCon("Webpart type is not implemented!");
                }
                return;
        };
		obj.Clear();        

        // Join room if RoomID is a set property of webpart
        if ((obj.Options.roomID != null) && (obj.Options.roomID > 0)) {

            // First webpart with setted room id
            if (that.RoomIDToJoin <= 0) {
                that.RoomIDToJoin = obj.Options.roomID;
            }
            else {

                // If webparts in same group have setted different room id
                if (that.RoomIDToJoin != obj.Options.roomID) {
                    that.ChatManager.GlobalError(null, null, "Webparts in group with ID " + that.GroupID + " are related to different rooms!");
                }
            }
        }
    };


	this.LeaveRoomSynchronous = function(){
		if (that.RoomID <= 0) {
			return;
		}

		$cmsj.ajax({
			type: 'POST',
			async: false,
			url: that.ChatManager.Settings.ChatServiceUrl + '/LeaveRoom',
			dataType: 'JSON',
			data: '{"roomID":' + that.RoomID + '}',
			contentType: 'application/json; charset=utf-8'
		});

		that.LeaveRoom(null, null, true);
	}


    this.LeaveRoom = function(fnok, leaveJoin, noCall, roomID) {
        if ((that.RoomID <= 0) && (roomID == null)) {
            return;
        }
        
        if ((roomID == null) || (roomID <= 0)){
	        roomID = that.RoomID;
        }
        
        // Check if there is no other group joined to the room
        var groupManagersCount = that.ChatManager.ChatGroupManagers.length;
        if (groupManagersCount > 1){
	        for (var i = 0; i < groupManagersCount; i++){
	        	var gm = that.ChatManager.ChatGroupManagers[i];
	        	if ((gm.GroupID != that.GroupID) && (gm.RoomID == roomID)){
	        		noCall = true;
	        		break;
	        	}
	        }
        }
        
        // Stop pinging the room
        clearTimeout(autoPingInterval);
        
        if(noCall == true) {
        	leaveRoomSuccess(fnok, leaveJoin, roomID);
        }
        else {
        	callWebService("LeaveRoom", function() {
	            	leaveRoomSuccess(fnok, leaveJoin, roomID);          
	        	}, function(response) {
	            	that.ChatManager.GlobalError(response);
	            	autoPingInterval = setTimeout(that.PingWebService, that.Tick); 
	            }, [roomID]);	
        }
    };


    this.JoinRoom = function (roomID, psw, fnok, fnerr) {

        // If method is called without arguments, try to use default values from added webparts
        if ((roomID == null) && (psw == null)) {
            if (that.RoomIDToJoin > 0) {
                roomID = that.RoomIDToJoin;
                psw = "";
            }
            else if ((that.RoomIDToJoinCookie > 0) && ChatManager.Login.IsLogging) {
                roomID = that.RoomIDToJoinCookie;
                psw = "";
            }
        }

        // If roomID is not correctly set, do nothing
        if ((roomID <= 0) || (roomID == that.RoomID)) {
            return;
        }

        if (($cmsj.cookie("chat_kick_roomid_" + roomID) != null) && that.ChatManager.Login.IsAnonymous) {
            alert(that.ChatManager.Settings.KickMessage);
            return;
        }

        if (isJoiningRoom) {
            return;
        }
        isJoiningRoom = true;

        // Join room
        callWebService("JoinRoom", function (roomInfo) {
            isJoiningRoom = false;

            if (that.RoomID > 0) {
                that.LeaveRoom(null, true, false, that.RoomID);
            }
            that.RoomUsersList = new Array();
            that.RoomMessagesList = new Array();

            that.RoomID = roomID;
            initRoomVariables();
            that.ClearAllWebparts();
            that.RoomInfo = roomInfo;

            that.ChatManager.ShowLoading(that.WebpartsMessages, true);
            that.ChatManager.ShowLoading(that.WebpartsUsers, true);
            if (that.WebpartsUsers.length > 0) {
                that.ChatManager.ShowLoading(that.WebpartsRooms, true);
            }
            that.ChatManager.ShowLoading(that.WebpartsRoomName, true);

            for (var i = 0; i < that.WebpartsRoomName.length; i++) {
                that.WebpartsRoomName[i].ProcessResponse(roomInfo);
            }

            that.PingWebService();

            var roomObj = that.ChatManager.ChatRoomsList[that.RoomID];
            if ((roomObj != null) && (that.WebpartsUsers.length > 0)) {
                that.ChatManager.ProcessChatRooms(null, [{
                    RoomID: that.RoomID,
                    UsersCount: roomObj.OnlineUsersCount + 1
                }], true);
            } else {
                that.ChatManager.ProcessChatRooms(null, null, true);
            }

            // save information to cookies (for joining after refresh)
            createRoomCookies();

            if (that.WebpartsUsers.length > 0) {
                that.ChatManager.JoinedRoomsList[roomID] = roomInfo;
            }

            if (fnok != null) {
                fnok();
            } else if (!that.ChatManager.IsPopupWindow && (roomID != that.RoomIDToJoin) && (roomID != that.RoomIDToJoinCookie) && (that.ChatManager.Settings.RedirectURLJoin.length > 0)) {
                self.window.location = that.ChatManager.Settings.RedirectURLJoin + "?roomid=" + roomID;
            }

        }, function (response) {
            isJoiningRoom = false;
            if (fnerr != null) {
                fnerr(response.StatusMessage);
            } else {
                that.ChatManager.GlobalError(response);
            }
        }, [roomID, psw], that);
    };
    

    this.SendMessage = function(msg, obj) {
        sendMessageToRoom(null, msg, obj)
    };
    
    
    this.SendMessageToUser = function(userID, msg, obj) {
        sendMessageToRoom(userID, msg, obj)
    };


    this.ClearAllWebparts = function() {
        clearWebparts(that.WebpartsMessages);
        clearWebparts(that.WebpartsUsers);
        clearWebparts(that.WebpartsSenders);
        clearWebparts(that.WebpartsRoomName);
        clearWebparts(that.WebpartsLeaveRooms);
    };


    this.LoggedOut = function() {
        that.StopChat();
        that.RoomID = -1;
        that.ClearAllWebparts();
        initRoomVariables();
        
        deleteRoomCookies();
        
        clearWebparts(that.WebpartsRooms);
    };

    
    this.StopChat = function() {
    	if (autoPingInterval != null) {
        	clearTimeout(autoPingInterval);
        	that.UnlockPingMutex();
        }
    };
   

    this.PingWebService = function() {
    
        // Asynchronous ping == stop planned ping
        if (autoPingInterval != null) {
            clearTimeout(autoPingInterval);
        }
        
        // Processing ping, can not ping until ping will be fully processed
        if (isCallingPing) {
        
            // Try call ping again after 5 ms
            autoPingInterval = setTimeout(that.PingWebService, 5);
            return;
        }
        
        // Close mutex for pinging
        isCallingPing = true;
        callWebService("PingRoom", processPingResponse, function(response) {
        
            // Unlock mutex
            isCallingPing = false; 
            
            // Plan next ping on error
            autoPingInterval = setTimeout(that.PingWebService, that.Tick);
            that.ChatManager.GlobalError(response);
        }, [that.RoomID, usrLastChange, msgLastChange, that.MsgCount], that);
    };

                
    this.UnlockPingMutex = function() {
        isCallingPing = false;
    }; 
    
    this.UnlockJoinRoomMutex = function(){
    	isJoiningRoom = false;
    }


	this.ProcessRoomUsers = function(users){
		
		// Process response to room uses list
		if(users != null){
			for(var i = 0; i < users.length; i++) {
				var usr = users[i];
	
				// deleted user
				if(usr.IsRemoved == true) {
					delete that.RoomUsersList[usr.ChatUserID];
				}
				// new or modified
				else {
					that.RoomUsersList[usr.ChatUserID] = usr;
				}
			}
		}
		
		var sortUsers = new Array();
		var onlineCount = 0;
		for (var u in that.RoomUsersList) {
			if (!that.RoomUsersList.hasOwnProperty(u)) {
				continue;
			}
			sortUsers.push(that.RoomUsersList[u]);
			if (that.RoomUsersList[u].IsOnline == true){
				onlineCount++;
			}			
		}
		
		// Actualize users in room counter
		var room = that.ChatManager.ChatRoomsList[that.RoomID];
		if((room != null) && (onlineCount != room.OnlineUsersCount)) {
			that.ChatManager.ProcessChatRooms(null, [{RoomID: that.RoomID, UsersCount: onlineCount}], true);
		}

		// Sort on nickname
		that.ChatManager.SortObjectArray(sortUsers, "Nickname");

		for(var i = 0; i < that.WebpartsUsers.length; i++) {
			that.WebpartsUsers[i].ProcessResponse(sortUsers);
		}
		for(var i = 0; i < that.WebpartsSenders.length; i++) {
			that.WebpartsSenders[i].ProcessResponse(sortUsers);
		}
    };


	this.ProcessRoomMessages = function(msgs, notifyUser) {
		for (var i = 0; i < that.WebpartsMessages.length; i++) {
			that.WebpartsMessages[i].ProcessResponse(msgs);
		}
		if (notifyUser && that.RoomInfo.IsOneToOne && typeof (window.ChatNotificationManager) != "undefined") {
		    var otherUserMessages = false;
		    for (var i = 0; i < msgs.length; i++) {
		        if (msgs[i].AuthorID != that.ChatManager.Login.UserID) {
		            otherUserMessages = true;
		            break;
		        }
		    }
		    if (otherUserMessages) {
		        window.ChatNotificationManager.Notify("newmessage");
		    }
		}
	};
	
	
	this.IsLeaveRoomRedirectSituation = function(roomID){
		var redirectSet = (that.ChatManager.Settings.RedirectURLLeave.length > 0) || (that.LeaveRoomURL.length > 0);
		var roomAffected = (roomID == that.RoomID); 
		return (roomAffected && redirectSet);
	}
	
	
	this.PassRecepient = function(userID, dialogID) {
		var success = true;
		for(var i = 0; i < that.WebpartsSenders.length; i++) {
			success = that.WebpartsSenders[i].SetRecipient(userID);
		}
		if(!success) {
			ChatManager.DialogsHelper.DisplayDialog(dialogID);
		}
	}


    // PRIVATE METHODS
    
    function sendMessageToRoom(userID, msg, obj){
    	if (isSendingMessage || (msg.length <= 0)) {
            return;
        }
        isSendingMessage = true;
        callWebService((userID != null) ? "PostMessageToUser" : "PostMessage", function(response) {
            isSendingMessage = false;
            if (obj != null){
            	obj.Clear();
            }
            that.ProcessRoomMessages([response], false);
        }, function(response) {
        	isSendingMessage = false;
        	that.ChatManager.GlobalError(response);
        }, (userID != null) ? [that.RoomID, userID, msg] : [that.RoomID, msg], that);
    }
    
    
    function leaveRoomSuccess(fnok, leaveJoin, roomID) {    	
    	if(that.ChatManager.JoinedRoomsList[roomID] != null) { 
			delete that.ChatManager.JoinedRoomsList[roomID];
		}
		
    	// If this is callback of leave room, but user is already joined to another room, do nothing
    	if ((roomID != that.RoomID) && !leaveJoin){
    		return;
    	}

		// Try to close potencionaly opended room-specific dialogs
		ChatDialogs.TryCloseDialog(["ChatInviteDialogBody"]);

		// leaveJoin == true mean this leave is called after new room is successfully joined
		if(!leaveJoin) {
			that.RoomID = -1;
			that.ClearAllWebparts();

			that.RoomUsersList = new Array();

			deleteRoomCookies();
		}

		// If "users in room counter" in rooms webpart was faked by this group manager
		var roomObj = that.ChatManager.ChatRoomsList[roomID];
		if((roomObj != null) && (that.WebpartsUsers.length > 0)) {
			that.ChatManager.ProcessChatRooms(null, [{
				RoomID : roomID,
				UsersCount : roomObj.OnlineUsersCount - 1
			}], true);
		} else if(!leaveJoin) {
			that.ChatManager.ProcessChatRooms();
		}

		if(fnok != null) {
			fnok();
		}
		else if(!leaveJoin && !that.ChatManager.IsPopupWindow) {
			if(that.ChatManager.Settings.RedirectURLLeave.length > 0) {
				self.window.location = that.ChatManager.Settings.RedirectURLLeave;
			} else if(that.LeaveRoomURL.length > 0) {
				self.window.location = that.LeaveRoomURL;
			}
		}
	};


    function processPingResponse(response) {
        var adminChanged = false;
        if (response.IsCurrentUserAdmin != that.RoomInfo.IsCurrentUserAdmin){
        	that.RoomInfo.IsCurrentUserAdmin = response.IsCurrentUserAdmin;
        	var room = that.ChatManager.ChatRoomsList[that.RoomID];
        	if (room != null){
        		room.CanManage = response.IsCurrentUserAdmin;
        		that.ChatManager.ProcessChatRooms();
        	}
        	adminChanged = true;
        }
                  
        // Chat users have changed?
        if ((that.WebpartsUsers.length > 0) || (that.WebpartsSenders.length > 0)) {
			if (response.Users != null){
				usrLastChange = response.Users.LastChange;
				that.ProcessRoomUsers(response.Users.List);
			}
			else if (adminChanged){
				that.ProcessRoomUsers();
			}
		}
        
        // Chat messages have changed?
        if (that.WebpartsMessages.length > 0) {
        	if (response.Messages != null){
        	    var msgs = response.Messages;
				msgLastChange = msgs.LastChange;
				if ((msgs.List != null) && (msgs.List.length > 0)){
					that.ProcessRoomMessages(msgs.List, true);
				}
				else {
					that.ChatManager.ShowLoading(that.WebpartsMessages, false);
				}
			} 
			else if (msgLastChange == 0){
				that.ChatManager.ShowLoading(that.WebpartsMessages, false);
			}
			
			if (adminChanged){
				for (var i = 0; i < that.WebpartsMessages.length; i++){
					that.WebpartsMessages[i].AdminChanged();
				}
			}
        }
        
        // Unlock mutex for pinging
        isCallingPing = false;
        
        // Plan next ping (pings are planned after response recieved)
        autoPingInterval = setTimeout(that.PingWebService, that.Tick);
    };
    

    function clearWebparts(wp) {
        for (var i = 0; i < wp.length; i++) {
            wp[i].Clear();
        }
    };

    
    function initRoomVariables(){
        if (msgLastChange != null){
        	msgLastChange = 0;	
        }
        if (usrLastChange != null){
        	usrLastChange = 0;
        }
    };
    
    
    function createRoomCookies(){
    	// Do not create cookies when Essential cookies are not allowed
    	if (ChatManager.Settings.CurrentCookieLevel < 0){
    		return;
    	}
    	var windowName = that.ChatManager.GetWindowName();
		if(windowName.length > 0) {
			windowName += "_" + that.GroupID;
			$cmsj.cookie(windowName + "_roomID", that.RoomID, {
				expires : 4
			});
		}
    }
    
    
    function deleteRoomCookies(){
    	// Do not create cookies when Essential cookies are not allowed
    	if (ChatManager.Settings.CurrentCookieLevel < 0){
    		return;
    	}
		var windowName = that.ChatManager.GetWindowName();
		if(windowName.length > 0) {
			windowName += "_" + that.GroupID;
			$cmsj.cookie(windowName + "_roomID", null);
		}	
    }
    
    
    
    this.DEBUGPingRoom = function(roomID){
    	callWebService("PingRoom", null, function(response) {
            that.ChatManager.GlobalError(response);
        }, [roomID, 0, 0, 1], that);		
    }
    
    this.DEBUGStopPingRoom = function(){
    	if (autoPingInterval != null) {
            clearTimeout(autoPingInterval);
        }    	
    } 
    
    
}; 






//  LOGIN OBJECT
/**
 * @class Login object holds all data related to current logged user.  
 *        It also provides methods to control logging to chat.
 *
 * @param chatMan Reference to ChatManager (only ChatManager can create this object)
 */
function LoginObj(chMan) {

    // PUBLIC PROPERTIES

    this.UserID = 0;

    this.Nickname = "";

    this.IsLoggedIn = false;

    this.IsAnonymous = true;

    this.IsLogging = false;

    this.IsSupporter = false;

    this.ChatManager = chMan;

    this.WebpartsLogin = [];

    this.UserPermissions = {ManageRooms: false, CreateRooms: false};
    

    // LOCAL VARIABLES 
    
    var that = this;


    // PUBLIC METHODS


    /**  
     *  @public
     *  @function CheckState
     *  @description Checks current user login state and if user is not logged in, try to login. 
     */ 
    this.CheckState = function(clientID, nickname, fnok) {
        callWebService("GetChatUserState", function(result) {
            if (result.IsLoggedIn || that.ChatManager.IsSupportPopup) {
            	if (!that.ChatManager.ChatInicialized){
            		that.ChatManager.ChatInicialized = true;
            	}
                that.SetUserAfterLogin(result, fnok);
            }
           else {
                if (result.IsAnonymous == false) {
					that.SetProperties(result)
				}
				if (!that.ChatManager.ChatInicialized) {
					that.ChatManager.ChatInicialized = true;
					that.SetProperties({
						"IsLoggedIn" : false
					});
					that.ChatManager.PingWebserviceGlobal();
					return;
				}

				if (that.ChatManager.Settings.AnonymsAllowedGlobally || !result.IsAnonymous) {

					// Try to log in
					that.Register(clientID, nickname, fnok);
				} else {
					that.ChatManager.GlobalError(null, false, that.ChatManager.Settings.AnonymsNotAllowedGlobalyMsg);
				}
			}
        }, that.ChatManager.GlobalError, []);
    };


    this.ChangeNickname = function(nick, clientID, cancel) {
    	nick = handleNickname(nick);
        if (cancel || (nick == that.Nickname) || (nick.length == 0)) {
            displayToWPs();
            return;
        }
        callWebService("ChangeMyNickname", function(result) {
            // OK
			that.SetProperties(result);
			
            var gm = that.ChatManager.ChatGroupManagers;
            for (var i = 0; i < gm.length; i++){
            	if (gm[i].RoomID > 0){
            		var g = gm[i];
            		var usr = g.RoomUsersList[that.UserID];
            		if (usr != null){
            			usr.Nickname = that.Nickname;
            			g.ProcessRoomUsers(null);
            		}          		
            	}
            }
            // Online users
            var usr = that.ChatManager.OnlineUsersList[that.UserID];
            if (usr != null){
            	usr.Nickname = that.Nickname;
            	that.ChatManager.ProcessOnlineUsers(null);
            }
            
        }, function(result) {
        
            // On error
            
            // Find webpart and display error message
            for (var i = 0; i < that.WebpartsLogin.length; i++) {
                if (that.WebpartsLogin[i].Options.clientID == clientID) {
                    that.WebpartsLogin[i].Error(result);
                    return;
                }
            }
        }, [nick]);
    };

    

    this.Register = function(clientID, nickname, fnok) {
    	if((nickname != null) && ((nickname.length == 0) || (nickname == that.ChatManager.Settings.GuestPrefix))) {
			nickname = null;
		}
    	var params = [];
    	var fn = "RegisterGuest";
    	if (nickname != null){
    		nickname = handleNickname(nickname);
    		fn = "Register";
    		params.push(nickname);
    	}
		callWebService(fn, function(response) {
			that.SetUserAfterLogin(response, fnok);
		}, function(response) {
			that.ChatManager.GetWebpart(-1, clientID, "ChatLogin").Error(response);
		}, params);
	};


    this.DisplayChangeNicknameForm = function(clientID) {
    
        // Find webpart by clientID
        for (var i = 0; i < that.WebpartsLogin.length; i++) {
            if (that.WebpartsLogin[i].Options.clientID == clientID) {
                that.WebpartsLogin[i].DisplayChangeNicknameForm();
                return;
            }
        }
    };
    

    this.ErrorClose = function() {
        for (var i = 0; i < that.WebpartsLogin.length; i++) {
            that.WebpartsLogin[i].ErrorClose();
        }
    };

    
    this.Logout = function(fnok) {
		callWebService("Logout", function(response) {
			if (fnok != null){
				that.ChatManager.ClearWindowName();
				fnok();
			} 
			
			if (response == null){			
				that.IsLoggedIn = false;
				that.IsAnonymous = true;
				that.UserID = -1;
				that.Nickname = "";
				displayToWPs();
			} 
			else {
				that.SetProperties(response);
			}
			
			// Set all webparts to appropriate state
			that.ChatManager.LoggedOut(fnok);
		}, that.ChatManager.GlobalError, []);
	};


    this.SetProperties = function(result) {
        if (result.ChatUserID != null) {
            that.UserID = result.ChatUserID;
        }
        if (result.Nickname != null) {
            that.Nickname = result.Nickname;
        }
        if (result.IsLoggedIn != null) {
            that.IsLoggedIn = result.IsLoggedIn;
        }
        if (result.IsAnonymous != null) {
            that.IsAnonymous = result.IsAnonymous;
        }
        displayToWPs();
    };
    

    this.SetUserAfterLogin = function(result, fnok) {
        that.SetProperties(result);
        var startingChat = that.IsLogging;
         
        that.GetUserPermissions(function() {
        	
        	var fnStartChat = null;
            
            // This is first login after page load
            if (that.IsLogging) {
            	fnStartChat = function(){
                	that.ChatManager.StartChat();
                	that.IsLogging = false;
                }
            }
            
            // Force ping and update chat rooms
            that.ChatManager.PingWebserviceGlobal(true, fnStartChat);
            
         });
        
        that.ChatManager.SetWindowName(false);
    	
    	if (fnok != null){
    		fnok();
    	}
    	
    	that.ChatManager.LoginLogoutClearWP();
        
        var gms = that.ChatManager.ChatGroupManagers;
        for (var i = 0; i < gms.length; i++) {
        	var gm = gms[i];
        	if (!startingChat){
        		gm.JoinRoom(null, null);
        	}
        	for (var j = 0; j < gm.WebpartsRoomName.length; j++) {
        		gm.WebpartsRoomName[j].Clear(); 
        	}	 
        }
    };

    
    this.GetUserPermissions = function(callbackFn) {
    	
        // Disable all rights when not logged in 
        if ((that.UserID <= 0) && (!that.IsLoggedIn)) {
            setUserPermissions({}, false);
        }
        else {
            callWebService("GetPermissions", function(response) {
                setUserPermissions(response);
                
                // If there is specified callback function
                if (callbackFn) {
                    callbackFn();
                }
            }, that.ChatManager.GlobalError, []);
        }
    };
    
    
    // PRIVATE METHODS 
    
    function setUserPermissions(permissions, force) {
        that.UserPermissions.ManageRooms = (permissions.ManageRooms || force);
        that.UserPermissions.CreateRooms = (permissions.CreateRooms || force);
    };

    
    function displayToWPs() {
        for (var i = 0; i < that.WebpartsLogin.length; i++) {
            that.WebpartsLogin[i].DisplayInfo();
        }
    };
    
    
    function handleNickname(nick){
    	if (nick != null){
    	    nick = $cmsj.trim(nick.replace(/\s+/g, " "));
    	}
    	return nick;
    }
    
};






function InicializeChatManager(){
	return new ChatManagerObj();
}


//  CHAT MANAGER OBJECT 
/**
 * @class Chat Manager is the top control layer. It manages whole chat, every Room Manager is controlled by Chat Manager.
 *        All webparts must be registered to chat using Chat Manager's method RegisterWebpart. 
 *        Chat Manager is global object and can be created only once (singleton pattern).
 *
 * @param stopRecursion If setted to true, stop recursion when creating new instance of object  
 */
function ChatManagerObj(stopRecursion) {

    // There can be only one instance of ChatManager
    if ($cmsj.ChatManager != null) {
        return $cmsj.ChatManager;
    }
    else if (!stopRecursion){
        $cmsj.ChatManager = new ChatManagerObj(true);
        window.ChatManager = $cmsj.ChatManager;
        return $cmsj.ChatManager;
    }

	
	// PUBLIC PROPERTIES

    this.ChatGroupManagers = [];

    this.WebpartsNotification = [];

    this.WebpartsErrors = [];

    this.WebpartsOnlineUsers = [];
    
    this.WebpartsOnlineUsersInvite = [];
    
    this.WebpartsSearchOnlineUsers = [];
    
    this.WebpartsChatWP = [];

    this.WebService = null;
	if ((typeof(window.chat) !== 'undefined') && (chat != null) && (chat.IChatService != null)){
		this.WebService = chat.IChatService;
	}

    this.Login = new LoginObj(this);

    this.StatusCodes = { 
        "OK" : 0,
        "NotLoggedIn" : 1,
        "AccessDenied" : 2,
        "MessageCanNotBeEmpty" : 3,
        "NotJoinedInARoom" : 4,
        "UnknownError" : 5,
        "ChatUserNotFound" : 6,
        "WrongSecondUser" : 7,
        "WrongDisplayName" : 8,
        "NicknameNotAvailable" : 9,
        "SupportIsNotOnline" : 10,
        "RoomNotFound" : 11,
        "WrongPassword" : 12,
        "AnonymsDisallowed" : 13,
        "BadWordsValidationFailed" : 14,
        "BadRequest" : 15,
        "BannedIP" : 16,
        "KickedFromRoom" : 17,
        "InvitationAlreadyAnswered": 31
    };

    /** @property ServiceFails Counter of service or connection fails.*/ 
    this.ServiceFails = 0;
    
    /** @property ServiceOK Counter of service ok responses.*/
    this.ServiceOK = 0;

	this.Settings = ChatSettings;
	
	this.ChatInicialized = false;
	
	this.OnlineUsersList = new Array();
	
	this.ChatRoomsList = new Array();
	
	this.JoinedRoomsList = new Array();
	
	this.NotificationsList = new Array();
	
	this.LogoutChatURL = "";

	this.BBCodeParser = window.ChatBBCodeParserObj ? new ChatBBCodeParserObj() : null;

	this.SmileysResolver = window.ChatSmileysResolverObj ? new ChatSmileysResolverObj(this.Settings.SmileysPath) : null;
	
	this.DialogsHelper = window.ChatDialogsObj ? new ChatDialogsObj() : null;
	
	this.ActiveNotificationWP = null;
	
	this.IsSupportPopup = $cmsj("div.ChatPopupWindow.IsOneToOne.IsSupport").length > 0;

	this.IsPopupWindow = false;

    // LOCAL VARIABLES


    var that = this,

    roomsLastChange = null,
    
    roomsUsersCountLastChange = null,

    notificationLastChange = null,

    onlineUsersLastChange = null,
    
    onlineUsersInviteLastChange = null,

    // Reference to timeout which call next ping
    roomsPingInterval = null,

    // Interval between central pings to webservice [ms]
    roomsRefreshRate = (ChatSettings.GlobalPingInterval) ? (ChatSettings.GlobalPingInterval * 1000) : 20000,
    
    // Indicates if some chat rooms webpart is present on the page
    isChatRoomsWP = false,
    
    // Mutex for synchronization - indicates that ping response is processing and now waiting for response
    isProcessingPing = 0,

    // Mutex for synchronization - indicates that ping was called and now waiting for response
    isCallingPing = false;
    
    
    if (roomsRefreshRate < 1000){
    	roomsRefreshRate = 1000;
    } 
    else if (roomsRefreshRate > 60000){
    	roomsRefreshRate = 60000;
    }


    // PUBLIC METHODS 


    this.RegisterWebpart = function(obj) {
    	
    	if (obj.UNREGISTER != null){
    		return;
    	}
    
        // First registering webpart - get setting and log in to system
        if (!that.ChatInicialized && (that.WebService != null)) {
        
            // If mutex is not closed
            if (!that.Login.IsLogging) {
            	
            	// Wait until settings are loaded 
            	if((that.Settings = ChatSettings) == null){
            		setTimeout(function(){that.RegisterWebpart(obj);}, 5);
            		return;
            	}
            
                // Close mutex
                that.Login.IsLogging = true;
                
                // Start logging and after login start pinging
                that.Login.CheckState();                
            }
        }
        
        // Register webpart
        switch (obj.Type) {
            case "ChatLogin":
                that.Login.WebpartsLogin.push(obj);
                if (that.LogoutChatURL.length == 0){
                	that.LogoutChatURL = obj.Options.redirectURLLogout;
                }
                break;
            case "ChatNotification":
                that.WebpartsNotification.push(obj);
                if (notificationLastChange == null){
                	notificationLastChange = 0;
                }
                break;
            case "ChatErrors":
                that.WebpartsErrors.push(obj);
                break;
            case "OnlineUsers":
                that.WebpartsOnlineUsers.push(obj);
	            if (onlineUsersLastChange == null){
	               	onlineUsersLastChange = 0;
	            }
                break;
            case "OnlineUsersInvite":
            	that.WebpartsOnlineUsersInvite.push(obj);
                if (onlineUsersInviteLastChange == null){
	              	onlineUsersInviteLastChange = 0;
	            }
            	break;
            case "SearchOnlineUsers":
                that.WebpartsSearchOnlineUsers.push(obj);
                break;   
            case "ChatWebpart":
            	that.WebpartsChatWP.push(obj);
            	break;
            case "ChatRooms":
                isChatRoomsWP = true;
                if (roomsLastChange == null){
                	roomsLastChange = 0;
                }
                if (roomsUsersCountLastChange == null){
                	roomsUsersCountLastChange = 0;
                }
                
                // No break! Chat rooms are Room Manager's webpart.
            default:
            
                //Room manager webparts will be added to GroupManager
                registerRMWebpart(obj, obj.Type, obj.Options.groupID);
                break;
        }
    };

    
    this.JoinRoom = function(groupID, roomID, psw, fnok, fnerr) {
        var groupManager = that.FindGroupManager(groupID); 
        if (groupManager == null) {
            return false;
        }
        groupManager.JoinRoom(roomID, psw, fnok, fnerr);
    };


    this.LeaveRoom = function(groupID, fnok) {
        var groupManager = that.FindGroupManager(groupID);
        if (groupManager == null) {
            return false;
        }
        groupManager.LeaveRoom(fnok);
    };


    this.LeaveRoomPermanently = function(roomID) {

        // Call webservice
        callWebService("LeaveRoomPermanently", function() {
            
            // Check if user is joined to room
            for (var i = 0; i < that.ChatGroupManagers.length; i++){
				if (roomID == that.ChatGroupManagers[i].RoomID){
					that.ChatGroupManagers[i].LeaveRoom();
				}
            }
            
            delete that.ChatRoomsList[roomID];
            that.ProcessChatRooms();
                        
        }, function(response) {
            that.ChatManager.GlobalError(response);
        }, [roomID]);
    };

    
	this.GetWebpart = function(groupID, clientID, type, groupWP) {
        var rm = null;
        if ((groupID != null) && (groupID.length > 0)) {
            rm = that.FindGroupManager(groupID);
            if (rm == null) {
                return null;
            }
        }
        else if (groupWP){        
			var wp = null;
			for(var i = 0; i < that.ChatGroupManagers.length; i++) {
				rm = that.ChatGroupManagers[i];
				switch (type) {
					case "ChatRooms":
						wp = findWebpart(clientID, rm.WebpartsRooms);
						break;
					case "ChatLeaveRoom":
						wp = findWebpart(clientID, rm.WebpartsLeaveRooms);
						break;
					case "ChatUsers":
						wp = findWebpart(clientID, rm.WebpartsUsers);
						break;
					case "ChatMessages":
						wp = findWebpart(clientID, rm.WebpartsMessages);
						break;
				}
				if(wp != null) {
					return wp;
				}
			}
			return null;
		}

        switch (type) {
            case "ChatRooms":
                return findWebpart(clientID, rm.WebpartsRooms);
                break;
            case "ChatLeaveRoom":
                return findWebpart(clientID, rm.WebpartsLeaveRooms);
                break;
            case "ChatUsers":
                return findWebpart(clientID, rm.WebpartsUsers);
                break;
            case "ChatMessages":
            	return findWebpart(clientID, rm.WebpartsMessages);
            	break;
            case "ChatNotification":
                return findWebpart(clientID, that.WebpartsNotification);
                break;
            case "ChatErrors":
                return findWebpart(clientID, that.WebpartsErrors);
                break;
            case "OnlineUsers":
                return findWebpart(clientID, that.WebpartsOnlineUsers);
                break;
            case "OnlineUsersInvite":
            	return findWebpart(clientID, that.WebpartsOnlineUsersInvite);
            	break;
            case "SearchOnlineUsers":
                return findWebpart(clientID, that.WebpartsSearchOnlineUsers);
                break;    
            case "ChatLogin":
            	return findWebpart(clientID, that.Login.WebpartsLogin);
            	break;
            default:
                return null;
        }
    };
    
    
    this.UnregisterWebpart = function(groupID, clientID, type) {
        var rm = null;
        if (groupID != null) {
            rm = that.FindGroupManager(groupID);
            if (rm == null) {
                return false;
            }
        }
        switch (type) { 
            case "ChatRooms":
                rm.WebpartsRooms(findWebpart(clientID, rm.WebpartsRooms, true));
                break;
            case "ChatLeaveRoom":
                rm.WebpartsLeaveRooms.splice(findWebpart(clientID, rm.WebpartsLeaveRooms, true));
                break;
            case "ChatUsers":
                rm.WebpartsUsers.splice(findWebpart(clientID, rm.WebpartsUsers, true));
                break;
            case "ChatNotification":
                that.WebpartsNotification.splice(findWebpart(clientID, that.WebpartsNotification, true));
                break;
            case "ChatErrors":
                that.WebpartsErrors.splice(findWebpart(clientID, that.WebpartsErrors, true));
                break;
            case "OnlineUsers":
                that.WebpartsOnlineUsers.splice(findWebpart(clientID, that.WebpartsOnlineUsers, true));
                if (that.WebpartsOnlineUsers.length == 0){
                	onlineUsersLastChange = null;
                }
                break;
            case "SearchOnlineUsers":
                that.WebpartsSearchOnlineUsers.splice(findWebpart(clientID, that.WebpartsSearchOnlineUsers, true));
                break;    
            default:
                return false;
        }
        return true;
    };


	this.LoginLogoutClearWP = function(){
		for(var i = 0; i < that.WebpartsNotification.length; i++) {
			that.WebpartsNotification[i].Clear();
			that.WebpartsNotification[i].ShowHide();
		}
		for(var i = 0; i < that.WebpartsOnlineUsers.length; i++) {
			that.WebpartsOnlineUsers[i].Clear();
		}
		for(var i = 0; i < that.WebpartsSearchOnlineUsers.length; i++) {
			that.WebpartsSearchOnlineUsers[i].Clear();
		}
		for (var i = 0; i < that.WebpartsChatWP.length; i++){
        	that.WebpartsChatWP[i].Clear();
        }
	}


	this.LoggedOut = function (fnok) {

	    // Try to close all potentionaly opened dialogs
	    ChatDialogs.TryCloseDialog(["ChatDialogBody"]);

	    // Reset all lastchange variables
	    if (roomsLastChange != null) {
	        roomsLastChange = 0;
	        that.ChatRoomsList.length = 0;
	    }
	    if (roomsUsersCountLastChange != null) {
	        roomsUsersCountLastChange = 0;
	    }
	    if (onlineUsersLastChange != null) {
	        onlineUsersLastChange = 0;
	        that.OnlineUsersList.length = 0;
	    }
	    if (notificationLastChange != null) {
	        notificationLastChange = 0;
	    }

	    that.LoginLogoutClearWP();

	    // Propagate to all group managers
	    for (var i = 0; i < that.ChatGroupManagers.length; i++) {
	        that.ChatGroupManagers[i].LoggedOut();
	    }

	    that.ClearWindowName();

	    that.JoinedRoomsList.length = 0;

	    if (typeof (window.ChatNotificationManager) != "undefined") {
		    window.ChatNotificationManager.StopNotify("newnotification");
	    }

	    if (fnok != null) {
	        fnok();
	    }
	    else if (!that.IsPopupWindow) {
	        if (that.Settings.RedirectURLLogout.length > 0) {
	            self.window.location = that.Settings.RedirectURLLogout;
	        }
	        else if (that.LogoutChatURL.length > 0) {
	            self.window.location = that.LogoutChatURL;
	        }
	    }

	};


    this.PingWebserviceGlobal = function(force, fnok) {
    
        // Clear planned ping (if asynchronous ping)
        if (roomsPingInterval != null) {
            clearTimeout(roomsPingInterval);
        }
        
        // Processing ping, can not ping until ping will be fully processed
        if (isProcessingPing || isCallingPing) {
            var callback = function() { that.PingWebserviceGlobal(force); };
            
            // Try call ping again after 5 ms
            roomsPingInterval = setTimeout(callback, 5);
            return;
        }
        
        // Close mutex for calling ping
        isCallingPing = true;
        
        var params = (that.Login.IsLoggedIn == true) ? [roomsLastChange, roomsUsersCountLastChange, onlineUsersLastChange, notificationLastChange] : [null,null,null,null];
        callWebService("Ping", function(result) {

            // Check user state    
            controlUserIdentity(result.CurrentChatUserState);
			
			if (that.Login.IsLoggedIn == true){
            
	            // Check Rooms list
	            if(isChatRoomsWP) {
	            	var usersCounts = null;
					if(result.UsersInRooms != null) {
						roomsUsersCountLastChange = result.UsersInRooms.LastChange;
						usersCounts = result.UsersInRooms.List;
					}
					var rooms = null;
					var firstLoad = (roomsLastChange == 0);
					if(result.Rooms != null) {
						roomsLastChange = result.Rooms.LastChange;
						rooms = result.Rooms.List;
					}
					if ((rooms != null) || (usersCounts != null) || firstLoad){
						that.ProcessChatRooms(rooms, usersCounts);
					}
				}
	
	            // Check Notifications
	            if ((that.WebpartsNotification.length > 0) && (result.Notifications != null)) {
	                showNotifications(result.Notifications);
	            }
	
	            // Check online users
	            if ((that.WebpartsOnlineUsers.length > 0) && (result.OnlineUsers != null)) {
	            	if(onlineUsersLastChange == 0){
	            		that.ShowLoading(that.WebpartsOnlineUsers, true);
	            	}
	            	onlineUsersLastChange = result.OnlineUsers.LastChange;
			        if(result.OnlineUsers.List != null) {
	                	that.ProcessOnlineUsers(result.OnlineUsers.List);
	               	}
	            }
	            
            }
            
            // Unlock mutex after callback
            isCallingPing = false;
            
            // Plan next ping
            roomsPingInterval = setTimeout(function() { 
            	that.PingWebserviceGlobal(false); 
            	}, roomsRefreshRate);
			
			if (fnok != null){
				fnok();
			}

        }, function(response) {

            // On error while pinging
            
            // Ulock mutex
            isCallingPing = false;
            
            // Plan next ping
            roomsPingInterval = setTimeout(function() { 
            	that.PingWebserviceGlobal(false); 
            	}, roomsRefreshRate);
            that.GlobalError(response);
        }, params);
    };


    this.CreateOneToOneChatRoom = function(chatuserid, func) {
        callWebService("CreateOneToOneChatRoom", func, that.GlobalError, [chatuserid]);
    };


    this.ProcessNotification = function(ID, actionType) {
        callWebService(actionType, function(response){ 
        	if ((response != null) && (response.ChatRoomID != null)){
        		response.OnlineUsersCount = 1;
        		that.ChatRoomsList[response.ChatRoomID] = response;
        		that.ProcessChatRooms();
        	}
        	
        	var notif = that.NotificationsList[ID];
        	if ((actionType == "AcceptInvitation") && (notif != null) && (notif.RoomID != null)){
				var wp = that.GetWebpart(null, that.ActiveNotificationWP, "ChatNotification");
		        var settings = {
			        roomID: notif.RoomID,
			        GUID: (wp != null) ? wp.Options.chatRoomGUID : null,
			        url: that.Settings.IsLiveSite ? that.Settings.PopupWindowURL : that.Settings.PopupWindowURLAdministration
	        };
				that.OpenChatWindow(settings);
			}
			
        	delete that.NotificationsList[ID];
        	for(var i = 0; i < that.WebpartsNotification.length; i++) {
				that.WebpartsNotification[i].ProcessResponse(that.NotificationsList, true);
			}
        }, function(response){
        	var notif = that.NotificationsList[ID];
        	delete that.NotificationsList[ID];
        	for(var i = 0; i < that.WebpartsNotification.length; i++) {
				that.WebpartsNotification[i].ProcessResponse(that.NotificationsList, true);
			}
        	alert(response.StatusMessage);
        }, [ID]);
    };
    
    
    this.RemoveAllNotifications = function(){
    	callWebService("CloseAllNotifications", function(response){
    			that.NotificationsList.length = 0;
    			for(var i = 0; i < that.WebpartsNotification.length; i++) {
					that.WebpartsNotification[i].ProcessResponse(that.NotificationsList, true);
				}
    		}, that.GlobalError, [notificationLastChange]);
    	
    };


    /**  
     *  @public
     *  @function GlobalError
     *  @param response General response, typicaly from webservice
     *  @param connection If true, error is with connection, not status response from service (optional)
     *  @param forceMsg If defined, forceMsg will be showed, other arguments will be ignored (optional)
     *  @description Handle global errors when calling webservice.
     */
    this.GlobalError = function(response, connection, forceMsg) {
        var msg = "";     
        
        // Forced message call
        if (forceMsg) {
            msg = forceMsg;
        }
        
        // Connection or webservice error
        else if (connection || ChatManager.StatusCodes.UnknownError == response.StatusCode) {
            msg = response.StatusMessage;	
            
            // Increment connection fails counter
            that.ServiceFails++;
            if (that.ServiceFails >= 5) {
            	if(!that.Settings.Debug){
                	that.StopChat();
                }
                msg = that.Settings.StoppingChatErrorMsg;
            }
        }
        
        // Response recieved, but with error status
        else { 
            msg = response.StatusMessage;
        }
        
        // If there is no message to display
        if (!msg || (msg.length == 0)){
        	return;	
        }
        
        // Display error to all error webparts
        if (that.WebpartsErrors.length > 0) {
        	if(response == null){
        		response = {};
        	}
        	response.StatusMessage = msg;
            for (var i = 0; i < that.WebpartsErrors.length; i++) {
                that.WebpartsErrors[i].ProcessResponse(response);
            }
        }
        else {
        
            // If there is no error webpart...
            alert(msg);
        }
    };


    this.StopChat = function() {
        clearTimeout(roomsPingInterval);
        that.UnlockPingMutex();
        for (var i = 0; i < that.ChatGroupManagers.length; i++) {
            that.ChatGroupManagers[i].StopChat();
        }
        that.ClearWindowName();
    };



    /**  
     *  @public
     *  @function SortObjectArray
     *  @param arr Array of objects
     *  @param prop Name of property which will be used for sorting
     */
    this.SortObjectArray = function(arr, prop) {
    	if (arr.length <= 1){
    		return;
    	}
    	prop = prop.split(".");
    	function fn(a, b){
    		for (var i = 0; i < prop.length; i++){
    			a = a[prop[i]];
    			b = b[prop[i]];
    		}

			if (!a || !b) {
				return 0;
			}

    		a = a.toLowerCase();
    		b = b.toLowerCase();
    		if (typeof a.localeCompare == "function"){
    			return a.localeCompare(b);
    		}
    		if (a == b){
    			return 0;
    		}
    		else if (a > b){
    			return 1;
    		}
    		else {
    			return -1;
    		}
    	}
    	arr.sort(fn);
    };


    this.CreateRoom = function(displayName, isPrivate, password, allowAnonym, description, fnok, fnerr) {
        callWebService("CreateChatRoom", function(room){
        	room.OnlineUsersCount = 0;
        	that.ChatRoomsList[room.ChatRoomID] = room;
            that.ProcessChatRooms();
            if (fnok != null){
    			fnok();
    		}
    	}, function(response){ 
    		if (fnerr != null){
    			fnerr(response.StatusMessage);
    		}
    		else {
    			that.GlobalError(response);
    		}
    	},  [displayName, isPrivate, password, allowAnonym, description]);
    };



    /**  
     *  @public
     *  @function KickUser
     *  @param roomID Room from which user will be kicked
     *  @param userID ID of kicked user
     *  @param groupID If setted, room manager will call asynchronous ping (optional)
     *  @param permanently True if user have to be kicked from room permanently
     *  @description Kick user from room.
     */
    this.KickUser = function(roomID, userID, groupID, permanently, clientID) {
        var fn = "KickUser";
        if (permanently) {
            fn = "KickUserPermanently"
        }
        var obj = null;
        var wpHtml = null;
        if (clientID != null){
        	obj = that.GetWebpart(groupID, clientID, "ChatUsers");
        	if(obj != null){
        	    wpHtml = $cmsj(obj.Options.contentClientID).html();
        		obj.ShowLoading(true);
        	}
        }
        
        callWebService(fn, function() {
            // Immediate synchronization
            if ((groupID!= null) && (groupID.length > 0)) {
                var rm = that.FindGroupManager(groupID);
                if (rm != null) {
                    delete rm.RoomUsersList[userID];
                    rm.ProcessRoomUsers(null);
                }
                else if(wpHtml != null){
                    $cmsj(obj.Options.contentClientID).html(wpHtml);
                }
            }
        }, function(response){
        	if ( (obj != null) && (wpHtml != null) ){
        	    $cmsj(obj.Options.contentClientID).html(wpHtml);
        	}
        	that.GlobalError(response);
        }, [roomID, userID]);
    };


    this.StartChat = function() {
    	var refreshedPage = false;
    	if (that.IsUsersUniqueWindowID(self.window.name)){
    		refreshedPage = true;
    	}
    	else {
    		that.SetWindowName(true);
    	}
    	    	
        for (var i = 0; i < that.ChatGroupManagers.length; i++) {
        	if (refreshedPage){
        		var windowName = that.GetUniqueWindowID(that.ChatGroupManagers[i].GroupID);
        		if (windowName != null){
        		    var roomID = $cmsj.cookie(windowName + "_roomID");
               		if (roomID != null){
               			that.ChatGroupManagers[i].RoomIDToJoinCookie =  parseInt(roomID);
               		}
                }
        	}
            // Try to join default room (if set)
            that.ChatGroupManagers[i].JoinRoom(null, null); 
        }
    };


    this.InitOneToOneChat = function(chatuserid, url, guid) {
        that.CreateOneToOneChatRoom(chatuserid, function(result) {
            var settings = {};
            settings.roomID = result.ChatRoomID;
            settings.GUID = guid;
            settings.url = url;
            that.OpenChatWindow(settings);
        });
    };


    this.OpenChatWindow = function (opt) {
        var win = window.open(opt.url + "?windowroomid=" + opt.roomID + ((opt.GUID != null) ? ("&popupSettingsId=" + opt.GUID) : ""), opt.roomID, "width=600,height=800,location=0,scrollbars=1,resizable=1");
        testWindow(win, 600, 800)
    };

     
    this.UnlockPingMutex = function() {
        isCallingPing = false;
        isProcessingPing = 0;
    };


    this.RejectMessage = function(msgID, groupID) {
        callWebService("RejectMessage", function() {
            var rm = that.FindGroupManager(groupID);
            if (rm != null){
	            var msg = rm.RoomMessagesList[msgID];
	            msg.IsRejected = true;
	            delete msg.RejectMessage;
	            rm.ProcessRoomMessages([msg], false);
            }
        }, that.GlobalError, [msgID]);
    };


    this.InviteToRoom = function(groupID, userID) {
        callWebService("InviteToRoom",
            function() { },
            that.GlobalError,
            [that.FindGroupManager(groupID).RoomID, userID]);
    };
    
    
    // Show or hide loading div in all webparts in array
    this.ShowLoading = function(arr, show){
    	if (arr == null){
    		return;
    	}
    	for (var i = 0; i < arr.length; i++){
    		arr[i].ShowLoading(show);
    	}
    };
    
    
    this.EditRoom = function(roomToEdit, name, isPrivate, hasPassword, password, allowAnonym, description, fnok, fnerr){
    	callWebService("ChangeChatRoom", 
    		function(room){
    			room.OnlineUsersCount = that.ChatRoomsList[roomToEdit].OnlineUsersCount; 
    			that.ChatRoomsList[room.ChatRoomID] = room;
            	that.ProcessChatRooms();
            	
    			var gms = that.ChatGroupManagers;
    			for (var i = 0; i < gms.length; i++){
    				var gm = gms[i];
    				if (gm.RoomID == roomToEdit){
    					room.IsCurrentUserAdmin = gm.RoomInfo.IsCurrentUserAdmin; 
    					gm.RoomInfo = room;
    					var wps = gm.WebpartsRoomName;
    					for (var j = 0; j < wps.length; j++){
    						wps[j].ProcessResponse(room);
    					}
    				}
    			}
    			if (fnok != null){
    				fnok();
    			}
    		}, function(response){ 
    			if (fnerr != null){
    				fnerr(response.StatusMessage);
    			}
    			else {
    				that.GlobalError(response);
    			}
    		}, 
    		[roomToEdit, name, isPrivate, hasPassword, password, allowAnonym, description]
    		);
    };
    
    
    this.DeleteRoom = function(roomToDelete){
    	callWebService("DeleteChatRoom", 
    		function(){
    			delete that.ChatRoomsList[roomToDelete];
            	that.ProcessChatRooms();
    		}, 
    		that.GlobalError, 
    		[roomToDelete]
    		);
    };
    
    
    this.AddAdmin = function(roomID, chatUserID, groupID, clientID){
    	AddDeleteAdmin("AddAdmin", roomID, chatUserID, groupID, clientID);
    };
    
    
    this.DeleteAdmin = function(roomID, chatUserID, groupID, clientID){
    	AddDeleteAdmin("DeleteAdmin", roomID, chatUserID, groupID, clientID);
    };
    
    
    this.SearchOnlineUsers = function(filter, objWP, roomID){
    	objWP.ShowLoading(true);
    	// Are online users in memory?
    	if (that.OnlineUsersList.length > 0){
    		var list = new Array();
    		var usr;
    		for (var i in that.OnlineUsersList) {
    			if (!that.OnlineUsersList.hasOwnProperty(i)) {
    				continue;
    			}
    			usr = that.OnlineUsersList[i];
    			if (usr.Nickname.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) >= 0){
    				list.push(usr);
    			}
    		}
    		that.SortObjectArray(list, "Nickname");
	    	objWP.ProcessResponse(list, true);
    	}
    	else {
	    	callWebService("SearchOnlineUsers",
	    		function(response){
	    			that.SortObjectArray(response, "Nickname");
	    			objWP.ProcessResponse(response);
	    		},
	    		function(response){
	    			objWP.ShowLoading(false);
	    			that.GlobalError(response);
	   },
	    		[filter, objWP.Options.maxUsers, roomID]
	    	);
    	}
    };
    
    
    this.ProcessChatRooms = function(rooms, usersCounts, force) {
		// Process response to online uses list
		if (rooms != null){
			for(var i = 0; i < rooms.length; i++) {
				var room = rooms[i];
	
				// deleted room
				if(room.IsRemoved == true) { 
					delete that.ChatRoomsList[room.ChatRoomID];
				}
				// new or modified
				else {
					var oldRoom = that.ChatRoomsList[room.ChatRoomID];
					var oldCount = (oldRoom != null) ? oldRoom.OnlineUsersCount : null; 
					room.OnlineUsersCount = ((oldCount != null) && (oldCount > 0)) ? oldCount : 0;
					that.ChatRoomsList[room.ChatRoomID] = room;
					
					// Handle actually joined rooms
					for(var j = 0; j < that.ChatGroupManagers.length; j++){
						var gm = that.ChatGroupManagers[j];
						if (gm.RoomID == room.ChatRoomID){
							room.IsCurrentUserAdmin = gm.RoomInfo.IsCurrentUserAdmin;
							gm.RoomInfo = room;
							gm.ProcessRoomUsers();
						}
					}
				}
			}
		}
		
		if (usersCounts != null){
			for (var i = 0; i < usersCounts.length; i++){
				var uc = usersCounts[i];
				if (!force && (that.JoinedRoomsList[uc.RoomID] != null)){
					continue;
				}
				var r = that.ChatRoomsList[uc.RoomID];
				if (r != null){
					r.OnlineUsersCount = (uc.UsersCount >= 0) ? uc.UsersCount : 0;
				}				
			}			
		}

		var sortRooms = new Array();
		for (var r in that.ChatRoomsList) {
			if (!that.ChatRoomsList.hasOwnProperty(r)) {
				continue;
			}
			sortRooms.push(that.ChatRoomsList[r]);
		}

		// Sort on nickname
		that.SortObjectArray(sortRooms, "DisplayName");
        
        for (var i = 0; i < that.ChatGroupManagers.length; i++) {
            var rm = that.ChatGroupManagers[i];
            for (var j = 0; j < rm.WebpartsRooms.length; j++) {
                rm.WebpartsRooms[j].ShowRooms(sortRooms);
            }
        }
    };    
    
    
    this.ProcessOnlineUsers = function(users) {

		// Process response to online uses list
		if (users != null){
			for(var i = 0; i < users.length; i++) {
				var usr = users[i];
	
				// deleted user
				if(usr.IsRemoved == true) { 
					delete that.OnlineUsersList[usr.ChatUserID];
				}
				// new or modified
				else {
					that.OnlineUsersList[usr.ChatUserID] = usr;
				}
			}
		}

		var sortUsers = new Array();
		for (var u in that.OnlineUsersList) {
			if (!that.OnlineUsersList.hasOwnProperty(u)) {
				continue;
			}
			sortUsers.push(that.OnlineUsersList[u]);
		}

		// Sort on nickname
		that.SortObjectArray(sortUsers, "Nickname");

		for(var i = 0; i < that.WebpartsOnlineUsers.length; i++) {
			that.WebpartsOnlineUsers[i].ProcessResponse(sortUsers);
		}

	};


	this.IsChatWindowID = function(id){
		var re=/chat\s[0-9]+\s\S+\s[0-9]+/i;
		if(id.match(re) != null){
			return true;
		}
		return false;
	}
	

    this.IsUsersUniqueWindowID = function(id){
		
		if (that.IsChatWindowID(id)){
			var arr = id.split(" ");
			var location = ("" + self.window.location).split("#",1)[0];
			if((arr[1] != null) && (arr[1] == that.Login.UserID) && (arr[2] != null) && (arr[2] == location)){
				return true;
			}
		}
		return false;	
    };
    
     
    this.GetUniqueWindowID = function(groupID){
    	if (groupID != null){
    		var name = self.window.name;
    		if(name.length == 0){
    			return null;
    		}
    		return name + "_" + groupID;
    	}
    	else {
    		if (that.Login.UserID < 0){
    			return "";
    		}
    		var d = new Date();
    		return "chat " + that.Login.UserID + " " + (""+self.window.location).split("#",1)[0] + " " + d.getTime();	
    	}	
    };
    
    
    this.GetWindowName = function(){
    	var name = self.window.name; 
    	if (!that.IsUsersUniqueWindowID(name)){
    		name = "";
    	}
    	return name;
    }
    
    
    this.ClearWindowName = function(){
    	if (that.IsUsersUniqueWindowID(self.window.name)){
    		self.window.name = "";
    	}
    	
    }
    
    
    this.SetWindowName = function(force){
    	if ((self.window.name.length == 0) || (force && that.IsChatWindowID(self.window.name))){
    		self.window.name = that.GetUniqueWindowID();
    	}
    }
    
    
    this.LoadUsersToInvitePrompt = function(clientID){
    	
    	var wp = that.GetWebpart(null, clientID, "OnlineUsersInvite");
    	if (wp == null){
    		return false;
    	}
    	
    	that.ShowLoading([wp],true);
    	
    	// function will be called when there are data
    	var fnok = function(){
    		var sortUsers = new Array();
    		for (var u in that.OnlineUsersList) {
				if (!that.OnlineUsersList.hasOwnProperty(u)) {
					continue;
				}
				sortUsers.push(that.OnlineUsersList[u]);
			}
	
			// Sort on nickname
			that.SortObjectArray(sortUsers, "Nickname");
	        
			wp.ProcessResponse(sortUsers);
    	};
    	
    	if (that.WebpartsOnlineUsers.length == 0){
    		// There are not data loaded
	        callWebService("Ping", function(result) {
	        	
				if(result.OnlineUsers != null) {
					onlineUsersInviteLastChange = result.OnlineUsers.LastChange;
					if(result.OnlineUsers.List != null) {
						that.ProcessOnlineUsers(result.OnlineUsers.List);
					}
				}
				fnok();

			}, function(response) {
				that.GlobalError(response);
			}, [null, null, onlineUsersInviteLastChange, null]);

	    }
	    else {
	    	// use online users in memory
	    	fnok();
	    }
		
    }
    
    
    this.FindGroupManager = function(group) {
        var groupManager = null;
        
        // Find existing group manager
        for (var i = 0; i < that.ChatGroupManagers.length; i++) {
            if (that.ChatGroupManagers[i].GroupID == group) {
                groupManager = that.ChatGroupManagers[i];
                break;
            }
        }
        return groupManager;
    };
    
    
    this.PassRecipient = function(groupID, userID, dialogID){
    	var gm = that.FindGroupManager(groupID);
    	if (gm != null){
    		gm.PassRecepient(userID, dialogID);
    	}
    }
    
    
    this.IsLogoutRedirectSet = function(){
    	return ((that.Settings.RedirectURLLogout.length > 0) || (that.LogoutChatURL.length > 0)); 
	}
	
	
	// Opens overlay prompt for editing chat room
	this.OpenEditRoomPrompt = function(groupID, clientID, roomID) {
		var crwp = ChatManager.GetWebpart(groupID, clientID, 'ChatRooms');
		if (crwp){
			crwp.OpenEditRoomPrompt(roomID);
		}
	}
	
	
	// Opens overlay prompt for deleting chat room
	this.OpenDeleteRoomPrompt = function(groupID, clientID, roomID) {
		var crwp = ChatManager.GetWebpart(groupID, clientID, 'ChatRooms');
		if (crwp){
			crwp.OpenDeleteRoomPrompt(roomID);
		}
	}
	
	
	// Opens overlay prompt for abandon chat room
	this.OpenAbandonRoomPrompt = function(groupID, clientID, roomID) {
		var crwp = ChatManager.GetWebpart(groupID, clientID, 'ChatRooms');
		if (crwp){
			crwp.OpenAbandonRoomPrompt(roomID);
		}
	}
	
	
    // PRIVATE METHODS
    

    function AddDeleteAdmin(operation, roomID, chatUserID, groupID, clientID){
    	var wp = that.GetWebpart(groupID, clientID, "ChatUsers");
    	wp.ShowLoading(true);
    	callWebService(operation,
    		function() {
            	var rm = that.FindGroupManager(groupID);
            	var usr = rm.RoomUsersList[chatUserID];
            	if(operation == "AddAdmin"){
            		usr.AdminLevel = 2;
            	}
            	else {
            		if (usr.IsOnline == true){
            			usr.AdminLevel = 1;
            		}
            		else {
            			delete rm.RoomUsersList[chatUserID];
            		}
            	}
            	rm.ProcessRoomUsers(null);
        	},
    		function(response){
    			wp.ShowLoading(false);
    			that.GlobalError(response);
    		},[roomID, chatUserID]
    	);
    };

    
    function findWebpart(clientID, wpList, index){
        for (var i = 0; i < wpList.length; i++){
            if(wpList[i].Options.clientID == clientID){
                return !index ? wpList[i] : i;
            }
        }
        return null;
    };

    
    function registerRMWebpart(obj, type, group) {
        var groupManager = that.FindGroupManager(group);
        if (groupManager == null) {
            groupManager = new ChatGroupManager(that, group);
            that.ChatGroupManagers.push(groupManager);
        }
        groupManager.AddWebpart(obj, type);
    };


    function controlUserIdentity(userState){
    	
    	// User has logged out
		if((userState.IsLoggedIn == false) && (that.Login.IsLoggedIn == true)) {
			
			// Set login webparts to logged out state
			that.Login.SetProperties(userState);
			that.Login.GetUserPermissions();
			
			// Clear webparts, stop pinging to room
			that.LoggedOut();

			// User is logged out, but pinging to chat will continue
			// with no influence to chat rooms webpart. Only waiting until new user is logged in.
		}
		
		if((userState.IsLoggedIn == true) && (that.Login.IsLoggedIn == false)) {
			
			// Set login webparts to logged out state
			that.Login.SetUserAfterLogin(userState);
		}

		// Loged user has changed
		else if(userState.ChatUserID != that.Login.UserID) {

			// Clear all webparts, stop pinging to room
			that.LoggedOut();

			// Set login webparts to actual user
			that.Login.SetUserAfterLogin(userState);
			
			// New user will not be joined in to room anymore, his chat rooms list will be displayed only
		}

		// user has changed his nickname
		else if(userState.Nickname != that.Login.Nickname) {

			// Set login webparts to actual user
			that.Login.SetProperties(userState);
		}
    };

    
	function showNotifications(response) {

		// Indicates if response is incremental or not (first response is NOT incremental)
		var incremental = true;
		if(!notificationLastChange) {
			//notificationLastChange = new Date(0);
			incremental = false;
		}
		notificationLastChange = response.LastChange || notificationLastChange;

		// If notifications are not incremental -> delete current notifications
		if(!incremental) {
			that.NotificationsList.length = 0;
		}

		var notifyUser = false,
		    stopNotify = false;

		// There are open notifications
		for(var i = 0; i < response.List.length; i++) {
			var notif = response.List[i];
			
			// new
			if (notif.IsRead == false) {
				notif.IsOneOnOne = notif.IsOneToOne;
                // Render the list items based on their types
                switch (notif.NotificationType) {
                	case 0:
                		if (!notif.IsOneOnOne && !that.Settings.IsLiveSite) {
			                continue;
		                }
                        notif.AcceptEvent = "ChatManager.ProcessNotification(" + notif.NotificationID + ", 'AcceptInvitation'); return false;";
                        notif.DeclineEvent = "ChatManager.ProcessNotification(" + notif.NotificationID + ", 'DeclineInvitation'); return false;";
                        break;
                    default:
                        notif.CloseEvent = "ChatManager.ProcessNotification(" + notif.NotificationID + ", 'CloseNotification'); return false;";
                        notif.KickTime = that.Settings.NotificationKickInterval;
                        break;
                }
                that.NotificationsList[notif.NotificationID] = notif;
			    notifyUser = true;
			}
			
			// delete
			else { 
				delete that.NotificationsList[notif.NotificationID];
				stopNotify = true;
			}
		}

		for(var i = 0; i < that.WebpartsNotification.length; i++) {
			that.WebpartsNotification[i].ProcessResponse();
		}
	    
		if ((typeof (window.ChatNotificationManager) != "undefined") && that.Settings.IsLiveSite) {
			if (notifyUser) {
				window.ChatNotificationManager.Notify("newnotification");
			}
			else if (stopNotify) {
				window.ChatNotificationManager.StopNotify("newnotification");
			}
		}
	};


	function testWindow(win, width, height) {
	    if (win == null || typeof (win) == 'undefined') {
	        alert(that.Settings.PopupWindowErrorMsg);
	    }
	    else {
	        win.focus();
	        if ($cmsj.browser.opera || $cmsj.browser.webkit) {
	            setTimeout(function () {
	                if (!win.closed && (win.innerHeight != height) && (win.innerWidth != width)) {
	                    alert(that.Settings.PopupWindowErrorMsg);
	                }
	            }, 1000);
	        }
	    }
	}


    
    this.DEBUGStopPing = function(){
    	clearInterval(roomsPingInterval);
    }
    
    this.DEBUGSetPingInterval = function(interval){
    	roomsRefreshRate = interval * 1000;
    }
    
    this.DEBUGGetPingInformation = function(){
    	var str = "";
    	str += "ping interval == " + roomsRefreshRate/1000 + "seconds.";
    	return str;
    }
    

};


