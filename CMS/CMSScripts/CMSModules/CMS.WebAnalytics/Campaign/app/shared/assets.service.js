(function (angular, assetResourceFactory, newsletterResourceFactory) {
    'use strict';

    angular.module('cms.webanalytics/campaign/assets.service', [
        'cms.webanalytics/campaign.service'
    ])
        .factory('newsletterResource', newsletterResourceFactory)
        .factory('assetResource', assetResourceFactory)
        .service('assetsService', assetsService);

    /*@ngInject*/
    function assetsService($q, campaignService, assetResource, messagesService, serverDataService) {
        var that = this,
            assets = serverDataService.getAssets();

        that.getInventoryItems = function () {
            var inventoryItems = {};
            angular.forEach(assets, function (item) {
                if (item.type !== 'newsletter.issue') {
                    inventoryItems[item.assetID] = item;
                }
            });
            return inventoryItems;
        };

        that.getPromotionItems = function () {
            var promotionItems = {};
            angular.forEach(assets, function (item) {
                if (item.type === 'newsletter.issue') {
                    promotionItems[item.assetID] = item;
                }
            });
            return promotionItems;
        };

        that.addAsset = function (id, type) {
            return campaignService.ensureCampaignExists().then(function () {
                var model = {
                    campaignId: campaignService.getCampaign().campaignID,
                    id: id,
                    type: type
                };
                
                return assetResource.create(model).$promise.then(addAssetFinished, requestFailed);
            })
        };

        that.addUrlAsset = function (asset) {
            return campaignService.ensureCampaignExists().then(function() {
                asset.campaignId = campaignService.getCampaign().campaignID;
                return assetResource.create(asset).$promise.then(addAssetFinished, requestFailed);
            });
        };

        that.updateAsset = function (asset) {
            return assetResource.update(asset).$promise.then(requestFinished, requestFailed);
        };

        that.removeAsset = function (id) {
            var model = {
                campaignAssetId: id
            };

            return assetResource.delete(model).$promise.then(requestFinished, requestFailed);
        };


        function addAssetFinished(response) {
            requestFinished(response);

            return {
                assetID: response.AssetID,
                id: response.ID,
                link: response.Link,
                name: response.Name,
                type: response.Type,
                campaignID: response.CampaignID,
                additionalProperties: response.AdditionalProperties
            };
        }

        function requestFailed(response) {
            if (response && response.data) {
                messagesService.sendError(response.data);
            }
            else {
                messagesService.sendError('campaign.assets.failed', true);
            }

            return $q.reject();
        }

        function requestFinished(response) {
            messagesService.sendSuccess('campaign.autosave.finished', true);
            return response;
        }
    }
}(angular, assetResourceFactory, newsletterResourceFactory));