var ChatSupportManager = (function () {
	var my = {},
		windowNameOnline = 'chat_support_online',
		lastChangeTimestamp = null,
		onlineSupportID = null,
		pingTimeout = null,
		proxy = null,
		isOnline = false,
		rooms = {},
		roomsInitiatedUserId = {},
		roomsInitiatedContactId = {};

	my.config = {
			tick: 5000,
			popupWindowUrl: "",
			resPopupWindowError: ""
		};
	my.ready = false;
	my.callbackError = $cmsj.Callbacks();
	my.callbackGoOnline = $cmsj.Callbacks();
	my.callbackGoOffline = $cmsj.Callbacks();
	my.callbackUpdateRooms = $cmsj.Callbacks();

	my.getOnlineStatus = function() {
		return isOnline;
	};

	my.goOnline = function (callback) {
		callSupportWebService('EnterSupport', function() {
			if (!window.name) {
				window.name = windowNameOnline;
			}
			processGoOnline();
			if (callback != null) {
				callback();
			}
		}, handleError, []);
	};

	my.goOffline = function () {
		callSupportWebService('LeaveSupport', processGoOffline, function(response) {
			handleError(response);
			processGoOffline();
		}, []);
	};

	my.takeSupportRoom = function (roomId) {
		var room = rooms[roomId];
		if (!room) {
			return;
		}
		// Always call service and take room, so taken date time will be updated on the server side and correct number of unread messages will be send to all windows
		callSupportWebService('SupportTakeRoom', function() {
			room.unreadMessagesCount = 0;
			room.isTaken = true;
			room.popup();
			my.callbackUpdateRooms.fire(rooms);
		}, handleError, [room.roomId]);
	};

	my.leaveSupportRoom = function (roomId, fnOk) {
		callSupportWebService('SupportLeaveRoom', function() {
			if (rooms[roomId]) {
				delete rooms[roomId];
				my.callbackUpdateRooms.fire(rooms);
			}
			if (fnOk) {
				fnOk();
			}
		}, handleError,
			[roomId]);
	};

	my.initiateChat = function(userId, contactId) {
		if (userId <= 0 && contactId <= 0) {
			return;
		}		

		// If userId is not set, use contactId and mark isContact as true (keep userId otherwise)
		var isContact = false;
		if (userId <= 0) {
			userId = contactId;
			isContact = true;
		}
		if (!isOnline) {
			my.goOnline(function() {
				initiateChatInternal(userId, isContact);
			});
		} else {
			initiateChatInternal(userId, isContact);
		}
	};

	my.init = function (options) {
		$cmsj.extend(my.config, options);

		if ((typeof (window.chat) !== 'undefined') && (window.chat) && (window.chat.IChatSupportService)) {
			proxy = window.chat.IChatSupportService;
			my.ready = true;
		} else {
			my.ready = false;
		}

		if (window.name == windowNameOnline) {
			// Behave as on-line user (it is not necessary to send EnterSupport request)
			my.goOnline();
		} else {
			// User is off-line
			processGoOffline();
		}
	};
	
	function processGoOnline() {
		isOnline = true;
		lastChangeTimestamp = null;
		my.callbackGoOnline.fire();
		ping();
	}
	
	function processGoOffline() {
		clearPingTimeout();
		isOnline = false;
		lastChangeTimestamp = null;
		onlineSupportID = null;
		rooms = {};
		roomsInitiatedContactId = {};
		roomsInitiatedUserId = {};
		if (window.name == windowNameOnline) {
			window.name = '';
		}
		my.callbackGoOffline.fire();
	}
	
	function initiateChatInternal(userContactId, isContact) {
		var roomId,
			room;

		if (isContact) {
			roomId = roomsInitiatedContactId[userContactId];
		} else {
			roomId = roomsInitiatedUserId[userContactId];
		}

		if (roomId) {
			room = rooms[roomId];
		}
		
		if (room) {
			room.take();
		} else {
			callSupportWebService(isContact ? 'InitiateChatByContactID' : 'InitiateChatByUserID', function (roomResponse) {
				var contactId = isContact ? userContactId : 0,
					userId = !isContact ? userContactId : 0,
					roomId = roomResponse.ChatRoomID;
				
				room = addRoom(roomId, roomResponse.DisplayName, roomResponse.UnreadMessagesCount, true, userId, contactId);
				if (isContact) {
					roomsInitiatedContactId[contactId] = roomId;
				}
				else {
					roomsInitiatedUserId[userId] = roomId;
				}
				room.popup();
				my.callbackUpdateRooms.fire(rooms);
			}, handleError, [userContactId]);
		}
	}
	
	function addRoom(roomId, name, messagesCount, isTaken, userId, contactId) {
		var room = new ChatSupportRoom(roomId, name, messagesCount, {
			isTaken: isTaken,
			userId: userId,
			contactId: contactId
		});
		rooms[roomId] = room;
		return room;
	};

	function ping() {
		callSupportWebService('SupportPing', processPingResponse, function(response) {
			resetPingTimeout();
			handleError(response);
		}, [lastChangeTimestamp]);
	}

	function resetPingTimeout() {
		clearPingTimeout();
		pingTimeout = setTimeout(ping, my.config.tick);
	}

	function clearPingTimeout() {
		if (pingTimeout !== null) {
			clearTimeout(pingTimeout);
		}
	}

	function processPingResponse(response) {
		var supporterID = response.OnlineSupportChatUserID;
		if (!supporterID) {
			// Supporter is not online anymore.
			processGoOffline();
			return;
		}
		
		if (!onlineSupportID) {
			onlineSupportID = supporterID;
		} else if (supporterID !== onlineSupportID) {
			// Different user logged in.
			processGoOffline();
			return;
		}

		var responseRooms = response.Rooms;
		if (responseRooms) {
			if (responseRooms.LastChange) {
				lastChangeTimestamp = responseRooms.LastChange;
			}
			processRooms(responseRooms.List);
		}
		resetPingTimeout();
	}
	
	function processRooms(list) {
		if (!list || !list.length) {
			return;
		}
		for (var i = 0; i < list.length; i++) {
			var receivedRoom = list[i],
			    roomId = receivedRoom.ChatRoomID,
				room = rooms[roomId];
			if (receivedRoom.IsRemoved) {
				if (room) {
					delete rooms[roomId];
				}
				continue;
			}
			if (!room) {
				addRoom(roomId, receivedRoom.DisplayName, receivedRoom.UnreadMessagesCount, receivedRoom.IsTaken);
			} else {
				room.lastChange = new Date();
				room.unreadMessagesCount = receivedRoom.UnreadMessagesCount;
				room.isTaken = receivedRoom.IsTaken;
			}
		}
		my.callbackUpdateRooms.fire(rooms);
	}

	function handleError(error) {
		var message;
		if (error.StatusMessage) {
			message = error.StatusMessage;
		} else if (error.get_message) {
			message = error.get_message();
		} else {
			message = error;
		}

		if (message) {
		    if (typeof (console) !== "undefined") {
				console.log(message);
			}
			my.callbackError.fire(message);
		}
	}	

	// Call webservice function and process response
	function callSupportWebService(fn, fnOk, fnErr, args) {
		if (!proxy) {
			return;
		}
		if (!args) {
			args = [];
		}
		args.push(function(response) {
			// 0 is OK
			if (response.StatusCode == 0) {
				if (fnOk) {
					if (response.Data) {
						fnOk(response.Data);
					} else {
						fnOk();
					}
				}
			} else if (fnErr){
				fnErr(response);
			}
		});
		args.push(function (response) {
			if (!response.get_timedOut() && response.get_message().match(/The server method '.+' failed./i)) {
				// This is typically caused by unloading the page during service request
				return;
			}

			fnErr(response);
			
			// User has logged out in another window
			if (response.get_statusCode() == 401) {
				processGoOffline();
			}
		});
		args.push(null);
		proxy[fn].apply(proxy, args);
	}

	return my;
}());


ChatSupportRoom = function(roomId, name, unreadMessagesCount, options) {
	this.roomId = roomId;
	this.name = name;
	this.unreadMessagesCount = unreadMessagesCount;
	this.lastChange = new Date();
	this.isTaken = options.isTaken ? options.isTaken : false;
	this.openedWindow = null;
	this.userId = options.userId ? options.userId : null;
	this.contactId = options.contactId ? options.contactId : null;

	var winUrl = ChatSupportManager.config.popupWindowUrl.replace("{RoomIDParam}", this.roomId);

	this.popup = function() {
		// Open window with empty address, so it won't be reloaded if it already exists (is open in another tab for example) http://stackoverflow.com/a/2455480/637677
		var height = 800,
			width = 600,
			win = window.open('', this.roomId, "width=" + width + ",height=" + height + ",location=0,scrollbars=1,resizable=1");
		if (win) {
			// If location is not correct url, window was not opened before and address should be set to the correct one
			if (win.location != winUrl) {
				win.location = winUrl;
			}
			win.focus();
			ensureWindow(win, width, height);
			this.openedWindow = win;
		} else {
			alert(ChatSupportManager.config.resPopupWindowError);
		}
	};

	this.take = function() {
		ChatSupportManager.takeSupportRoom(this.roomId);
	};

	function ensureWindow(win, width, height) {
	    if ($cmsj.browser.opera || $cmsj.browser.webkit) {
			setTimeout(function() {
				if (!win.closed && (win.innerHeight != height) && (win.innerWidth != width)) {
					alert(ChatSupportManager.config.resPopupWindowError);
				}
			}, 1000);
		}
	}
};
