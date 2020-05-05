(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/tabSwitcher.component', [])
        .component('cmsTabSwitcher', tabSwitcher());

    function tabSwitcher() {
        return {
            bindings: {
                selectedIndex: '<',
                tabs: '<',
                onTabChange: '&'
            },
            templateUrl: 'cms.webanalytics/campaignreport/reportTabs/tabSwitcher.component.html',
            controller: controller
        };
    }

    function controller() {
        var ctrl = this;

        ctrl.activeTab = this.selectedIndex || 0;

        ctrl.changeTab = function (index) {
            ctrl.activeTab = index;
            ctrl.onTabChange({index: index});
        }
    }
}(angular));