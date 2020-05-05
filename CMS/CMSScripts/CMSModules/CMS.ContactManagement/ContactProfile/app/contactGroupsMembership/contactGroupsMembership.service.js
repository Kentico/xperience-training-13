(function(angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/contactGroupsMembership.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('contactGroupsMembershipResourceFactory', contactGroupsMembershipResourceFactory)
        .service('contactGroupsMembershipService', contactGroupsMembershipService);
    

    /*@ngInject*/
    function contactGroupsMembershipResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactGroupsMembership?contactId=:contactId');
    };


    /*@ngInject*/
    function contactGroupsMembershipService(contactGroupsMembershipResourceFactory) {
        this.getMembershipsForContact = function(contactId) {
            return contactGroupsMembershipResourceFactory.query({ contactId: contactId }).$promise;
        }
    };

}(angular));