(function (angular, cmsChart) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignFunnelChart.component', [
        'cms.webanalytics/campaignreport/campaignFunnel.service'
    ])
        .component('cmsCampaignFunnelChart', funnel());

    function funnel() {
        return {
            bindings: {
                chartId: '@',
                conversions: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/campaignJourney/campaignFunnelChart.component.html',
            controller: controller
        };
    }

    /*@ngInject*/
    function controller($timeout, campaignFunnelService) {
        var ctrl = this,
            chartData = campaignFunnelService.initChartData(ctrl.conversions);

        ctrl.legendId = ctrl.chartId + 'legend';

        ctrl.hasData = function () {
            return chartData && chartData.maxValue;
        };

        // Chart requires fully rendered HTML (to locate containers by ID) - this is why it's called from $timeout
        $timeout(function () {
            if (ctrl.hasData()) {
                createChart();
            }
        });

        function createChart () {
            cmsChart({
                chartDiv: ctrl.chartId,
                legendDiv: ctrl.legendId,
                data: chartData.data,
                maxValue: chartData.maxValue
            });
        }
    }
}(angular, cmsChart));