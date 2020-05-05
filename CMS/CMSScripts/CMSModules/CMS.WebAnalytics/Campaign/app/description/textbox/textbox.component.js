(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/description/textbox.component', [])
        .component('cmsCampaignTextbox', textbox());

    function textbox() {
        return {
            bindings: {
                id: '@',
                label: '@',
                pattern: '@',
                required: '@',
                maxlength: '@',
                value: '=',
                patternError: '@',
                enabled: '<',
                formElement: '<',
                serverValidationMessage: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/description/textbox/textbox.component.html',
            replace: true
        };
    }
}(angular));