(function (angular, _) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/objective.component', [        
        'cms.webanalytics/campaign/reportSetup/objective/addObjective.component',
        'cms.webanalytics/campaign/reportSetup/objective/editObjective.component',
        'cms.webanalytics/campaign/objective.service',
        'CMS/Filters.NumberShortener'
    ])
    .component('cmsCampaignObjective', campaignObjectives());

    function campaignObjectives() {
        return {
            bindings: {
                campaign: '<',
                conversions: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/objective/objective.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(confirmationService, objectiveService) {
        var ctrl = this;

        objectiveService.registerResetObjectiveCallback(resetObjective);
        ctrl.objective = objectiveService.getObjective();
        ctrl.selectedConversionName = getSelectedConversionName(ctrl.objective);       

        ctrl.canAddObjective = function () {
            var hasAnyConversion = _.find(ctrl.conversions, function(conversion) {
                return !conversion.isFunnelStep;
            });
            return !ctrl.campaign.isFinished() && hasAnyConversion && !ctrl.objective;
        }

        ctrl.addObjective = function (objective) {
            ctrl.objective = objective;
            ctrl.selectedConversionName = getSelectedConversionName(objective);
        };

        ctrl.removeObjective = function () {
            if (!ctrl.campaign.isFinished() && ctrl.objective && confirmationService.canRemoveObjective()) {
                objectiveService.removeObjective(ctrl.objective.id).then(ctrl.removeObjectiveFinished);
            }
        };

        ctrl.updateObjective = function (objective) {
            angular.copy(objective, ctrl.objective);
            ctrl.selectedConversionName = getSelectedConversionName(objective);
        }

        ctrl.removeObjectiveFinished = function () {
            ctrl.objective = null;
        };

        function getSelectedConversionName(objective) {

            if (!objective) {
                return null;
            }

            var selectedConversion = _.find(ctrl.conversions, function (conversion) {
                return conversion.id === objective.conversionID;
            });

            var conversionName = selectedConversion.activityName;
            if (selectedConversion.name !== '') {
                conversionName = conversionName + ': ' + selectedConversion.name;
            }

            return conversionName;
        }

        function resetObjective(conversion) {
            if (ctrl.objective && !ctrl.objective.isFunnelStep && ctrl.objective.conversionID === conversion.id) {
                ctrl.objective = null;
            }
        }
    }
}(angular, _));