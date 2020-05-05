(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/description/textarea.component', [])
        .component('cmsCampaignTextarea', textarea());

    function textarea() {
        return {
            bindings: {
                value: '=',
                title: '@',
                rows: '@',
                label: '@',
                id: '@'
            },
            templateUrl: 'cms.webanalytics/campaign/description/textarea/textarea.component.html',
            replace: true
        }
    }
}(angular));