$cmsj(function () {
    (function ($) {
        // When clicking the dropdown menu, it will NOT disappear, unless a link was clicked.
        $(".dropdown-menu").click(function (event) {
            if (!$(event.target).closest('a').length) {
                event.stopPropagation();
            }
        });

        // Prevent hiding anchor dropup menu after clicking a link inside of dropup.
        $(".anchor-dropup .dropdown-menu a").click(function (event) {
            event.stopPropagation();
        });

        // Disable hiding anchor dropup menu when clicking outside of it.
        var allowClose = false;
        $('.anchor-dropup').on({
            "shown.bs.dropdown": function () { allowClose = false; },
            "click": function () { allowClose = true; },
            "hide.bs.dropdown": function () { if (!allowClose) return false; }
        });
        
        // On/off switcher
        $('.has-switch').click(function () {
            $(this).find('.switch').toggle();
            var $switch = $(this).find('input[type=checkbox]');
            if ($switch.prop('checked')) {
                $switch.prop('checked', false);
            } else {
                $switch.prop('checked', true);
            }
        });

        // Fix position of localization flag icon for scrolling textareas   
        $('.cms-input-group').each(function () {
            $(this).on('checkScrollbar', function () {
                var $textarea = $(this).find('textarea');
                if ($textarea.length > 0) {
                    if ($textarea[0].clientHeight < $textarea[0].scrollHeight) {
                        $(this).addClass("has-scroller");
                    } else {
                        $(this).removeClass("has-scroller");
                    }
                }
            });
        });
        $('textarea').bind('keyup mouseup mouseout', function () {
            $(this).parent('.input-localized').trigger('checkScrollbar');
        });
        $('.input-localized').each(function () {
            $(this).trigger('checkScrollbar');
        });
    }((function ($) {

        var scopedjQuery = function (selector) {
            var $elem = $(selector),
                $emptyElement = $(),
                $elemClosestBootstrapParent;

            if ($elem.length === 0) {
                // jQuery element object itself was not found in the document,
                // we need to return the empty $element to follow the jQuery-way
                // of handling non-found elements
                return $emptyElement;
            }

            if ($elem.parent().length === 0) {
                // $element could be Document or Window, or it is not in the DOM at all (
                // it lives only in memory as jQuery object)
                // in this case it doesn't have to have '.cms-bootstrap' as its parent
                return $elem;
            }

            $elemClosestBootstrapParent = $elem.closest('.cms-bootstrap, .cms-bootstrap-js');
            if ($elemClosestBootstrapParent && ($elemClosestBootstrapParent.length > 0)) {
                // All other elements need to have '.cms-bootstrap' as their parent element
                return $elem;
            }

            // Otherwise we need to return the empty $element to follow the jQuery-way
            // of handling non-found elements
            return $emptyElement;
        };

        // Copy all jQuery properties into
        // scopedjQuery so they can be used later
        for (var k in $) {
            if ($.hasOwnProperty(k)) {
                scopedjQuery[k] = $[k];
            }
        }

        return scopedjQuery;

    }($cmsj))));
});