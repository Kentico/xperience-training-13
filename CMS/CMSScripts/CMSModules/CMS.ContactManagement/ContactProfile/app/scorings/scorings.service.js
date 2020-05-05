(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/scorings.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('scoringResourceFactory', scoringResourceFactory)
        .service('scoringService', scoringService);


    /*@ngInject*/
    function scoringResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/ContactScoring?contactId=:contactId');
    };


    /*@ngInject*/
    function scoringService(scoringResourceFactory) {
        this.getScorings = function(contactId) {
            return scoringResourceFactory.query({ contactId: contactId }).$promise;
        };
    };
}(angular));