(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/description.component', [
            'cms.webanalytics/campaign.service',
            'cms.webanalytics/campaign/autosave.directive',
            'cms.webanalytics/campaign/description/textbox.component',
            'cms.webanalytics/campaign/description/textarea.component'
        ])
        .component('cmsCampaignDescription', description());

    function description() {
        return {
            bindings: {
                campaign: '='
            },
            templateUrl: 'cms.webanalytics/campaign/description/description.component.html',
            replace: true,
            controller: controller
        };
    }

    /*@ngInject*/
    function controller (campaignService) {
        var ctrl = this;
        ctrl.serverValidationMessage = undefined;

        ctrl.saveCampaign = function () {
            ctrl.serverValidationMessage = '';
            campaignService.saveCampaign(function (message) {
                ctrl.serverValidationMessage = message;
            });
            
        };
    }
}(angular));