var STRANDS = (function (my, $) {

    /**
     * Asynchronously loads templates and puts them into drop down list.
     */
    my.initWebTemplateSelector = function (args) {
        var $dropDownList = $("#" + args.dropDownList),
            hdn = $("#" + args.hdnSelectedTemplate);

        my.webMethodCall("LoadAllWebTemplates", {}).done(function (result) {

            var data = result.d,
                first = true;

            $dropDownList.empty().removeAttr("disabled");

            for (var key in data) {
                if (data.hasOwnProperty(key)) {

                    var optGroup = $("<optgroup label=\"" + key + "\"></optgroup>");

                    $.each(data[key], function (index, item) {

                        var identifier = item.Type + ";" + item.ID;

                        // Selects first option as default (after inserting webpart without specifying template, first one will be used)
                        if (first && hdn.val() === "") {
                            hdn.val(identifier);
                            first = false;
                        }

                        optGroup.append("<option value=\"" + identifier + "\">" + item.Title + " (" + item.ID + ")</option>");
                    });

                    $dropDownList.append(optGroup);
                }
            };

            $dropDownList.val(args.defaultValue);
        }).fail(function (result, textStatus) {
            var response;
            if (result.responseText == "") {
                response = textStatus;
            } else {
                // If exception is thrown, show message instead of drop-down list
                response = JSON.parse(result.responseText).Message;
            }

            hdn.val("");
            $dropDownList.after(my.buildErrorLabel(response))
                .remove();
        });

        $dropDownList.change(function () {
            hdn.val($(this).val());
        });
    };

    return my;
}(STRANDS || {}, $cmsj));