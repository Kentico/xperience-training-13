(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/newsletterSubscriptions.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('newsletterSubscriptionsResourceFactory', newsletterSubscriptionsResourceFactory)
        .service('newsletterSubscriptionsService', newsletterSubscriptionsService);


    /*@ngInject*/    
    function newsletterSubscriptionsResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactNewsletterSubscriptions?contactId=:contactId');
    };


    /*@ngInject*/
    function newsletterSubscriptionsService(newsletterSubscriptionsResourceFactory) {
        this.getSubscriptionsForContact = function (contactId) {
            return newsletterSubscriptionsResourceFactory.query({ contactId: contactId }).$promise;
        }
    };

}(angular));