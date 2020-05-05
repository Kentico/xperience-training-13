cmsdefine(['CMS/Core', 'CMS/EventHub', 'CMS/Application', 'jQuery'], function (Core, hub, cmsapp, $) {

	var defaultData = {
		    toolbarPlaceholderId: 'cms-header-contexthelp'
	    },

	    Module = function (opt) {
		    var core = new Core(opt, defaultData),
		        ctx = core.ctx,
		        data = ctx.data,
		        $toolbar = $('#' + data.toolbarId),
		        $helpTopicsMenuItem = $('#' + data.helpTopicsMenuItemId),
		        $helpTopicsList = $helpTopicsMenuItem.find('ul.dropdown-menu'),
		        $searchMenuItem = $('#' + data.searchMenuItemId),
		        $searchInput = $searchMenuItem.find('input'),
		        searchUrlPattern = data.searchUrlPattern,
		        $descriptionMenuItem = $('#' + data.descriptionMenuItemId),
		        $contextHelpBtn = $('.js-context-help'),
		    
		        updateContextHelp = function (app, w) {
		            // Disable F1 in IE
		            $(w).bind('help', function () { return false; });
		        
		    	    var applicationContext = {},
			            helpTopics = getWindowContextHelp(window.top, applicationContext),
			            pageData = {};
		    	    if (applicationContext.application) {
		    		    pageData.description = applicationContext.application.description;
		    		    pageData.helpTopics = applicationContext.application.helpTopics.concat(helpTopics);
		    	    } else {
		    		    pageData.helpTopics = helpTopics;
		    	    }
		    	    displayContextHelp(pageData);
		        },
		        
			    displayContextHelp = function (pageData) {
				    if (!pageData) {
					    pageData = {};
				    }
				    renderHelpTopics(pageData.helpTopics);
				    renderDescription(pageData.description);
			    },

		        getWindowContextHelp = function (win, applicationContext) {
			        var resultHelpTopics = [],
			            winHelpTopics = [],
			            subPagesHelpTopics = [],
			            winContext;

		    	    try {
		    		    // Get this window's context
		    		    winContext = cmsapp.getData('contexthelp', win);
		    		    if (winContext) {
		    			    if (winContext.suppressContextHelp) {
		    				    return resultHelpTopics;
		    			    }
		    			    if (!applicationContext.application && winContext.contextHelp.application) {
		    				    applicationContext.application = winContext.contextHelp.application;
		    			    }
		    			    winHelpTopics = winContext.contextHelp.helpTopics;
		    		    }

		    		    // Get help topics from inner frames
		    		    if (win.frames.length) {
		    			    for (var i = 0; i < win.frames.length; i++) {
		    				    subPagesHelpTopics = subPagesHelpTopics.concat(getWindowContextHelp(win.frames[i], applicationContext));
		    			    }
		    		    }

		    		    if (subPagesHelpTopics.length) {
		    			    if ($(win.document).find('iframe').length) {
		    				    // Inner iframe's helps are added to window help
		    				    resultHelpTopics = winHelpTopics.concat(subPagesHelpTopics);
		    			    } else {
		    				    // Inner frame's helps are used instead of window help
		    				    resultHelpTopics = subPagesHelpTopics;
		    			    }
		    		    } else {
		    			    // Window help is used
		    			    resultHelpTopics = winHelpTopics;
		    		    }
			        } catch(err) {
				        // exception is thrown when there is document from different domain
			        }

			        return resultHelpTopics;
		        },
		        
	    	    renderDescription = function(description) {
	    		    if (!description) {
	    			    $descriptionMenuItem.hide();
	    			    return;
	    		    }
	    		    $descriptionMenuItem.empty();
	    		    $descriptionMenuItem.append(getLink(description));
	    		    $descriptionMenuItem.show();
	    	    },

			    renderHelpTopics = function (topics) {
	    		    if (!topics || !topics.length) {
	    			    $helpTopicsMenuItem.hide();
	    			    return;
	    		    }
	    		    $helpTopicsList.empty();
	    		    for (var i = 0; i < topics.length; i++) {
	    			    var topic = topics[i];
	    			    $helpTopicsList.append($('<li></li>')
	    				    .append(getLink(topic))
	    			    );
	    		    }
	    		    $helpTopicsMenuItem.show();
			    },
	    	    
                // Both URL and Name are properly encoded by the server - do not re-encode
			    getLink = function (topic) {
			        return $('<a target="_blank" href="' + topic.url + '"></a>')
					    .html(topic.name);
			    },
	    	    
			    search = function () {
				    var text = encodeURIComponent($searchInput.val().trim());
				    if (text) {
					    window.open(searchUrlPattern.replace('{0}', text));
					    $searchInput.val('');
				    }
			    },
		        
                // Handle key press function
                keyPressed = function (e) {
                    // Show/Hide for F1 key(112)
                    if (e.key == 112) {
                        $contextHelpBtn.click();
                        e.wasHandled = true;
                    }
                },
		        
			    init = function() {
			        $toolbar.appendTo($('#' + data.toolbarPlaceholderId))
				        .removeClass('hide')
                        .on('shown.bs.collapse', function() {
					        $searchInput.focus();
				        })
                        .on('hidden.bs.collapse', function () {
				            $searchInput.val('');
				        });
			        displayContextHelp(null);

			        // Resize content on context help toolbar show/hide
			        $contextHelpBtn.click(function () {
			    	    // Make sure that layout resizing will happen after
			    	    // the context help toolbar rerendering finishes
			    	    setTimeout(function () {
			    		    window.top.layouts[0].resizeAll();
			    	    }, 0);
			        });

				    $searchInput.keyup(function(event) {
					    if (event.which == 13) {
						    event.preventDefault();
						    search();
					    }
				    });

				    hub.subscribe('PageLoaded', updateContextHelp);

				    hub.subscribe('ShowContextHelp', function () {
					    $contextHelpBtn.click();
				    });
			    
			        // Shortcut key
			        hub.subscribe('KeyPressed', keyPressed);
			    };

			init();
	    };
    
	return Module;
});