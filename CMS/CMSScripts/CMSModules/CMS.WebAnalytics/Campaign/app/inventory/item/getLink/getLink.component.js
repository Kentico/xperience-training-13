(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/inventory/item/getLink.component', [
        'cms.webanalytics/campaign/dialogHeader.component',
        'cms.webanalytics/campaign/dialogFooter.component',
        'cms.webanalytics/campaign/cms.urlHelper.service',
        'cms.webanalytics/campaign/inventory/item/getLink.service'
    ])
        .component('cmsGetLink', getLink());


    function getLink() {
        return {
            bindings: {
                assetLink: '<',
                utmCode: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/item/getLink/getLink.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller($uibModal) {
        var ctrl = this;

        ctrl.openDialog = function () {
            $uibModal.open({
                size: {width: '60%', height: '70%'},
                templateUrl: 'cms.webanalytics/campaign/inventory/item/getLink/getLink.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: modalController,
                resolve: {
                    assetLink: function () { return ctrl.assetLink; },
                    utmCampaign: function () { return ctrl.utmCode; }
                }
            });
        };
    }

    /*@ngInject*/
    function modalController($uibModalInstance, assetLink, utmCampaign, resolveFilter, getLinkService) {
        var ctrl = this;

        ctrl.emptyUtmSourceText = resolveFilter("campaign.getcontentlink.dialog.emptyutmsource");
        ctrl.link = ctrl.emptyUtmSourceText;

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss();
        };

        ctrl.onChange = function () {
            ctrl.link = getLinkService.buildLink(assetLink, utmCampaign, ctrl.utmSource, ctrl.utmMedium, ctrl.utmContent, ctrl.emptyUtmSourceText);
        };

        ctrl.textAreaClick = function (event) {
            event.target.select();
        };
    }
}(angular));
