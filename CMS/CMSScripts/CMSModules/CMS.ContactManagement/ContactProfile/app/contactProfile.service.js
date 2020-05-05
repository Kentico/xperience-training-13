(function(angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactProfile.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
    ])
    .factory('contactProfileResourceFactory', contactProfileResourceFactory)
    .service('contactProfileService', contactProfileService);


    /*@ngInject*/
    function contactProfileResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactprofile?contactId=:contactId');
    };
    
    
    /*@ngInject*/
    function contactProfileService(contactProfileResourceFactory) {
        this.getContact = function (contactId) {
            return contactProfileResourceFactory.get({ contactId: contactId }).$promise;
        }
    };

}(angular));