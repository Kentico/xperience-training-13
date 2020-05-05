cmsdefine([], function () {
    return [function () {
        return {
            restrict: 'E',
            require: 'ngModel',
            replace: true,
            template:
'<span>' +
    '<button class="btn btn-default" cms-modal-dialog data-dialog-url="{{dialogUrl}}" data-dialog-name={{dialogName}} ' +
            'data-dialog-width={{dialogWidth}} data-dialog-height={{dialogHeight}} id="{{buttonId}}" data-ng-disabled="disabled()" data-ng-click="ngClick()">{{text}}</button>' +
'</span>',
            scope: {
                id: "@",
                buttonId: "@",
                text: "@",
                value: "=ngModel",
                dialogUrl: "@",
                dialogName: "@",
                dialogWidth: "@",
                dialogHeight: "@",
                form: "@",
                change: "&",
                disabled: "&",
                click: "&"
            },
            link: function (scope, elm, attrs, ctrl) {
                scope.ngClick = function () {
                    if (!scope.click()) {
                        scope.eventCancelled = true;
                    }
                };
                window["US_SelectItems_" + scope.id] = function (items) {
                    scope.value = items;
                    ctrl.$setViewValue(scope.value);
                    scope.change();
                };
            }
        };
    }];
});