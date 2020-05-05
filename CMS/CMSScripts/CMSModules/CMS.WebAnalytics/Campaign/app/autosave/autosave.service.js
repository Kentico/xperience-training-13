(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/autosave.service', [
        'cms.webanalytics/campaign/messages.service'
    ])
        .service('autosaveService', autosaveService);

    /*@ngInject*/
    function autosaveService($timeout, messagesService) {
        var that = this,
            forms = {};

        that.autosave = function ($scope, $form, saveMethod) {
            // Add form to the scope so it can be watched
            var formName = $form.$name;
            if (formName === '') {
                return;
            }

            $scope[formName] = $form;

            forms[formName] = {
                $form: $form,
                saveMethod: saveMethod
            };

            // Watch for changes
            $scope.$watch(formName + '.$dirty', function () {
                if ($form.$dirty) {
                    processFormChange(forms[formName]);
                }
            });
        };

        that.saveForm = function (formName) {
            var formData = forms[formName];

            // Cancel queried save
            cancelSave(formData);
            performSave(formData);

            // Set form untouched so next changes can be detected
            formData.$form.$setPristine();
        };

        function processFormChange (formData) {
            cancelSave(formData);
            
            formData.savePromise = $timeout(function () {
                performSave(formData);
            }, 1000);

            formData.$form.$setPristine();
        }

        function cancelSave (formData) {
            if (formData.savePromise) {
                $timeout.cancel(formData.savePromise);
            }
        }

        function performSave (formData) {
            var formName = formData.$form.$name;
            formData.savePromise = null;
            
            // Validity could be violated since the last check, make sure it still persists
            if (formData.$form.$valid) {
                messagesService.clearFormError(formName);
                formData.saveMethod();
            }
            else {
                messagesService.sendFormError(formName, 'campaign.autosave.validationFailed', true);
            }
        }
    }
}(angular));