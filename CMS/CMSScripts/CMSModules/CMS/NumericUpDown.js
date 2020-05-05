cmsdefine(['jQuery', 'CMS/WebFormCaller', 'jQueryUI'], function ($, caller) {
    var Module = function (serverData) {

        function init() {
            var $control = $("#" + serverData.controlId),
                $input = $control.find("#" + serverData.textBoxId),
                textBoxUniqueId = serverData.textBoxUniqueId,
                upElementId = serverData.upElementId,
                downElementId = serverData.downElementId,
                minimum = serverData.minimum,
                maximum = serverData.maximum,
                step = serverData.step,
                raisePostBackOnChange = serverData.raisePostBackOnChange;

            function stepUp() {
                $input.spinner("stepUp");
                propagateChangedValue();
            }

            function stepDown() {
                $input.spinner("stepDown");
                propagateChangedValue();
            }

            function propagateChangedValue() {
                if (raisePostBackOnChange) {
                    // Field has depending fields -> post the changed value back to the server
                    caller.doPostback({
                        targetControlUniqueId: textBoxUniqueId,
                        args: $input.spinner("value")
                    });
                }
                else {
                    // Value in the text box was changed via javascript -> "CheckChangesScript" must be notified about the changed value manually
                    notifyCheckChangesScript();
                }
            }

            function notifyCheckChangesScript() {
                if (window.Changed != null) {
                    window.Changed();
                }
            }

            // Initialize spinner
            $input.spinner({
                min: minimum,
                max: maximum,
                step: step,
            });

            // Bind click events to arrows
            $control.find("#" + upElementId)
                        .off("click")
                        .on("click", stepUp);
            $control.find("#" + downElementId)
                        .off("click")
                        .on("click", stepDown);
        }

        // Do not render the default up/down spinner buttons. Define buttons HTML explicitly in the user control for better styling abilities.
        $.widget("ui.spinner", $.ui.spinner, {
            _buttonHtml: function () {
                return "";
            },
        });

        init();
    };

    return Module;
});