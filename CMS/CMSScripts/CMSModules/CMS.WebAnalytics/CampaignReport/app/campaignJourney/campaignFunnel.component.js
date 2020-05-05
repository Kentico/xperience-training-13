(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignFunnel.component', [
        'cms.webanalytics/campaignreport/campaignFunnelChart.component',
        'cms.webanalytics/campaignreport/campaignFunnelTable.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCampaignFunnel', funnel());

    function funnel() {
        return {
            bindings: {
                report: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/campaignJourney/campaignFunnel.component.html',
            controller: controller
        };
    }

    function controller() {
        var ctrl = this;

        /* Filter only conversions which belong to funnel */
        ctrl.conversions = ctrl.report.conversions.filter(function(conversion) {
            return conversion.isFunnelStep;
        });
    }

}(angular));