(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactprofile/detail.component', [
        'cms.contactmanagement/contactprofile/detail.service',
        'cms.contactmanagement/contactprofile/detail/accounts.component',
        'cms.contactmanagement/contactprofile/detail/campaign/campaign.component',
        'CMS/Filters.Resolve'
    ])
    .component('cmsDetail', details());


    function details() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/detail/detail.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(detailService) {
        function activate() {
            detailService.getDetail(this.contactId)
                .then(onSuccess.bind(this));
        };

        function onSuccess(fields) {
            this.fields = fields;
        };

        activate.apply(this);
    };
}(angular));