(function(angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/categoryHeading.component', [])
        .component('cmsCategoryHeading', categoryHeading());

    function categoryHeading() {
        var component = {
            bindings: {
                heading: '@'
            },
            transclude: true,
            templateUrl: 'cms.contactmanagement/contactprofile/shared/categoryHeading.component.html'
        };

        return component;
    };

}(angular));