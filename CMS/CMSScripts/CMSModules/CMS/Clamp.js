/*
 * Ensures that target elements does not overflow. Adds an ellipsis at the beginning.
 */
cmsdefine(['jQuery'], function ($) {
    var Module = function (serverData) {
        $(serverData.elementWithTextSelector).each(function () {
            clampElement($(this));
        });        

        function clampElement(clampedElement) {
            if (clampedElement[0].clientWidth >= clampedElement[0].scrollWidth) {
                return;
            }

            clampedElement.text('…' + clampedElement.text().trim());

            var previousScrollWidth = 0;
            while (clampedElement[0].clientWidth < clampedElement[0].scrollWidth) {
                clampedElement.text(function (index, text) {
                    return '…' + text.slice(2);
                });

                if (previousScrollWidth === clampedElement[0].scrollWidth) {
                    return;
                }

                previousScrollWidth = clampedElement[0].scrollWidth;
            }
        }
    };

    return Module;
});