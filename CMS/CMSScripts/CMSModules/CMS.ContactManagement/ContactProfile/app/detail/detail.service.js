(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/detail.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('detailResourceFactory', detailResourceFactory)
        .service('detailService', detailService);


    /*@ngInject*/
    function detailResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/ContactDetails?contactId=:contactId');
    };


    /*@ngInject*/
    function detailService(detailResourceFactory) {
        this.getDetail = function (contactId) {
            return detailResourceFactory.query({ contactId: contactId }).$promise;
        };
    };
}(angular));