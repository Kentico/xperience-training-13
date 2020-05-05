cmsdefine(['jQuery', 'Underscore', 'CMS/Application'], function ($, _, application) {

    function MapDataLoader(controller, worldAction, usaAction, parameters) {

        this.controller = controller;
        this.worldAction = worldAction;
        this.usaAction = usaAction;
        this.parameters = parameters;
        this.application = application;
    }


    MapDataLoader.prototype.loadMapDataAsync = function (mapId) {
        var action = mapId === 'world' ? this.worldAction : this.usaAction,
            prefix = mapId === 'world' ? '' : 'US-';

        return $.get(this._getApiUrl(action), this.parameters).then(function (data) {
            return this._prepareMapData(data, prefix);
        }.bind(this));
    };


    MapDataLoader.prototype._prepareMapData = function (data, prefix) {
        var result = {};

        data.forEach(function (item) {
            result[prefix + item.LocationKey] = item.NumberOfContacts;
        }.bind(this));

        return result;
    };


    MapDataLoader.prototype._getApiUrl = function (action) {
        return this.application.getData('applicationUrl') + 'cmsapi/' + this.controller + '/' + action;
    };


    return MapDataLoader;
});