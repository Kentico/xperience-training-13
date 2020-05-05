(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/inventory/selectPage.component', [
        'cms.webanalytics/campaign/dialogHeader.component',
        'cms.webanalytics/campaign/dialogFooter.component',
        'cms.webanalytics/campaign/cms.application.service',
        'CMS/Filters.Resolve'
    ])
        .component('cmsSelectPage', component());

    function component() {
        return {
            bindings: {
                enabled: '<',
                onSelect: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/inventory/selectPage/selectPage.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller($uibModal) {
        var ctrl = this;

        ctrl.openDialog = function () {
            $uibModal.open({
                size: { height: '220px', width: '500px' },
                templateUrl: 'cms.webanalytics/campaign/inventory/selectPage/selectPage.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: modalController
            })
                .result.then(ctrl.onConfirm);
        };

        ctrl.onConfirm = function (item) {
            if (item) {
                ctrl.onSelect(item);
            }
        };
    }

    /*@ngInject*/
    function modalController($uibModalInstance, $interpolate, applicationService) {
        var ctrl = this,
            application = applicationService.application;

        ctrl.restUrl = application.getData('applicationUrl') + 'cmsapi/CampaignConversionPage';
        ctrl.restUrlParams = { objType: "page" };

        ctrl.resultTemplate = function (selectedPage) {
            var itemTemplate =
                    '<div class="campaign-conversion-page-item ">' +
                    '<div class="campaign-conversion-page-icon"> {{icon}} </div>' +
                    '<div class="campaign-conversion-page-content">' +
                    '<strong> {{text}} </strong>' +
                    '<div> {{path}} </div>' +
                    '</div>' +
                    '</div>',

                pageView = angular.copy(selectedPage);

            // Ensure escaping for path and text
            pageView.path = _.escape(pageView.path);
            pageView.text = _.escape(pageView.text);

            return $interpolate(itemTemplate)(pageView);
        };

        ctrl.isSelectionValid = function () {
            return ctrl.detail != null ? ctrl.detail.id > 0 : true;
        }

        ctrl.confirm = function () {
            ctrl.form.$setSubmitted();
            if (ctrl.isSelectionValid()) {
                $uibModalInstance.close(ctrl.detail.id);
            }
        };

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss();
        }
    }
}(angular));
