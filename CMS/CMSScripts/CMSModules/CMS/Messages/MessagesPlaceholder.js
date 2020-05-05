cmsdefine(['CMS/EventHub', 'CMS/Application', 'CMS/Messages/Message', 'Underscore'], function (eventHub, app, Message, _) {

    var Controller = function ($scope, messageHub) {
        var that = this;
        this.$scope = $scope;
        this.$scope.model = this.$scope.model || { messages: [] };

        messageHub.subscribeToError(function (text, description) {
            if (!that.messageExists(text, description, 'error')) {
                that.$scope.model.messages.push(new Message(text, description, 'error', 'icon-times-circle'));
            }
        });

        messageHub.subscribeToInfo(function (text, description) {
            if (!that.messageExists(text, description, 'info')) {
                that.$scope.model.messages.push(new Message(text, description, 'info', 'icon-i-circle'));
            }
        });

        messageHub.subscribeToSuccess(function (text, description) {
            if (!that.messageExists(text, description, 'success')) {
                that.$scope.model.messages.push(new Message(text, description, 'success', 'icon-check-circle'));
            }
        });

        messageHub.subscriberToClear(function() {
            that.$scope.model.messages = [];
        });

        eventHub.subscribe('cms.angularViewLoaded', function () {
            that.$scope.model.messages = [];
        });
    };

    Controller.prototype.messageExists = function (text, description, type) {
        return _.find(this.$scope.model.messages, function (message) {
            return message.text === text && message.description === description && message.type === type;
        });
    };

    var directive = function () {
        return {
            scope: {},
            templateUrl: app.getData('applicationUrl') + 'CMSScripts/CMSModules/CMS/Templates/MessagesPlaceholderTemplate.html',
            controller: ['$scope', 'messageHub', Controller]
        };
    };

    return [directive];
})