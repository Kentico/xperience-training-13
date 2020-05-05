(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/conversions/conversion.component', [
        'cms.webanalytics/campaign/reportSetup/conversions/editConversion.component',
        'CMS/Filters.Resolve'
    ])
    .component('cmsCampaignConversion', campaignConversion());

    function campaignConversion() {
        return {
            bindings: {
                conversion: '=',
                isFunnel: '<',
                removable: '<',
                editable: '<',
                removeConversion: '&'
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/conversions/conversion/conversion.component.html',
            replace: true,
            controller: controller
        };
    }

    function controller() {
        var ctrl = this;

        ctrl.updateConversion = function (conversion) {
            angular.copy(conversion, ctrl.conversion);
        }
    }
}(angular));