/**
 * Wraps the CMS.Application module into Angular service to leverage Angular DI system and simplify the tests.
 */
(function (angular, application) {
	'use strict';

    angular.module('cms.contactmanagement/contactprofile/cms.application.service', [])
        .service('applicationService', applicationService);


    /*@ngInject*/    
    function applicationService() {
        this.application = application;
    }

}(angular, application));