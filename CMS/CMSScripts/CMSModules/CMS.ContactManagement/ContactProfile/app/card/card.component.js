(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactprofile/card.component', [
        'cms.contactmanagement/contactprofile/card/marketingEmailStatus.component',
        'cms.contactmanagement/contactprofile/card/address.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCard', card());


    function card() {
        var component = {
            bindings: {
                contact: '<',
                displayEditButton: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/card/card.component.html',
            controller: controller
        };

        return component;
    }


    /*@ngInject*/
    function controller() {
        this.contactCard = this.contact;

        this.showContactAddress = false;
        this.showContactAddress = this.contact.ContactEmail || this.contact.ContactAddress ||
            (this.contact.ContactName && (this.contact.ContactName.indexOf('Anonymous') !== 0));
    };

}(angular));