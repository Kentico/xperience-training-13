cmsdefine(['CMS/EventHub', 'jQuery', 'jQueryJScrollPane'], function (EventHub, $) {

    var ScrollPane = function (options) {
        if (!options || !options.selector) {
            return;
        }

        var $dropdownMenu = $(options.selector);

        $dropdownMenu.click(function (event) {
            // Check if click was made on link otherwise stop propagation
            if (!$(event.target).closest('a').length) {
                event.stopPropagation();
            }
        });

        // Init scrolling
        $dropdownMenu.jScrollPane();

        // Fix HTML validity
        $('.jspContainer').wrap('<li></li>');
        $('.jspPane').wrapInner('<ul class="dropdown-menu dropdown-menu-inner-wrap"></ul>');

        // Subscribe to GlobalClick event for hiding language menu when click is outside menu
        EventHub.subscribe("GlobalClick", function () {
            $('.language-menu').removeClass('open');
        });
    };

    return ScrollPane;
})