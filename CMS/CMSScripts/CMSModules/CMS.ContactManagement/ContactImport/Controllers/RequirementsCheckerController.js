cmsdefine(['CMS/EventHub'], function (EventHub) {
    return ['$scope', 'messageHub', 'resolveFilter', '$location', function ($scope, messageHub, resolveFilter, $location) {
        if (!window.FileReader) {
            messageHub.publishError(resolveFilter('om.contact.importcsv.notsupportedbrowser'));
            return;
        }

        EventHub.publish('cms.angularViewLoaded');
        $location.path('/upload');
    }];
});