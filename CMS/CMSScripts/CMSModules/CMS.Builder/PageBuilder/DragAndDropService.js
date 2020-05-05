/**
 * Service for cancelling drag and drop from CMS frames.
 */
cmsdefine([
    'CMS.Builder/MessageTypes'
], function (messageTypes) {
    var REMOVE_EVENTS_CSS = 'a, button, select, li, img, i { pointer-events:none !important; } * { cursor:not-allowed !important; }';
    var REMOVE_EVENTS_DATA_ATTRIBUTE = 'data-kentico-dnd-style';
    var ESC_KEY_CODE = 27;

    function addDnDCancellationEvents() {
        var currentWindow = window;

        while (currentWindow !== currentWindow.parent) {
            addDndCancellationEventsInternal(currentWindow);
            currentWindow = currentWindow.parent;
        }

        // The top most window won't be handled in the loop as if window does not have a parent window.parent will return the window itself
        addDndCancellationEventsInternal(currentWindow);
    }

    function addDndCancellationEventsInternal(window) {
        // Set cursor to not allowed in all of the parent frames
        window.document.body.style.cursor = 'not-allowed';

        // Add style node with CSS that removes pointer events
        var removePointerEventsNode = document.createElement('style');
        removePointerEventsNode.setAttribute(REMOVE_EVENTS_DATA_ATTRIBUTE, '');
        removePointerEventsNode.innerHTML = REMOVE_EVENTS_CSS;

        // Add the style node and cancellation events to the document
        window.document.querySelector('head').appendChild(removePointerEventsNode);
        window.addEventListener('mouseup', triggerMouseUpFromOtherFrames);
        window.addEventListener('keyup', triggerESCFromOtherFrames);
    }

    function removeDnDCancellationEvents() {
        var currentWindow = window;

        while (currentWindow !== currentWindow.parent) {
            // Set cursor in parent frames to auto
            removeDnDCancellationEventsInternal(currentWindow);
            currentWindow = currentWindow.parent;
        }

        // The top most window won't be handled in the loop as if window does not have a parent window.parent will return the window itself
        removeDnDCancellationEventsInternal(currentWindow);
    }

    function removeDnDCancellationEventsInternal(window) {
        window.document.body.style.cursor = 'auto';

        // Remove pointer event disabling style node from parent frames
        window.document.head.removeChild(window.document.querySelector('[' + REMOVE_EVENTS_DATA_ATTRIBUTE + ']'));

        // Remove DnD cancellation events from parent frames
        window.removeEventListener('mouseup', triggerMouseUpFromOtherFrames);
        window.removeEventListener('keyup', triggerESCFromOtherFrames);
    }

    function triggerMouseUpFromOtherFrames() {
        sendEndDnDMessage();
    }

    function triggerESCFromOtherFrames(evt) {
        if (evt.which === ESC_KEY_CODE) {
            sendEndDnDMessage();
        }
    }

    function sendEndDnDMessage() {
        if (window.frames.length > 0) {
            window.frames[0].window.postMessage({ msg: messageTypes.MESSAGING_DRAG_STOP }, '*');
        }
    }

    return {
        addDnDCancellationEvents: addDnDCancellationEvents,
        removeDnDCancellationEvents: removeDnDCancellationEvents
    };
});
