(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/promotion/promotionItem.component', [
        'cms.webanalytics/campaign/autosave.directive',
        'cms.webanalytics/campaign/assets.service'
    ])
        .component('cmsCampaignPromotionItem', component());

    function component() {
        return {
            bindings: {
                asset: '=',
                removeItem: '&',
                editable: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/promotion/promotionItem/promotionItem.component.html',
            controller: controller,
            replace: true
        }
    }

    /*@ngInject*/
    function controller($scope, assetsService) {
        var ctrl = this;

        ctrl.inputId = 'email_' + ctrl.asset.id;
        ctrl.formName = ctrl.asset.id === -1 ? '' : 'promotionForm' + ctrl.asset.id;

        ctrl.getUtmSourceField = function () {
            if (!ctrl.utmSourceField && !ctrl.isDeleted()) {
                ctrl.utmSourceField = $scope[ctrl.formName][ctrl.inputId];
            }

            return ctrl.utmSourceField;
        };

        ctrl.isDeleted = function () {
            return ctrl.asset.id === -1;
        };

        ctrl.save = function () {
            assetsService.updateAsset(ctrl.asset);
        };
    }
}(angular));