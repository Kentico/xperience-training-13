/** 
 * EventHub helper module
 * Adds publish/subscribe functionality.
 *
 * Subscribe example usage:
 * EventHub.subscribe('click', function () { alert('clicked!'); });
 *
 * Publish example usage:
 * EventHub.publish('click');
 */
cmsdefine(['Underscore', 'CMS/Application'], function (underscore, cmsapp) {

    var top = window.top,
        _ = window._,


        /**
         * Pointer to global event hub
         */
        globalEventHub,


        /**
         * Initializes global event hub, all its functions and returns its object.
         * @param  {Object} _ Reference to Underscore library object for performance reasons.
         * @return {Object}   Object with event hub's methods
         */
        initializeGlobalEventHub = function (_) {
            var publishersQueue = {},
                subscribersQueue = {},


                /**
                 * Number of publishes to remember for new subscribers
                 * @type {Number}
                 */
                publishQueueMax = 1,


                /**
                 * Shortcut to underscores flatten function with second
                 * parameter set to true to do only shallow flattening
                 * @param  {Array} o Array on which flattening should be done
                 * @return {Array}   Flattened array
                 */
                shallowFlatten = function (o) {
                    return _.flatten(o, true);
                },

                /**
                 * Gets window position in horizontal direction (in frames tree)
                 * @param  {Window} w Window object
                 * @return {Integer}  Horizontal position of window in frames tree
                 */
                getWindowPosition = function (w) {
                    return _.toArray(w.parent.frames).indexOf(w);
                },

                /**
                 * Method for subscribing to a given hub
                 * @param  {String}   windowId Id of window from which this method is called
                 * @param  {String}   key      Hub identifier
                 * @param  {Function} cb       Callback function called on publishers
                 * @param  {Object}   ctx      Context, if specified it will be used as context for callback invocation
                 * @return {undefined}
                 */
                subscribe = function (windowId, key, cb, ctx) {
                    var config;

                    // Handle advanced settings
                    if (!_.isString(key)) {
                        config = key;
                        key = config.name;
                    }

                    // Init queues or reuse existing
                    publishersQueue[key] = publishersQueue[key] || {};
                    subscribersQueue[key] = subscribersQueue[key] || {};

                    // Do not proceed if callback or key is not defined
                    if (!key || !cb) {
                        return;
                    }

                    var publishers;

                    // If context is not defined, use topMost window by default
                    ctx = ctx || top;

                    // If not data yet on windowId position in subscribersQueue, initialize
                    // it with empty array. Then push callback and context to it so it can 
                    // be called on publish event later on.
                    subscribersQueue[key][windowId] = subscribersQueue[key][windowId] || [];
                    subscribersQueue[key][windowId].push({
                        cb: cb,
                        ctx: ctx
                    });

                    publishers = _.compose(shallowFlatten, _.toArray)(publishersQueue[key]);

                    // If publishers are presented yet on given key,
                    // call this subscriber immediatelly
                    if (publishers) {
                        _.each(publishers, function (publisherArgs) {
                            cb.apply(ctx, publisherArgs);
                        });
                    }
                },


                /**
                 * Method for unsubscribing from a given hub
                 * @param  {String}   windowId Id of window from which this method is called
                 * @param  {String}   key      Hub identifier
                 * @param  {Function} cb       Callback function, should be the same as used for subscriber
                 * @return {undefined}
                 */
                unsubscribe = function (windowId, key, cb) {
                    var config,
                        subscribers,
                        lvlKey,
                        i, imax;

                    // Handle advanced settings
                    if (!_.isString(key)) {
                        config = key;
                        key = config.name;
                    }

                    // Initialize subscribers with empty object if not initialized yet
                    subscribers = subscribersQueue[key] = subscribersQueue[key] || {};

                    // Iterate through subscribers and try to find one with
                    // the same callback as the cb parameter. When found,
                    // return immediatelly because we want to
                    // unsubscribe only for the first found.
                    for (lvlKey in subscribers) {
                        if (subscribers.hasOwnProperty(lvlKey)) {
                            for (i = 0, imax = subscribers[lvlKey].length; i < imax; i++) {
                                if (subscribers[lvlKey][i].cb === cb) {
                                    subscribers[lvlKey].splice(i, 1);
                                    return;
                                }
                            }
                        }
                    }
                },

                /**
                 * Method which safely calls all callees
                 * @callees  {Array}    array of callee objects { cb, ctx }
                 * @args     {Array}    arguments for the calls
                 * @key      {String}   event key
                 * @contFunc {Function} function to check if the execution can continue with next callees
                 * @return {undefined}
                 */
                callEach = function (callees, args, key, contFunc) {
                    var cont = true,
                        i = callees.length - 1,
                        callee;

                    while (cont && (i >= 0)) {
                        // Catch errors to prevent cancelling all subscribers
                        try {
                            while (cont && (i >= 0)) {
                                callee = callees[i];

                                if (contFunc && !contFunc.apply(callee.ctx, args)) {
                                    cont = false;
                                } else {
                                    callee.cb.apply(callee.ctx, args);
                                }

                                i--;
                            }
                        } catch (e) {
                        	if (console && console.log && (!cmsapp || cmsapp.getData('isDebuggingEnabled'))) {
		                        console.error('Event "' + key + '" fired an error:\n' + e + '\n' + e.stack);
	                        }
                            i--;
                        }
                    }
                },

                /**
                 * Method for publishing to a given group
                 * @param {...} arguments   Variable number of arguments for subscribers
                 * @return {undefined}
                 */
                publish = function () {
                    var args = _.toArray(arguments),
                                windowId = args[0],
                                key = args[1],
                                argsRest = args.slice(2),
                                publishers,
                                subscribers,
                                config,
                                contFunc = null,
                                onlySubs = false;

                    // Handle advanced settings
                    if (!_.isString(key)) {
                        config = key;
                        key = config.name;
                        contFunc = config.checkContinue;
                        onlySubs = config.onlySubscribed;
                    }

                    // Init queues or reuse existing
                    publishersQueue[key] = publishersQueue[key] || {};
                    subscribersQueue[key] = subscribersQueue[key] || {};

                    // Add publish arguments to queue for future use by new subscribers
                    if (!onlySubs) {
                        publishers = publishersQueue[key][windowId] || [];
                        publishers.unshift(argsRest);
                        publishersQueue[key][windowId] = publishers.slice(0, publishQueueMax);
                    }

                    // Get all subscribers, ignore windowId to recall all of them
                    subscribers = _.compose(shallowFlatten, _.toArray)(subscribersQueue[key]);

                    if (subscribers) {
                        callEach(subscribers, argsRest, key, contFunc);
                    }
                },


                /**
                 * Gets (or creates if not presented) unique window id based on its position
                 * in window frames tree. If the windowId was not presented, it generates it based
                 * on window's parent's windowId, thus it can recursively loop through parents
                 * until it finds first windowId (or reaches top).
                 * @param  {Window} w                               Window object of which id should be returned
                 * @param  {Array} nonInitializedWindowIdentifiers  Array where this function will store nonInitializedWindowIdentifier
                 * @param  {Bool} runnedRecursively                 Indicator if this function was runned from itself or not.
                 * @return {String}                                 WindowId based on window's position in frames tree
                 */
                getWindowPositionIdentifier = function (w, nonInitializedWindowIdentifiers, runnedRecursively) {
                    // Ensure CMS namespace is initialized in this window and its parent
                    w.CMS = w.CMS || {};
                    if (w.parent == null) {
                        w.parent = w.self;
                    }
                    w.parent.CMS = w.parent.CMS || {};

                    var windowId,
                        parentWindowId,
                        parsedParentId,
                        parentLevel,
                        myLevel, myPosition;

                    if (w.parent === w.self) {
                        // TopMost window
                        windowId = w.CMS.windowId;
                        if (!windowId) {
                            // Create windowId manually
                            windowId = '0_0';

                            // Get all children window ids and push them to nonInitializedWindowIdentifiers, to free them later
                            nonInitializedWindowIdentifiers.push.apply(nonInitializedWindowIdentifiers, getChildrenWindowsIds(windowId));
                        }
                    } else {
                        // Other windows
                        windowId = w.CMS.windowId;
                        parentWindowId = w.parent.CMS.windowId;

                        if (!windowId) {
                            // I have to get new windowId becouse it is not defined yet,
                            // that means I am working with newly created window on this position
                            if (!parentWindowId) {
                                parentWindowId = getWindowPositionIdentifier(w.parent, nonInitializedWindowIdentifiers, true);
                            }

                            // Parse parentID to get its level and compute my level based on that
                            parsedParentId = parentWindowId.split('|').pop().split('_');
                            parentLevel = parsedParentId[0],
                                myLevel = parseInt(parentLevel, 10) + 1,
                                myPosition = getWindowPosition(w);

                            // New windowId is the composition of parent's, my level and my position
                            windowId = parentWindowId + '|' + myLevel + '_' + myPosition;

                            // WindowId was not present, these ids should be cleaned up
                            if (!runnedRecursively) {
                                // Run cleanup on this window's children
                                nonInitializedWindowIdentifiers.push.apply(nonInitializedWindowIdentifiers, getChildrenWindowsIds(windowId));
                            } else {
                                // Run cleanup only on this window
                                nonInitializedWindowIdentifiers.push(windowId);
                            }
                        }
                    }

                    // Store windowId for future use
                    w.CMS.windowId = windowId;
                    return windowId;
                },


                /**
                 * Cleans objects in publishers or subscribers queue on windowId positions.
                 * @param  {Object} queue             Pointer to publishers or subscribers queue where to delete from
                 * @param  {Array} windowIdentifiers  Array of window identifiers to delete
                 * @return {undefined}
                 */
                cleanGarbage = function (queue, windowIdentifiers) {
                    var idx;

                    _(queue).each(function (queueItem, key) {
                        for (idx = windowIdentifiers.length - 1; idx >= 0; idx--) {
                            delete queueItem[windowIdentifiers[idx]];
                        };
                    });
                },


                /**
                 * Generates local event handler method (publish, subscribe, ...) based on
                 * global event handler (which is located in window.top)
                 * @param  {Window}   w              Window from which this method was invoked (and where the localEventHub is stored)
                 * @param  {Function} eventHubMethod Global event hub's function on which the method should be geneared
                 * @return {Function}                Generated method
                 */
                generateLocalEventHubMethod = function (w, eventHubMethod) {
                    return function () {
                        var args = _.toArray(arguments),
                            config,
                            key = args[0],
                            nonInitializedWindowIdentifiers = [],
                            windowIdentifier;

                        // Handle advanced settings
                        if (!_.isString(key)) {
                            config = key;
                            key = config.name;
                            w = config.window || w;
                        }

                        windowIdentifier = getWindowPositionIdentifier(w, nonInitializedWindowIdentifiers, false);
                        
                        if (nonInitializedWindowIdentifiers.length > 0) {
                            // Clean garbage when there is some
                            cleanGarbage(publishersQueue, nonInitializedWindowIdentifiers);
                            cleanGarbage(subscribersQueue, nonInitializedWindowIdentifiers);
                        }

                        // Put window object at the end of the arguments
                        // and windowIdentifier at the beginning of the arguments
                        args.push(w);
                        args.unshift(windowIdentifier);
                        eventHubMethod.apply(top.CMS.GlobalEventHub, args);
                    };
                },


                /**
                 * Gets all children windows keys by looping through all availible keys
                 * in publishers or subscribers queue and searching for myKey.
                 * @param  {String} myKey Current windowId key for which to search for
                 * @return {Array}        Array of unique founded children ids
                 */
                getChildrenWindowsIds = function (myKey) {
                    return _(_(publishersQueue).values().concat(_(subscribersQueue).values()))
                        .chain()      // Start chaining
                        .map(_.keys)  // Get keys of each object in array
                        .flatten()    // Flaten result
                        .uniq()       // Take only unique values
                        .filter(function (key) { return key.indexOf(myKey) === 0; }) // Filter only keys that starts with myKey
                        .value();     // Stop chaining, get result
                };

            return {
                subscribe: subscribe,
                unsubscribe: unsubscribe,
                publish: publish,
                generateLocalEventHubMethod: generateLocalEventHubMethod
            };
        };

    // Initialize CMS namespace
    top.CMS = top.CMS || {};

    // Ensure global EventHub is always initialized
    if (!top.CMS.GlobalEventHub) {
        top.CMS.GlobalEventHub = initializeGlobalEventHub(_);
    }

    // Create a shortcut to globalEventHub
    globalEventHub = top.CMS.GlobalEventHub;

    // Local EventHub function expose
    return {
        publish: globalEventHub.generateLocalEventHubMethod(window, globalEventHub.publish),
        subscribe: globalEventHub.generateLocalEventHubMethod(window, globalEventHub.subscribe),
        unsubscribe: globalEventHub.generateLocalEventHubMethod(window, globalEventHub.unsubscribe),
    };
});