(function (angular, dataFromServer) {
    'use strict';

    angular.module('cms.contactmanagement/contactProfileSimple.component', [
        'cms.contactmanagement/contactprofile/card.component',
        'cms.contactmanagement/contactprofile/categoryHeading.component',
        'cms.contactmanagement/contactprofile/contactGroupsMembership.component',
        'cms.contactmanagement/contactprofile/newsletterSubscriptions.component',
        'cms.contactmanagement/contactprofile/notes.component',
        'CMS/Filters.Resolve'
    ])
    .component('cmsContactProfileSimple', contact());


    function contact() {
        var component = {
            bindings: {
                contact: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/contactProfileSimple.component.html',
            controller: controller
        };

        return component;
    };

    function controller() {
        this.newsletterModuleAvailable = dataFromServer.newsletterModuleAvailable;
        this.displayGroupMemberships = dataFromServer.displayGroupMemberships;
        this.displayEditButton = dataFromServer.displayEditButton;
    };

}(angular, dataFromServer));