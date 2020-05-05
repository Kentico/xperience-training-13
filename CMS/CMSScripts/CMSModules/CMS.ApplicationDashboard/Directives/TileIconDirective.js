cmsdefine([], function () {
    var directive = function () {
        return {
            restrict: 'A',
            scope: {
                iconAlternativeText: '= ',
                iconStyle: '@',
                icon: '=tileIcon',
            },
            templateUrl: 'tileIconTemplate.html'
        };
    };

    return [directive];
});
