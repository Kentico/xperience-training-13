(function (angular) {
    'use strict'

    angular.module('cms.webanalytics/campaign/reportSetup/reportSetupDialog.service', [
        'cms.webanalytics/campaign/reportSetup/modifyConversion.controller'
    ])
        .service('reportSetupDialogService', reportSetupDialogService);

    /*@ngInject*/
    function reportSetupDialogService($uibModal, resolveFilter, serverDataService) {
        var that = this,
            height = '450px';

        that.openDialog = function (conversion, title) {
            return $uibModal.open({
                size: { height: height, width: '600px' },
                templateUrl: 'cms.webanalytics/campaign/reportSetup/dialog/modifyConversion.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: 'ModifyConversionController',
                resolve: {
                    conversion: function () { return conversion },
                    title: function () { return resolveFilter(title) }
                }
            });
        }
    }
}(angular));