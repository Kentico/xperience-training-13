cmsdefine(['jQuery'], function ($) {

    return function () {

        var ensureButtonToggling = function () {
            $('[data-toggle="buttons"] .btn').on('click', function () {
                var $this = $(this);
                $this.parent().find('.active').removeClass('active');
                $this.parent().addClass('active');
            });
        }

        ensureButtonToggling();
    }
});