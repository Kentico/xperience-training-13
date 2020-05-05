(function($) {
    $.fn.equalHeight = function(minimalHeight, maximalHeight) {
        var heighest = (minimalHeight) ? minimalHeight : 0;

        this.each(function() {
            height = $(this).height();
            heighest = heighest > height ? heighest : height;
        });

        heighest = ((maximalHeight) && (heighest > maximalHeight)) ? maximalHeight : heighest;

        return this.height(heighest).css("overflow", "auto");
    };
})($cmsj);