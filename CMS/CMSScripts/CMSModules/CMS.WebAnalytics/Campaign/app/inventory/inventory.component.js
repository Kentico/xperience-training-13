(function (angular) {
    'use strict';
    
    angular.module('cms.webanalytics/campaign/inventory.component', [
            'cms.webanalytics/campaign.service',
            'cms.webanalytics/campaign/assets.service',
            'cms.webanalytics/campaign/messages.service',
            'cms.webanalytics/campaign/inventory/addUrlAsset.component',
            'cms.webanalytics/campaign/inventory/item.component',
            'cms.webanalytics/campaign/confirmation.service',
            'CMS/Filters.Resolve',
            'ngResource'
    ])
        .component('cmsCampaignInventory', inventory());

    function inventory() {
        return {
            bindings: {
                assets: '=',
                campaign: '=',
                urlAssetTargetRegex: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/inventory.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(assetsService, confirmationService) {
        var ctrl = this;

        ctrl.addPageAsset = function (pageId) {
            assetsService.addAsset(pageId, "cms.document").then(addAssetToListing);
        };
        
        ctrl.addUrlAsset = function (asset) {
            assetsService.addUrlAsset(asset).then(addAssetToListing);
        };

        ctrl.removeAsset = function (asset) {
            if (confirmationService.canRemoveAsset()) {
                assetsService.removeAsset(asset.assetID).then(function () {
                    delete ctrl.assets[asset.assetID];
                })
            }
        };

        function addAssetToListing(newAsset) {
            ctrl.assets[newAsset.assetID] = newAsset;
        }
    }
}(angular));