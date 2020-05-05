(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/autosave.directive', [
        'cms.webanalytics/campaign/autosave.service'
    ])
        .directive('cmsAutosave', autosave);

    /*@ngInject*/
    function autosave(autosaveService) {
        return {
            restrict: 'A',
            require: '^form',
            scope: {
                callback: '&cmsAutosave'
            },
            link: function ($scope, $element, $attrs, $formController) {
                autosaveService.autosave($scope, $formController, $scope.callback);
            }
        }
    }
}(angular));