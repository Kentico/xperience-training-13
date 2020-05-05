(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/dialog/selectParameter.directive', [])
        .directive('cmsSelectParameter', directive);

    /*@ngInject*/
    function directive($compile) {
        return {
            restrict: 'E',
            scope: {
                configuration: '=',
                selectedConversion: '='
            },
            templateUrl: 'cms.webanalytics/campaign/reportSetup/dialog/selectParameter.directive.html',
            link: function(scope, element) {
                scope.$watch('configuration', function (newValue, oldValue) {
                    if(!angular.equals(newValue, oldValue)){
                        $compile(element.contents())(scope); 
                    }
                });
            }
        };
    }

}(angular));
