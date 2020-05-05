cmsdefine(['CMS/Core', 'jQuery'], function (Core, $) {

    var Module = function (opt) {
        var core = new Core(opt, null),
            ctx = core.ctx,
            data = ctx.data,
            // The data has to contain the following
            $testButton = $("#" + data.testButtonId),
            $statusLabel = $("#" + data.statusLabelId),
            reportProgressMessage = data.inprogressMessage,
            
            // Report progress to user when $testButton is clicked
            reportProgress = function () {
                $statusLabel.text(reportProgressMessage);
            },
            
            // Hook onclick event on $testButton
            init = function () {
                $testButton.click(
                    function () {
                        reportProgress();
                    }
                );
            };

        init();
    };

    return Module;
});
