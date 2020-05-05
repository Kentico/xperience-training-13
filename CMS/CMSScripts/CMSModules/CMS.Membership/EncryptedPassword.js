cmsdefine(['jQuery'], function ($) {

    var Module = function (serverData) {
        // Select the whole password (placeholder) on focus and prevent immediate deselection.
        $(serverData.textboxSelector).focus(function () {
            $(this).one('mouseup', function (e) {
                e.preventDefault();
            }).select();
        });
    };

    return Module;
});