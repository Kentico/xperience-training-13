(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/conversions/editConversion.component', [
        'cms.webanalytics/campaign/conversions.service',
        'cms.webanalytics/campaign/reportSetup/conversionsConfiguration.service',
        'cms.webanalytics/campaign/reportSetup/reportSetupDialog.service',
        'cms.webanalytics/campaign/confirmation.service'
    ])
        .component('cmsEditConversion', component());

    /*@ngInject*/
    function component() {
        return {
            bindings: {
                conversion: '<',
                isFunnel: '<',
                enabled: '<',
                onUpdated: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/conversions/conversion/editConversion.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(reportSetupDialogService, conversionsService, confirmationService, conversionsConfigurationService) {
        var ctrl = this;

        ctrl.configuration = conversionsConfigurationService.getConfiguration(ctrl.isFunnel);
        
        ctrl.openDialog = function () {
            if (confirmationService.canEditConversion(ctrl.isFunnel)) {
                reportSetupDialogService.openDialog(ctrl.conversion, ctrl.configuration.dialogHeading)
                    .result.then(updateConversion);
            }
        };

        function updateConversion(conversion) {
            conversionsService.updateConversion(conversion).then(ctrl.onUpdated);
        }
    }
}(angular));