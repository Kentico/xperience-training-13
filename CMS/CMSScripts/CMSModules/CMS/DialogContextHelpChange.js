cmsdefine(['CMS/Core', 'CMS/EventHub'], function (Core, hub) {

    var Module = function (opt) {
        var core = new Core(opt, null),
            ctx = core.ctx,
            data = ctx.data,
            // The data has to contain the following
            helpName = data.helpName,
            helpUrl = data.helpUrl,

        init = function () {
            hub.publish('contextHelpChanged', data);
        };

        init();
    };

    return Module;
});
