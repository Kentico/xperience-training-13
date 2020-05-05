function ChatNotification(opt) {
    
    var defaults = {
        template: "",
        clientID: -1,
        pnlChatNotificationEmpty: "",
        pnlChatNotificationFull: "",
        btnChatNotificationFullLink: "",
        lblChatNotificationFullTextNumber: "",
        pnlChatNotificationNotifications: "",
        pnlChatNotificationNotificationsList: "",
        chatRoomGUID: "",
        btnChatNotificationPromptClose: "",
        btnChatNotificationFullLink: "",
        wpPanelID: "",
        envelopeID: "",
        bubbleBtnShow: "",
        bubbleBtnClose: "",
        bubbleLabel: "",
        bubblePanel: "",
        strNoNotif: "",
        strNewNotif: "",
        bubbleEnabled: true,
        isPreview: false
    };
    
    var that = this;

    this.Type = "ChatNotification";
    this.GroupManager = null;
    
    this.Options = defaults;
    $cmsj.extend(that.Options, opt);

    var envelope = $cmsj(that.Options.envelopeID),
        NotificationsList = ChatManager.NotificationsList,
        pnlStatEmpty = $cmsj(that.Options.pnlChatNotificationEmpty),
        pnlStatNotif = $cmsj(that.Options.pnlChatNotificationFull),
        lblNumberOfNotif = $cmsj(that.Options.lblChatNotificationFullTextNumber),
        pnlNotifications = $cmsj(that.Options.pnlChatNotificationNotifications),
        pnlNotificationsList = $cmsj(that.Options.pnlChatNotificationNotificationsList),
        buildTemplate = "Notification" + that.Options.clientID,
        bubbleLabel = $cmsj(that.Options.bubbleLabel),
        bubblePanel = $cmsj(that.Options.bubblePanel)
        ;

    Inicialize();


    this.ShowHide = function() {
        if (ChatManager.Login.IsLoggedIn == true) {
            envelope.show();
        }
        else {
            envelope.hide();
        }
    }

   
    // Processes response from ChatManager, display number of notifications
    this.ProcessResponse = function () {
        var numberOfNotifications = that.CheckNotifications();

        if (pnlNotifications.is(":visible")) {
            if (numberOfNotifications == 0) {
                ChatManager.DialogsHelper.CloseDialog(pnlNotifications);
            }
            else {
                that.DisplayNotificationList();
            }
        }
        else if (that.Options.bubbleEnabled) {
            if ((numberOfNotifications > 0) && ($cmsj(".ChatNotificationBubble:visible").length == 0)) {
                that.ShowBubble();
            }
            else if (numberOfNotifications == 0) {
                that.HideBubble();
            }
        }
    }
    

    // Checks if there are open notifications and updates info panel
    this.CheckNotifications = function () {

        // Count notifications
        var numberOfNotifications = 0;
        for (var indx in NotificationsList) {
            if (!isNaN(indx) && NotificationsList[indx] != null) {
                numberOfNotifications++;
            }
        }

        // Check if there are notifications
        if (numberOfNotifications == 0) {
            pnlStatEmpty.show();
            pnlStatNotif.hide();
            bubbleLabel.text(that.Options.resNoNotif);
        }
        else {
            pnlStatEmpty.hide();
            pnlStatNotif.show();
            lblNumberOfNotif.text(numberOfNotifications);
            bubbleLabel.text(that.Options.resNewNotif + " (" + numberOfNotifications + ")");
        }
        return numberOfNotifications;
    }
    

    // Displays notification list
    this.DisplayNotificationList = function () {

        // Empty panel with notifications
        pnlNotificationsList.empty();
        that.HideBubble();

        // Start the list
        var list = $cmsj("<table class=\"ChatNotificationList\">");

        // Add action buttons to the data in list
        for (var i in NotificationsList) {
            if (!isNaN(i) && (NotificationsList[i] != null)) {
                $cmsj.tmpl(buildTemplate, NotificationsList[i]).appendTo(list);
            }
        }

        // Finish the list
        pnlNotificationsList.append(list);

        // Open dialog
        if (!pnlNotifications.is(":visible")) {
            ChatManager.DialogsHelper.DisplayDialog(pnlNotifications);
            ChatManager.ActiveNotificationWP = that.Options.clientID;
        }
    }


    // Clears the content of the webpart - used when user logs out
    this.Clear = function () {
        ChatManager.NotificationsList.length = 0;
        if (ChatManager.Login.IsLoggedIn) {
            envelope.show();
        }
        else {
            envelope.hide();
        }
        pnlStatNotif.hide();
        bubblePanel.hide();
    }


    // Shows notification bubble
    this.ShowBubble = function () {
        bubblePanel.slideDown(1000);
    }


    // Hides notification bubble
    this.HideBubble = function () {
        $cmsj(".ChatNotificationBubble:visible").slideUp(1000);
    }

    // Initialize ChatRooms object
    function Inicialize() {

        // Build jQuery templates
        $cmsj.template(buildTemplate, "<tr id=\"ChatNotificationID_${NotificationID}\">" + that.Options.template + "</tr>");

        // Define overlay for displaying notifications
        ChatManager.DialogsHelper.SetDialogOverlay(pnlNotifications);

        // Set event handling for closing notification prompt
        $cmsj(that.Options.btnChatNotificationPromptClose).click(function() {
            ChatManager.DialogsHelper.CloseDialog(pnlNotifications);
            return false;
        });

        // Set event handling for opening notification prompt
        $cmsj(that.Options.btnChatNotificationFullLink).click(function() {
            that.DisplayNotificationList();
            return false;
        });

        // Set event handling for opening notification prompt from info bubble
        $cmsj(that.Options.bubbleBtnShow).click(function () {
            that.DisplayNotificationList();
            return false;
        });

        // Set event handling for closing notification info bubble
        $cmsj(that.Options.bubbleBtnClose).click(function () {
            that.HideBubble();
            return false;
        });
        
        if (typeof (ChatNotificationManagerSetup) !== 'undefined') {
            ChatNotificationManagerSetup(opt.notificationManagerOptions);
        }
    }
    
    that.ShowHide();
}


// Inits chat notification web part functionality
function InitChatNotification(opt) {
    InicializeChatManager();
    ChatManager.RegisterWebpart(new ChatNotification(opt));
}