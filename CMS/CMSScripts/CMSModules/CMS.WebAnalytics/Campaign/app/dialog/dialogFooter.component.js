(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/dialogFooter.component', [])
        .component('cmsDialogFooter', dialogFooter());

    function dialogFooter() {
        return {
            bindings: {
                confirmLabel: '@',
                dismissLabel: '@',
                onConfirm: '<',
                onDismiss: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/dialog/dialogFooter.component.html',
            replace: true
        }
    }
}(angular));
