cmsdefine([
    'CMS/MessageService',
    'CMS.Builder/MessageTypes',
    'CMS.Builder/ModalDialogService'
], function (msgService, messageTypes, ModalDialogService) {

    var Module = function (serverData) {
        var frame = document.getElementById(serverData.frameId);
        var targetOrigin = serverData.origin;
        var modalService = new ModalDialogService();

        var receiveMessage = function (event) {            
            if (event.origin !== targetOrigin) {
                return;
            }

            switch (event.data.msg) {
                case messageTypes.MESSAGING_ERROR:
                    msgService.showError(event.data.data, true);
                    frame.src = "about:blank";
                    break;

                case messageTypes.MESSAGING_EXCEPTION:
                    msgService.showError(event.data.data);
                    frame.src = "about:blank";
                    break;

                case messageTypes.MESSAGING_WARNING:
                    msgService.showWarning(event.data.data, true);
                    break;

                case messageTypes.CANCEL_SCREENLOCK:
                case messageTypes.CONFIGURATION_CHANGED:
                    window.top.CancelScreenLockCountdown && window.top.CancelScreenLockCountdown();
                    break;

                case messageTypes.OPEN_MODAL_DIALOG:
                    modalService.addModalDialogOverlay(document.body);
                    break;

                case messageTypes.CLOSE_MODAL_DIALOG:
                    modalService.removeModalDialogOverlay();
                    break;

                case messageTypes.CONFIRM:
                    const result = window.confirm(event.data.data.message);
                    frame.contentWindow.postMessage({ msg: messageTypes.CONFIRM_RESPONSE, result }, targetOrigin);
                    break;
            }
        };

        var registerPostMessageListener = function () {
            window.addEventListener('message', receiveMessage);
        };

        registerPostMessageListener();

        window.CMS = window.CMS || {};
    };

    return Module;
});
