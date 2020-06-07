(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup.component', [
        'cms.webanalytics/campaign/reportSetup/conversions.component',
        'cms.webanalytics/campaign/reportSetup/objective.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCampaignReportSetup', reportSetup());

    function reportSetup() {
        return {
            bindings: {
                conversions: '=',
                campaign: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/reportSetup.component.html',
            replace: true
        };
    }
}(angular));