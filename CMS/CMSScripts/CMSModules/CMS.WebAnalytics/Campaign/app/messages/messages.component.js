(function (angular) {
    'use strict';
    
    angular.module('cms.webanalytics/campaign/messages.component', [
        'cms.webanalytics/campaign/messages.service'
    ])
        .component('cmsCampaignMessages', messages());
    
    function messages() {
        return {
            bindings: {
                id: '<'
            },
            templateUrl: 'cms.webanalytics/campaign/messages/messages.component.html',
            replace: true,
            controller: controller
        }
    }

    /*@ngInject*/
    function controller($timeout, messagesService) {
        var ctrl = this;

        ctrl.temporaryMessage = "";
        ctrl.permanentErrors = {};

        messagesService.addListener(ctrl.id, showError, showSuccess);

        ctrl.getPermanentError = function () {
            var firstErrorKey = Object.keys(ctrl.permanentErrors)[0];
            return ctrl.permanentErrors[firstErrorKey];
        };

        function showError (message, context) {
            if (!message) {
                delete ctrl.permanentErrors[context];
            }
            else if (context) {
                ctrl.permanentErrors[context] = message;
            }
            else {
                showTemporary(message, "alert-error");
            }
        }
        
        function showSuccess (message) {
            showTemporary(message, "alert-info");
        }

        function showTemporary (message, messageClass) {
            if (ctrl.getPermanentError()) {
                return;
            }

            ctrl.temporaryMessage = message;
            ctrl.temporaryMessageClass = messageClass;

            cancelMessageTimeout();
            ctrl.messageTimeout = $timeout(function () {
                ctrl.temporaryMessage = "";
            }, 5000);
        }

        function cancelMessageTimeout() {
            if (ctrl.messageTimeout) {
                $timeout.cancel(ctrl.messageTimeout);
            }
        }
    }
}(angular));