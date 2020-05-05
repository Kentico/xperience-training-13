ChatSupportHeader = function (options) {
    var $ = window.$cmsj,
        config = {
		    headerIconId: '',
		    btnLoginId: '',
		    btnLoginShortcutId: '',
		    btnLogoutId: '',
		    btnSettingsId: '',
		    loginShortcutWrapperId: '',
		    lblNotificationNumberId: '',
		    ulActiveRequestsId: '',
		    ulNewRequestsId: '',
		    lnkNewRequestsId: '',
		    lblNewRequestsId: '',
		
		    resRoomNewMessagesFormat: '',
		    resNewRequestsSingular: '',
		    resNewRequestsPlural: '',

		    settingsUrl: '',
		    notificationManagerOptions: null
        };

	$.extend(config, options);

	var $btnLogin = $('#' + config.btnLoginId),
	    $btnLoginShortcut = $('#' + config.btnLoginShortcutId),
	    $loginShortcutWrapper = $('#' + config.loginShortcutWrapperId),
	    $btnSettings = $('#' + config.btnSettingsId),
	    $btnLogout = $('#' + config.btnLogoutId),
	    $statusIcon = $('#' + config.headerIconId + ' i.icon-bubble'),
	    $navbar = $("#cms-nav-chat"),
	    $lblNotificationNumber = $('#' + config.lblNotificationNumberId),
	    $ulActiveRequests = $('#' + config.ulActiveRequestsId),
	    $ulNewRequests = $('#' + config.ulNewRequestsId),
	    $lnkNewRequests = $('#' + config.lnkNewRequestsId),
	    $liNewRequestsLink = $lnkNewRequests.parent('li'),
	    $lblNewRequests = $('#' + config.lblNewRequestsId),	    
	    notifyRequestEventName = 'supportnewrequest',
	    notifyMessageEventName = 'supportnewmessage',
	    focusTimeStamp = new Date(0),
	    roomsLastUpdate = new Date(0);
	
	function goOnline() {
		$btnLogin.hide();
		$loginShortcutWrapper.hide();
		$statusIcon.addClass('chat-navbar-online');
		$btnLogout.removeClass('hide');
		$lnkNewRequests.removeClass('hide');
		showRooms();
	}
	
	function goOffline() {
		$btnLogin.show();
		$loginShortcutWrapper.show();
		$statusIcon.removeClass('chat-navbar-online');
		$btnLogout.addClass('hide');
		$lnkNewRequests.addClass('hide');
		showRooms();
	}
	
	function showRooms(rooms) {
		if (!rooms) {
			rooms = {};
		}
		var activeRequests = [],
		    requests = [],
		    // How many requests requires user's attention (waiting requests and active requests with unread messages)
		    notifyCount = 0,
		    requestsLastChange = 0,
		    activeUnreadLastChange = 0,
		    activeReadLastChange = 0,
		    newMessagesInClosedWindow = false;

		for (var key in rooms) {
			var room = rooms[key];
			if (room.isTaken) {
				// Active request
				if (room.unreadMessagesCount > 0) {
					if (room.lastChange > activeUnreadLastChange) {
						activeUnreadLastChange = room.lastChange;
						if (!newMessagesInClosedWindow && !room.openedWindow || room.openedWindow.closed) {
							newMessagesInClosedWindow = true;
						}
					}
					notifyCount++;
				} else if (room.lastChange > activeReadLastChange) {
					activeReadLastChange = room.lastChange;
				}
				activeRequests.push(room);
			} else {
				// New request
				if (room.lastChange > requestsLastChange) {
					requestsLastChange = room.lastChange;
				}
				requests.push(room);
			}
		}
		
		if (requestsLastChange > roomsLastUpdate) {
			// There are new support requests - play sound and start changing title
			notifyNewRequests();
		}
		else if (activeUnreadLastChange > roomsLastUpdate) {
			// There are new messages in active requests - start changing title
			notifyNewMessages(newMessagesInClosedWindow);
		}
		else if (!rooms.length || (activeReadLastChange > roomsLastUpdate && activeUnreadLastChange < focusTimeStamp && requestsLastChange < focusTimeStamp)) {
			// Messages in all active rooms were read and there are no new requests - stop changing title
			notifyStop();
		}

		notifyCount += requests.length;
		if (notifyCount) {
			$lblNotificationNumber.removeClass('hide').text(notifyCount);
		} else {
			$lblNotificationNumber.addClass('hide').empty();
		}
		
		showRequests(requests);
		showActiveRequests(activeRequests);
		resizeFrame();
		roomsLastUpdate = new Date();
	}
	
	function showRequests(requests) {
		var requestsCount = requests.length;
		$lblNewRequests.text((requestsCount === 1 ? config.resNewRequestsSingular : config.resNewRequestsPlural).replace('{0}', requestsCount));
		if (requestsCount) {
			$lnkNewRequests.removeClass('disabled');
			$lnkNewRequests.attr('href', '#');
		} else {
			$lnkNewRequests.addClass('disabled');
			$liNewRequestsLink.removeClass('open');
			$lnkNewRequests.removeAttr('href');
		}
		$ulNewRequests.empty();
		for (var i = 0; i < requestsCount; i++) {
			var request = requests[i];
			$ulNewRequests.append($('<li></li>')
				.append($('<a href="#"></a>')
					.text(getRoomName(request))
					.click(generateTakeRoomFn(request.roomId))
				)
			);
		}
	}
	
	function showActiveRequests(requests) {
		requests = requests.sort(compareRoomsByName);
		$ulActiveRequests.empty();
		for (var i = 0; i < requests.length; i++) {
			var request = requests[i];
			$ulActiveRequests.append($('<li' + (request.unreadMessagesCount > 0 ? ' class="active"' : '') + '></li>')
				.append($('<a href="#"></a>')
					.text(getRoomName(request))
					.click(generateTakeRoomFn(request.roomId))
				)
			);
		}
	}
	
	function generateTakeRoomFn(roomId) {
		return function() {
			ChatSupportManager.takeSupportRoom(roomId);
			return false;
		};
	}
	
	function compareRoomsByName(roomA, roomB) {
		return roomA.name - roomB.name;
	}
	
	function getRoomName(room) {
		return room.unreadMessagesCount ? config.resRoomNewMessagesFormat.replace('{0}', room.unreadMessagesCount).replace('{1}', room.name) : room.name;
	}
	
	function notifyNewRequests() {
		if (ChatNotificationManager) {
			ChatNotificationManager.NotifySound(notifyRequestEventName, true);
			ChatNotificationManager.NotifyTab(notifyRequestEventName);
		}
	}

	function notifyNewMessages(playSound) {
		if (ChatNotificationManager) {
			ChatNotificationManager.NotifyTab(notifyMessageEventName);
			if (playSound) {
				ChatNotificationManager.NotifySound(notifyMessageEventName, true);
			}
		}
	}
	
	function notifyStop() {
		if (ChatNotificationManager) {
			ChatNotificationManager.StopNotify(notifyRequestEventName);
			ChatNotificationManager.StopNotify(notifyMessageEventName);
		}
	}
	
	function showError(message) {
		alert(message);
	}

	function resizeFrame() {
	    // Make sure that layout resizing will happen after
	    // the DOM rerendering finishes
	    setTimeout(function () {
	        window.top.layouts[0].resizeAll();
	    }, 0);
	}
	
	function init() {
		if (!ChatSupportManager || !ChatSupportManager.ready) {
			$('#' + config.headerIconId).hide();
			return;
		}

		// Move chat bar into placeholder in header
	    $navbar.appendTo($("#cms-header-placeholder"))
	        .on("show.bs.collapse", resizeFrame)
	        .on("hide.bs.collapse", resizeFrame);
		
		// Subscribe to callbacks
		ChatSupportManager.callbackGoOnline.add(goOnline);
		ChatSupportManager.callbackGoOffline.add(goOffline);
		ChatSupportManager.callbackError.add(showError);
		ChatSupportManager.callbackUpdateRooms.add(showRooms);
		
		if (ChatSupportManager.getOnlineStatus()) {
			goOnline();
		} else {
			goOffline();
		}

		// Support login button function
		var loginFn = function (e) {
			e.preventDefault();
			ChatSupportManager.goOnline();
			return false;
		};

		$btnLogin.click(loginFn);
		$btnLoginShortcut.click(loginFn);

		// Support logout button function
		$btnLogout.click(function (e) {
			e.preventDefault();
			ChatSupportManager.goOffline();
			return false;
		});

		// Support setting button function
		$btnSettings.click(function (e) {
			e.preventDefault();
			modalDialog(config.settingsUrl, 'SupportChatSettings', 700, 530);
			return false;
		});
		
		if (ChatNotificationManagerSetup && config.notificationManagerOptions) {
			ChatNotificationManagerSetup({
				eventName: notifyRequestEventName,
				soundFile: config.notificationManagerOptions.soundFileRequest,
				notifyTitle: config.notificationManagerOptions.notifyTitle
			});
			ChatNotificationManagerSetup({
				eventName: notifyMessageEventName,
				soundFile: config.notificationManagerOptions.soundFileMessage,
				notifyTitle: config.notificationManagerOptions.notifyTitle
			});

			$cmsj(window).focus(function () {
				focusTimeStamp = new Date();
			});
		}
	}

	init();
};