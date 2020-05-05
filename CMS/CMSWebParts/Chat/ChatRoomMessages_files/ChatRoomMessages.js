function ChatRoomMessagesWP(opt) {
	var defaults = {
		chatMessageTemplate: "",
		contentClientID: "",
		clientID: 0,
		groupID: 0,
		roomID: 0,
		count: 0,
		enableBBCode: false,
		displayInline: false,
		direction: 0,
		loadingDiv: "",
		envelopeID: "",
		pnlInformDialog:"", 
		btnInformDialogClose:"",
		container: ""
	};

	var that = this; 

	this.GroupManager = null;
	this.Type = "ChatMessages";
	
	this.Options = defaults;
	this.Options = $cmsj.extend(that.Options, opt);
	if (that.Options.count < 0) {
		that.Options.count = ChatSettings.FirstLoadMessagesCount;
	}


	var msgDivPrefix = "ChatMessage_ID_",
		msgDivPrefixSharp = "#" + msgDivPrefix,
		BBCodeParser = ChatManager.BBCodeParser,
		SmileysResolver = ChatManager.Settings.EnableSmileys ? ChatManager.SmileysResolver : null,
		FirstMessageTime = 0,
		chatMessageTemplateB = "chatMessageTemplateB_" + that.Options.clientID,
		envelope = $cmsj(that.Options.envelopeID),
		content = $cmsj(that.Options.contentClientID),
		container = $cmsj(that.Options.container),
		oneToOne = false,
		support = false,
		resolveUrlEnabled = ChatSettings.ResolveURLEnabled,
		RoomMessagesList; 

	Inicialize();


	this.Clear = function() {
		content.text("");
		envelope.hide();
	};


	this.ShowLoading = function(show) {
		if (show) {
			content.html(that.Options.loadingDiv);
			envelope.show();
		}
		else {
			content.find(".ChatMessagesWPLoading").remove();
		}
	};


	this.GetEnvelope = function() {
		if (envelope.length > 0) {
			return envelope;
		}
		return null;
	};


	// Show chat room messages
	this.ProcessResponse = function (messages) {
		RoomMessagesList = that.GroupManager.RoomMessagesList
		envelope.show();
		that.ShowLoading(false);

		if ((messages == null) || (messages.length == 0)) {
			return;
		}

		if (FirstMessageTime == 0) {
			FirstMessageTime = messages[0].PostedTime;
		}

		// Delete rejected messages which are doubled in list (marked as new and after that marked as rejected)
		for (var i = 0; i < messages.length; i++) {
			if (messages[i].IsRejected) {
				for (var j = 0; j < i; j++) {
					if (messages[j].MessageID == messages[i].MessageID) {
						messages[j].SkipThisMessage = true;
					}
				}
			}
		}

		var pinScrollbar = checkScrollbar();
		oneToOne = that.GroupManager.RoomInfo.IsOneToOne;
		support = that.GroupManager.RoomInfo.IsSupport;

		for (var i = 0; i < messages.length; i++) {
			var msg = messages[i];
			if (msg.SkipThisMessage) {
				continue;
			}

			// Save to global list
			var oldMessage = RoomMessagesList[msg.MessageID];
			if (oldMessage) {

				// if message is modified, we have to keep old sender's nickname
				msg.Nickname = oldMessage.Nickname;
			}
			RoomMessagesList[msg.MessageID] = msg;

			// Set-up message
			var isAuthorCurrentUser = ChatManager.Login.UserID == msg.AuthorID;
			msg.IsAuthorCurrentUser = isAuthorCurrentUser;
			msg.IsOneOnOne = oneToOne;
			msg.IsSupport = support;
			msg.SelectRecipient = false;

			// System message
			if (msg.SystemMessageType > 1) {
				msg.System = msg.SystemMessageType;
			}
			// Non-system message 
			else {
				if (!that.GroupManager.RoomInfo.IsSupport) {
					if (!isAuthorCurrentUser) {
						msg.SelectRecipient = "ChatManager.PassRecipient('" + that.Options.groupID + "','" + msg.AuthorID + "', '" + that.Options.pnlInformDialog + "'); return false;";
					} else if (msg.RecipientID != null) {
						msg.SelectPrevRecipient = "ChatManager.PassRecipient('" + that.Options.groupID + "','" + msg.RecipientID + "', '" + that.Options.pnlInformDialog + "'); return false;";
					}
				}

				// Whisper messgae
				if (msg.SystemMessageType == 1) {
					msg.Whisper = true;
				}
				else {
					msg.Whisper = false;

					// User has permission to reject message
					if (that.GroupManager.RoomInfo.IsCurrentUserAdmin && !oneToOne) {
						msg.RejectMessage = "ChatManager.RejectMessage(" + msg.MessageID + ",\"" + that.Options.groupID + "\"); return false;";
					}
					else {
						delete msg.RejectMessage;
					}
				}
			}

			// Display message
			if (msg.IsRejected) {
				rejectMessage(msg);
			}
			else if (msg.LastModified.valueOf() != msg.PostedTime.valueOf()) {
				modifyMessage(msg);
			}
			else {
				addNewMessage(msg);
			}
		}

		// Go through the messages and insert line breaks where needed
		if (!that.Options.displayInline) {
			insertLines(messages);
		}

		if (pinScrollbar) {
			that.AdjustScrollbar();
		}
	};


	// Adjusts the position of the scrollbar
	this.AdjustScrollbar = function() {
		container.scrollTop(content.outerHeight(true) - container.height());
	};


	this.AdminChanged = function() {
		for (var i in RoomMessagesList) {
			var msg = RoomMessagesList[i];
			if (that.GroupManager.RoomInfo.IsCurrentUserAdmin && (msg.Whisper == false)) {
				msg.RejectMessage = "ChatManager.RejectMessage(" + msg.MessageID + ",\"" + that.Options.groupID + "\"); return false;";
			}
			else {
				delete msg.RejectMessage;
			}
			modifyMessage(msg, true);
		}
	};


	function addNewMessage(msg) {
		// If message was inserted by client after post message, it must be deleted
		var messageDiv = content.find(msgDivPrefixSharp + msg.MessageID);
		if (messageDiv.length != 0) {
			messageDiv.remove();    
		}

		if (that.Options.direction == 1) {
		    $cmsj.tmpl(chatMessageTemplateB, [msg]).appendTo(content);
		}
		else {
		    $cmsj.tmpl(chatMessageTemplateB, [msg]).prependTo(content);
		}
		parseBBCodeAndSmileys(msg);
	};


	function modifyMessage(msg, adminChangedOnly) {
		var messageDiv = content.find(msgDivPrefixSharp + msg.MessageID);
		if (messageDiv.length == 0) {
			if (!adminChangedOnly && (msg.PostedTime > FirstMessageTime) && !msg.IsRejected) {
				addNewMessage(msg);
			}
			return;
		}

		// Set modified flag for transformation
		if (!adminChangedOnly) {
			msg.Modified = true;
		}
		messageDiv.empty();
		$cmsj.tmpl(chatMessageTemplateB, [msg]).prependTo(messageDiv);
		parseBBCodeAndSmileys(msg);
	};


	function rejectMessage(msg) {
		msg.Rejected = true;
		delete msg.RejectMessage;
		var msgDiv = content.find(msgDivPrefixSharp + msg.MessageID);
		if (msgDiv.length == 0) {
			if (that.Options.direction == 1) {
			    $cmsj.tmpl(chatMessageTemplateB, [msg]).appendTo(content);
			}
			else {
			    $cmsj.tmpl(chatMessageTemplateB, [msg]).prependTo(content);
			}
		}
		else {
			msgDiv.empty();
			$cmsj.tmpl(chatMessageTemplateB, [msg]).prependTo(msgDiv);
		}
		parseBBCodeAndSmileys(msg);
	};
   

	// Insert HTML tags for new lines to all messages in list
	function insertLines(messages) {
		var msgContent = null;
		for (var i = 0; i < messages.length; i++) {
			msgContent = content.find(msgDivPrefixSharp + messages[i].MessageID + ' span.Message');
			if ((msgContent != null) && (msgContent.length > 0)) {
				msgContent.html(msgContent.html().replace(/\n/g, "<br />"));
			}
		}
	}

	
	// parse BBCode and smileys in message
	function parseBBCodeAndSmileys(msg) {
		if (resolveUrlEnabled || that.Options.enableBBCode || (oneToOne && support) || (SmileysResolver != null)) {
			var msgContent = content.find(msgDivPrefixSharp + msg.MessageID + ' span.Message');
			if ((msgContent != null) && (msgContent.length > 0)) {
				var html = msgContent.html();
				if (SmileysResolver != null) {
					html = SmileysResolver.ResolveSmileys(html);
				}
				if (that.Options.enableBBCode || (oneToOne && support)) {
					html = BBCodeParser.ParseBBCode(html);
				}
				else if (resolveUrlEnabled) {
					html = BBCodeParser.ResolveURLs(html);
				}
				msgContent.html(html);
			}
		}
	}
	

	// Checks where the scrollbar is and if the message window should be scrolled down
	function checkScrollbar() {
	    var paddingTop = ($cmsj.browser.msie || $cmsj.browser.mozilla) ? container.css("padding-top").replace("px", "") : 0;
		var maxScrollTop = content.height() - container.height() - paddingTop;
		if (maxScrollTop < 0) {
			maxScrollTop = 0; 
		}
		if ((that.Options.direction == 1) && (container.scrollTop() >= maxScrollTop)) {
			return true;
		}
		return false;
	}


	function Inicialize() {
		// Build jQuery template    
		that.Options.chatMessageTemplate = '<div id="' + msgDivPrefix + '${MessageID}">' + that.Options.chatMessageTemplate + '</div>';
		$cmsj.template(chatMessageTemplateB, that.Options.chatMessageTemplate);

		// Define overlay for inform dialog
		ChatManager.DialogsHelper.SetDialogOverlay(that.Options.pnlInformDialog);

		// Set event handling for closing inform dialog
		$cmsj(that.Options.btnInformDialogClose).click(function () {
			ChatManager.DialogsHelper.CloseDialog(that.Options.pnlInformDialog);
			return false;
		});
	}
};


// Start function
function InitChatMessagesWebpart(opt) {
	InicializeChatManager();
	ChatManager.RegisterWebpart(new ChatRoomMessagesWP(opt));
};