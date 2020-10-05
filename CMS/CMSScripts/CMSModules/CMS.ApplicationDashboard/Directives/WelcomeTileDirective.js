cmsdefine(["require", "exports", 'CMS/EventHub'], function(cmsrequire, exports, eventHub) {
    exports.Directive = [
        'cms.resource.welcomeTile', function (__welcomeTile) {
            var tile = {
                restrict: 'A',
                scope: {},
                link: function ($scope) {
                    var welcomeTileDataModel;

                    $scope.model = {};
                    $scope.model.visible = false;

                    __welcomeTile.get(function (wtData) {
                        welcomeTileDataModel = wtData;

                        $scope.model.visible = wtData.Visible;
                        $scope.model.header = wtData.Header;
                        $scope.model.description = wtData.Description;
                        $scope.model.browseApplicationsText = wtData.BrowseApplicationsText;
                        $scope.model.openHelpText = wtData.OpenHelpText;
                    });

                    $scope.model.hide = function () {
                        welcomeTileDataModel.Visible = false;

                        __welcomeTile.update(welcomeTileDataModel, function () {
                            $scope.model.visible = welcomeTileDataModel.Visible;
                        });
                    };
                },
                templateUrl: 'welcomeTileTemplate.html'
            };

            return tile;
        }
    ];
});
