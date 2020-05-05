/**
 * Filter module
 * Adds functionality for filtering list based on text input. Works either with flat lists and with
 * lists divided by some categories/parents.
 *
 * Options for initialization:
 * @param {string} parentSelector - CSS selector that matches all parents. If not specified, wrapper element will be used as one parent.
 * @param {string} itemSelector - CSS selector that matches all filterable items.
 * @param {string} textInputSelector - CSS selector that matches all input[type=text] for filtering.
 * @param {Function} onStartCb - Callback function called after filter starts (when first char occures in input).
 * @param {Function} onEndCb - Callback function called when filter finishes (when all chars are removed from input).
 * @param {Function} onParentEmptyCb - Callback function called when all items within the given parent are filtered (hidden).
 * @param {Function} onParentNonemptyCb - Callback function called when some items within the given parent are not filtered (visible).
 * @param {Function} onEnterHitted - Callback function called when enter was hitted from search input.
 */

cmsdefine(["Underscore", "CMS/Eventhub", "jQuery"], function (_, EventHub, $) {

        // Default options
    var options = {
            parentSelector: '.js-filter-parent',
            itemSelector: '.js-filter-item',
            filterEmptyMessageSelector: '.js-filter-empty',
            selectedItemClass: 'highlighted',
            onItemActivated: function (selectedItem) {
                if (selectedItem) {
                    selectedItem.$link.click();
                }
            },
            onBeforeItemChange: function () { },
            onChange: function () { },
            specialKeyActions: {
                40: function (ev) { this.selectNext(ev); }, // Up arrow
                38: function (ev) { this.selectPrevious(ev); }, // Down arrow
                9: function (ev) { this.selectNext(ev); }, // Tab
                13: function (ev) { // Enter
                    if (this.selectedItem && !this.isEditable) {
                        this.options.onItemActivated(ev, this.selectedItem);
                    }
                }
            }
        },

        /**
         * Filter function constructor
         * Runs some initializations, attaches listener to 'keyup' event.
         *
         * @param {jQuery} $wrapper - wrapper element
         * @param {Object} data - Data for filter that will be extended with default options.
         */
        Filter = function ($wrapper, $textInput, data) {
            var that = this;

            this.active = false;
            this.options = {};
            this.isEditable = false;

            _.extend(this.options, options, data);

            this.$textInput = $textInput;
            this.$emptyMessage = $wrapper.find(this.options.filterEmptyMessageSelector);
            this.$parents = $wrapper.find(this.options.parentSelector);

            this.onChangeCallbacks = [];
            this.onChangeCallbacks.push(this.options.onChange);

            this.onChangeCallbacks.push(function () {
                that.selectedItem = undefined;
                if (that.visibleItems.length > 0) {
                    that.selectFirst();
                }
            });

            // Hide empty message by default
            this.$emptyMessage.hide();

            this.textInputValue = '';
            this.initItems($wrapper);

            this.visibleItems = [];
            this.selectedItem = undefined;

            // Remember last onKeyDown value
            this.onKeyDownValue = undefined;

            // Flag that is set to true everytime keyUp is called
            this.keyUpCalled = false;

            this.$textInput.on('keydown', function (ev) {
                // Remember last onKeyDownValue but only when 
                // keyUp was called before previous remembering
                // This ensures that holding keydown (and thus firing keydown event)
                // will remember value before last keyup.
                if (that.keyUpCalled) {
                    that.keyUpCalled = false;
                    that.onKeyDownValue = this.value;
                }

                that.onInputKeyDown(ev);
            });

            this.$textInput.on('keyup', function (ev) {
                that.onInputKeyUp(ev, this.value);
            });

            var eventCallback = function (isEditable) {
                that.isEditable = isEditable;
            }

            EventHub.subscribe('cms.applicationdashboard.DashboardEditableModeChanged', eventCallback);
            EventHub.subscribe('DashboardClicked', function () { eventCallback(false); });
        };


    /**
     * Resets filter properties to their defaults
     */
    Filter.prototype.resetFilter = function () {
        var onResetCb = this.options.onReset;

        this.$textInput.val('');
        this.textInputValue = '';
        this.selectedItem = undefined;

        onResetCb && onResetCb();
        this.update(''); // Show all items

        this.active = false;

        // Remove item select
        this.unselectAllItems();
    };


    /**
     * Callback called on keyDown event in search input
     */
    Filter.prototype.onInputKeyDown = function (ev) {
        if (!this.active) {
            return;
        }

        // Prevent default actions on special actions so that for example arrow up doesn't do anything in the textbox
        if (_.isFunction(this.options.specialKeyActions[ev.which])) {
            ev.preventDefault();
            this.options.specialKeyActions[ev.which].call(this, ev);
        }
    };


    /**
     * Callback called on keyDown event in search input
     */
    Filter.prototype.onInputKeyUp = function (ev, value) {
        var onStartCb = this.options.onStartCb;

        // Set flag
        this.keyUpCalled = true;
        this.textInputValue = value.toLowerCase();

        if (this.textInputValue) {
            // Start callback
            if (!this.active) {
                onStartCb && onStartCb();
                this.active = true;
            }

            this.update(this.textInputValue);
        }

        // On key down there was something but now there is nothing,
        // we are finishing and calling reset filter
        if (!this.textInputValue && this.onKeyDownValue) {
            this.resetFilter();
        }
    };


    /**
     * [Private] Searches through all DOM elements and creates
     * private in-memory representation for this elements for performance reason.
     *
     * @param {jQuery} $wrapper - filter wrapper element
     */
    Filter.prototype.initItems = function ($wrapper) {
        var that = this;

        this.items = [];
        if (!this.$parents) {
            // No parents/categories, use main wrapper as one parent
            this.$parents = $wrapper;
        }

        _.each(this.$parents, function (value) {
            var $parent = $(value),
                $items = $parent.find(that.options.itemSelector),
                parentItems = [];

            _.each($items, function (val) {
                var $item = $(val);
                parentItems.push({
                    text: $item.text().trim().toLowerCase(),
                    $el: $item,
                    $link: $item.find('a'),
                    visible: true
                });
            });

            that.items.push({
                $parent: $parent,
                items: parentItems,
                visible: true
            });
        });
    };


    /**
     * Selects first item in visibleItems array
     */
    Filter.prototype.selectFirst = function () {
        this.selectedItem = this.visibleItems[0];
        this.selectItem(this.selectedItem.$el);
    };


    /**
     * Selects next item in visibleItems array
     */
    Filter.prototype.selectNext = function (ev) {
        // Go back on shift + tab
        if (ev.keyCode === 9 && ev.shiftKey) {
            this.selectPrevious(ev);
            return;
        }

        var visibleItemsMaxIndex = this.visibleItems.length - 1,
            newselectedItem,
            currentlyselectedItemIndex = this.visibleItems.indexOf(this.selectedItem);

        if (currentlyselectedItemIndex < visibleItemsMaxIndex) {
            // Select next item
            newselectedItem = this.visibleItems[currentlyselectedItemIndex + 1];
        } else {
            // The current item is the last one,
            // select the first item
            newselectedItem = this.visibleItems[0];
        }

        if (newselectedItem) {
            this.selectItem(newselectedItem.$el);
            this.selectedItem = newselectedItem;
        }
    };


    /**
     * Selects previous item in visibleItems array
     */
    Filter.prototype.selectPrevious = function () {
        var newselectedItem,
            currentlyselectedItemIndex = this.visibleItems.indexOf(this.selectedItem);

        if (currentlyselectedItemIndex > 0) {
            // Select previous item
            newselectedItem = this.visibleItems[currentlyselectedItemIndex - 1];
        } else {
            // The current item is the first one,
            // select the last item
            newselectedItem = this.visibleItems[this.visibleItems.length - 1];
        }

        if (newselectedItem) {
            this.selectItem(newselectedItem.$el);
            this.selectedItem = newselectedItem;
        }
    };


    /**
     * [Private] Updates items visibility based on new value.
     * 
     * @param {string} query - New value from filter input.
     */
    Filter.prototype.update = function (query) {
        var allItemsHidden = true,
            that = this;

        if (!this.active || this.currentQuery === query) {
            // No need to update when filter is inactive
            // or the query has not changed (the input was 'ctrl' key
            // for example, which caused the update call)
            return;
        }

        this.visibleItems = [];
        this.eachParent(function ($parent, parent) {
            var parentIsEmpty = that.updateParent($parent, parent.items, query);

            if (!parentIsEmpty) {
                allItemsHidden = false;
            }
        });

        // Update current query
        this.currentQuery = query;

        // Toggle $emptyMessage visibility,
        // show it when all items are hidden or
        // hide it when some item is visible
        this.$emptyMessage.toggle(allItemsHidden);

        // Whole filter has updated
        this.filterUpdated();
    };


    /**
     * Updates concrete parent element
     * @param  {jQuery}   $parent     Parent element specified in filter configuration
     * @param  {jQuery[]} parentItems Array of jQuery elements containing each parent item
     * @param  {String}   query       New value from filter input.
     * @return {Boolean}              Indication, whether the parent has some items visible or not,
     *                                            after the update has completed.
     */
    Filter.prototype.updateParent = function ($parent, parentItems, query) {
        var onBeforeItemChange = this.options.onBeforeItemChange,
            startsWithMatches = [],     // Array of items that starts with a query text
            containsMatches = [],       // Array of items that contains a query text, but does not starts with it
            $realParent = null,         // Real item parent element (can differ from $parent provided in filter configuration)
            parentIsEmpty = true;       // Indicates whether the parent has some visible items after the update is completed.

        _.each(parentItems, function (parentItem) {
            onBeforeItemChange(parentItem.$el);

            var matchIndexPosition = parentItem.text.indexOf(query);

            // try regular expression fallback
            if (matchIndexPosition < 0) {
                var saveQuery = query.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&");
                var m = new RegExp(saveQuery.split(" ").join(".* ")).exec(parentItem.text);
                if (m === null) {
                    matchIndexPosition = -1;
                } else {
                    matchIndexPosition = m.index;
                }
            }

            // Value is contained within the text
            if (matchIndexPosition >= 0) {
                parentItem.$el.show();
                parentIsEmpty = false;

                if ($realParent === null) {
                    $realParent = parentItem.$el.parent();
                }

                if (matchIndexPosition === 0) {
                    startsWithMatches.push(parentItem);
                } else {
                    containsMatches.push(parentItem);
                }
            } else {
                parentItem.$el.hide();
            }
        });

        // Append filtered items to their real parent element
        this.appendItems(startsWithMatches, $realParent);
        this.appendItems(containsMatches, $realParent);

        // Push startsWith items to visible items array first,
        // then add contains items.
        this.visibleItems = this.visibleItems.concat(startsWithMatches.concat(containsMatches));

        // Parent has updated
        this.parentUpdated(parentIsEmpty, $parent);

        return parentIsEmpty;
    };


    /**
     * Removes select from currently selected item and adds select to the new one.
     * 
     * @param {jQuery} $newItem - Item to select.
     */
    Filter.prototype.selectItem = function ($newItem) {
        this.unselectAllItems();

        $newItem.addClass(this.options.selectedItemClass);

        if (_.isFunction(this.options.onItemSelected)) {
            this.options.onItemSelected($newItem);
        }
    };


    /**
     * Unselects all items
     */
    Filter.prototype.unselectAllItems = function () {
        _(this.visibleItems).each(function (item) {
            item.$el.removeClass(this.options.selectedItemClass);
        }, this);
    };


    /**
     * Calls callback on each parent element.
     * 
     * @param {Function} cb - Callback to call.
     */
    Filter.prototype.eachParent = function (cb) {
        var i = 0,
            parents = this.items,
            parentsNum = parents.length;

        for (i; i < parentsNum; i++) {
            cb.call(this, parents[i].$parent, parents[i]);
        }
    };


    /**
     * Appends array of jQuery items to the specific target
     * @param  {jQuery[]} items   Array of jQuery items
     * @param  {jQuery} $target   Target element
     */
    Filter.prototype.appendItems = function (items, $target) {
        _.each(items, function (item) {
            $target.append(item.$el);
        });
    };


    /**
     * Function called on each parent update
     * @param  {Boolean} allItemsHidden Indicates whether all items inside parent element are hidden or not
     * @param  {jQuery} $parent         Parent element
     */
    Filter.prototype.parentUpdated = function (allItemsHidden, $parent) {
        if (allItemsHidden) {
            this.options.onParentEmptyCb && this.options.onParentEmptyCb($parent);
        } else {
            this.options.onParentNonemptyCb && this.options.onParentNonemptyCb($parent);
        }
    };


    /**
     * Function called on whole filter update
     */
    Filter.prototype.filterUpdated = function () {
        // Call filter changed callback
        _.each(this.onChangeCallbacks, function (fn) {
            fn();
        });
    };



    return Filter;
});