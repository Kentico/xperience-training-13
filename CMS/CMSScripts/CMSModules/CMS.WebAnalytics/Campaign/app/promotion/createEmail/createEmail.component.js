(function (angular){
    'use strict';

    angular.module('cms.webanalytics/campaign/promotion/createEmail.component', [
        'cms.webanalytics/campaign/promotion/createEmail.controller'
    ])
        .component('cmsCreateEmail', createEmail());

    function createEmail() {
        return {
            bindings: {
                onCreate: '<',
                enabled: '<',
                items: '='
            },
            templateUrl: 'cms.webanalytics/campaign/promotion/createEmail/createEmail.component.html',
            replace: true,
            controller: controller
        }
    }

    /*@ngInject*/
    function controller ($uibModal) {
        var ctrl = this;

        ctrl.openDialog = function () {
            $uibModal.open({
                size: {width: "50%", height: "80%"},
                templateUrl: 'cms.webanalytics/campaign/promotion/createEmail/createEmail.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: 'CreateEmailController',
                resolve: {
                    items: function () {
                        return ctrl.items;
                    }
                }
            })
                .result.then(ctrl.onCreate);
        };
    }
}(angular));