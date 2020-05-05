(function (angular, moduleName, urlHelper, numberShortener) {
    'use strict';
    angular.module(moduleName, [
            moduleName + "app.templates",
            'cms.webanalytics/campaignreport/campaignReport.component'
    ])
    .controller('app', controller);

    numberShortener(angular);
    
    function controller($location) {
        this.campaignId = urlHelper.getParameters($location.absUrl()).campaignid;
    }

})(angular, moduleName, urlHelper, numberShortener);