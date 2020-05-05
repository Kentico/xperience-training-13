cmsdefine(['CMS/EventHub'], function (EventHub) {
    return ['$scope', '$location', 'csmContactImportService', '$timeout', function ($scope, $location, contactImportService, $timeout) {
        EventHub.publish('cms.angularViewLoaded');

        $scope.$on('firstNRowsLoaded', function (e, data) {
            $timeout(function() {
                if (data) {
                    contactImportService.setFileStream(data.fileStream);
                    contactImportService.setSourceFileName(data.sourceFileName);
                    contactImportService.setParsedLines(data.parsedLines);
                    $location.path('/mapping');
                }
            });
        });
    }];
});