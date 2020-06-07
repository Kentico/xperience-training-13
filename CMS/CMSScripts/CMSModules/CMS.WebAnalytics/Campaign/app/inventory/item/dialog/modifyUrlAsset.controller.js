(function (angular, _) {
    'use strict';

    angular.module('cms.webanalytics/campaign/inventory/modifyUrlAsset.controller', [
        'cms.webanalytics/campaign/cms.application.service'
    ])
        .controller('ModifyUrlAssetController', controller);

    /*@ngInject*/
    function controller($uibModalInstance, serverDataService, asset, title) {
        var ctrl = this;

        ctrl.title = title;
        ctrl.urlAssetTargetRegex = serverDataService.getUrlAssetTargetRegex();
        ctrl.asset = angular.copy(asset)

        if (ctrl.asset != null) {
            ctrl.asset.campaignID = String(ctrl.asset.campaignID);
        }

        ctrl.confirm = function () {
            ctrl.form.$setSubmitted();
            if (ctrl.form.$valid) {

                if (!assetChanged()) {
                    ctrl.dismiss();
                }

                $uibModalInstance.close(ctrl.asset);
            }
        };

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss();
        };

        function assetChanged() {
            return !asset
                || asset.name !== ctrl.asset.pageTitle
                || asset.link !== ctrl.asset.target;
        }

    }
}(angular, _));