window.AdvancedModalDialogs = true;

var $document = $cmsj(document);
var $window = $cmsj(window);
var $visiblePopup = null;

function generateGuid() {
    var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

function getAbsolute(percent, ref) {
    if (percent.substr && percent.substr(percent.length - 1) === '%') {
        var leftPercent = percent.substr(0, percent.length - 1);
        percent = ref * leftPercent / 100;
    }
    return percent;
}

function getAbsoluteSize(width, height) {
    width = getAbsolute(width, $window.width());
    height = getAbsolute(height, $window.height());
    return { width: width, height: height };
}

function storeModal(iframeElem) {
    var index = 0;
    var stored = false;
    for (var i = 0; i < window.allModals.length; i++) {
        if (!window.allModals[i]) {
            // Add to free space in array
            window.allModals[i] = iframeElem;
            index = i;
            stored = true;
        }
    }
    if (!stored) {
        // Add to the end of array
        index = window.allModals.length;
        window.allModals[window.allModals.length] = iframeElem;
    }
    return index;
}

function getPercentSize(width, height) {
    var percentWidth;
    var percentHeight;
    if (width.substr && width.substr(width.length - 1) === '%') {
        percentWidth = width;
    } else {
        percentWidth = width / ($window.width() / 100);
        if (percentWidth > 95) {
            percentWidth = 95;
        }
        percentWidth += '%';
    }

    if (height.substr && height.substr(height.length - 1) === '%') {
        percentHeight = height;
    } else {
        percentHeight = height / ($window.height() / 100);
        if (percentHeight > 95) {
            percentHeight = 95;
        }
        percentHeight += '%';
    }

    return { width: percentWidth, height: percentHeight };
}

function containsWindow(popupWindow, windowToFind, recursive) {
    if (recursive == undefined) {
        recursive = true;
    }
    if (popupWindow == windowToFind) {
        return true;
    } else {
        if (popupWindow.frames && popupWindow.frames.length > 0) {
            for (var i = 0; i < popupWindow.frames.length; i++) {
                var frame = popupWindow.frames[i];
                if (recursive) {
                    if (containsWindow(frame, windowToFind)) {
                        return true;
                    }
                } else if (popupWindow == windowToFind) {
                    return true;
                }
            }
        }
    }
    return false;
}

function isFullScreen() {
    var dialogElem = $visiblePopup.parent();
    return dialogElem.data('fullScreen');
}

function toggleFullScreen() {
    var fullScreenSize = { width: '95%', height: '95%' };
    var position = {};
    var currentOffset;
    var dialogElem = $visiblePopup.parent();
    var size;
    if (isFullScreen()) {
        var origSize = dialogElem.data('originalSize');
        size = getAbsoluteSize(origSize.width, origSize.height);
        currentOffset = dialogElem.data('originalOffset');
        dialogElem.find('.ui-resizable-handle').show();
    } else {
        size = getAbsoluteSize(fullScreenSize.width, fullScreenSize.height);
        currentOffset = { top: (($window.height() - size.height) / 2), left: (($window.width() - size.width) / 2) };
        dialogElem.find('.ui-resizable-handle').hide();
    }

    dialogElem.data('actualOffset', currentOffset);
    dialogElem.data('actualSize', size);

    position.left = currentOffset.left + $document.scrollLeft();
    position.top = currentOffset.top + $document.scrollTop();

    $visiblePopup.width(size.width).height(size.height);
    dialogElem.width(size.width).height(size.height);
    dialogElem.css('left', position.left).css('top', position.top);

    dialogElem.data('fullScreen', !dialogElem.data('fullScreen'));
}

function getExistingModal(name, opener) {
    for (var i = 0; i < window.allModals.length; i++) {
        var frame = window.allModals[i];
        if (frame != null) {
            var frameObject = frame.get(0);
            var win = frameObject.contentWindow;
            if ((frameObject.name == name) && (win.wopener == opener)) {
                return { frame: frame, index: i };
            }
        }
    }
    return null;
}

function addBackgroundClickHandler(win) {
    var topOverlaySelector = '.ui-widget-overlay:last';
    $cmsj(topOverlaySelector).on('click', function () {
        $cmsj(this).unbind('click');
        closeDialog(win);
    });
}

function removeBackgroundClickHandler() {
    var topOverlaySelector = '.ui-widget-overlay:last';
    $cmsj(topOverlaySelector).unbind('click');
}

function normalizeDimension(d) {
    if (d.toString().indexOf('px') != -1) {
        return parseInt(d, 10);
    }
    return d;
}

function advancedModal(url, name, width, height, otherParams, noWopener, forceModal, setTitle, opener) {
    if (window.allModals == undefined) {
        window.allModals = [];
    }

    // Create dialog if it doesn't exist already
    if (getExistingModal(name, opener) == null) {
        if (width == null) {
            width = '95%';
        }
        if (height == null) {
            height = '95%';
        }
        width = normalizeDimension(width);
        height = normalizeDimension(height);
        var percentSize = getPercentSize(width, height);
        width = percentSize.width;
        height = percentSize.height;

        if (typeof otherParams == 'string') {
            var parameters = otherParams.split(',');
            otherParams = {};
            parameters.forEach(function (property) {
                var splits = property.trim().split('=');
                var splitValue = splits[1].toLowerCase().trim();
                otherParams[splits[0].trim()] = splitValue === 'yes' ||
                                                splitValue === 'true' ||
                                                splitValue === '1';
            });
        }

        // Create new frame
        var $modalFrame = $cmsj(document.createElement('iframe'));
        storeModal($modalFrame);

        // Append frame to wrapping element
        var $modalWrap = $cmsj('#modalWrap');
        $modalWrap.append($modalFrame);

        // Get absolute size
        var size = getAbsoluteSize(width, height);

        // Init frame params
        $modalFrame.attr('width', size.width);
        $modalFrame.attr('height', size.height);
        $modalFrame.attr('class', 'UIPopupDialog');
        $modalFrame.attr('frameBorder', 0);
        $modalFrame.attr('id', generateGuid());
        $modalFrame.attr('name', name);
        $modalFrame.attr('src', 'about:blank');

        // Set currently visible popup
        $visiblePopup = $modalFrame;

        if (!setTitle) {
            $modalFrame.get(0).stopAutoTitle = true;
        }
        storeWopener($modalFrame, opener);

        // Set interaction criteria for allowing eventual interaction with outside elements
        $cmsj.widget('ui.dialog', $cmsj.ui.dialog, {
            _allowInteraction: function (event) {
                return window.CMS.AdvancedPopupHandler.getInteractionAllowance(event) || this._super(event);
            }
        });

        var $modalDialog = $modalFrame.dialog({
            resizable: otherParams.resizable,
            modal: otherParams.modal || forceModal,
            closeOnEscape: false,
            width: size.width,
            height: size.height,
            zIndex: 1000,
            create: function () {
                // Load frame when dialog is shown to prevent multiple loading
                $modalFrame.attr('src', url);
            },
            // Append loader and show loading message
            open: function (event) {
                var iframe = event.target;
                var dialogIsLoaded = false;

                $cmsj(iframe).ready(function () {
                    // Stop propagation of events
                    if (window.CMS && window.CMS.AdvancedPopupHandler) {
                        window.CMS.AdvancedPopupHandler.stopPropagation(iframe.contentWindow);
                    }

                    if (!dialogIsLoaded) {
                        displayLoader();
                    }
                });

                $cmsj(iframe).load(function () {
                    // Stop propagation of events
                    if (window.CMS && window.CMS.AdvancedPopupHandler) {
                        window.CMS.AdvancedPopupHandler.stopPropagation(iframe.contentWindow);
                    }

                    // There is special case for framesets in webkit browsers.
                    // If there is frameset we need to check if is loaded or still loading
                    // to properly hide loading message.
                    if (containsFrame(iframe.contentDocument) && $cmsj.browser.webkit) {
                        var framesCount = 0;
                        $cmsj('frame', iframe.contentDocument).each(function (index, frame) {
                            if (!frame.contentDocument.pageLoaded) {
                                framesCount++;
                                frame.onload = function () {
                                    framesCount--;

                                    if (allFramesLoaded(framesCount)) {
                                        hideLoader();
                                    }
                                };
                            }
                        });

                        if (allFramesLoaded(framesCount)) {
                            hideLoader();
                        }
                    } else {
                        hideLoader();
                    }
                });

                function allFramesLoaded(count) {
                    return count < 1;
                }

                function containsFrame(element) {
                    return $cmsj('frame', element).length !== 0;
                }

                function displayLoader() {
                    if (window.Loader) {
                        window.Loader.show();
                    }
                }

                function dialogContainsCloseButton(iframe) {
                    var result = false;

                    if (containsFrame(iframe.contentDocument)) {
                        $cmsj('frame', iframe.contentDocument).each(function (index, frame) {
                            if ($cmsj('.close-button', frame.contentDocument).length > 0) {
                                result = true;
                            }
                        });
                    } else {
                        result = $cmsj('.close-button', iframe.contentDocument).length > 0;
                    }

                    return result;
                }

                function hideLoader() {
                    if (window.Loader) {
                        window.Loader.hide();
                    }
                    dialogIsLoaded = true;

                    if (dialogContainsCloseButton(iframe)) {
                        removeBackgroundClickHandler();
                    }
                }
            }
        })
        .css({
            width: size.width + 'px',
            height: size.height + 'px',
            padding: 0
        });

        // Get current offset and dimensions
        var dialogElem = $visiblePopup.parent();
        var offset = dialogElem.offset();
        var completeOffset = { left: offset.left - $document.scrollLeft(), top: offset.top - $document.scrollTop() };
        dialogElem.data('originalOffset', completeOffset);
        dialogElem.data('actualOffset', completeOffset);

        var actualSize = { width: dialogElem.width(), height: dialogElem.height() };
        dialogElem.data('originalSize', actualSize);
        dialogElem.data('actualSize', actualSize);

        // Add cms-bootstrap css class to support
        // loader styles in on-site editing dialogs
        dialogElem.addClass('cms-bootstrap');

        // Add a CMS specific class to the dialog overlay to be able to set specific styles only to the CMS overlays
        // This way we avoid collisions between CMS and customers' jQuery-UI styles
        $cmsj('.ui-widget-overlay').addClass('cms-ui-widget-overlay');

        // Ensure closing by clicking on background overlay
        // Currently enabled only till dialog successfully loads, after that user should use close button
        // If dialog is not loaded correctly, dialog can be closed by click on overlay
        addBackgroundClickHandler($modalDialog.get(0).contentWindow);

        // Ensure fullscreen if requested
        if (otherParams.openInFullscreen && !isFullScreen()) {
            toggleFullScreen();
        }
    }
}

function isTitleWindow(popupWindow, windowToExamine) {
    if (popupWindow == windowToExamine) {
        return true;
    } else {
        if (popupWindow.frames && popupWindow.frames.length > 0) {
            var frame = popupWindow.frames[0];
            if (containsWindow(frame, windowToExamine, false)) {
                return true;
            } else {
                return isTitleWindow(frame, windowToExamine);
            }
        }
    }
    return false;
}

function closeDialog(win) {
    for (var i = 0; i < window.allModals.length; i++) {
        var frame = window.allModals[i];
        var currentWindow = frame.get(0).contentWindow;
        // Find the iframe to hide
        if (containsWindow(currentWindow, win)) {
            // Allow propagation of events
            if (window.CMS && window.CMS.AdvancedPopupHandler) {
                window.CMS.AdvancedPopupHandler.allowPropagation(currentWindow);
            }

            // Prevent IE to reload iframe on close
            if (!$cmsj.browser.opera) {
                frame.attr('src', 'about:blank');
            }
            // Remove window opener from collection
            removeWopener(win);

            // Remove iframe from DOM
            frame.remove();

            // Remove iframe from collection
            window.allModals.splice(i, 1);

            $cmsj(':visible:first').focus();

            break;
        }
    }
    for (i; i >= 0; i--) {
        // Get previous iframe used as popup
        var hiddenFrame = window.allModals[i];
        if (hiddenFrame != null) {
            // Set it as currently visible
            $visiblePopup = hiddenFrame;
            break;
        }
        else {
            $visiblePopup = null;
        }
    }
}

function initWopeners() {
    if (window.allWopeners == undefined) {
        window.allWopeners = [];
    }
}

function storeWopener(frame, win) {
    initWopeners();
    window.allWopeners[frame.get(0).id] = win;
}

function removeWopener(win) {
    initWopeners();
    var key = getWopenerKey(win);
    if (key) {
        allWopeners[key] = null;
    }
}

function getWopenerKey(win) {
    initWopeners();
    if (window.allModals) {
        for (var i = 0; i < window.allModals.length; i++) {
            var wopenerFrame = window.allModals[i];
            if (wopenerFrame != null) {
                var wopWin = wopenerFrame.get(0).contentWindow;
                if (containsWindow(wopWin, win)) {
                    return wopenerFrame.get(0).id;
                }
            }
        }
    }
    return null;
}

function getWopener(win) {
    initWopeners();
    var key = getWopenerKey(win);
    if (key) {
        return allWopeners[key];
    } else {
        return null;
    }
}

function getTop(win) {
    if (window.allModals && (win != top)) {
        for (var i = 0; i < window.allModals.length; i++) {
            var frame = window.allModals[i];
            var currentWindow = frame.get(0);
            // Find the top frame
            if (containsWindow(currentWindow.contentWindow, win)) {
                return currentWindow.contentWindow;
            }
        }
    }
    return window.top;
}

cmsdefine(['CMS/EventHub'], function (hub) {
    var interactionHandlers,

        keyPressed = function (e) {
            e.notContinue = true;
        },

        clicked = function (e) {
            e.notContinue = true;
        },

        allowHandle = function (w) {
            return (w.location.href != 'about:blank');
        },

        stopPropagation = function (w) {
            if (allowHandle(w) && !w.propagationStopped) {
                w.propagationStopped = true;

                // Stop propagation of key press and click above the dialog
                hub.subscribe({
                    name: 'KeyPressed',
                    window: w
                }, keyPressed);

                hub.subscribe({
                    name: 'GlobalClick',
                    window: w
                }, clicked);
            }
        },

        allowPropagation = function (w) {
            if (allowHandle(w) && w.propagationStopped) {
                w.propagationStopped = false;

                // Allow propagation of key press and click above the dialog
                hub.unsubscribe({
                    name: 'KeyPressed',
                    window: w
                }, keyPressed);

                hub.unsubscribe({
                    name: 'GlobalClick',
                    window: w
                }, clicked);
            }
        },

        /**
         * Set interaction criteria for allowing eventual interaction with outside elements.
         * @param   {String}    name    Name of given function, so that it can be deleted later.
         * @param   {Function}  handler Function to trigger when interaction allowance is computed.
         */
        addInteractionHandler = function (name, handler) {
            interactionHandlers = interactionHandlers || {};
            interactionHandlers[name] = handler;
        },

        /**
         * Delete interaction criteria by name.
         * @param   {String}    name    Name of the function to remove.
         */
        removeInteractionHandler = function (name) {
            if (interactionHandlers && interactionHandlers.hasOwnProperty(name)) {
                delete interactionHandlers[name];
            }
        },

        /**
         * Get allowance of given event according to added custom interaction handlers.
         * @param   {Event}     event  Event that is checked (e.g. using event target) by interaction criteria.
         */
        getInteractionAllowance = function (event) {
            var result = false;

            if (interactionHandlers) {
                for (name in interactionHandlers) {
                    if (interactionHandlers.hasOwnProperty(name)) {
                        result |= interactionHandlers[name](event);
                    }
                }
            }

            return result;
        },

    AdvancedPopupHandler = function () {
        this.stopPropagation = stopPropagation;
        this.allowPropagation = allowPropagation;
        this.addInteractionHandler = addInteractionHandler;
        this.removeInteractionHandler = removeInteractionHandler;
        this.getInteractionAllowance = getInteractionAllowance;

        window.CMS = window.CMS || {};
        window.CMS.AdvancedPopupHandler = this;
    };

    return AdvancedPopupHandler;
});