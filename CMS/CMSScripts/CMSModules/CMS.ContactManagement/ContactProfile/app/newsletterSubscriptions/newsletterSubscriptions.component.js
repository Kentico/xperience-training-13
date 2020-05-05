(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/newsletterSubscriptions.component', [
            'cms.contactmanagement/contactprofile/newsletterSubscriptions.service'
        ])
        .component('cmsNewsletterSubscriptions', newsletterSubscriptions());


    function newsletterSubscriptions() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/newsletterSubscriptions/newsletterSubscriptions.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(newsletterSubscriptionsService) {
        activate.apply(this);

        function activate() {
            newsletterSubscriptionsService.getSubscriptionsForContact(this.contactId)
                .then(onSuccess.bind(this));
        };

        function onSuccess(subscriptions) {
            this.subscriptions = subscriptions;
        }
    };

}(angular));