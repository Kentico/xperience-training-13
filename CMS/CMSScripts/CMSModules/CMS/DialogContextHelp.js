cmsdefine(['CMS/Core', 'CMS/EventHub', 'jQuery'], function (Core, hub, $) {

    var Module = function (opt) {
        var core = new Core(opt, null),
            ctx = core.ctx,
            data = ctx.data,
            helpName = data.helpName,
            helpHidden = data.helpHidden,
            $helpLink = $("#" + data.helpLinkId),

        loadDialogContextHelp = function (contextHelpData) {
            if (contextHelpData.helpName === helpName) {
                $helpLink.attr('href', contextHelpData.helpUrl);
                if (!contextHelpData.helpUrl) {
                    $helpLink.addClass('hidden');
                }
                else {
                    $helpLink.removeClass('hidden');
                }
            }
        },

        init = function () {
            if (!helpHidden) {
                $helpLink.removeClass('hidden');
            }

            hub.subscribe('contextHelpChanged', function(params) {
                loadDialogContextHelp(params);
            } );
        };

        init();
    };

    return Module;
});
