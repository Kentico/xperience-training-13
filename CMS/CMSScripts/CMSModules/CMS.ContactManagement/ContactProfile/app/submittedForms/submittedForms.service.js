(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/submittedForms.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('submittedFormsResourceFactory', submittedFormsResourceFactory)
        .service('submittedFormsService', submittedFormsService);


    /*@ngInject*/
    function submittedFormsResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactSubmittedForms?contactId=:contactId');
    };


    /*@ngInject*/
    function submittedFormsService(submittedFormsResourceFactory) {
        this.getSubmittedForms = function (contactId) {
            return submittedFormsResourceFactory.query({ contactId: contactId }).$promise;
        }
    };

}(angular));