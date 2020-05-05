(function (angular, Tabs) {
    'use strict';

    angular.module('cms.webanalytics/tabs.service', [])
        .service('tabsService', tabsService);

    /*@ngInject*/
    function tabsService() {
        var that = this,
            $tabsElement = getTabsElement(),
            $scheduleTab = getScheduleTab();

        that.updateTabLinks = function (campaignId) {
            var tabs = new Tabs();

            tabs.ensureQueryParamForTabs($tabsElement, "campaignid", campaignId);
            tabs.ensureQueryParamForTabs($tabsElement, "objectid", campaignId);
        };

        that.navigateScheduleTab = function () {
            $scheduleTab.click();
        };

        function getTabsElement() {
            return $cmsj("ul.nav.nav-tabs", window.parent.document);
        }

        function getScheduleTab() {
            return $cmsj($tabsElement.find('li[data-href] > a')[1]);
        }
    }
}(angular, Tabs));