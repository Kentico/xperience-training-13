(function (angular){
    'use strict';

    angular.module('cms.webanalytics/campaign/promotion/selectEmail.component', [
        'cms.webanalytics/campaign/serverData.service',
        'cms.webanalytics/campaign/cms.dialogs.module'
    ])
        .component('cmsSelectEmail', selectItem());

    function selectItem() {
        return {
            bindings: {
                onSelect: '<',
                enabled: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/promotion/selectEmail/selectEmail.component.html',
            replace: true,
            controller: controller
        }
    }

    /*@ngInject*/
    function controller(serverDataService) {
        var ctrl = this;

        ctrl.dialogUrl = serverDataService.getSelectEmailDialogUrl();

        ctrl.onChange = function () {
            if (ctrl.newEmailAsset) {
                ctrl.onSelect(ctrl.newEmailAsset);
            }
        };

        ctrl.onClick = function () {
            return true;
        }
    }
}(angular));