(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignObjective.component', [
        'CMS/Filters.Resolve',
        'CMS/Filters.NumberShortener'
    ])
        .component('cmsCampaignObjective', objectiveComponent());

    function objectiveComponent() {
        return {
            bindings: {
                objective: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/campaignObjective.component.html'
        };
    }
}(angular));