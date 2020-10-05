cmsdefine(['CMS/Filter', 'CMS/EventHub', 'CMS/NavigationBlocker', 'CMS/Application', 'jQuery', 'Underscore', 'CMS/UrlHelper', 'jQueryJScrollPane', 'CMS/Loader'], function (Filter, EventHub, NavigationBlocker, Application, $, _, UrlHelper) {

    var AppList,

        // Default AppList data
        data = {
            // Effects variables
            duration: 200,
            opacity: '0.4',
            scrollStep: 20,

            applicationListBaseUrl: '',
            defaultAppUrl: '',
            defaultAppName: '',
            launchAppWithQuery: '',
            targetFrame: 'cmsdesktop',

            // Element selectors
            appListWrapperElemSelector: '#cms-applist',
            appListPanelSelector: '#cms-applist-panel',
            appListOverLayerSelector: '#cms-overlayer',
            appListToggleSelector: '#cms-applist-toggle',
            appListToggleCloseSelector: '#cms-applist-toggle-close',
            appListHeaderSelector: "#cms-applist-header",
            appListContentSelector: "#cms-applist",
            appListLiveSiteButtonSelector: ".btn-livesite",
            appListScrollableElemSelector: ".js-scrollable",
            appListScrollShadowElemSelector: '<div class="scroll-shadow"></div>',
            appListSearchInputSelector: '.js-filter-search'
        },

        // Dashboard application ID const
        DASHBOARD_APP_ID = null,

        APPLICATION_IS_ON_DASHBOARD_CSSCLASS = 'pinned',
        DASHBOARD_EDIT_MODE_CSSCLASS = 'dashboard-edit-mode',

        DASHBOARD_APPLICATION_FULL_PIN_CSSCLASS = 'icon-pin',
        DASHBOARD_APPLICATION_OUTLINED_PIN_CSSCLASS = 'icon-pin-o',

        navigationBlocker = new NavigationBlocker(),

        // Indicates whether app list is used
        isUsed = false,

        // Indicates wheter this module is initialized
        isInitialized = false,

        // Livesite indentation, 4 * @base-unit
        liveSiteIndentation = 64,

        // List of applications
        applications = [],

        // Current application id
        currentAppId = DASHBOARD_APP_ID,

        // Launch handlers
        onLaunchAppListeners = [],

        // On application show callbacks
        onAppShowCallbacks = [],

        // On application show callbacks
        onAppHideCallbacks = [],

        // Indicates whether application is loading
        appLoading = false,

        // Indicates whether application list shoul be hidden after application load
        hideAfterLoad = true,

        // Indicates whether hashtags are used for application change
        hashTagsUsed = false,

        // Indicates whether the application has been shown already
        wasShown = false,

        // Indicates whether hash change should be ignored
        ignoreHash = false,

        // Indicates whether is app list in editable mode
        isInEditableMode = false,

        // Target frame reference
        targetFrame = window.frames[data.targetFrame],

        // Elements
        $window,
        $documentBody,
        $appListWrapper,
        $appListPanel,
        $appListOverLayer,
        $appListToggle,
        $appListToggleClose,
        $appListHeader,
        $appListContent,
        $appListLiveSiteButton,
        $scrollShadowTopElement,
        $scrollShadowBottomElement,

        // Scroller
        scrollerInstance,
        $scrollableElem,


        // Concatenates strings and ignores undefined and null values
        concatenateStrings = function () {
            var args = _.toArray(arguments),
                result = '';

            _.each(args, function (argument) {
                if (argument === undefined || argument === null) {
                    argument = '';
                }

                result += argument;
            });

            return result;
        },

        // Shows application panel and disable user interface
        show = function () {
            var $currentlySelected;

            if (appLoading) {
                hideAfterLoad = false;
            }

            $appListPanel.show();
            EventHub.publish({
                name: 'ApplicationListShown',
                onlySubscribed: true
            });

            // Expand category of currently selected category
            if (currentAppId) {
                $currentlySelected = $appListPanel.find('*[data-appguid=' + currentAppId + ']');
                expandCurrentCategory($currentlySelected);
            }

            // Expand first category in editable mode
            if (isInEditableMode) {
                expandFirstCategory();
            } else {
                // Do not show overlay in editable mode
                $appListOverLayer.show();
                $appListOverLayer.animate({ opacity: data.opacity }, { duration: data.duration, queue: false });
            }

            var appListWidth = $appListPanel.width();
            $documentBody.animate({ left: appListWidth, right: -appListWidth }, {
                duration: data.duration,
                queue: false,
                complete: function () {
                    // Run tasks on first show
                    if (!wasShown) {
                        initScroller();
                        wasShown = true;
                    }

                    runOnAppShowCallbacks();
                }
            });

            isUsed = true;
        },


        // Hides application panel and enable user interface
        hide = function () {
            EventHub.publish({
                name: 'ApplicationListHidden',
                onlySubscribed: true
            });

            $appListOverLayer.animate({
                opacity: '0.0'
            }, {
                duration: data.duration, queue: false
            });
            $documentBody.animate({ left: '0px', right: '0px' }, data.duration, function () {
                $appListOverLayer.hide();
                runOnAppHideCallbacks();
            });

            isUsed = false;
        },


        expandFirstCategory = function () {
            expandCurrentCategory($appListContent.find('.js-filter-item').first());
        },

        expandCurrentCategory = function ($currentlySelected) {
            var $parent = $($currentlySelected.closest('.panel.panel-default')[0]),
                $toggleBtn = $parent.find('.accordion-toggle'),
                $dataWrapper = $parent.find('.panel-collapse');

            // Stop transitions and simmulate mouse click on button
            $dataWrapper.addClass('no-transition');
            $toggleBtn.click();

            // Make sure that the rendering engine finishes before
            // returning 'no-transition' class back on
            setTimeout(function () {
                $dataWrapper.removeClass('no-transition');
            }, 0);
        },

        onScrollerArrowChange = function (event, isAtTop, isAtBottom) {
            if (isAtTop) {
                $scrollShadowTopElement.hide();
            } else {
                $scrollShadowTopElement.show();
            }

            if (isAtBottom) {
                $scrollShadowBottomElement.hide();
            } else {
                $scrollShadowBottomElement.show();
            }
        },

        onScrollerInitialized = function (event, isScrollable) {
            if (!isScrollable) {
                $scrollShadowTopElement.hide();
                $scrollShadowBottomElement.hide();
            } else {
                onScrollerArrowChange(
                    event,
                    scrollerInstance.getPercentScrolledY() === 0,
                    scrollerInstance.getPercentScrolledY() === 1
                );
            }
        },

        initScroller = function () {
            var appListOffset = $scrollableElem.offset();

            $scrollableElem.css({
                position: 'absolute',
                top: appListOffset.top + 'px',
                left: 0,
                right: 0,
                bottom: liveSiteIndentation + 'px',
                outline: 'none'
            });

            $scrollableElem.jScrollPane({ verticalGutter: -8 });
            scrollerInstance = $scrollableElem.data('jsp');

            $scrollableElem.bind('jsp-arrow-change', onScrollerArrowChange);
            $scrollableElem.bind('jsp-initialised', onScrollerInitialized);

            // Insert shadow before the app list and hide it defaultly
            $scrollShadowTopElement.prependTo($scrollableElem);
            $scrollShadowBottomElement.prependTo($scrollableElem);
        },


        // Show or hide application list with dependence on current status
        toggleAppList = function () {
            if (isUsed) {
                hide();
            } else {
                show();
            }
        },

        relaunchApplication = function (p) {
            if (!p.received) {
                p.received = true;

                if (currentAppId) {
                    launchApplication({ appId: appId });
                }
            }
        },


        // Load application into inner frame
        launchApplication = function (appParams) {
            currentAppId = appParams.appId;
            var appName = getApplicationName(appParams.appId),
                // hash parameters and element guid are set only when the application directly opens specified single object
                redirectToSingleObject = !!appParams.hashParameters && !!appParams.hashParameters.elementguid,
                additionalQuery = '',
                additionalQueryObject = appParams.hashParameters || {};

            additionalQueryObject.displaytitle = 'false';

            // Single object can either need specific UI element or can work with the application ID as well.
            // In the former case the element GUID was already defined while parsing hash query, in the latter current application ID has to be used.
            additionalQueryObject.elementguid = additionalQueryObject.elementguid || currentAppId;

            additionalQuery = UrlHelper.buildQueryString(additionalQueryObject);

            if (redirectToSingleObject) {
                // This property has to be set in order to avoid unwanted redirection caused by locationHashChanged() method
                // as soon as the hash changes.
                ignoreHash = true;
            }

            // Propagate additional query parameters
            if (data.launchAppWithQuery.length > 0) {
                additionalQuery += '&' + data.launchAppWithQuery;
            }

            // Load application into the inner content
            setWindowLocation(targetFrame, 'href', data.applicationListBaseUrl + additionalQuery, function () {
                //  Show loading status
                if (window.Loader) {
                    window.Loader.show();
                }
                appLoading = true;

                // Notify onLaunchApplication listeners
                notifyLaunchAppListeners(currentAppId, appName);

                if (!additionalQueryObject.persistent) {
                    window.location.hash = currentAppId;
                }
                else {
                    // Since the link is persistent, there is no need to ignore the hash, locationHashChanged() won't be called at all
                    ignoreHash = false;
                }

                // Call event so breadcrumbs could receive required data
                if (redirectToSingleObject) {
                    EventHub.publish("NavigatingToSingleObject", appParams);
                } else {
                    EventHub.publish("NavigationToApplication", currentAppId);
                }
            });
        },


        // Sets the window location attributes based on the newLocation
            // object, but only if there are no unsaved changes in the UI
        setWindowLocation = function (w, newLocationKey, newLocationValue, successCallback) {
            if (navigationBlocker.canNavigate()) {
                w.location[newLocationKey] = newLocationValue;

                // Call success callback if defined
                successCallback && successCallback();
            } else {
                hide();
            };
        },


        // Launch application handler
        onLaunchApplication = function (callBack) {
            onLaunchAppListeners.push(callBack);
        },


        // Notify onLaunchApplication listeners
        notifyLaunchAppListeners = function (appId, appName) {
            for (var i = 0; i < onLaunchAppListeners.length; i++) {
                onLaunchAppListeners[i](appId, appName);
            }
        },


        // Handle application select by GUID in URL
        handleApplicationSelection = function () {
            var href = location.href;
            var appId = null;

            // Try get app id form url
            if (href.indexOf('#') >= 0) {
                appId = href.substring(href.indexOf('#') + 1);
            }

            openApplication(appId);

            // Do not handle query parameters for next load
            data.launchAppWithQuery = '';
        },


        // Logs the application usage by incrementing its open counter within Module usage tracking
        logApplicationUsage = function (appId) {
            $.post(Application.getData('applicationUrl') + 'cmsapi/ApplicationUsage/Log', '=' + appId);
        },


        // Opens the given application in the content frame. If no application is specified, default application is chosen
        openApplication = function (appQuery) {

            if (SetLiveSiteURL) {
                // Reset live site URL
                SetLiveSiteURL();
            }

            var appParams = getApplicationParams(appQuery);

            // Launch application if is defined
            if (checkApplicationIdValidity(appParams.appId)) {
                launchApplication(appParams);
            }
            else { // Display the default application when no specific application or page is requested
                appParams.appId = null;

                // Try get requested application from special handling e.g. logon page
                if (typeof (Storage) !== "undefined") {
                    appParams = getApplicationParams(sessionStorage.cmsLatestApp);
                    sessionStorage.cmsLatestApp = null;
                }

                if ((appParams.appId !== null) && checkApplicationIdValidity(appParams.appId)) {
                    // Do not refresh launched app after hash join
                    ignoreHash = true;
                    setWindowLocation(window, 'hash', '#' + appParams.appId);
                    launchApplication(appParams);
                }
                else {
                    setWindowLocation(targetFrame, 'href', data.defaultAppUrl);

                    // Notify onLaunchApplication listeners
                    notifyLaunchAppListeners(0, data.defaultAppName);
                }
            }
        },


        // Parses hash query to application guid and additional hash queries
        getApplicationParams = function (url) {
            if (url && url.indexOf("#") !== -1) {
                url = url.split("#")[1];
            }

            var appParams = {};

            if (url) {
                var appProperties = url.split("&");
                appParams.appId = appProperties[0];

                var restProperties = _.rest(appProperties);
                if (restProperties && restProperties.length) {
                    appParams.hashParameters = {};

                    restProperties.forEach(function (parameter) {
                        var splittedParameter = parameter.split("=");
                        appParams.hashParameters[splittedParameter[0]] = splittedParameter[1];
                    });
                }
            }

            return appParams;
        },


        // Checks whether application available
        checkApplicationIdValidity = function (appId) {
            return ((appId != null) && (appId.length > 0) && applicationExists(appId));
        },


        // Handle key press function
        keyPressed = function (e) {
            // Show/Hide for F2 key(113), or hide for Esc(27)
            if ((e.key == 27) && isUsed || (e.key == 113)) {
                toggleAppList();
                e.wasHandled = true;
            }
                // F4 = Dashboard
            else if (e.key == 115) {
                setWindowLocation(window, 'hash', '#');
                EventHub.publish('DashboardClicked');
                e.wasHandled = true;
            }
        },

        // Run onAppShow callbacks
        runOnAppShowCallbacks = function () {
            for (var i = 0; i < onAppShowCallbacks.length; i++) {
                onAppShowCallbacks[i]();
            }
        },

        // Run onAppHide callbacks
        runOnAppHideCallbacks = function () {
            for (var i = 0; i < onAppHideCallbacks.length; i++) {
                onAppHideCallbacks[i]();
            }
        },

        launchApplicationHandler = function (e) {
            // Check changes
            var appId = $(this).data('appguid'),
                currentAppReload = (currentAppId === appId),
                        unsavedChanges = false,
                        origin = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port : '');

            // Try to remove the query string from window.location without actually reloading the page
            // It is possible through HTML5 history api, which is not availible in IE9,
            // so the url won't be changed there
            if (window.history && window.history.pushState) {
                window.history.pushState({}, '', origin + window.location.pathname + window.location.hash);
            }

            // Ignore click raised by middle button
            if (e.which !== 2) {
                // Process click action if should be used
                if ((!hashTagsUsed || currentAppReload)) {
                    launchApplication({ appId: appId });
                } else {
                    if (!navigationBlocker.canNavigate()) {
                        unsavedChanges = true;
                        hide();
                    };
                }
            }

            if (unsavedChanges) {
                e.preventDefault();
                return false;
            } else {
                return true;
            }
        },

        // Collect applications
        collectApplications = function () {
            $('#cms-applist [data-appguid]')
                .each(function () {
                    var $el = $(this),
                        appId = $el.attr('data-appguid'),
                        appName = '',
                        textNode = $el.contents().filter(function () {
                            return this.nodeType === 3;
                        });

                    if (textNode !== null) {
                        appName = textNode[0].data;
                    }

                    applications.push($el);
                    setApplicationItem(appId, appName);
                })
                .click(launchApplicationHandler);
        },


        // Indicates whether application is defined
        applicationExists = function (appId) {
            if (getApplicationName(appId) !== undefined) {
                return true;
            }
            return false;
        },


        // Gets specific application item
        getApplicationName = function (appId) {
            return applications[appId];
        },


        // Set application item
        setApplicationItem = function (appId, appName) {
            applications[appId] = appName;
        },


        // Launch application after hash change
        locationHashChanged = function (e) {
            var hashParameters = getApplicationParams(e.oldURL).hashParameters;

            // If previous url had ignore hash parameter, do not open application
            ignoreHash = ((hashParameters && hashParameters.ignorehash) || ignoreHash);
            if (!ignoreHash) {
                var appId = location.hash.substring(1);
                openApplication(appId);
            }

            ignoreHash = false;
        },

        // Set up filtering
        setupFilter = (function () {
            var onStartOnEachParentCallback = function ($parent) {
                $parent.removeClass('panel');
                $parent.children('.panel-collapse')
                       .removeClass('collapse')
                       .addClass('in')
                       .css('height', 'auto');

                // Disable collapsing on this element
                $parent.find('.accordion-toggle').attr('data-toggle', '');
            },

                onEndOnEachParentCallback = function ($parent) {
                    $parent.addClass('panel');
                    $parent.children('.panel-collapse')
                            .removeClass('in')
                            .addClass('collapse');

                    // Enable collapsing on this element
                    $parent.find('.accordion-toggle').attr('data-toggle', 'collapse');
                };

            return function ($appListWrapper, $searchInput) {
                var f = new Filter($appListWrapper, $searchInput, {
                    onStartCb: function () {
                        f.eachParent(onStartOnEachParentCallback);
                    },
                    onReset: function () {
                        f.eachParent(onEndOnEachParentCallback);
                    },
                    onParentEmptyCb: function ($parent) {
                        $parent.hide();
                    },
                    onParentNonemptyCb: function ($parent) {
                        $parent.show();
                    },
                    onBeforeItemChange: function ($item) {
                        // Remove the tooltip from the child item
                        $($item.children()[0]).tooltip('hide');
                    },
                    onChange: function () {
                        if (scrollerInstance) {
                            scrollerInstance.reinitialise();
                        }
                    },
                    onItemActivated: function (event, selectedItem) {
                        var newLocation = selectedItem.$link.attr('href');
                        if (event.ctrlKey) {
                            // Open that in new tab
                            window.top.open(newLocation, '_blank');
                        } else {
                            if (window.location.hash !== newLocation) {
                                setWindowLocation(window, 'href', newLocation);
                            } else {
                                hide();
                            }
                        }

                        event.preventDefault();
                        return false;
                    },
                    onItemSelected: function ($item) {
                        // Do manual scrolling on up/down arrow keys
                        var itemTopPosition = $item.position().top,
                            itemHeight = $item.height(),
                        scrollableElemHeight = $scrollableElem.height();

                        if ((itemTopPosition + itemHeight) > (scrollableElemHeight + scrollerInstance.getContentPositionY())) {
                            scrollerInstance.scrollToY(itemTopPosition + itemHeight - scrollableElemHeight);
                        } else if (itemTopPosition < scrollerInstance.getContentPositionY()) {
                            scrollerInstance.scrollToY(itemTopPosition);

                            // If only category height is remaining from top, finish that scroll
                            if (scrollerInstance.getContentPositionY() < 50) {
                                scrollerInstance.scrollToY(0);
                            }
                        }
                    }
                });

                return f;
            };
        }()),

        // Event called on every category show/hide
        // When one shows, the previously shown will hide defaultly
        // make sure that the event will fire only once.
        onCategoryToggle = _.debounce(function () {
            if (scrollerInstance) {
                scrollerInstance.reinitialise();
            }
        }, 20),


        /**
         * Toggles whether the application is pinned to application list or not.
         *
         * @param e Mouse click event
         */
        pinOrUnpinDashboardApplication = function (e) {
            e.preventDefault();

            var $this = $(this);
            if ($this.attr("data-pending")) {
                return;
            }

            if ($this.hasClass(APPLICATION_IS_ON_DASHBOARD_CSSCLASS)) {
                EventHub.publish({
                    name: 'cms.applicationdashboard.ApplicationRemoved',
                    onlySubscribed: true
                }, e.data.guid);
            } else {
                $this.attr("data-pending", true);
                EventHub.publish({
                    name: 'cms.applicationdashboard.ApplicationAdded',
                    onlySubscribed: true
                }, e.data.guid);
            }
        },


        /**
         * Subscribes to changes of applications within the dashboard. If application is removed, removes the pin icon and vice versa.
         * If dashboard has been loaded, re-enables clicking on the pin.
         */
        subscribeToDashboardAppChanges = function () {
            EventHub.subscribe('cms.applicationdashboard.ApplicationRemoved', function (appGuid) {
                handleDashboardAppToggle(false, appGuid);
            });

            EventHub.subscribe('cms.applicationdashboard.ApplicationAdded', function (appGuid) {
                handleDashboardAppToggle(true, appGuid);
            });

            EventHub.subscribe('cms.applicationdashboard.DashboardItemLoaded', function (appGuid) {
                var $app = _.find(applications, function (app) { return app.attr('data-appguid') === appGuid; });
                $app.removeAttr("data-pending");
            });

        },


        /**
         * Adds or removes the pin in the application list according to the given parameter.
         *
         * @param pinned boolean   If true, pin is added; otherwise, false
         * @param appGuid guid     Guid of the application
         */
        handleDashboardAppToggle = function (pinned, appGuid) {
            var $app = _.find(applications, function (app) { return app.attr('data-appguid') === appGuid; });

            if (!$app) {
                return;
            }

            var $pin = $app.find('.dashboard-pin');

            if (pinned) {
                $app.addClass(APPLICATION_IS_ON_DASHBOARD_CSSCLASS);

                $pin.addClass(DASHBOARD_APPLICATION_FULL_PIN_CSSCLASS);
                $pin.removeClass(DASHBOARD_APPLICATION_OUTLINED_PIN_CSSCLASS);

            } else {
                $app.removeClass(APPLICATION_IS_ON_DASHBOARD_CSSCLASS);

                $pin.addClass(DASHBOARD_APPLICATION_OUTLINED_PIN_CSSCLASS);
                $pin.removeClass(DASHBOARD_APPLICATION_FULL_PIN_CSSCLASS);
            }
        },

        handleDashboardModeChange = function (isEditable, checkedApplicationsGuids) {
            subscribeToDashboardAppChanges();
            isInEditableMode = isEditable;

            if (isInEditableMode) {
                $appListContent.addClass(DASHBOARD_EDIT_MODE_CSSCLASS);

                _.each(applications, function ($app) {
                    var appGuid = $app.attr('data-appguid');

                    // Disable current click handler
                    $app.off('click');

                    // Enable click handler on application
                    $app.on('click', {
                        guid: appGuid
                    }, pinOrUnpinDashboardApplication);

                    $app.append($('<i aria-hidden="true" class="icon-pin cms-icon-80 dashboard-pin"></i>'));
                    $app.tooltip('disable');

                    // Add checked class if appGuid is contained within checkedApplicationsGuids
                    handleDashboardAppToggle(_(checkedApplicationsGuids).contains(appGuid), appGuid);
                });
            }
            else {
                if (isUsed) {
                    hide();
                }

                $appListContent.removeClass(DASHBOARD_EDIT_MODE_CSSCLASS);

                _.each(applications, function ($app) {
                    $app.off('click');
                    $app.on('click', launchApplicationHandler);

                    $app.children('i.dashboard-pin').remove();
                    $app.tooltip('enable');
                });
            }
        };

    /**
     * AppList API
     */

    // AppList initializer
    AppList = function (serverData) {
        var that = this;

        // Extend default data with server data
        $.extend(data, serverData);

        // Cache elements
        $window = $(window);
        $documentBody = $(document.body);
        $appListWrapper = $(data.appListWrapperElemSelector);
        $appListPanel = $(data.appListPanelSelector);
        $appListOverLayer = $(data.appListOverLayerSelector);
        $appListToggle = $(data.appListToggleSelector);
        $appListToggleClose = $(data.appListToggleCloseSelector);
        $appListHeader = $(data.appListHeaderSelector);
        $appListContent = $(data.appListContentSelector);
        $appListLiveSiteButton = $(data.appListLiveSiteButtonSelector);
        $scrollableElem = $(data.appListScrollableElemSelector);
        $scrollShadowTopElement = $(data.appListScrollShadowElemSelector);
        $scrollShadowBottomElement = $(data.appListScrollShadowElemSelector).addClass('bottom');

        // Collect applications
        collectApplications();

        // Set overlay behavior
        $appListOverLayer.click(function () {
            hide();
        });

        // Toggle button functionality
        $appListToggle.click(function () {
            show();
        });

        $appListToggleClose.click(function () {
            hide();
        });

        $appListLiveSiteButton.click(function () {
            hide();
        });

        // Disable livesite indentation based on server's flag
        if (!data.indentLiveSite) {
            liveSiteIndentation = 0;
        }

        // Handle hash change => change application
        var docmode = document.documentMode;
        if ('onhashchange' in window && (docmode === undefined || docmode > 7)) {
            hashTagsUsed = true;

            // IE and Trident (Edge) do not support oldURL property in onhashchange event so do the check for the changed hash every 100ms and call the function
            if (navigator.userAgent.match(/msie|trident/i)) {
                var prevHash = window.location.hash;
                var prevURL = window.location.href;
                setInterval(function () {
                    var nextHash = window.location.hash;
                    var nextURL = window.location.href;
                    if (prevHash === nextHash) return;
                    locationHashChanged.call(window, {
                        type: 'hashchange',
                        newURL: nextURL,
                        oldURL: prevURL
                    });
                    prevHash = nextHash;
                    prevURL = nextURL;
                }, 100);
            } else {
                window.onhashchange = locationHashChanged;
            }
        }

        // Initialize loading
        $("[name='" + data.targetFrame + "']").load(function () {
            // Hide loading element
            if (window.Loader) {
                window.Loader.hide();
            }
            appLoading = false;

            if (isUsed && hideAfterLoad) {
                hide();
            }

            hideAfterLoad = true;
        });

        // Handle LogonScreen when manipulating with applist
        $appListPanel.on('click', function () {
            if (window.top.HideScreenLockWarningAndSync) {
                window.CancelScreenLockCountdown();
                window.top.HideScreenLockWarningAndSync(data.screenLockInterval);
            }
        });

        // Handle application load with parameter
        handleApplicationSelection();

        // Bind events to category show/hide
        $appListWrapper.on({
            'shown.bs.collapse': onCategoryToggle,
            'hidden.bs.collapse': onCategoryToggle
        });

        // Reinitialize scroller on every window.resize
        $window.resize(_.debounce(function () {
            if (isUsed && scrollerInstance) {
                scrollerInstance.reinitialise();
            }
        }, 1000 / 4)); // 4 FPS

        // Setup filter
        this.$searchInput = $appListPanel.find(data.appListSearchInputSelector);
        this.filter = setupFilter($appListWrapper, this.$searchInput);

        this.onShow(function () {
            that.$searchInput.focus();

            if (scrollerInstance) {
                scrollerInstance.reinitialise();
            }
        });

        this.onHide(function () {
            that.filter.resetFilter();
        });

        // Ensure dashboard is reloaded always
        EventHub.subscribe('DashboardClicked', function (e) {
            if (currentAppId !== DASHBOARD_APP_ID) {
                // DashboardClicked event called from some application,
                // remove hash from main window url and navigate to dashboard
                setWindowLocation(window, 'hash', '');
                e.preventDefault();
            } else {
                // DashboardClicked event called from dashboard, manually
                // refresh content frame
                setWindowLocation(targetFrame, 'href', data.defaultAppUrl);
            }

            // Update currentAppId to dashboard (null)
            currentAppId = DASHBOARD_APP_ID;

            handleDashboardModeChange(false);
        });

        EventHub.subscribe('cms.applicationdashboard.DashboardEditableModeChanged', handleDashboardModeChange);

        // Subscribe to event so another modules can manipulate with applist
        EventHub.subscribe('ShowApplicationList', show);
        EventHub.subscribe('HideApplicationList', hide);
        EventHub.subscribe('ToggleApplicationList', toggleAppList);

        // Shortcut key
        EventHub.subscribe('KeyPressed', keyPressed);

        // Back in tabs
        EventHub.subscribe('Tabs_Back_0', relaunchApplication);

        // Application breadcrumb click when single object navigation was used
        EventHub.subscribe("SingleObjectNavigationAppBreadcrumbClick", function (appParams) {
            var query = '';
            if (appParams.tabName) {
                query = '&tabname=' + appParams.tabName;
            }
            openApplication(appParams.appId + query);
        });

        // Subscribe to the application launch event to log application usage
        EventHub.subscribe('NavigationToApplication', logApplicationUsage);
    };

    AppList.prototype.onShow = function (cb) {
        onAppShowCallbacks.push(cb);
    };

    AppList.prototype.onHide = function (cb) {
        onAppHideCallbacks.push(cb);
    };

    // CurrentAppId getter
    AppList.prototype.getCurrentAppId = function () {
        return currentAppId;
    };

    // Methods to expose
    AppList.prototype.getApplicationName = getApplicationName;
    AppList.prototype.onLaunchApplication = onLaunchApplication;
    AppList.prototype.openApplication = openApplication;

    var appListInstance,
        AppListFacade = function (serverData) {
            if (serverData) {
                appListInstance = new AppList(serverData);
            }
        };

    AppListFacade.prototype.openApplication = function (origin, pathName, queryString, hash) {
        var topWindow = window.top,
            originalQueryString = topWindow.location.search;


        // Use current site's origin by default
        if (origin === undefined) {
            origin = topWindow.location.protocol + "//" + topWindow.location.hostname + (topWindow.location.port ? ':' + topWindow.location.port : '');
        }

        // Use current site's pathname by default
        if (pathName === undefined) {
            pathName = topWindow.location.pathname;
        }

        setWindowLocation(topWindow, 'href', concatenateStrings(origin, pathName, queryString, hash));

        if (originalQueryString === queryString) {
            // Reload topWindow when the only thing that has changed is hash
            topWindow.location.reload(true);
        }
    };


    AppListFacade.prototype.openApplicationInNewWindow = function (origin, pathName, queryString, hash, newWindowName) {
        if ($.browser.msie && $.browser.version === '9.0' && hash) {
            // The browser is IE9, which does not support openning application in new window when a hash is specified,
            // so open this application at least in current window. More info:
            // http://stackoverflow.com/questions/9091968/ie-does-not-open-a-new-window-with-the-hash-available-after-clicking-on-an-ancho
            this.openApplication(origin, pathName, queryString, hash);
        } else {
            window.top.open(concatenateStrings(origin, pathName, queryString, hash), newWindowName);
        }
    };

    return AppListFacade;
});