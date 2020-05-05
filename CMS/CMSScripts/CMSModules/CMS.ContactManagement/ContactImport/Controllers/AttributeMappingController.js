cmsdefine(['CMS/EventHub'], function (EventHub) {
    return ['$scope', 'messageHub', 'resolveFilter', '$location', 'csmContactImportService', 'serverData', '$timeout', function ($scope, messageHub, resolveFilter, $location, contactImportService, serverData, $timeout) {
        var fileStream = contactImportService.getFileStream(),
            parsedLines = contactImportService.getParsedLines(),
            sourceFileName = contactImportService.getSourceFileName();

        EventHub.publish('cms.angularViewLoaded');

        if (!fileStream || !parsedLines || !sourceFileName) {
            $location.path('/');
            return;
        }

        $scope.fileStream = fileStream;
        $scope.parsedLines = parsedLines;
        $scope.sourceFileName = sourceFileName;
        $scope.contactFields = serverData.contactFields;
        $scope.contactGroups = serverData.contactGroups;
        $scope.contactGroup = {};
        $scope.segmentationType = 'new';

        $scope.$on("mappingFinished", function (e, data) {
            $timeout(function () {
                if (data) {
                    contactImportService.setContactFieldsMapping(data.mapping);
                    contactImportService.setContactGroup(data.contactGroup);
                    $location.path('/import');
                }

            });
        });
    }];
});