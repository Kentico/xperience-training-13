// Bug fix for "__pendingCallbacks is not defined"
function WebForm_CallbackComplete_SyncFixed() {
    if (window.__pendingCallbacks) {
        for (var i = 0; i < window.__pendingCallbacks.length; i++) {
            callbackObject = window.__pendingCallbacks[i];
            if (callbackObject && callbackObject.xmlRequest && (callbackObject.xmlRequest.readyState == 4)) {
                if (!window.__pendingCallbacks[i].async) {
                    __synchronousCallBackIndex = -1;
                }
                window.__pendingCallbacks[i] = null;

                var callbackFrameID = "__CALLBACKFRAME" + i;
                var xmlRequestFrame = document.getElementById(callbackFrameID);
                if (xmlRequestFrame) {
                    xmlRequestFrame.parentNode.removeChild(xmlRequestFrame);
                }

                WebForm_ExecuteCallback(callbackObject);
            }
        }
    }
}

// Page completeness checking on postback
var notLoadedText = 'The page is not yet fully loaded, please wait until it loads.';
var originalUnload;

function __doPostBackWithCheck(eventTarget, eventArgument) {
    var editorsLoaded = true;
    if ((typeof (CKEDITOR) != 'undefined') && ((CKEDITOR.status == 'unloaded') || (CKEDITOR.status == 'loading'))) {
        editorsLoaded = false;
    }

    if (document.pageLoaded && editorsLoaded) {
        originalUnload = document.onunload;

        originalPostback(eventTarget, eventArgument);
    }
    else {
        alert(notLoadedText);
    }
}

function __OnUnload() {
    document.pageLoaded = false;

    originalUnload();
}