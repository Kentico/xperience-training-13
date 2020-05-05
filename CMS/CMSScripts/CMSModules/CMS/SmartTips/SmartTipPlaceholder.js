cmsdefine(['CMS/SmartTips/SmartTip'], function (SmartTip) {

    
    var directive = function (resources) {
        return {
            restrict: 'E',
            scope: {
                smartTip: '<',
                onReady: '&'
            },
            link: function (scope, element) {
                element.attr('id', scope.smartTip.selector.substring(1));
                scope.smartTip.resources = resources;
                SmartTip(scope.smartTip, scope.onReady);
            }  
        };
    };

    return ['resources', directive];
})
