(function (angular, dataFromServer) {
    'use strict';
    
    angular.module('cms.contactmanagement/contactprofile/card/address.component', [])
        .component('cmsCardAddress', cardAddress());

    function cardAddress() {
        var component = {
            bindings: {
                contactCard: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/card/address/address.component.html',
            controller: controller
        }
        return component;
    }

    /*@ngInject*/
    function controller() {
        this.isMale = this.contactCard.ContactGender === 'Male';
        this.isFemale = this.contactCard.ContactGender === 'Female';
        this.showTags = this.contactCard.IsCustomer || this.contactCard.IsUser;
        this.showMarketingEmailStatus = dataFromServer.newsletterModuleAvailable;
    }
}(angular, dataFromServer));