cmsdefine(['jQuery', 'CMS/Application', 'CMS.Maps/Map'], function ($, application, Map) {

    return function (config) {
        if (!config) {
            throw 'Incorrect argument, config object expected';
        }

        if (!config.mapDiv) {
            throw 'Incorrect argument, "mapDiv" property of config object has to be defined';
        }

        if (!config.mapDataLoader) {
            throw 'Incorrect argument, "mapDataLoader" property of config object has to be defined';
        }


        var maps = {},
            mapDiv = config.mapDiv,
            commonConfig = {
                mapDataLoader: config.mapDataLoader,
                language: application.getData('language'),
                backToWorldMapLabel: config.resourceStrings['map.backtoworldmap']
            },
            worldMapConfig = $.extend(commonConfig, {
                clickableAreas: config.clickableAreas,
                mapSource: 'world'
            }),


            changeToMap = function (mapSource) {
                var map = maps[mapSource];

                if (!map) {
                    maps[mapSource] = initializeMap($.extend(commonConfig, {
                        mapSource: mapSource
                    }));
                }
                else {
                    map.restore();
                }
            },


            mapObjectClicked = function (event) {
                changeToMap(event.object.mapObject.id);
            },


            initializeMap = function (config) {
                var map = new Map(config);

                map.init(mapDiv);
                map.addListener('clickMapObject', mapObjectClicked);

                return map;
            };

        changeToMap('world');
    }
});