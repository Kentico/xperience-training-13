cmsdefine([
    'CMS/Messages/MessagesPlaceholder',
    'CMS/Messages/MessageHub'
], function (
    message,
    messageHub
    ) {

    return function(angular) {
        var moduleName = 'cms.messages';
        angular.module(moduleName, [])
            .directive('cmsMessagesPlaceholder', message)
            .service('messageHub', messageHub);

        return moduleName;
    };
})