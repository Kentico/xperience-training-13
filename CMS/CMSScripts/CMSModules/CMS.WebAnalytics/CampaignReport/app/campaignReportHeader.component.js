(function (angular, campaignStatusComponent) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignReportHeader.component', [
        'cms.webanalytics/campaignreport/campaignObjective.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCampaignStatus', campaignStatusComponent)
        .component('cmsCampaignReportHeader', header());

    function header() {
        var component = {
            bindings: {
                report: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/campaignReportHeader.component.html'
        };

        return component;
    }
}(angular, campaignStatusComponent));