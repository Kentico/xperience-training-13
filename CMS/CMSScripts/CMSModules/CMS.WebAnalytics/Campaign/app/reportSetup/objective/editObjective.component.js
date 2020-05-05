(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/objective/editObjective.component', [
        'cms.webanalytics/campaign/reportSetup/objective/objectiveSetupDialog.service',
        'cms.webanalytics/campaign/objective.service',
        'cms.webanalytics/campaign/confirmation.service'
    ])
        .component('cmsEditObjective', component());

    function component() {
        return {
            bindings: {
                objectiveConversions: '<',
                enabled: '<',
                onUpdated: '<',
                objective: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/objective/editObjective.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(objectiveSetupDialogService, objectiveService, confirmationService) {
        var ctrl = this;

        ctrl.openDialog = function () {
            if (confirmationService.canEditObjective()) {
                objectiveSetupDialogService.openDialog(ctrl.objectiveConversions, ctrl.objective, 'campaign.objective.defineobjective')
                    .then(updateObjective);
            }
        };

        function updateObjective(objective) {
            objectiveService.updateObjective(objective).then(ctrl.onUpdated);
        }
    }
}(angular));