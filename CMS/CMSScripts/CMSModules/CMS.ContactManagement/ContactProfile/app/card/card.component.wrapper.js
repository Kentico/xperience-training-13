(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactprofile/card.component.wrapper', [
        'cms.contactmanagement/contactprofile/card.component',
        'cms.contactmanagement/contactProfile.service',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCardWrapper', card());


    function card() {
        var component = {
            bindings: {
                contactId: '<',
                displayEditButton: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/card/card.component.wrapper.html',
            controller: controller
        };

        return component;
    }


    /*@ngInject*/
    function controller(contactProfileService) {
        activate.apply(this);

        function activate() {
            contactProfileService.getContact(this.contactId)
                .then(onSuccess.bind(this))
                .catch(onError.bind(this));
        }

        function onSuccess(contact) {
            this.contact = contact;
        }

        function onError() {
            this.error = true;
        }
    }

}(angular));