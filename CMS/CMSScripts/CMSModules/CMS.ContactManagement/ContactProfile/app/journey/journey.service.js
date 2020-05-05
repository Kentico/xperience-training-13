(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/journey.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('journeyResourceFactory', journeyResourceFactory)
        .service('journeyService', journeyService);


    /*@ngInject*/
    function journeyResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactJourney?contactId=:contactId');
    };


    /*@ngInject*/
    function journeyService(journeyResourceFactory) {
        this.getJourneyForContact = function (contactId) {
            return journeyResourceFactory.get({ contactId: contactId }).$promise;
        }
    };

}(angular));