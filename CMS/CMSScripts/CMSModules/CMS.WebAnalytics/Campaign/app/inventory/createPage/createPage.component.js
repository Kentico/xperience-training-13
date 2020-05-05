(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/inventory/createPage.component', [
        'cms.webanalytics/campaign/serverData.service',
        'cms.webanalytics/campaign/cms.dialogs.module'
    ])
        .component('cmsCreatePage', createPage());

    function createPage() {
        return {
            bindings: {
                onCreate: '<',
                enabled: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/createPage/createPage.component.html',
            replace: true,
            controller: controller
        }
    }

    /*@ngInject*/
    function controller (serverDataService) {
        var ctrl = this;
        
        ctrl.dialogUrl = serverDataService.getCreatePageDialogUrl();

        ctrl.dataChange = function () {
            if (ctrl.newPageAsset) {
                ctrl.onCreate(ctrl.newPageAsset);
            }
        };

        ctrl.dataClick = function () {
            return true;
        }
    }
}(angular));