function ChatLogin(opt) {
    var defaults = {
        clientID: -1,
        pnlChatUserLoggedIn: "",
        pnlChatUserChangeNicknameForm: "",
        pnlChatUserLoginError: "",
        pnlChatUserLoginRelog: "",
        txtChatUserLoginRelogNickname: "",
        lblChatUserLoginRelogNickname: "",
        btnChatUserLoginRelog: "",
        lblChatUserLoggedInInfoValue: "",
        btnChatUserChangeNicknameButton: "",
        txtChatUserChangeNicknameInput: "",
        lblChatUserLoginErrorText: "",
        btnLogout: "",
        resStrLogout: "",
        redirectURLLogout:"", 
        redirectURLEnter:"",
        lblChatUserLoginRelogText: "", 
        resStrLogAsAnonym: "", 
        resStrLogAsCMS: "",
        resStrNoService: ""
    };
    var that = this;

    this.Type = "ChatLogin";
    this.Options = defaults;
    $cmsj.extend(that.Options, opt);
    
    // Convert appropriate parameters to DOM objects
    var clientID = that.Options.clientID, 
        
        pnlLoggedIn = $cmsj("#" + that.Options.pnlChatUserLoggedIn),
        pnlChangeNicknameForm = $cmsj("#" + that.Options.pnlChatUserChangeNicknameForm),
        pnlError = $cmsj("#" + that.Options.pnlChatUserLoginError),
        pnlLoginRelog = $cmsj("#" + that.Options.pnlChatUserLoginRelog),

        lblLoginRelogText = $cmsj("#" + that.Options.lblChatUserLoginRelogText),
        txtLoginRelogNickname = $cmsj("#" + that.Options.txtChatUserLoginRelogNickname),
        lblLoginRelogNickname = $cmsj("#" + that.Options.lblChatUserLoginRelogNickname),
        lblLoggedInInfo = $cmsj("#" + that.Options.lblChatUserLoggedInInfoValue),

        txtChangeNickname = $cmsj("#" + that.Options.txtChatUserChangeNicknameInput),
        btnChangeNickname = $cmsj("#" + that.Options.btnChatUserChangeNicknameButton),

        lblError = $cmsj("#" + that.Options.lblChatUserLoginErrorText),

        btnLogout = $cmsj("#" + that.Options.btnLogout),

        loginFunction = function() {
            var fnok = null;
            if (that.Options.redirectURLEnter.length > 0) {
                fnok = function() { self.window.location = that.Options.redirectURLEnter; };
            }
            ChatManager.Login.CheckState(clientID, txtLoginRelogNickname.val(), fnok);
            return false;
        },

        logoutFunction = function () {
            var fnok = null;
            if (that.Options.redirectURLLogout.length > 0) {
                fnok = function () { self.window.location = that.Options.redirectURLLogout; };
            }
            ChatManager.Login.Logout(fnok);
            return false;
        };

    Inicialize(opt);
    

    // Displays login info form
    this.DisplayInfo = function () {

        // User is logged in
        if (ChatManager.Login.IsLoggedIn) {
            pnlChangeNicknameForm.hide();
            pnlLoginRelog.hide();
            lblLoggedInInfo.text(ChatManager.Login.Nickname + " ");
            btnLogout.val(that.Options.resStrLogout);
            pnlLoggedIn.show();
        }

        // User not logged in
        else {
            pnlLoggedIn.hide();
            pnlChangeNicknameForm.hide();

            if ((ChatManager.Settings.AnonymsAllowedGlobally == false) && ChatManager.Login.IsAnonymous) {
                pnlLoginRelog.hide();
                that.Error({ StatusMessage: ChatManager.Settings.AnonymsNotAllowedGlobalyMsg });
                return;
            }

            that.DisplayLoginUserForm();
            pnlLoginRelog.show();
        }
        that.ErrorClose();
    }


    // Displays login form
    this.DisplayLoginUserForm = function() {
        
        // User is anonymous
        if (ChatManager.Login.Nickname.length == 0) {
            lblLoginRelogNickname.hide();
            txtLoginRelogNickname.show();
            lblLoginRelogText.text(that.Options.resStrLogAsAnonym);
        }
        else {
            lblLoginRelogNickname.text($cmsj.ChatManager.Login.Nickname);
            txtLoginRelogNickname.hide();
            lblLoginRelogNickname.show();
            lblLoginRelogText.text(that.Options.resStrLogAsCMS);
        }
        pnlLoginRelog.show();
    }
    

    // Displays change nickname form
    this.DisplayChangeNicknameForm = function() {
        txtChangeNickname.val(ChatManager.Login.Nickname);
        pnlLoggedIn.hide();
        pnlLoginRelog.hide();
        pnlChangeNicknameForm.show();
    }
    

    // Displays error mesage
    this.Error = function(status) {
        lblError.html(status.StatusMessage);
        pnlError.show();
    }
    

    // Closes error message
    this.ErrorClose = function() {
        pnlError.hide();
    }


    this.HideContent = function () {
        pnlChangeNicknameForm.hide();
        pnlLoggedIn.hide();
        pnlLoginRelog.hide();
        that.Error({StatusMessage: that.Options.resStrNoService});
    }


    function Inicialize(opt) {

        // Change nickname when enter key pressed
        txtChangeNickname.bind("keydown", function(evt) {
            var e = window.event || evt;
            var key = e.keyCode;

            if (key == 13) {
                if (e.preventDefault) e.preventDefault();
                if (e.stopPropagation) e.stopPropagation();
                e.returnValue = false;

                ChatManager.Login.ChangeNickname(txtChangeNickname.val(), clientID);
                return false;
            }
        });

        $cmsj("#" + that.Options.btnChatUserLoginRelog).click(loginFunction);
        $cmsj("#" + that.Options.btnLogout).click(logoutFunction);

        // Login user when enter key pressed
        txtLoginRelogNickname.bind("keydown", function(evt) {
            var e = window.event || evt;
            var key = e.keyCode;

            if (key == 13) {
                if (e.preventDefault) e.preventDefault();
                if (e.stopPropagation) e.stopPropagation();
                e.returnValue = false;

                loginFunction();
                return false;
            }
        });
    }
    
    that.DisplayInfo();
};


// Inits chat login web part functionality
function InitChatLogin(opt) {
    InicializeChatManager();
    var logWp = new ChatLogin(opt);
    ChatManager.RegisterWebpart(logWp);
    if (ChatManager.WebService == null) {
        logWp.HideContent();
    }
}

