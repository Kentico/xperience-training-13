(function (angular, dataFromServer) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignReport.component', [
        'cms.webanalytics/campaignreport/campaignReportHeader.component',
        'cms.webanalytics/campaignreport/reportTabs.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCampaignReport', report());

    function report() {
        var component = {
            bindings: {
                campaignId: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/campaignReport.component.html',
            controller: controller
        };

        return component;
    }

    function controller() {
        this.report = dataFromServer.report;
    }
}(angular, dataFromServer));