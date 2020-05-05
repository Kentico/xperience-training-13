(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/inventory/addUrlAsset.component', [
        'cms.webanalytics/campaign/dialogHeader.component',
        'cms.webanalytics/campaign/dialogFooter.component'
    ])
        .component('cmsAddUrlAsset', addUrlAsset());

    function addUrlAsset () {
        return {
            bindings: {
                onAdd: '<',
                campaign: '<',
                urlAssetTargetRegex: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/addUrlAsset/addUrlAsset.component.html',
            controller: controller,
            replace: true
        }
    }

    /*@ngInject*/
    function controller($uibModal) {
        var ctrl = this;

        ctrl.openDialog = function () {
            $uibModal.open({
                size: { height: '416px', width: '600px' },
                templateUrl: 'cms.webanalytics/campaign/inventory/addUrlAsset/addUrlAsset.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: modalController,
                resolve: {
                    urlAssetTargetRegex: function () { return ctrl.urlAssetTargetRegex; }
                }
            })
                .result.then(onConfirm);
        };

        function onConfirm(item) {
            if (item) {
                item.campaignID = ctrl.campaign.campaignID;
            }

            ctrl.onAdd(item);
        }
    }

    /*@ngInject*/
    function modalController($scope, $uibModalInstance, urlAssetTargetRegex) {
        var ctrl = this;
        ctrl.urlAssetTargetRegex = urlAssetTargetRegex;

        ctrl.confirm = function () {
            if ($scope.newAssetUrlForm.$invalid) {
                $scope.newAssetUrlForm.$setSubmitted();
                return;
            }

            $uibModalInstance.close(prepareAssetToCreate());
        };

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss();
        };

        function prepareAssetToCreate() {
            return {
                type: "analytics.campaignasseturl",
                id: 0,
                name: ctrl.pageTitle,
                link: ctrl.target
            };
        }
    }

}(angular));