cmsdefine(['jQuery'], function ($) {

    var Module = function (data) {
        var $previewPresentationUrl = $('#' + data.previewPresentationURLID);
        var $visitorCulture = $('#' + data.visitorCultureID);
        var $visitorCultureRow = $visitorCulture.closest('div.form-group');

        function updateVisitorCultureVisibility() {
            var state = $previewPresentationUrl.val().trim() === '';
            if (state) {
                $visitorCultureRow.addClass('hidden');
            }
            else {
                $visitorCultureRow.removeClass('hidden');
            }
        }

        updateVisitorCultureVisibility();

        $previewPresentationUrl.bind('input', updateVisitorCultureVisibility);
    };

    return Module;
});
