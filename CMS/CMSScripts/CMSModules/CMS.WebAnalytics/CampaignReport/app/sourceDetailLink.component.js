(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/sourceDetailLink.component', [])
        .component('cmsSourceDetailLink', report());

    function report() {
       return {
            bindings: {
                link: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/sourceDetailLink.component.html'
        };
    }
}(angular));