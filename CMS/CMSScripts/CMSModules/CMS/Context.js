cmsdefine(['Underscore', 'jQuery'], function (_, $) {
    
    // Local pointers to outer scope
    var $body = $('body'),
        isRTL = $body.hasClass('RTL'),
    
        // Add ModuleContext constructor into CMS.Core namespace
        ModuleContext = function (moduleData, defaultData) {
            moduleData = _.extend(moduleData, defaultData);

            var $moduleWrapper = $('#' + moduleData.wrapperId),
                moduleElems = $(moduleData.elemsSelector),
                elems = [], elemsData = [];

            $.each(moduleElems, function (idx, el) {
                var $el = $(el);
                elems.push($el);
                elemsData.push({
                    $el: $el,
                    $elData: $el.data()
                });
            });

            // Fill context properties
            this.data = moduleData;
            this.$wrapper = $moduleWrapper;
            this.$elems = elems;
            this.$elemsData = elemsData;
        };
    
    // Iterate over elems and call
    // cb on each of them with particular
    // elements and its data as attributes
    ModuleContext.prototype.eachElem = function (cb) {
        $.each(this.$elemsData, function (key, o) {
            cb(o.$el, o.$elData);
        });
    };

    // Checks whether the specified elem is
    // contained within a module's elems
    ModuleContext.prototype.isMyElem = function ($el) {
        return _.contains(this.$elems, $el);
    };

    // Finds element that matches specified selector inside my elements
    ModuleContext.prototype.findInMyElems = function (selector) {
        var found = undefined;

        $.each(this.$elems, function (key, el) {
            if (el.is(selector)) {
                found = el;
                return;
            }
        });

        return found;
    };
    
    ModuleContext.prototype.$body = $body;
    ModuleContext.prototype.isRTL = isRTL;
    return ModuleContext;
})
