cmsdefine(['CMS/EventHub', 'CMS/Application', 'jQuery', 'CMS/UrlHelper', 'jQueryJScrollPane'], function (hub, application, $, urlHelper) {
    // Default Tabs data
    var tabsData = {
        // Element selectors
        tabsScrollableElemSelector: ".nav-tabs-container-vertical",
        tabsScrollShadowElemSelector: '<div class="scroll-shadow" style="display:none;"></div>'
    },

    level = application.getWindowLevel(window),
    layout = null,
    back = null,
    panes = null,
    contentFrame = null,
    hidden = false,
    tabsPane = null,
    tabs = null,
    hiddenTabs = null,
    partial = false,
    overlayer = null,
    allowBack = false,
    hiddenTabsHidden = false,
    busy = 0,
    queue = [],
    queueInterval,
    queueBusy = false,
    cancelHide = false,
    slide = null,
    $window,
    // Scroller
    scrollerInstance = null,
    $scrollableElem,
    // Shadows
    $scrollShadowTopElement,
    $scrollShadowBottomElement,

    exec = function (f) {
        if (!f.notBusy) {
            busy++;
        }

        f.func.apply(f.sender, f.args);

        if (!f.notBusy) {
            busy--;
        }
    },

    runQueued = function () {
        var f;

        if (!queueBusy) {
            queueBusy = true;

            while ((busy <= 0) && (queue.length > 0)) {
                f = queue.shift();
                if (f != null) {
                    exec(f);
                }
            }

            queueBusy = false;
        }
    },

    run = function (func, args, notBusy) {
        var f = {
            func: func,
            args: args,
            sender: this,
            notBusy: notBusy
        };

        if (busy > 0) {
            queue.push(f);
        } else {
            exec(f);

            runQueued();
        }
    },

    safe = function (f, notBusy) {
        return function () {
            run(f, arguments, notBusy);
        };
    },

    asBusy = function (f) {
        busy++;
        f();
        busy--;

        runQueued();
    },

    hideTabs = safe(function (p) {
        if (!p.received) {
            p.received = true;

            partial = p.partial;

            if (partial) {
                hidePartial();
            } else {
                asBusy(function () {
                    hidden = true;
                    layout.hide('west');
                });
            }
        }
    }, true),

    hidePartial = safe(function () {
        if (hidden) {
            return;
        }

        busy++;

        reset();

        cancelHide = false;
        hidden = true;
        hiddenTabsHidden = true;

        tabsPane.addClass('nav-tabs-vertical-hiding');

        if (window.Loader) {
            window.Loader.delay();
        }

        slideTabs();
    }, true),

    slideTabs = function () {
        var tabWidth, hiddenWidth;

        if (!cancelHide) {
            reinitialiseScroller();
            tabWidth = parseInt(contentFrame.css('left'), 10);
            hiddenWidth = parseInt(hiddenTabs.css('width'), 10);

            slide = $.merge(panes, $('.loader, .overlayer'));

            slide.animate({
                'marginLeft': '-=' + (tabWidth - hiddenWidth) + "px",
            }, {
                duration: 500,
                complete: slideComplete,
            });
        }
    },

    slideComplete = function () {
        if (!cancelHide) {
            if (window.Loader) {
                window.Loader.delay();
            }

            tabsPane.addClass('nav-tabs-vertical-hidden');
            tabsPane.removeClass('nav-tabs-vertical-hiding');

            layout.sizePane('west', hiddenTabs.width());

            panes.css('marginLeft', 0);

            setTimeout(function () {
                if (!cancelHide) {
                    tabsPane.addClass('nav-tabs-vertical-hidden-hover');
                }
            }, 500);

            busy--;
            runQueued();
        }
    },

    raiseBack = function (ev) {
        var data = { sender: window };

        if (allowBack) {
            // Back handled by breadcrumbs if not in dialog
            if (level < 100) {
                hub.publish('Breadcrumbs_Back', data);
            }

            if (!data.back) {
                // Back handled by tabs
                hub.publish('Tabs_Back_' + (level - 1), data);
                if (data.back) {
                    hideTabs({});
                }
            }
        }

        if (ev) {
            ev.preventDefault();
        }
    },

    goBack = function (p) {
        var src, newSrc;

        if (!p.received) {
            p.received = true;

            if (!hidden) {
                hideTabs({});
                return;
            }

            src = contentFrame.prop('contentWindow').location.href;
            newSrc = Tabs.getHistory(0).url;

            if (newSrc.substr(0, 1) == '/') {
                src = "/" + src.replace(/^(?:\/\/|[^\/]+)*\//, "");
            }

            p.back = (src != newSrc);

            if (p.back) {
                Tabs.back(0);
            }
            showTabs();
        }
    },

    reset = function () {
        cancelHide = true;

        if (slide) {
            slide.stop(true, true);
            slide = null;
        }

        tabsPane.removeClass('nav-tabs-vertical-hiding');
        tabsPane.removeClass('nav-tabs-vertical-hidden');
        tabsPane.removeClass('nav-tabs-vertical-hidden-hover');
        tabsPane.removeClass('nav-tabs-vertical-hidden-show');

        panes.css('marginLeft', 0);
    },

    showTabs = safe(function () {
        hidden = false;

        reset();
        tabs.show();
        overlayer.hide();

        layout.sizePane('west', 'auto');
        layout.show('west');

        reinitialiseScroller();
    }),

    toggleHiddenTabs = safe(function () {
        if (partial) {
            if (hiddenTabsHidden) {
                showHiddenTabs();
            } else {
                hideHiddenTabs();
            }
        }
    }, true),

    showHiddenTabs = safe(function () {
        if (partial && hidden) {
            hiddenTabsHidden = false;

            tabsPane.addClass('nav-tabs-vertical-hidden-show');

            overlayer.show();
        }
    }),

    hideHiddenTabs = safe(function () {
        if (partial && hidden) {
            hiddenTabsHidden = true;

            tabsPane.removeClass('nav-tabs-vertical-hidden-show');

            overlayer.hide();
        }
    }),

    raiseHide = function () {
        hub.publish('Tabs_Hide_' + (level - 1), {});
    },

    contentLoaded = safe(function () {
        var cw = contentFrame.prop('contentWindow'),
            isVert = application.getData('isVerticalTabs', cw),
            hide = application.getData('hideVerticalTabs', cw);

        if (hide) {
            hideHiddenTabs();
        } else if (isVert) {
            hideTabs({});
        } else {
            showTabs();
        }

    }, true),

    // Handle key press function
    keyPressed = safe(function (e) {
        if (e.ctrlKey && e.altKey) {
            // CTRL + ALT + Left arrow = Back
            if (e.key == 37) {
                if (partial) {
                    if (!hiddenTabsHidden && allowBack) {
                        raiseBack();
                    } else {
                        toggleHiddenTabs();
                    }
                } else {
                    raiseBack();
                }
                e.wasHandled = true;
            }
        }
    }),

    initScroller = function () {
        // Cache elements
        $window = $(window);
        $scrollableElem = $(tabsData.tabsScrollableElemSelector);
        $scrollShadowTopElement = $(tabsData.tabsScrollShadowElemSelector);
        $scrollShadowBottomElement = $(tabsData.tabsScrollShadowElemSelector).addClass('bottom');

        // Run tasks on first show
        if ($('.jspContainer').length === 0) {
            // Init jScrollPane
            initScrollPane();
        }

        // Bind events to submenu show/hide
        $scrollableElem.on({
            'shown.bs.collapse': onTabToggle,
            'hidden.bs.collapse': onTabToggle
        });

        // Reinitialize scroller on every window.resize
        $window.resize(_.debounce(function () {
            reinitialiseScroller();
        }, 1000 / 4)); // 4 FPS
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

    initScrollPane = function () {
        $scrollableElem.css({
            position: 'absolute',
            top: $scrollableElem.position().top + 'px',
            left: 0,
            right: 0,
            bottom: 0,
            outline: 'none'
        });

        // Ensures correct padding under site selector
        if ($('.nav-tabs-site-selector').length > 0) {
            $('ul.TabControlTable').css('padding-top', 0);
        }

        $scrollableElem.jScrollPane({
            verticalGutter: -8,
            contentWidth: 208 // 26 * @base-unit
        });
        scrollerInstance = $scrollableElem.data('jsp');

        $scrollableElem.bind('jsp-arrow-change', onScrollerArrowChange);
        $scrollableElem.bind('jsp-initialised', onScrollerInitialized);

        // Insert shadow before the app list and hide it defaultly
        $scrollShadowTopElement.prependTo($scrollableElem);
        $scrollShadowBottomElement.prependTo($scrollableElem);
    },

    reinitialiseScroller = function () {
        if (scrollerInstance) {
            scrollerInstance.reinitialise();
            $scrollableElem.css('width', 'auto');
        }
    },

    // Event called on tab toggle
    onTabToggle = _.debounce(function () {
        reinitialiseScroller();
    }, 20),

    VT = function (data) {
        var hideKey, backKey, backAvailable,
            id = data.id;

        layout = window["layout_" + id];

        contentFrame = $('#' + id + '_c');
        contentFrame.load(contentLoaded);

        panes = $('.ui-layout-pane, .ui-layout-pane-visible');

        tabsPane = $('.ui-layout-pane-west');
        tabs = $('.nav-tabs-container-vertical-background');

        hiddenTabs = $('.nav-tabs-container-vertical-hidden');
        hiddenTabs.click(showHiddenTabs);

        tabs.click(showHiddenTabs);

        overlayer = $('#tabOverlayer');
        overlayer.click(hideHiddenTabs);
        
        // Allow back if 
        backAvailable =
            // Window is not dialog
            !application.getData('isDialog', window) &&
            // And if there is a parent
            (parent != window) && (
                // And parent is app list and did not raise this page (the page is not app root)
                application.getData('isAppList', parent) && !application.getData('isApplication') ||
                // Or parent is vertical tabs
                application.getData('isVerticalTabs', parent) ||
                urlHelper.getParameter(window.location.href, 'allownavigationtolisting')
            );

        if (backAvailable) {
            back = $('.nav-tabs-back');
            tabs.addClass('nav-tabs-no-padding');
            back.show();
            back.find('button').click(raiseBack);
            allowBack = true;
        }

        hideKey = 'Tabs_Hide_' + level;
        hub.subscribe(hideKey, hideTabs);

        backKey = 'Tabs_Back_' + level;
        hub.subscribe(backKey, goBack);

        layout.sizePane('west', 'auto');
        raiseHide();

        // Shortcut key
        hub.subscribe('KeyPressed', keyPressed);

        queueInterval = setInterval(runQueued, 500);

        if (data.scrollable) {
            initScroller();
        }
    };

    return VT;
});
