//
// ChatMessageSendWP object
//
function ChatMessageSendWP(opt) {
	var defaults = {
		roomID: 0,
		groupID: 0,
		inputClientID: "",
		bbCodeClientID: "",
		buttonClientID: "",
		chbWhisperClientID: "",
		drpRecipientClientID: "",
		pnlRecipientContainerClientID: "",
		noneLabel: "",
		enableBBCode: false,
		btnCannedResponses: "",
		pnlContent: "",
		envelopeID: "",
		informDialogID: "",
		btnInformDialogClose: ""
	};

	var that = this;
	this.Options = defaults;
	this.GroupManager = null;
	this.Type = "ChatSender";

	setOptions(opt);
	
    var input = $cmsj(this.Options.inputClientID),
		submit = $cmsj(this.Options.buttonClientID),
		drpRecipient = $cmsj(this.Options.drpRecipientClientID),
	    pnlRecipientContainer = $cmsj(this.Options.pnlRecipientContainerClientID),
		chbWhisper = $cmsj(this.Options.chbWhisperClientID),
		btnCannedResponses = $cmsj(this.Options.btnCannedResponses),
		pnlContent = $cmsj(this.Options.pnlContent),
		envelope = $cmsj(this.Options.envelopeID),
		enableBBCode = this.Options.enableBBCode,
		bbCodeParser = null,
		pnlInformDialog = $cmsj(this.Options.informDialogID);
	if (enableBBCode) {
		bbCodeParser = ChatManager.BBCodeParser;
	}
	
	var none = $cmsj('<option></option>').val(0).text(this.Options.noneLabel);
	initSender();
	

	// Clear form
	this.Clear = function() {
		input.val('');
		if (that.GroupManager.RoomID > 0) {
			envelope.show();
			input.focus();
		} else {
			drpRecipient.empty();
			envelope.hide();
		}
	};


	this.ProcessResponse = function(users) {
		if (drpRecipient.length > 0 && !that.GroupManager.RoomInfo.IsSupport) {
			pnlRecipientContainer.show();
			var selected = drpRecipient.val();
			drpRecipient.find("option").remove();

			if (none) {
				drpRecipient.append(none);
			}

			for (var i = 0; i < users.length; i++) {
				var usr = users[i];
				if ((ChatManager.Login.UserID == usr.ChatUserID) || (usr.IsOnline == false)) {
					continue;
				}
				var o = {
					value: usr.ChatUserID,
					text: usr.Nickname
				};
				drpRecipient.append(
					$cmsj('<option></option>').val(o.value).text(o.text)
				);

			}
			drpRecipient.val(selected);
		} else {
			pnlRecipientContainer.hide();
		}
	};

	this.SetRecipient = function(id) {
		if (drpRecipient.length == 0) {
			return true;
		}
		drpRecipient.val(id);
		if (drpRecipient.val() == id) {
			input.focus();
			return true;
		}
		return false;
	};


	// Private methods

	// Set parameters
	function setOptions(opt) {
		if (typeof (opt) != 'object') {
			opt = {};
		}
		that.Options = $cmsj.extend(that.Options, opt);
	}


	// Init sender
	function initSender() {
		if ((drpRecipient.length > 0) && (chbWhisper.length > 0)) {
			drpRecipient.append(none);
		}

		btnCannedResponses.click(function() {
			input.autocomplete("search", "#");
			input.focus();
			return false;
		});
	
		// Register event to buttton
		submit.click(sendMessage);

		// Register pressing enter to same action.
		input.bind("keydown", function(evt) {
			var e = window.event || evt;
			var key = e.keyCode;

			if ((key == 13) && (!$cmsj.ChatManager.IntellisenseActive)) {
				if (e.preventDefault) e.preventDefault();
				if (e.stopPropagation) e.stopPropagation();
				e.returnValue = false;

				if (e.shiftKey || e.ctrlKey) {
					var newline = $cmsj.browser.opera ? '\r\n' : '\n';

					if (input.getSelection().length == 0) {
						input.insertAtCaretPos(newline);
					} else {
						input.replaceSelection(newline);
					}
					input.focus();
				} else {
					sendMessage();
				}
			}
		});

		// Define overlay for inform dialog
		ChatManager.DialogsHelper.SetDialogOverlay(pnlInformDialog);

		// Set event handling for closing inform dialog
		$cmsj(that.Options.btnInformDialogClose).click(function () {
			ChatManager.DialogsHelper.CloseDialog(pnlInformDialog);
			return false;
		});
	}


	// Send message to ChatManager
	function sendMessage() {

		var msg = trim(input.val());
		if(msg.length == 0){
			return false;
		}
		
		if (enableBBCode) {
			 // message can't be empty after resolving bbcode
			 if (trim(bbCodeParser.ParseBBCode(msg, true)) == "") {
				 ChatManager.DialogsHelper.DisplayDialog(pnlInformDialog);
				 return false;
			 }
		 }

		if ((drpRecipient.length > 0) && (drpRecipient.val() > 0)) {
			that.GroupManager.SendMessageToUser(drpRecipient.val(), input.val(), that);
		} else {
			that.GroupManager.SendMessage(input.val(), that);
		}

		if ((chbWhisper.length > 0) && !chbWhisper.attr('checked')) {
			drpRecipient.val(0);
		}

		triggerPostMessageEvent();
		return false;
	}
	

	function trim(stringToTrim) {
		return stringToTrim.replace(/^\s+|\s+$/g, "");
	}


	function triggerPostMessageEvent() {
		if (window.PostMessageEvent) {
			window.PostMessageEvent();
		}
	}
};


// Start function
function InitChatSenderWebpart(opt) {
	InicializeChatManager();
	ChatManager.RegisterWebpart(new ChatMessageSendWP(opt));
};