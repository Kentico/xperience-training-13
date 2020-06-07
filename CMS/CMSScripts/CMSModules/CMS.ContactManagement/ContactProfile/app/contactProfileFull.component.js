(function (angular, dataFromServer) {
    'use strict';

    angular.module('cms.contactmanagement/contactProfileFull.component', [
        'cms.contactmanagement/contactprofile/card.component',
        'cms.contactmanagement/contactprofile/categoryHeading.component',
        'cms.contactmanagement/contactprofile/contactGroupsMembership.component',
        'cms.contactmanagement/contactprofile/newsletterSubscriptions.component',
        'cms.contactmanagement/contactprofile/persona.component',
        'cms.contactmanagement/contactprofile/scorings.component',
        'cms.contactmanagement/contactprofile/submittedForms.component',
        'cms.contactmanagement/contactprofile/notes.component',
        'cms.contactmanagement/contactprofile/detail.component',
        'cms.contactmanagement/contactprofile/journey.component',
        'cms.contactmanagement/contactprofile/link.component',
        'CMS/Filters.Resolve'
    ])
    .component('cmsContactProfileFull', contact());


    function contact() {
        var component = {
            bindings: {
                contact: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/contactProfileFull.component.html',
            controller: controller
        };

        return component;
    };

    function controller() {
        this.personaModuleAvailable = dataFromServer.personaModuleAvailable;
        this.formModuleAvailable = dataFromServer.formModuleAvailable;
        this.newsletterModuleAvailable = dataFromServer.newsletterModuleAvailable;
        this.activitiesExist = dataFromServer.activitiesExist;
        this.displayGroupMemberships = dataFromServer.displayGroupMemberships;
        this.displayNotes = dataFromServer.displayNotes;
        this.displayContactInformations = dataFromServer.displayContactInformations;
        this.displayEditButton = dataFromServer.displayEditButton;
        this.contactUrl = dataFromServer.contactUrl;
    };

}(angular, dataFromServer));