/** 
 * Breadcrumbs module
 * 
 * Uses Mole.js module for collecting data from other frames
 * and renders this data into header as breadcrumb items
 */
cmsdefine(['CMS/EventHub', 'CMS/UrlHelper', 'CMS/NavigationBlocker', 'Underscore', 'jQuery'], function (hub, urlhelper, NavigationBlocker, _, $) {

    var breadcrumbsRightSpace = 30, // Empty space after last breadcrumb
        navigationBlocker = new NavigationBlocker(),

        /**
         * Breadcrumbs click callback generator, behaves differently for first breadcrumb than the rest.
         *
         * @param {Bool} isRootBreadcrumb - indicates whether the generated callback should be for root breadcrumb or not
         * @param {String} url - breadcrumb url
         * @param {frame} frame - target frame that the click should affect
         * @param {String} onClientClick - JavaScript code from server to call on breadcrumb click
         */
        generateOnBreadcrumbClickCallback = function (isRootBreadcrumb, url, frame, onClientClick) {
            var that = this;
            return function (e) {
                if (navigationBlocker.canNavigate()) {

                    // Makes sure that after redirect the breadcrumbs will be again rendered automatically
                    that.breadcrumbsOverwritten = false;

                    // If its being navigated by single object link, change the application breadcrumbs link to call the event that will open application using the app list
                    if (isRootBreadcrumb && that.isSingleObject) {
                        hub.publish('SingleObjectNavigationAppBreadcrumbClick', { appId: that.appId, tabName: that.tabName });
                        return true;
                    }

                    var queryString,
                        onClientClickResult = true;

                    if (onClientClick) {
                        // Get result of onClientClick by evaluating it inside self-invoking function
                        onClientClick = '(function () {' + onClientClick + '}());';
                        onClientClickResult = frame.eval(onClientClick);
                    }

                    if (onClientClickResult !== false) {
                        // Simulate URL change on click
                        if (isRootBreadcrumb && url.indexOf('?') != -1) {
                            // It is root breadcrumb and the url contains query string,
                            // remove parentObjectId from that query string
                            // It is for UIs which are data driven and contains tree, to ensure
                            // navigating to main application instead of parent element
                            queryString = urlhelper.getQueryString(url);
                            queryString = urlhelper.removeParameter(queryString, 'parentobjectid');
                            url = url.slice(0, url.indexOf('?')) + queryString;
                        }

                        frame.location.href = url;
                    }
                } else {
                    e.preventDefault();
                    return false;
                };
            };
        },


        Breadcrumbs = function (data) {
            var that = this;

            this.moreBreadcrumbsText = data.moreBreadcrumbsText;
            this.splitViewText = data.splitViewModeText;
      
            // Breadcrumb elements wrapper
            this.$breadcrumbsWrapper = $('#js-nav-breadcrumb');
            this.$navBar = $('.navbar.cms-navbar');
            this.$navBarRightBtns = $('.navbar.cms-navbar .nav.navbar-right');

            // Navigation bar width
            this.navBarWidth = this.$navBar.width();

            // Array for all breadcrumb elements, for future clearing
            this.$breadcrumbs = [];
            this.breadcrumbsData = [];
            this.rootBreadcrumbData = {};

            // For single object navigation
            this.isSingleObject = false;
            this.appId = null;
            this.tabName = null;
            
            // For enabling manual override of breadcrumb rendering
            this.breadcrumbsOverwritten = false;

            // Subscribe to Breadcrumbs_Back to allow sub-pages to go back in the hierarchy level
            hub.subscribe('Breadcrumbs_Back', function (p) {
                that.goBack(p);
            });

            // Shortcut key
            hub.subscribe('KeyPressed', function (e) {
                if (e.ctrlKey && e.altKey) {
                    // CTRL + ALT + Left arrow = Back
                    if (e.key == 37) {
                        that.goBack({});
                        e.wasHandled = true;
                    }
                }
            });

            // Subscribe to PageLoaded, which is fired from every page at
            // the load time. Then use the Mole to get information about
            // current Breadcrumbs and render them
            hub.subscribe('ApplicationChanged', function (application) {

                // If property was set to true, manual overwriting of breadcrumbs is performing,
                // automatic processing of breadcrumbs could be performed later and therefore 
                // it would overwrite manually set breadcrumbs.
                if (that.breadcrumbsOverwritten) {
                    return;
                }
                
                var flattenedApplication = _.flatten(application, true);

                // Stop whole breadcrumb rerendering when there is a 'breadcrumbsRefresh'
                // flag set to false in the first level of an application data
                if (!that.continueProcessing(flattenedApplication[0])) {
                    return;
                }

                that.clearBreadcrumbs();

                // Flatten first level of application data before processing and rendering,
                // we don't need the information about application level here
                that.processData(flattenedApplication);
                that.renderBreadcrumbs();
            });

            window.addEventListener('resize', _.debounce(function () {
                // Update breadcrumbs on every window.resize event
                that.navBarWidth = that.$navBar.width();
                that.clearBreadcrumbs();
                that.renderBreadcrumbs();
            }, 1000 / 30)); // 30 FPS is far enough

            // Subscribe to navigation to single object
            hub.subscribe("NavigatingToSingleObject", function (appParams) {
                that.isSingleObject = true;
                that.appId = appParams.appId;
                if (appParams.hashParameters) {
                    that.tabName = appParams.hashParameters.parenttabname;
                }
            });

            // Subscribe to navigation to application
            hub.subscribe("NavigationToApplication", function () {
                that.isSingleObject = false;
                that.appId = null;
            });

            // Subscribe to breadcrumbs overwrite event. This event is used when breadcrumbs has to be 
            // rendered manually and not automatically from frames. This should be performed only in rare 
            // scenarios, e.g. when redirecting to single object deeper in the tree hierarchy.
            hub.subscribe("OverwriteBreadcrumbs", function(breadcrumbs) {
                that.breadcrumbsOverwritten = true;
                that.clearBreadcrumbs();

                if (breadcrumbs && breadcrumbs.data && breadcrumbs.data.length) {
                    breadcrumbs.data.forEach(function(breadcrumb) {
                        var suffix = breadcrumb.suffix ? " (" + breadcrumb.suffix + ")" : "";
                        breadcrumb.frame = window;
                        
                        that.renderBreadcrumb(breadcrumb, $.prototype.appendTo, that.$breadcrumbsWrapper, !breadcrumb.redirectUrl, breadcrumb.isRoot, suffix, breadcrumb.suffix ? "breadcrumb-last" : "");
                    });
                }

                // Make sure that every change of the hash reenables automatical rendering
                $(window).one("hashchange", function () {
                    that.breadcrumbsOverwritten = false;
                });
            });
        };

    /**
     * Goes back to the closest clickable breadcrumb
     */
    Breadcrumbs.prototype.goBack = function (p) {
        var breadcrumbs = this.$breadcrumbs,
            breadcrumbsLength = this.$breadcrumbs.length,
            $bc, i, $breadcrumbLink, breadcrumbEvents, clickBc;

        if (!p.received) {
            p.received = true;

            for (i = breadcrumbsLength - 1; i >= 0; i--) {
                $bc = breadcrumbs[i];

                $breadcrumbLink = $bc.children();
                // Get events data from private data object
                breadcrumbEvents = $breadcrumbLink.get(0) && $._data($breadcrumbLink.get(0), "events");

                if (breadcrumbEvents && breadcrumbEvents.click) {
                    if ((p.sender == null) || ($bc.breadcrumbData.frame == p.sender)) {
                        // Breadcrumb that targets that specific sender window has priority
                        // this targets the right breadcrumbs when called from other than last frame in the hierarchy
                        clickBc = $breadcrumbLink;
                        break;
                    }
                    else if (clickBc == null) {
                        // Otherwise fallback to last active
                        clickBc = $breadcrumbLink;
                    }
                }
            }

            if (clickBc != null) {
                clickBc.click();

                p.back = true;
                return true;
            }
        }

        return false;
    };

    /**
     * Clears all breadcrumbs elements from DOM
     */
    Breadcrumbs.prototype.clearBreadcrumbs = function () {
        var breadcrumbs = this.$breadcrumbs,
            breadcrumbsLength = this.$breadcrumbs.length, i;

        for (i = 0; i < breadcrumbsLength; i++) {
            breadcrumbs[i].remove();
        }

        this.$breadcrumbs = [];
    };


    /**
     * Processes application data into internal flat-array representation of breadcrumbs
     */
    Breadcrumbs.prototype.processData = function (data) {
        var breadcrumbs = [],
            previousLevelLastBreadcrumbIndex = -1,
            firstLevelFirstBreadcrumb,
            firstLevelFrame,
            firstLevelDidReframe = false,
            firstLevelParamsInitialized = false,
            rootBreadcrumbName,
            breadcrumbsSuffix = '',
            i, imax, levelData,
            frame,
            levelBreadcrumbs,
            levelBreadcrumb,
            reframe,
            previousLevelLastBreadcrumb,
            j, jmax;

        // Map breadcrumbs to internal flat-array
        // representation for easier work later
        for (i = 0, imax = data.length; i < imax; i++) {
            levelData = data[i];
            frame = levelData.frame;

            if (!this.continueProcessing(levelData[0])) {
                break;
            }

            // Get root breadcrumb name from application
            if (!rootBreadcrumbName && levelData.applicationName) {
                rootBreadcrumbName = levelData.applicationName;
            }

            // Set breadcrumbs suffix from application
            if (levelData.hasOwnProperty('breadcrumbsSuffix')) {
                if (levelData.breadcrumbsSuffix) {
                    breadcrumbsSuffix = ' (' + levelData.breadcrumbsSuffix + ')';
                } else {
                    breadcrumbsSuffix = '';
                }
            }

            // Skip breadcrumb processing on split view frame,
            // but create fake breadcrumb indicating that split view is on
            if (levelData.isSplitView) {
                // Fake breadcrumb
                levelData.breadcrumbs = levelData.breadcrumbs || {};
                levelData.breadcrumbs.data = [{
                    text: this.splitViewText
                }];

                // Stop looping
                i = imax;
            }

            if (!levelData.breadcrumbs) {
                continue;
            }

            reframe = levelData.breadcrumbs.reframe;
            levelBreadcrumbs = levelData.breadcrumbs.data;

            if (levelBreadcrumbs.length > 0 && !levelData.isDialog) {
                if (previousLevelLastBreadcrumbIndex >= 0 && reframe) {
                    // Remember last breadcrumb from previous level breadcrumbs list
                    previousLevelLastBreadcrumb = breadcrumbs[previousLevelLastBreadcrumbIndex];

                    // Change URL and onClientClick of previous level last breadcrumb to current level
                    // first breadcrumb URL, to preserve selected tab
                    previousLevelLastBreadcrumb.redirectUrl = levelBreadcrumbs[0].redirectUrl;
                    previousLevelLastBreadcrumb.onClientClick = levelBreadcrumbs[0].onClientClick;

                    // Target frame should also be changed to preserve selected tab
                    if (levelBreadcrumbs[0].target && levelBreadcrumbs[0].target === '_parent') {
                        previousLevelLastBreadcrumb.frame = frame.parent;
                    } else {
                        previousLevelLastBreadcrumb.frame = frame;
                    }
                }

                if (!firstLevelParamsInitialized) {
                    // Initialize properties on first iteration
                    firstLevelDidReframe = reframe;
                    firstLevelFrame = frame;
                    firstLevelFirstBreadcrumb = levelBreadcrumbs[0];

                    firstLevelParamsInitialized = true;

                    // Use parent frame for the first level
                    if (levelBreadcrumbs[0].target && levelBreadcrumbs[0].target === '_parent') {
                        firstLevelFrame = frame.parent;
                    }
                }

                j = reframe ? 1 : 0; // Skip 1st breadcrumb when reframe mode is enabled
                jmax = levelBreadcrumbs.length;
                for (; j < jmax; j++) {
                    levelBreadcrumb = levelBreadcrumbs[j];
                    levelBreadcrumb.frame = frame;

                    breadcrumbs.push(levelBreadcrumb);
                    previousLevelLastBreadcrumbIndex++;
                }
            }
        }

        // Create breadcrumb element from first level data
        this.rootBreadcrumbData = {};
        this.rootBreadcrumbData.text = rootBreadcrumbName;
        this.rootBreadcrumbData.frame = firstLevelFrame;

        if (firstLevelDidReframe) {
            this.rootBreadcrumbData.redirectUrl = firstLevelFirstBreadcrumb.redirectUrl;
            this.rootBreadcrumbData.onClientClick = firstLevelFirstBreadcrumb.onClientClick;
        }


        this.breadcrumbsSuffix = breadcrumbsSuffix;
        this.breadcrumbsData = breadcrumbs;
    };


    /**
     * Checks if the breadcrumbs processing should continue or not
     * @param {Object} data     Data from application level
     */
    Breadcrumbs.prototype.continueProcessing = function (data) {
        if (data && 'breadcrumbsRefresh' in data && data.breadcrumbsRefresh === false) {
            return false;
        }

        return true;
    };


    /**
     * Renders breadcrumbs to the header element.
     */
    Breadcrumbs.prototype.renderBreadcrumbs = function () {
        var breadcrumbs = this.breadcrumbsData,
            breadcrumbsLength = breadcrumbs.length,
            breadcrumbsWidthTotal,
            breadcrumbsWidths = [],
            breadcrumbWidth,
            breadcrumbsMaxWidth = this.navBarWidth - (this.$breadcrumbsWrapper.position().left + this.$breadcrumbsWrapper.width()) - this.$navBarRightBtns.width() - breadcrumbsRightSpace,
            title, $selector, i;

        // Handle root breadcrumb
        if (this.rootBreadcrumbData.text) {
            breadcrumbWidth = this.renderBreadcrumb(this.rootBreadcrumbData, $.prototype.appendTo, this.$breadcrumbsWrapper, breadcrumbsLength === 0, true);
            breadcrumbsMaxWidth -= breadcrumbWidth;
        }

        // Render all other breadcrumbs from internal flat-array
        breadcrumbsWidthTotal = this.renderBreadcrumbsInternal(breadcrumbs, $.prototype.appendTo, this.$breadcrumbsWrapper, false, breadcrumbsWidths);

        if (breadcrumbs.length > 1 && breadcrumbsWidthTotal > breadcrumbsMaxWidth) {
            // Breadcrumbs size is larger than the header width, rerender the breadcrumbs as hiddenBreadcrumbs
            this.clearBreadcrumbs();
            this.renderHiddenBreadcrumbs(breadcrumbsWidths, breadcrumbsMaxWidth);
        }

        // Update window title
        $selector = $('.cms-nav-breadcrumb select');

        if ($selector.val() != '0') {
            title = $selector.find('option:selected').html();
        } else {
            title = 'Administration';
        }

        if (this.rootBreadcrumbData.text) {
            title += ' / ' + this.rootBreadcrumbData.text;
        }

        for (i = 0; i < breadcrumbs.length; i++) {
            title += ' / ' + breadcrumbs[i].text;
        }

        // Decode text
        window.document.title = $("<div/>").html(title).text();
    };


    /**
     * Renders breadcrumbs to the header element, hides some of them to meet the window space requirements.
     *
     * @param {Array} breadcrumbsWidth - Array of breadcrumbs widths in the same order as this.breadcrumbsData
     * @param {Integer} breadcrumbsMaxWidth - Maximum width of rendered breadcrumbs (without root breadcrumb)
     */
    Breadcrumbs.prototype.renderHiddenBreadcrumbs = function (breadcrumbsWidths, breadcrumbsMaxWidth) {
        var breadcrumbs = this.breadcrumbsData,
            breadcrumbsLength = breadcrumbs.length,
            breadcrumbsWidthsLength = breadcrumbsWidths.length,
            i, imax,
            $hiddenBreadcrumb,
            $hiddenBreadcrumbInner,
            toShowBreadcrumbs,
            toHideBreadcrumbs;

        // Render root breadcrumb
        if (this.rootBreadcrumbData.text) {
            this.renderBreadcrumb(this.rootBreadcrumbData, $.prototype.appendTo, this.$breadcrumbsWrapper, breadcrumbsLength === 0, true, '', 'ico-after');
        }

        // Create placeholder for hidden breadcrumbs and render it to header
        $hiddenBreadcrumb = $('<li class="dropdown no-ico"><a data-toggle="dropdown" class="dropdown-toggle" href="javascript:void(0)"><i aria-hidden="true" class="icon-ellipsis cms-icon-80"></i><span class="sr-only">' + this.moreBreadcrumbsText + '</span></a></li>'),
        $hiddenBreadcrumb.appendTo(this.$breadcrumbsWrapper);

        // Compute index of the last element in breadcrumbsWidths to be shown
        var toShowWidth = $hiddenBreadcrumb.width() + breadcrumbsWidths[breadcrumbsWidthsLength - 1];
        for (i = breadcrumbsWidthsLength - 2, imax = 1; i >= imax; i--) {
            toShowWidth += breadcrumbsWidths[i];
            if (toShowWidth > breadcrumbsMaxWidth) {
                break;
            }
        }

        // Go one step back because this element is one extra
        i++;

        // Based on index get breadcrumbs to show and breadcrumbs to hide
        toShowBreadcrumbs = this.breadcrumbsData.slice(i);
        toHideBreadcrumbs = this.breadcrumbsData.slice(0, i);

        // Render hidden breadcrumbs
        $hiddenBreadcrumbInner = $('<ul class="dropdown-menu"></ul>');
        this.renderBreadcrumbsInternal(toHideBreadcrumbs, $.prototype.appendTo, $hiddenBreadcrumbInner, true);
        $hiddenBreadcrumbInner.appendTo($hiddenBreadcrumb);
        this.$breadcrumbs.push($hiddenBreadcrumb);

        // Render shown breadcrumbs
        this.renderBreadcrumbsInternal(toShowBreadcrumbs, $.prototype.appendTo, this.$breadcrumbsWrapper, false);
    };


    /**
     * Renders jQuery Breadcrumbs element From internal breadcrumbs representation
     * 
     * @param {Array} breadcrumbs - Array of internal breadcrumb items
     * @param {Bool} allClickable - Indicates whether all breadcrumbs (or only the last one) should all be rendered as clickable
     * @param {Array} [out] breadcrumbsWidth -  Widths of the rendered breadcrumbs
     */
    Breadcrumbs.prototype.renderBreadcrumbsInternal = function (breadcrumbs, renderFunc, renderTarget, allClickable, breadcrumbsWidths) {
        var i, imax,
            breadcrumbWidth,
            breadcrumbsWidthTotal = 0,
            breadcrumb, notClickable,
            isRootBreadcrumb = false,
            isLastBreadcrumb,
            cssClass,
            suffix;

        // Render breadcrumbs from internal flat-array
        for (i = 0, imax = breadcrumbs.length; i < imax; i++) {
            breadcrumb = breadcrumbs[i];
            isLastBreadcrumb = i === imax - 1;
            cssClass = '';
            suffix = '';

            if (allClickable) {
                notClickable = false;
            } else {
                notClickable = isLastBreadcrumb;
                cssClass = (isLastBreadcrumb ? 'breadcrumb-last' : '');
                suffix = (isLastBreadcrumb ? this.breadcrumbsSuffix : '');
            }

            breadcrumbWidth = this.renderBreadcrumb(
                breadcrumb,
                renderFunc,
                renderTarget,
                notClickable,
                isRootBreadcrumb,
                suffix,
                cssClass
            );

            breadcrumbsWidthTotal += breadcrumbWidth;
            if (breadcrumbsWidths) {
                breadcrumbsWidths.push(breadcrumbWidth);
            }
        }

        return breadcrumbsWidthTotal;
    };


    /**
     * Renders jQuery Breadcrumb element from breadcrumb data object
     * @param  {Object}    breadcrumb       Breadcrumb item
     * @param  {Function}  renderFunc       Function for rendering
     * @param  {Element}   renderTarget     Element where to render the breadcrumb
     * @param  {Boolean}   isNotClickable   Indicates whether the breadcrumb should be rendered as clickable or not
     * @param  {Boolean}   isRootBreadcrumb Indicates whether the breadcrumb is root or not
     * @param  {String}    breadcrumbSuffix Breadcrumb suffix that will be rendered differently into html
     * @param  {String}    cssClass         CSS class for breadcrumb <li> wrapper
     * @return {Integer}                    Width of the rendered breadcrumb
     */
    Breadcrumbs.prototype.renderBreadcrumb = function (breadcrumb, renderFunc, renderTarget, isNotClickable, isRootBreadcrumb, breadcrumbSuffix, cssClass) {
        // Default value for isNotClickable is false
        isNotClickable = typeof isNotClickable !== 'undefined' ? isNotClickable : false;

        var $breadcrumbLink,
            $breadcrumb = $('<li></li>'),
            breadcrumbText = breadcrumb.text;

        if (isRootBreadcrumb) {
            $breadcrumb.addClass('no-ico');
        }
        
        if (isNotClickable) {
            $breadcrumb.html(breadcrumbText);
        } else {
            $breadcrumbLink = $('<a href="javascript:void(0)">' + breadcrumbText + '</a>');
            $breadcrumbLink.click(generateOnBreadcrumbClickCallback.call(this, isRootBreadcrumb, breadcrumb.redirectUrl, breadcrumb.frame, breadcrumb.onClientClick));
            $breadcrumb.append($breadcrumbLink);
        }

        if (breadcrumbSuffix) {
            $breadcrumb.append('<span>' + breadcrumbSuffix + '</span>');
        }

        $breadcrumb.addClass(cssClass);
        $breadcrumb.breadcrumbData = breadcrumb;

        this.$breadcrumbs.push($breadcrumb);
        renderFunc.call($breadcrumb, renderTarget);

        return $breadcrumb.width();
    };

    return Breadcrumbs;
});