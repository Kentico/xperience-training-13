(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/messages.service', [
        'CMS/Filters.Resolve'
    ])
        .service('messagesService', messagesService);

    /*@ngInject*/
    function messagesService(resolveFilter) {
        var that = this,
            listeners = {};

        that.addListener = function (id, onError, onSuccess) {
            listeners[id] = {
                onError: onError,
                onSuccess: onSuccess
            };
        };

        that.sendError = function (message, resolve, listenerId) {
            message = localize(message, resolve);
            send(message, listenerId, 'onError');
        };

        that.sendSuccess = function (message, resolve, listenerId) {
            message = localize(message, resolve);
            send(message, listenerId, 'onSuccess');
        };

        that.sendFormError = function (form, message, resolve, listenerId) {
            message = localize(message, resolve);
            send(message, listenerId, 'onError', form);
        };

        that.clearFormError = function (form, listenerId) {
            send(null, listenerId, 'onError', form);
        };

        function send(message, listenerId, method, context) {

            if (listenerId) {
                listeners[listenerId][method](message);
            }
            else {
                angular.forEach(listeners, function (listener) {
                    listener[method](message, context);
                });
            }
        }

        function localize(message, resolve) {
            return resolve ? resolveFilter(message) : message;
        }
    }
}(angular));