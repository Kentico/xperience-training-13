(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/objective/addObjective.component', [
        'cms.webanalytics/campaign/reportSetup/objective/objectiveSetupDialog.service',
        'cms.webanalytics/campaign/objective.service'
    ])
        .component('cmsAddObjective', addObjective());

    function addObjective() {
        return {
            bindings: {
                objectiveConversions: '<',
                enabled: '<',
                onCreated: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/objective/addObjective.component.html',
            controller: controller,
            replace: true
        };
    }

    /*@ngInject*/
    function controller(objectiveSetupDialogService, objectiveService) {
        var ctrl = this;

        ctrl.openDialog = function () {
            objectiveSetupDialogService.openDialog(ctrl.objectiveConversions, null, 'campaign.objective.defineobjective')
                .then(addObjective);
        };

        function addObjective(objective) {
            objectiveService.addObjective(objective).then(ctrl.onCreated);
        }
    }
}(angular));
