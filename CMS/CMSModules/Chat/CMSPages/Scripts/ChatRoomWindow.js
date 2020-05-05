function ChatRoomWindow(opt) {
	var options = {
		roomID: 0,
		password: "",
		pnlChatRoomPasswordPrompt: "",
		txtChatRoomPasswordPromptInput: "",
		btnChatRoomPasswordPromptSubmit: "",
		isOneToOne: true,
		pnlPasswordPromptError: "",
		pnlChatRoomWindow: "",
		ChatRoomMessagesClientID: "",
		btnClose: "",
		notificationManagerOptions: null
};
	
	$cmsj.extend(options, opt);
	$cmsj(Inicialize);

    var pnlChatRoomWindowTop = $cmsj(options.pnlChatRoomWindow),
	    messagesEnvelope = null,
	    that = this;

	this.RecalculatePositions = function () {
		if (messagesEnvelope == null) {
			var wp = ChatManager.GetWebpart(null, options.ChatRoomMessagesClientID, "ChatMessages", true);
			if (wp) {
				messagesEnvelope = wp.GetEnvelope();
			}
		}
		if (messagesEnvelope) {
			messagesEnvelope.offset({ top: pnlChatRoomWindowTop.outerHeight(true) + 35 });
		}
	}


	function Inicialize() {
		// Window event
	    $cmsj(window).bind("beforeunload", leaveRoomSynchronous);
	    $cmsj(window).bind("unload", leaveRoomSynchronous);

		ChatManager.IsPopupWindow = true;

		$cmsj(options.btnClose).click(function () {
			if (ChatManager.ChatGroupManagers[0].RoomID > 0) {
				leaveRoom(closeWindow);
			} else {
				closeWindow();
			}
			return false;
		});

		pnlChatRoomWindowTop.resize(that.RecalculatePositions);

		// Send mail to support
		if (options.isCustomerSupport) {
		    $cmsj(options.hplSupportSendMailClientID).click(function (event) {
				var params = "?roomid=" + options.roomID;

				// Open offline support request form.
				var win = window.open(ChatManager.Settings.SupportMailDialogURL + params, "OfflineSupportForm", ChatManager.Settings.SupportMailDialogSettings);
				win.focus();

				return false;
			});
		} 

		// Deal with password protected rooms
		if (options.password) {
			
			// Define overlay for password prompt
			ChatManager.DialogsHelper.SetDialogOverlay(options.pnlChatRoomPasswordPrompt);

			// Set event handling for input
			$cmsj(options.btnChatRoomPasswordPromptSubmit).click(enterPassword);

			$cmsj(options.txtChatRoomPasswordPromptInput).bind("keydown", function (evt) {
				var e = window.event || evt;
				var key = e.keyCode;

				if (key == 13) {
					if (e.preventDefault) e.preventDefault();
					if (e.stopPropagation) e.stopPropagation();
					e.returnValue = false;

					enterPassword();
				}
			});

			// Display prompt
			$cmsj(options.pnlPasswordPromptError).empty();
			ChatManager.DialogsHelper.DisplayDialog(options.pnlChatRoomPasswordPrompt);
			$cmsj(options.txtChatRoomPasswordPromptInput).focus();
		} 
		else {
			joinRoom(null);
		}

		RecalculatePositions();
		
		if (typeof (ChatNotificationManagerSetup) !== 'undefined') {
			ChatNotificationManagerSetup(options.notificationManagerOptions);
		}
	};
   
	
	function joinRoom(password, fnok, fnerr) {
	    for (var i = 0; i < $cmsj.ChatManager.ChatGroupManagers.length; i++) {
		    $cmsj.ChatManager.ChatGroupManagers[i].JoinRoom(options.roomID, password, fnok, fnerr);
		}
	};


	function leaveRoom(fnOk) {
		for (var i = 0; i < ChatManager.ChatGroupManagers.length; i++) {
			ChatManager.ChatGroupManagers[i].LeaveRoom(fnOk);
		}
	}


	function leaveRoomSynchronous() {
		if (ChatManager.ChatGroupManagers[0].RoomID > 0) {
			ChatManager.ChatGroupManagers[0].LeaveRoomSynchronous();
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


	function enterPassword() {
	    joinRoom($cmsj(options.txtChatRoomPasswordPromptInput).val(), function () {

			// When success, close the overlay
			ChatManager.DialogsHelper.CloseDialog(options.pnlChatRoomPasswordPrompt);
		}, function(err) {
		    $cmsj(options.pnlPasswordPromptError).html(err);
		});
		return false;
	};
}