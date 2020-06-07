(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/inventory/editItem.component', [
        'cms.webanalytics/campaign/inventory/urlAssetSetupDialog.service',
        'cms.webanalytics/campaign/assets.service',
        'cms.webanalytics/campaign/confirmation.service'
    ])
        .component('cmsEditItem', editItem());

    function editItem() {
        return {
            bindings: {
                asset: '<',
                campaign: '<',
                editable: '<',
                urlAssetTargetRegex: '<',
                onUpdated: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/item/editItem.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(urlAssetSetupDialogService, assetsService, confirmationService) {
        var ctrl = this;

        ctrl.openDialog = function () {
            if (confirmationService.canEditAsset()) {
                urlAssetSetupDialogService.openDialog(ctrl.asset, 'contentedit.header')
                    .then(updateAsset);
            }
        };

        function updateAsset(asset) {
            assetsService.updateAsset(asset).then(ctrl.onUpdated);
        }
    }
}(angular));