(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/persona.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('personaResourceFactory', personaResourceFactory)
        .service('personaService', personaService);


    /*@ngInject*/
    function personaResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/ContactPersona?contactId=:contactId');
    };


    /*@ngInject*/
    function personaService(personaResourceFactory) {
        this.getPersona = function (contactId) {
            return personaResourceFactory.get({ contactId: contactId }).$promise;
        }
    };

}(angular));