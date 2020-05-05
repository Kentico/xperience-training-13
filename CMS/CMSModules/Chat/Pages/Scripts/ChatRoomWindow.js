ChatSupportWindow = function(opt) {
	var options = {
			roomId: 0,
			pnlChatRoomWindowClientId: '',
			pnlTopClientId: '',
			pnlBottomClientId: '',
			ChatRoomMessagesClientId: '',
			btnCloseClientId: '',
			notificationManagerOptions: null,
			title: '',
			isSupport: true
		};
	    
	$cmsj.extend(options, opt);

	var $pnlChatRoomWindowTop = $cmsj('#' + options.pnlChatRoomWindowClientId),
	    $divTop = $cmsj('#' + options.pnlTopClientId),
	    $divBottom = $cmsj('#' + options.pnlBottomClientId),
	    $sendMessageBtn = $cmsj('.ChatSendAreaButton .SubmitButton'),
	    $txtAreaSendMessage = $cmsj('.ChatSendAreaButton textarea'),
	    $messagesEnvelope = null,
	    messagesWebPart = null;

	$cmsj(init);

	function init() {
		window.top.document.title = options.title;
		ChatManager.Login.IsSupporter = options.isSupport;
		ChatManager.IsPopupWindow = true;
		if ($sendMessageBtn && $txtAreaSendMessage) {
		    $sendMessageBtn.parent().width($sendMessageBtn.outerWidth(true));
		}
		$cmsj('#' + options.btnCloseClientId).click(function() {
			if (ChatManager.ChatGroupManagers[0].RoomID > 0) {
				leaveRoom(leaveConversation);
			} else {
				leaveConversation();
			}

			return false;
		});

		if (!options.isSupport && !$cmsj.browser.msie) {
			// Support room is not leaved when window is closed!			
			$cmsj(window).bind("beforeunload", function (event) {
				if (ChatManager.ChatGroupManagers[0].RoomID > 0) {
					leaveRoom();
				}
			});
			
		}

		if (ChatNotificationManagerSetup) {
			ChatNotificationManagerSetup(options.notificationManagerOptions);
		}
		
		$pnlChatRoomWindowTop.resize(recalculatePositions);
		$divTop.resize(recalculatePositions);
		$divBottom.resize(recalculatePositions);
		for (var i = 0; i < ChatManager.ChatGroupManagers.length; i++) {
			ChatManager.ChatGroupManagers[i].JoinRoom(options.roomId);
		}
	}

	function leaveConversation() {
		if (options.isSupport) {
			window.opener.ChatSupportManager.leaveSupportRoom(options.roomId, closeWindow);
		} else {
			closeWindow();
		}
	}

	function closeWindow() {
		if ($cmsj.browser.safari) {
			setTimeout(window.close, 300);
		}
		else {
			window.close();
		}
	}

	function leaveRoom(fnOk) {
		for (var i = 0; i < ChatManager.ChatGroupManagers.length; i++) {
			ChatManager.ChatGroupManagers[i].LeaveRoom(fnOk);
		}
	}

	function recalculatePositions() {
		if ($messagesEnvelope == null) {
			messagesWebPart = ChatManager.GetWebpart(null, options.ChatRoomMessagesClientId, "ChatMessages", true);
			if (messagesWebPart) {
				$messagesEnvelope = messagesWebPart.GetEnvelope();
			}
		}
		if ($messagesEnvelope) {
			$messagesEnvelope.offset({ top: $pnlChatRoomWindowTop.outerHeight(true) + $divTop.position().top });
			$messagesEnvelope.height($divBottom.position().top - ($divTop.position().top + $pnlChatRoomWindowTop.outerHeight(true)));
			messagesWebPart.AdjustScrollbar();
		}
	}
};