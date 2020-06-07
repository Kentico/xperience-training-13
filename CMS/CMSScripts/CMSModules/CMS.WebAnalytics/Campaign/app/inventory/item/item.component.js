(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/inventory/item.component', [
            'cms.webanalytics/campaign/inventory/item/getLink.component',
            'cms.webanalytics/campaign/inventory/editItem.component'
        ])
        .component('cmsCampaignInventoryItem', item());

    function item() {
        return {
            bindings: {
                asset: '=',
                editable: '<',
                utmCode: '<',
                removeAsset: '&'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/item/item.component.html',
            replace: true,
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

        ctrl.updateAsset = function (asset) {
            ctrl.asset.name = asset.Name;
            ctrl.asset.link = asset.Link;
        }
    }
}(angular));