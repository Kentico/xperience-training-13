cmsdefine([
    'CMS/MessageService',
    'CMS/UrlHelper',
    'CMS/CurrentUrlHelper',
    'CMS.Builder/PageBuilder/DragAndDropService',
    'CMS.Builder/ModalDialogService',
    'CMS.Builder/MessageTypes',
    'CMS.Builder/Constants'
], function (msgService, urlHelper, currentUrlHelper, dndService, ModalDialogService, messageTypes, constants) {
    var DISPLAYED_WIDGET_VARIANTS_SESSION_STORAGE_KEY;
    var targetOrigin;
    var originalScript;
    var modalService = new ModalDialogService();
    var frame;
    var loadFrame;
    var instanceGuid;
    var contentModified;

    var Module = function (serverData) {
        var frameLoaded = false;
        var frameUrl = serverData.frameUrl;
        var documentGuid = serverData.documentGuid;
        var deleteDisplayedVariants = serverData.deleteDisplayedVariants;
        var applicationPath = serverData.applicationPath;
        var blankPage = "about:blank";
        var mixedContentMessage = serverData.mixedContentMessage;
        var that = this;

        instanceGuid = serverData.guid;
        contentModified = false;
        DISPLAYED_WIDGET_VARIANTS_SESSION_STORAGE_KEY = 'Kentico.DisplayedWidgetVariants|' + documentGuid;
        targetOrigin = serverData.origin;
        frame = document.getElementById(serverData.frameId);

        var deleteDisplayedWidgetVariants = function () {
            var displayedWidgetVariants = sessionStorage.getItem(DISPLAYED_WIDGET_VARIANTS_SESSION_STORAGE_KEY);

            if (deleteDisplayedVariants && displayedWidgetVariants) {
                sessionStorage.removeItem(DISPLAYED_WIDGET_VARIANTS_SESSION_STORAGE_KEY);
            }
        };

        var registerPostMessageListener = function () {
            window.removeEventListener('message', that.receiveMessage);
            window.addEventListener('message', that.receiveMessage);
        };

        var registerOnLoadListener = function () {
            frame.addEventListener('load', deleteDisplayedWidgetVariants);
        };

        var saveConfiguration = function (script) {
            if (frameLoaded === false) return;

            originalScript = script;

            frame.contentWindow.postMessage({ msg: messageTypes.SAVE_CONFIGURATION, guid: instanceGuid, contentModified: contentModified }, targetOrigin);
        };

        var bindSaveChanges = function () {
            window.CMSContentManager && window.CMSContentManager.eventManager.on('contentChanged', function (event, isModified) {
                contentModified = isModified;
            });
        };

        var bindFrameLoad = function () {
            frame.addEventListener('load', function () {
                frameLoaded = true;
                if (window.parent.Loader) {
                    window.parent.Loader.hide();
                }
            });
        };

        var handleFrameHeight = function () {
            var resize = function () {
                var panel = document.getElementsByClassName('preview-edit-panel')[0];

                if (panel) {
                    var height = document.body.offsetHeight - panel.offsetHeight;
                    frame.height = height;
                }
            };

            // Use jQuery to handle cross-browser compatibility
            $cmsj(window).bind('resize', resize);
            $cmsj(document).ready(resize);
        };

        loadFrame = function () {
            if (urlHelper.isUrlSecure(currentUrlHelper.getCurrentUrl()) && !urlHelper.isUrlSecure(frameUrl)) {
                msgService.showWarning(mixedContentMessage, true);
                frame.setAttribute("src", blankPage);
            }
            else {
                var url = urlHelper.addParameterToUrl(
                    frameUrl,
                    constants.ADMINISTRATION_DOMAIN_PARAMETER_NAME,
                    urlHelper.getHostWithScheme(currentUrlHelper.getCurrentUrl()) + applicationPath);

                frame.setAttribute("src", url);
            }

            if (window.parent.Loader) {
                window.parent.Loader.show();
            }
        };

        handleFrameHeight();
        bindSaveChanges();
        bindFrameLoad();
        registerPostMessageListener();
        registerOnLoadListener();

        loadFrame();

        window.CMS = window.CMS || {};
        var pageBuilder = window.CMS.PageBuilder = window.CMS.PageBuilder || {};
        pageBuilder.save = saveConfiguration;
    };

    Module.prototype.receiveMessage = function (event) {
        if (event.origin !== targetOrigin) {
            return;
        }

        switch (event.data.msg) {
            case messageTypes.CONFIGURATION_STORED:
                var eventData = event.data && event.data.data;
                if (eventData) {
                    sessionStorage.setItem(DISPLAYED_WIDGET_VARIANTS_SESSION_STORAGE_KEY, event.data.data);
                }

                eval(originalScript);
                break;

            case messageTypes.CANCEL_SCREENLOCK:
                window.top.CancelScreenLockCountdown && window.top.CancelScreenLockCountdown();
                break;

            case messageTypes.CONFIGURATION_CHANGED:
                window.CMSContentManager && window.CMSContentManager.changed(true);
                window.top.CancelScreenLockCountdown && window.top.CancelScreenLockCountdown();
                break;

            case messageTypes.MESSAGING_ERROR:
                msgService.showError(event.data.data, true);
                break;

            case messageTypes.MESSAGING_EXCEPTION:
                msgService.showError(event.data.data);
                break;

            case messageTypes.MESSAGING_WARNING:
                msgService.showWarning(event.data.data, true);
                break;

            case messageTypes.MESSAGING_DRAG_START:
                dndService.addDnDCancellationEvents();
                break;

            case messageTypes.MESSAGING_DRAG_STOP:
                dndService.removeDnDCancellationEvents();
                break;

            case messageTypes.SAVE_TEMP_CONFIGURATION:
                frame.contentWindow.postMessage({ msg: messageTypes.SAVE_TEMP_CONFIGURATION, guid: instanceGuid, contentModified: contentModified }, targetOrigin);
                break;

            case messageTypes.CHANGE_TEMPLATE:
                frame.contentWindow.postMessage({ msg: messageTypes.CHANGE_TEMPLATE, guid: instanceGuid, contentModified: contentModified, template: event.data.data }, targetOrigin);
                break;

            case messageTypes.TEMP_CONFIGURATION_STORED:
                msgService.clear();
                loadFrame();
                break;

            case messageTypes.GET_DISPLAYED_WIDGET_VARIANTS:
                var displayedWidgetVariants = sessionStorage.getItem(DISPLAYED_WIDGET_VARIANTS_SESSION_STORAGE_KEY);

                if (displayedWidgetVariants) {
                    frame.contentWindow.postMessage({ msg: messageTypes.LOAD_DISPLAYED_WIDGET_VARIANTS, data: displayedWidgetVariants }, targetOrigin);
                }
                break;

            case messageTypes.OPEN_MODAL_DIALOG:
                modalService.addModalDialogOverlay(document.querySelector('.page-builder'));
                break;

            case messageTypes.CLOSE_MODAL_DIALOG:
                modalService.removeModalDialogOverlay();
                break;
        }
    };

    return Module;
});
