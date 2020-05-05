(function (angular, application) {
    'use strict';

    angular.module('cms.webanalytics/campaign/cms.application.service', [])
        .service('applicationService', applicationService);

    function applicationService () {
        this.application = application;
    };

}(angular, application));
