(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/scorings.component', ['cms.contactmanagement/contactprofile/scorings.service'])
        .component('cmsScorings', scorings());


    function scorings() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/scorings/scorings.component.html',
            controller: controller
        };

        return component;
    }


    /*@ngInject*/
    function controller(scoringService) {
        activate.apply(this);

        function activate() {
            scoringService.getScorings(this.contactId)
                .then(onSuccess.bind(this));
        };

        function onSuccess(scorings) {
            this.scorings = scorings;
        }
    }
}(angular));