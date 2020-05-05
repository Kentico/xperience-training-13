/**
 * Class image selector module
 */
cmsdefine(["CMS/EventHub", "jQuery"], function (hub, $) {

    /**
     * Class image selector
     * @constructor
     * @param {Object} data - Data passed from the server
     */
    var ClassImageSelector = function (data) {
        var itemSelected = function () {
            var $metafileImg = $(".FlatSelectedItem img");
            var guid = $metafileImg.data("metafile-guid");
            var extensionlessFileName = $metafileImg.data("metafile-extensionless-filename");

            if (guid) {
                hub.publish(data.eventId, {
                    metafileGuid: guid,
                    metafileExtensionlessFileName: extensionlessFileName
                });
            }

            CloseDialog();
        };

        // Publish guid of the selected metafile on submit
        $("#" + data.okButtonId).click(itemSelected);

        $(data.itemsCSSSelector).each(function () {
            $(this).bind('dblclick', itemSelected);
        });
    };

    return ClassImageSelector;
});