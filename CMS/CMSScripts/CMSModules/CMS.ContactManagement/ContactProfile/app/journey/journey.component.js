(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactprofile/journey.component', [
        'cms.contactmanagement/contactprofile/journey.service'
    ])
    .component('cmsContactJourney', journey());


    function journey() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/journey/journey.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(journeyService) {
        function activate() {
            journeyService.getJourneyForContact(this.contactId).then(onSuccess.bind(this));
        };

        function onSuccess(journey) {
            this.journey = journey;
        };

        activate.apply(this);
    };
}(angular));