cmsdefine([], function () {
    return [function () {
        return {
            restrict: 'E',
            require: 'ngModel',
            replace: true,
            template:
'<span>' +
    '<input style="display: none;" id="{{id}}" type="text" class="form-control" data-ng-model="value" data-ng-change="onChange()"></input>' +
    '<button class="btn btn-default" cms-modal-dialog data-dialog-url={{dialogUrl}} data-dialog-name={{dialogName}} data-dialog-width={{dialogWidth}} data-dialog-height={{dialogHeight}} id="{{buttonId}}" data-ng-disabled="disabled()" data-ng-click="ngClick()">{{text}}</button>' +
'</span>',
            scope: {
                id: "@",
                buttonId: "@",
                value: "=ngModel",
                text: "@",
                dialogUrl: "@",
                dialogName: "@",
                dialogWidth: "@",
                dialogHeight: "@",
                change: "&",
                form: "@",
                disabled: "&",
                click: "&"
            },
            link: function (scope, elm, attrs, ctrl) {
                scope.ctrl = ctrl;

                scope.ngClick = function () {
                    if (!scope.click()) {
                        scope.eventCancelled = true;
                    }
                };

                scope.onChange = function () {
                    ctrl.$setViewValue(scope.value);
                    scope.change();

                    // set the input value back to empty, so that ng-change would be triggered again even if the same asset is chosen next time
                    scope.value = "";
                    ctrl.$setViewValue("");
                };
            }
        };
    }];
});