cmsdefine(['jQuery', 'CMS/Application', 'CMS.Maps/MapDataLoader', 'CMS.Maps/worldWithCountriesMap'], function ($, application, MapDataLoader, worldWithCountriesMap) {

    return function (config) {
        worldWithCountriesMap($.extend(config, {
            mapDataLoader: new MapDataLoader('ContactDemographics', 'GetGroupedByCountry', 'GetGroupedByUSAStates', config.parameters),
            clickableAreas: ['US']
        }));
    }
});