cmsdefine(["require", "exports", 'CMS/Application'], function (cmsrequire, exports, application) {
    exports.Resource = [
        '$resource',
        function($resource) {
            var baseUrl = application.getData('applicationUrl') + 'cmsapi/Tile/',
                getTileId = function(tileModel) {
                    var id = "";
                    switch (tileModel.TileModelType) {
                        case "ApplicationTileModel":
                        case "ApplicationLiveTileModel":
                            id = tileModel.ApplicationGuid;
                            break;
                        case "SingleObjectTileModel":
                            id = tileModel.UIElementGuid + '+' + tileModel.ObjectName + '+' + tileModel.SiteName;
                            break;
                    }
                    return id;
                },
                transformApplicationsToUserSettings = function(tiles) {
                    var result = [];
                    tiles.forEach(function(tile) {
                        switch (tile.TileModelType) {
                            case "ApplicationTileModel":
                            case "ApplicationLiveTileModel":
                                result.push({ applicationGuid: tile.ApplicationGuid });
                                break;
                            case "SingleObjectTileModel":
                                result.push({
                                    applicationGuid: tile.ApplicationGuid,
                                    elementGuid: tile.UIElementGuid,
                                    objectName: tile.ObjectName,
                                    objectSiteName: tile.ObjectSiteName
                                });
                                break;
                        }
                    });
                    return result;
                },
                prepareTile = function(tile) {
                    tile.Id = getTileId(tile);
                    return tile;
                },
                saveAction = {
                    url: baseUrl + 'save',
                    method: 'POST',
                    transformRequest: function (applications) {
                        return JSON.stringify(transformApplicationsToUserSettings(applications));
                    }
                },
                loadAction = {
                    url: baseUrl + 'loadTile',
                    method: 'GET',
                    transformResponse: function(tile) {
                        var parsedTile = JSON.parse(tile);
                        return prepareTile(parsedTile);
                    },
                },
                loadAllAction = {
                    url: baseUrl + 'get',
                    method: 'GET',
                    isArray: true,
                    transformResponse: function(tiles) {
                        var parsedTiles = JSON.parse(tiles);
                        return parsedTiles.map(prepareTile);
                    }
                };
        
            return $resource(baseUrl, {}, {
                save: saveAction,
                load: loadAction,
                query: loadAllAction,
            });
        }
    ];
});
