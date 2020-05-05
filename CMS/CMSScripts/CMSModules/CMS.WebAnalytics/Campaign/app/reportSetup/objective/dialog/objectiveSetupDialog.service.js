(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/objective/objectiveSetupDialog.service', [
        'cms.webanalytics/campaign/reportSetup/objective/modifyObjective.controller'
    ])
        .service('objectiveSetupDialogService', objectiveSetupDialogService);

    /*@ngInject*/
    function objectiveSetupDialogService($uibModal, resolveFilter) {
        var that = this;

        that.openDialog = function (objectiveConversions, objective, title) {
            return $uibModal.open({
                size: { height: '400px', width: '600px' },
                templateUrl: 'cms.webanalytics/campaign/reportSetup/objective/dialog/modifyObjective.dialog.html',
                bindToController: true,
                controllerAs: '$ctrl',
                controller: 'ModifyObjectiveController',
                resolve: {
                    objectiveConversions: function () { return objectiveConversions },
                    objective: function () { return objective },
                    title: function () { return resolveFilter(title) }
                }
            }).result;
        }
    }
}(angular));