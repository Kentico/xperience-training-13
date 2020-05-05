function localizeRangeCalendar(mainDiv, input, isRTL, isFixed) {
    var dpWidth = mainDiv.outerWidth();
    var dpHeight = mainDiv.outerHeight();
    var inputWidth = input.outerWidth();
    var inputHeight = input.outerHeight();
    var viewWidth = document.documentElement.clientWidth + $cmsj(document).scrollLeft();
    var viewHeight = document.documentElement.clientHeight + $cmsj(document).scrollTop();
    var offset = input.offset();

    offset.top += inputHeight + 1;

    offset.left -= isRTL ? (dpWidth - inputWidth) : 0;
    offset.left -= (isFixed && offset.left == input.offset().left) ? $cmsj(document).scrollLeft() : 0;
    offset.top -= (isFixed && offset.top == (input.offset().top + inputHeight)) ? $cmsj(document).scrollTop() : 0;

    // now check if datepicker is showing outside window viewport - move to a better place if so.
    offset.left -= Math.min(offset.left, (offset.left + dpWidth > viewWidth && viewWidth > dpWidth) ?
			Math.abs(offset.left + dpWidth - viewWidth) : 0);
    offset.top -= Math.min(offset.top, (offset.top + dpHeight > viewHeight && viewHeight > dpHeight) ?
			Math.abs(dpHeight + inputHeight) : 0);

    return offset;
}

function isDateTimeValid(id) {
    var cal = $cmsj('#' + id);
    var dt = cal.cmsdatepicker("getDate");
    
    if (dt != null) {
        var min = cal.cmsdatepicker("option", "minDate");
        var max = cal.cmsdatepicker("option", "maxDate");
        var now = new Date();

        if ((min != null) && (min != 0)) {
            min = new Date().setDate(now.getDate() + min);
            var minDate = new Date(new Date(min).setHours(0, 0, 0, 0));
            if (minDate > dt) {
                return false;
            }
        }

        if ((max != 0) && (min != null)) {
            max = new Date().setDate(now.getDate() + max);
            var maxDate = new Date(new Date(max).setHours(0, 0, 0, 0));
            if (maxDate < dt) {
                return false;
            }
        }
    }

    return true;
}