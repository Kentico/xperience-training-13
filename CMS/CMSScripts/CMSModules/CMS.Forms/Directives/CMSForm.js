cmsdefine([], function () {
    return [function () {
        return {
            restrict: "E",
            template: 
'<form>' +
    '<div data-ng-transclude="" class="form-horizontal"></div>' +
'</form>',
            transclude: true,
            replace: true
        };
            
    }];
});