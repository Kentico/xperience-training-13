/** 
 * ContentMenu module
 * 
 * Module attributes
 * @param {string} data.elemsSelector - CSS selector that matches all module's elements
 * @param {string} data.defaultSelection - Indicates which elem is selected by default.
 *
 * Element's specific attributes
 * @data-viewMode {string} View mode of a given element
 */
cmsdefine(['CMS/Core', 'CMS/EventHub'], function (Core, EventHub) {

    var w = window,
        _ = w._,
        hub = EventHub,
        defaultData = {
            selectedClass: 'active'
        },
        
        // Do item selection
        selectItem = function ($item) {
            this.$curSelectedEl = $item;
            $item.addClass(this.ctx.data.selectedClass);
        },

        ContentMenu = function (data) {
            var core = new Core(data, defaultData),
                that = this;

            this.ctx = core.ctx;
            this.ctx.eachElem(function ($el, $elData) {
                // Bind highlight click event to each button
                $el.bind('click', function (e) {
                    if (!w.CheckChanges || !w.CheckChanges()) {
                        return;
                    }
                    
                    w.SetSelectedMode && w.SetSelectedMode($elData.viewMode);
                    w.DisplayDocument && w.DisplayDocument();
                });
            });

            // If default selection is set, select that button immediately
            if (this.ctx.data.defaultSelection) {
                var $defaultSelectedEl = this.ctx.findInMyElems(this.ctx.data.defaultSelection);
                if ($defaultSelectedEl) {
                    selectItem.call(this, $defaultSelectedEl);
                }
            }

            // Subscribe to my highlight group
            hub.subscribe('Content.viewModeChanged', function (d) {
                that.onModeChanged(d.mode);
            });
        };

    // On external click callback
    ContentMenu.prototype.onModeChanged = function (viewMode) {
        // Find element with specified viewMode and if it is found, select that element
        var mapped = _.map(this.ctx.$elemsData, function (item) { return { $el: item.$el, 'viewMode': item.$elData.viewMode }; });
        var found = _.find(mapped, function (el) { return el.viewMode === viewMode; });
        if (!found) {
            found = mapped[0]; // take first element (edit) by default
        }

        if (this.$curSelectedEl) {
            this.$curSelectedEl.removeClass(this.ctx.data.selectedClass);
        }

        selectItem.call(this, found.$el);
    };

    return ContentMenu;
});