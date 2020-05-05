(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/conversions/addConversion.component', [
        'cms.webanalytics/campaign/conversions.service',
        'cms.webanalytics/campaign/reportSetup/conversionsConfiguration.service',
        'cms.webanalytics/campaign/reportSetup/reportSetupDialog.service'
    ])
        .component('cmsAddConversion', addConversion());

    function addConversion() {
        return {
            bindings: {
                onCreated: '<',
                campaignId: '<',
                enabled: '<',
                isFunnel: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/conversions/conversion/addConversion.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(reportSetupDialogService, conversionsService, conversionsConfigurationService) {
        var ctrl = this;

        ctrl.configuration = conversionsConfigurationService.getConfiguration(ctrl.isFunnel);

        ctrl.openDialog = function () {
            reportSetupDialogService.openDialog(ctrl.conversion, ctrl.configuration.dialogHeading)
                .result.then(addConversion);
        };

        function addConversion (conversion) {
            conversion.isFunnelStep = ctrl.isFunnel;
            conversionsService.addConversion(conversion).then(ctrl.onCreated, undefined);
        }
    }
}(angular));
