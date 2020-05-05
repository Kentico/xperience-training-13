cmsdefine([], function () {
    return [function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.bind('click', function () {
                    if (scope.eventCancelled) {
                        scope.eventCancelled = false;
                    } else {
                        modalDialog(attrs.dialogUrl, attrs.dialogName, attrs.dialogWidth, attrs.dialogHeight);
                    }
                });
            }
        };
    }];
});