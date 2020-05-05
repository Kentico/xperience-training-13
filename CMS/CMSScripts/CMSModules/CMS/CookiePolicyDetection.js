/**
 * Detects whether the browser blocks third-party cookies for nested frames. If so it displays a message.
 */
cmsdefine(['CMS/MessageService'], function (MessageService) {
    return function (data) {
        var iframeSrc = data.iframeUrl,
            cookieName = data.cookieName,
            cookieValue = data.cookieValue,
            targetOrigin = data.targetOrigin,
            messageText = data.message,
            timeoutTimer = null,
            showMessage = function () {
                MessageService.showWarning(messageText, true);
            };

        var iframeElement = document.createElement('iframe');

        iframeElement.style.display = "none";
        iframeElement.src = iframeSrc;
        iframeElement.onload = function () {
            iframeElement.contentWindow.postMessage(cookieName, targetOrigin);

            // Schedule callback that displays the message in case when the iframe doesn't respond to the post message.
            timeoutTimer = setTimeout(showMessage, 2e3);
        };

        window.addEventListener('message', function (event) {
            clearTimeout(timeoutTimer);

            if (event.origin === targetOrigin) {
                // If event.data is falsy it means that browser hasn't set the cookie
                // If event.data contains a value, the value might not have been set by the iframe, so let's compare that value to the expected one.
                if (!event.data || event.data !== cookieValue) {
                    showMessage();
                }
            }
        });

        // Adding to the DOM needs to be the last action as it causes the browser to begin loading the page in the iframe.
        document.lastChild.appendChild(iframeElement);
    };
});