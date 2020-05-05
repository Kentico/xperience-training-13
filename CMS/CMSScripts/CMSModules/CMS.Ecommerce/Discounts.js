cmsdefine(['jQuery'], function ($) {

    var Module = function () {

        function displayRedirectionMessage() {
            if ($('#CouponCheckBox input[type=checkbox]').is(':checked')) {
                $('#CouponsInfoLabel').show();
            }
            else {
                $('#CouponsInfoLabel').hide();
            }
        }

        function ensureRedirectionMessage() {
            if ($('.discountUsesCouponsValue input[type=hidden]').val() === 'false') {
                displayRedirectionMessage();
            }
        }

        var init = function () {
            ensureRedirectionMessage();
            $('#CouponCheckBox input[type=checkbox]').change(ensureRedirectionMessage);
        };

        init();
    };

    return Module;
});
