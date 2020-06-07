(function (angular, _) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/conversions.component', [
        'cms.webanalytics/campaign/conversions.service',
        'cms.webanalytics/campaign/reportSetup/conversionsConfiguration.service',
        'cms.webanalytics/campaign/reportSetup/conversions/conversion.component',
        'cms.webanalytics/campaign/reportSetup/conversions/addConversion.component',
        'cms.webanalytics/campaign/confirmation.service',
        'cms.webanalytics/campaign/objective.service',
        'CMS/Filters.Resolve'
    ])
    .component('cmsCampaignConversions', campaignConversions());

    function campaignConversions() {
        return {
            bindings: {
                isFunnel: '<',
                conversions: '=',
                campaign: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/conversions/conversions.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(confirmationService, conversionsConfigurationService, conversionsService, objectiveService) {
        var ctrl = this,
            conversionToRemove;

        ctrl.configuration = conversionsConfigurationService.getConfiguration(ctrl.isFunnel);

        ctrl.addConversionToList = function (conversion) {
            if (!containsConversion(conversion)) {
                ctrl.conversions.push(conversion);
            }
        };

        ctrl.removeConversion = function (conversion) {
            if (confirmationService.canRemoveConversion(ctrl.isFunnel)) {
                conversionToRemove = conversion;
                conversionsService.removeConversion(conversion.id).then(ctrl.removeConversionFinished);
            }
        };

        ctrl.removeConversionFinished = function () {
            objectiveService.resetObjective(conversionToRemove);
            ctrl.conversions.splice(ctrl.conversions.indexOf(conversionToRemove), 1);
        };

        ctrl.isRemovable = function () {
            if (ctrl.isFunnel) {
                return ctrl.campaign.isDraft() || !ctrl.campaign.isFinished();
            }
            else {
                var nonFunnelConversions = ctrl.conversions.filter(function (conversion) {
                    return !conversion.isFunnelStep;
                });

                return ctrl.campaign.isDraft() || (!ctrl.campaign.isFinished() && nonFunnelConversions.length !== 1);
            }
        };

        function containsConversion(conversion) {
            return _.find(ctrl.conversions, function (c) {
                return c.id === conversion.id
            });
        }
    }
}(angular, _));