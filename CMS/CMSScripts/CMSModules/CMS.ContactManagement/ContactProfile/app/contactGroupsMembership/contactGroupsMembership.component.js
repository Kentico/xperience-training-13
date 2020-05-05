(function(angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/contactGroupsMembership.component', [
            'cms.contactmanagement/contactprofile/contactGroupsMembership.service'
        ])
        .component('cmsContactGroupsMembership', contactGroupsMembership());


    function contactGroupsMembership() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/contactGroupsMembership/contactGroupsMembership.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(contactGroupsMembershipService) {
        activate.apply(this);

        function activate() {
            contactGroupsMembershipService.getMembershipsForContact(this.contactId)
                .then(onSuccess.bind(this));
        };
                
        function onSuccess(memberships) {
            this.memberships = memberships;
        }
    };

}(angular));