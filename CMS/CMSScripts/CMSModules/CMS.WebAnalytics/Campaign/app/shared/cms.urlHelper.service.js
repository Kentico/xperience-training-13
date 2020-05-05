(function (angular, urlHelper) {
    'use strict';

    angular.module('cms.webanalytics/campaign/cms.urlHelper.service', [])
        .service('urlHelperService', urlHelperService);
    
    function urlHelperService() {
        this.urlHelper = urlHelper;
    }
}(angular, urlHelper));
