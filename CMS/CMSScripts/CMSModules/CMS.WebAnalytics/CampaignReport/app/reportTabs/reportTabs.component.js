(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/reportTabs.component', [
        'cms.webanalytics/campaignreport/conversionReport.component',
        'cms.webanalytics/campaignreport/tabSwitcher.component',
        'cms.webanalytics/campaignreport/campaignFunnel.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsReportTabs', reportTabs());

    function reportTabs() {
        return {
            bindings: {
                report: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/reportTabs/reportTabs.component.html',
            controller: controller
        };
    }

    /*@ngInject*/
    function controller(resolveFilter) {
        var ctrl = this;

        ctrl.nonFunnelConversions = ctrl.report.conversions.filter(function (c) {
            return !c.isFunnelStep
        });
        
        ctrl.tabIndex = 0;
        ctrl.tabs = [
            resolveFilter('campaign.conversions'),
            resolveFilter('campaign.journey')
        ];

        ctrl.changeTab = function (index) {
            ctrl.tabIndex = index;
        }
    }
}(angular));