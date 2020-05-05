/**
 * PopUp window module
 */
cmsdefine(['CMS/EventHub'], function (hub) {

    var popUpElement = null;
    /*
     * Event raised by the window when close button is clicked.
     */
    var POP_UP_CLOSE_EVENT = 'closePopup';

    /*
     * Event raised by currently open popup when it is about to be closed.
     */
    var CLOSE_EVENT_NAME = 'PopUpWindowClose';
    
  function currentDocumentOnClick(event) {
        if (!popUpElement.contains(event.target)) {
            close();
        }
    }

    function attachClosingEvents(target) {
        target.addEventListener(POP_UP_CLOSE_EVENT, close);
        document.addEventListener('mousedown', currentDocumentOnClick);
        window.addEventListener('blur', close);
    }

    function detachClosingEvents(target) {
        target.removeEventListener(POP_UP_CLOSE_EVENT, close);
        document.removeEventListener('mousedown', currentDocumentOnClick);
        window.removeEventListener('blur', close);
    }

    function open(target) {
        popUpElement = target;

        setTimeout(function () {
            attachClosingEvents(popUpElement);
        }, 0);
        popUpElement.classList.add('open');
    }

  function close() {
    if (popUpElement) {
            detachClosingEvents(popUpElement);
            popUpElement.classList.remove('open');

            hub.publish(CLOSE_EVENT_NAME);
            popUpElement = null;
        }
    }

    function toggle(target) {
        if (popUpElement) {
            var openTarget = target !== popUpElement;

            close();

            if (openTarget) {
                open(target);
            }
        } else {
            open(target);
        }
    }

    return {
        /**
         * Toggles the target pop up window and closes any other opened pop up windows.
         */
        toggle: toggle,
        closeEventName: CLOSE_EVENT_NAME
    };
});
