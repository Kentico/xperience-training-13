(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/inventory/urlAssetSetupDialog.service', [
        'cms.webanalytics/campaign/inventory/modifyUrlAsset.controller'
    ])
        .service('urlAssetSetupDialogService', urlAssetSetupDialogService);

    /*@ngInject*/
    function urlAssetSetupDialogService($uibModal, resolveFilter) {
        var that = this;

        that.openDialog = function (asset, title) {
            return $uibModal.open({
                size: { height: '416px', width: '600px' },
                templateUrl: 'cms.webanalytics/campaign/inventory/item/dialog/modifyUrlAsset.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: 'ModifyUrlAssetController',
                resolve: {
                    asset: function () { return asset },
                    title: function () { return resolveFilter(title) }
                }
            }).result;
        }
    }
}(angular));