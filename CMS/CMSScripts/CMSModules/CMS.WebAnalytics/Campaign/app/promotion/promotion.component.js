(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/promotion.component', [
        'cms.webanalytics/campaign/assets.service',
        'cms.webanalytics/campaign/promotion/createEmail.component',
        'cms.webanalytics/campaign/promotion/selectEmail.component',
        'cms.webanalytics/campaign/promotion/promotionItem.component',
        'cms.webanalytics/campaign/confirmation.service',
        'CMS/Filters.Resolve'
    ])
    .component('cmsCampaignPromotion', promotion());

    function promotion() {
        return {
            bindings: {
                items: '=',
                campaign: '='
            },
            templateUrl: 'cms.webanalytics/campaign/promotion/promotion.component.html',
            replace: true,
            controller: controller
        };
    }

    /*@ngInject*/
    function controller(assetsService, confirmationService) {
        var ctrl = this;

        ctrl.addPromotionItem = function (itemId) {
            assetsService.addAsset(itemId, 'newsletter.issue').then(addItemToListing);
        };

        ctrl.removeItem = function (item) {
            if (confirmationService.canRemoveAsset()) {
                assetsService.removeAsset(item.assetID).then(function () {
                    delete ctrl.items[item.assetID];
                })
            }
        };

        function addItemToListing(newItem) {
            ctrl.items[newItem.assetID] = newItem;
        }
    }
}(angular));