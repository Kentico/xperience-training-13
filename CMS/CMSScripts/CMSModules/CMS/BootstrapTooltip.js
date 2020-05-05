cmsdefine(['jQuery'], function ($) {

    var Module = function(data) {
        var ttElems = $(data.selector);
        ttElems.mouseover(function() {
            $(this).parents('[title]').each(
                function() {
                    var elem = $(this);
                    elem.attr('data-orig-title', elem.attr('title'));
                    elem.removeAttr('title');
                }
            );
        });
        ttElems.mouseout(function() {
            $(this).parents('[data-orig-title]').each(
                function() {
                    var elem = $(this);
                    elem.attr('title', elem.attr('data-orig-title'));
                    elem.removeAttr('data-orig-title');
                }
            );
        });
        ttElems.tooltip({
            delay: { show: 700, hide: 100 },
            container: 'body',
            placement: 'auto right',
            template: data.templateSelector ? data.templateSelector : undefined
        });
    };

    return Module;
});