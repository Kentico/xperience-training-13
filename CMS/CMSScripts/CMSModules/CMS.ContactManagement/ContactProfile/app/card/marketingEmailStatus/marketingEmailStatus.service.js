(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/card/marketingEmailStatus.service', [
            'cms.contactmanagement/contactprofile/cms.application.service',
            'ngResource'
        ])
        .factory('marketingEmailStatusResourceFactory', marketingEmailStatusResourceFactory)
        .service('marketingEmailStatusService', marketingEmailStatusService);


    /*@ngInject*/
    function marketingEmailStatusResourceFactory($resource, applicationService) {
        return $resource(applicationService.application.getData('applicationUrl') + 'cmsapi/contactMarketingEmailStatus?contactId=:contactId');
    };


    /*@ngInject*/
    function marketingEmailStatusService(marketingEmailStatusResourceFactory) {
        this.getMarketingEmailStatus = function (contactId) {
            return marketingEmailStatusResourceFactory.get({ contactId: contactId }).$promise;
        }
    };

}(angular));