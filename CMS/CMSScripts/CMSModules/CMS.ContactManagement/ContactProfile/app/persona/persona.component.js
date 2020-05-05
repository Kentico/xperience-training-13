(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/persona.component', ['cms.contactmanagement/contactprofile/persona.service'])
        .component('cmsPersona', persona());


    function persona() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/persona/persona.component.html',
            controller: controller
        };

        return component;
    }


    /*@ngInject*/
    function controller(personaService) {
        activate.apply(this);

        function activate() {
            personaService.getPersona(this.contactId)
                .then(onSuccess.bind(this));
        };

        function onSuccess(persona) {
            this.persona = persona;
        }
    }

}(angular));