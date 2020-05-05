(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactProfile.component', [
        'cms.contactmanagement/contactProfileSimple.component',
        'cms.contactmanagement/contactProfileFull.component',
        'cms.contactmanagement/contactProfile.service',
        'CMS/Filters.Resolve'
    ])
    .component('cmsContactProfile', contact());


    function contact() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/contactProfile.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(contactProfileService) {
        activate.apply(this);

        function activate() {
            contactProfileService.getContact(this.contactId)
                .then(onSuccess.bind(this))
                .catch(onError.bind(this));
        };

        function onSuccess(contact) {
            this.contact = contact;
            if (contact.ContactType === "Simple") {
                this.simpleContact = true;
            } else {
                this.fullContact = true;
            }
        };

        function onError() {
            this.error = true;
        };
    };
}(angular));