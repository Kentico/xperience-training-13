(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/inventory/item.component', [
            'cms.webanalytics/campaign/inventory/item/getLink.component'
        ])
        .component('cmsCampaignInventoryItem', item());

    function item() {
        return {
            bindings: {
                asset: '=',
                siteIsContentOnly: '<',
                editable: '<',
                utmCode: '<',
                removeAsset: '&'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/item/item.component.html',
            controller: controller
        };
    }

    function controller() {
        var ctrl = this;

        ctrl.isDeleted = function () {
            return ctrl.asset.id === -1;
        };

        ctrl.isExistingDocument = function () {
            return !ctrl.isDeleted() && (ctrl.asset.type === "cms.document");
        };

        ctrl.isPublished = function () {
            return ctrl.asset.additionalProperties.isPublished;
        };
    }
}(angular));