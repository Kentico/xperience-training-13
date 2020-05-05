/* Global variables that needs to be exposed because of external legacy code */
var getResultShort, ScreenLockLogoutUser, ScreenLockValidateUser, ScreenLockRedirectToLogon,
    HideScreenLockWarning, HideScreenLockWarningAndSync, CallServer,
    CancelScreenLockCountdown, ScreenLockEnterHandler;


(function() {

    var screenLockWarningOn = false,
        screenLockWarningDialogOn = false,
        screenLockIsLocked = false,
        screenLockCounter = 0,
        screenLockIntervalCounter = null,
        screenLockTimeout = null,

        longPing = 300000,
        shortPing = 60000,
        waitForPasscodVal = false,

        ARGUMENTS_SEPARATOR = "|",
            
        showScreenLockWarning = function () {
            screenLockWarningOn = true;
            if (screenLockIntervalCounter == null) {
                screenLockIntervalCounter = setInterval(updateScreenLockWarning, 1000);
            }

            var dialogOn = ($cmsj('#modalBack').is(':visible')) ? true : false;
            if (dialogOn) {
                screenLockWarningDialogOn = true;
            }

            $cmsj('#screenLockWarningDialog').show();

            updateScreenLockWarning();

            window.top.layouts[0].resizeAll();

            // Create and dispatch 'screenLockDialogShow' event.
            var event = document.createEvent('Event');
            event.initEvent('screenLockDialogShow', true, true);
            $cmsj('#screenLockWarningDialog')[0].dispatchEvent(event);
        },
        
        updateScreenLockWarning = function () {
            if (screenLockWarningOn) {
                if (screenLockCounter > 0) {
                    screenLockCounter--;
                }
                else {
                    clearInterval(screenLockIntervalCounter);
                    screenLockIntervalCounter = null;
                    CallServer(0);
                }
                $cmsj('#screenLockTime').html(screenLockCounter);
                $cmsj('#screenLockTimeDialog').html(screenLockCounter);

                var dialogOn = ($cmsj('#modalBack').is(':visible')) ? true : false;
                if (!dialogOn) {
                    if (screenLockWarningDialogOn) {
                        $cmsj('#screenLockWarningDialog').hide();

                        screenLockWarningOn = false;
                        screenLockWarningDialogOn = false;

                        // Dialog was closed while countdown
                        serverRequest('action');
                    }
                }
            }
        },

        showScreenLockDialog = function () {
            HideScreenLockWarning();
            addInteractionHandlers();

            showModalPopup('screenLockDialog', 'screenLockBackground');
            var txtPassword = $cmsj('#screenLockDialog input[type="password"]');
            txtPassword.val('');
            txtPassword.focus();
            screenLockIsLocked = true;
        },


        addInteractionHandlers = function () {
            // Extend focus handling in jQuery UI, so that screen lock can be focused above modal dialog
            if (window.CMS && window.CMS.AdvancedPopupHandler) {
                window.CMS.AdvancedPopupHandler.addInteractionHandler('ScreenLockInteraction',
                    function (event) {
                        return !!$cmsj(event.target).closest('#screenLockDialog').length;
                    });
            }
        },


        deleteInteractionHandlers = function () {
            if (window.CMS && window.CMS.AdvancedPopupHandler) {
                window.CMS.AdvancedPopupHandler.removeInteractionHandler('ScreenLockInteraction');
            }
        };


    getResultShort = function (result) {
        var resultArguments = new Array();

        if (result.indexOf('|') > -1) {
            resultArguments = result.split(ARGUMENTS_SEPARATOR);
        } else {
            resultArguments[0] = result;
        }
    
        switch (resultArguments[0]) {
            case 'logout':
                parent.location.replace(result.substring(7));
                break;
            case 'missingToken':
                if (resultArguments.length < 4) {
                    break;
                }
                $cmsj('[id$=_lblInstructions]').text(resultArguments[1]);
                $cmsj('[id$=_lblTokenInfo]').text(resultArguments[2]);
                $cmsj('[id$=_lblTokenID]').text(resultArguments[3]);
                $cmsj('#tokenBox').removeClass("hide");
            case 'waitingForPasscode':
                $cmsj('#screenLockDialogWarning').addClass("hide");
                $cmsj('#passcodeBox').removeClass("hide");
                $cmsj('#passwordBox').addClass("hide");
                $cmsj('#usernameBox').addClass("hide");
                waitForPasscodVal = true;
                break;
            case 'wrongPassc':
                $cmsj('#screenLockDialogWarning').text(resultArguments[1]);
                $cmsj('#screenLockDialogWarning').removeClass("hide");
                break;
            case 'valbad':
                $cmsj('#screenLockDialogWarning').removeClass("hide");
                $cmsj('[id$=_lblScreenLockWarningLogonAttempts]').addClass("hide");
                $cmsj('#screenLockDialog input[type="password"]').focus();
                break;
            case 'valok':
                hideModalPopup('screenLockDialog', 'screenLockBackground');
                deleteInteractionHandlers();
                $cmsj('#screenLockDialogWarning').addClass("hide");
                $cmsj('#passcodeBox').addClass("hide");
                $cmsj('#tokenBox').addClass("hide");
                $cmsj('[id$=_txtPasscode]').val("");
                $cmsj('#passwordBox').removeClass("hide");
                $cmsj('#usernameBox').removeClass("hide");
                screenLockIsLocked = false;
                CallServer(resultArguments[1] * 1000);
                waitForPasscodVal = false;
                break;
            case 'isLocked':
                if (result.substring(9) == 'True') {
                    if (!screenLockIsLocked) {
                        showScreenLockDialog();
                    }
                }
                CallServer(longPing);
                break;
            case 'lockScreen':
                if (!screenLockIsLocked) {
                    showScreenLockDialog();
                }
                CallServer(longPing);
                break;
            case 'hideWarning':
                hideModalPopup('screenLockDialog', 'screenLockBackground');
                deleteInteractionHandlers();
                screenLockIsLocked = false;
                if (screenLockWarningOn) {
                    HideScreenLockWarning();
                }
                CallServer(result.substring(12) * 1000);
                break;
            case 'showWarning':
                hideModalPopup('screenLockDialog', 'screenLockBackground');
                deleteInteractionHandlers();
                screenLockIsLocked = false;
                screenLockCounter = result.substring(12);
                showScreenLockWarning();
                CallServer(shortPing);
                break;
            case 'cancelOk':
                HideScreenLockWarning();
                CallServer(result.substring(9) * 1000);
                break;
            case 'disabled':
                HideScreenLockWarning();
                screenLockEnabled = false;
                clearTimeout(screenLockTimeout);
                break;
            case 'accountLocked':
                $cmsj('#screenLockDialogWarning').addClass("hide");
                $cmsj('[id$=_lblScreenLockWarningLogonAttempts]').removeClass("hide");
                $cmsj('#screenLockSignInButton').show();
                $cmsj('#screenLockUnlockButton').hide();
                $cmsj('[id$=_btnScreenLockSignOut]').hide();
                if (!screenLockIsLocked) {
                    showScreenLockDialog();
                }
                break;
        }
    };


    ScreenLockLogoutUser = function () {
        serverRequest('logout');
    };


    ScreenLockValidateUser = function () {
        if (waitForPasscodVal) {
            serverRequest('validPasscode|' + $cmsj('[id$=_txtPasscode]').val());
            return;
        }
        serverRequest('validate|' + $cmsj('[id$=_txtScreenLockDialogPassword]').val());
    };


    ScreenLockRedirectToLogon = function (logonpage) {
        parent.location.replace(logonpage);
    };


    HideScreenLockWarning = function () {
        clearInterval(screenLockIntervalCounter);
        screenLockIntervalCounter = null;

        $cmsj('#screenLockWarningDialog').hide();

        screenLockWarningOn = false;
        screenLockWarningDialogOn = false;

        window.top.layouts[0].resizeAll();

        // Create and dispatch 'screenLockDialogHide' event.
        var event = document.createEvent('Event');
        event.initEvent('screenLockDialogHide', true, true);
        $cmsj('#screenLockWarningDialog')[0].dispatchEvent(event);
    };


    HideScreenLockWarningAndSync = function (timeout) {
        if (timeout > 0) {
            HideScreenLockWarning();
        }
        CallServer(timeout * 1000);
        screenLockPinging = true;
    };


    CallServer = function (timeoutPeriod) {
        clearTimeout(screenLockTimeout);
        screenLockTimeout = setTimeout('serverRequest("isLocked");', timeoutPeriod);
    };


    CancelScreenLockCountdown = function () {
        serverRequest('cancel');
    };


    ScreenLockEnterHandler = function (event) {
        if (event.which == 13 || event.keyCode == 13) {
            ScreenLockValidateUser();
            return false;
        }
        return true;
    };

}());