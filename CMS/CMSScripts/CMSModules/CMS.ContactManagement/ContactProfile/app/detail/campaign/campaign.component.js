(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactprofile/detail/campaign/campaign.component', [])
        .component('cmsCampaign', campaign());

    function campaign() {
        var component = {
            bindings: {
                field: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/detail/campaign/campaign.component.html',
        };

        return component;
    };
}(angular));