cmsdefine([], function () {
    return [function () {
        return {
            restrict: 'E',
            require: 'ngModel',
            replace: true,
            template:
'<span>' +
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
                window.RefreshWOpener = function(w) {
                    w.CloseDialog();
                };

                scope.ngClick = function () {
                    if (!scope.click()) {
                        scope.eventCancelled = true;
                    }
                };
                
                window.PassSavedNodeId = function (nodeID) {
                    scope.value = nodeID;
                    ctrl.$setViewValue(scope.value);
                    scope.change();
                };
            }
        };
    }];
});