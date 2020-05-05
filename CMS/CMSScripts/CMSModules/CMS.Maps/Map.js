cmsdefine([
    'CMS/ObjectWithEvents',
    'CMS.Maps/mapSourceLoader',
    'CMS.Maps/mapLocalizationLoader',
    'CMS.Maps/mapProjections',
    'CMS.Maps/mapIconConverter',
    'CMS.Maps/mapLegendSetter',
    'CMS/interceptor',
    'jQuery',
    'amcharts.ammap',
    'amcharts.cmstheme'
    ], function (
        ObjectWithEvents,
        mapSourceLoader,
        mapLocalizationLoader,
        mapProjections,
        mapIconConverter,
        mapLegendSetter,
        interceptor,
        $
    ) {

    function Map(config) {
        ObjectWithEvents.call(this);

        if (!config) {
            throw 'Config object is required';
        }

        if(!config.mapDataLoader)
        {
            throw 'Map data loader has to be specified';
        }

        this.mapDataLoader = config.mapDataLoader;
        this.mapSourceLoader = mapSourceLoader;
        this.mapProjections = mapProjections;
        this.mapIconConverter = mapIconConverter;
        this.mapLocalizationLoader = mapLocalizationLoader;
        this.mapLegendSetter = mapLegendSetter;
        this.language = config.language || 'en';
        this.mapSource = config.mapSource || 'world';
        this.clickableAreas = config.clickableAreas || [];
        this.backToWorldMapLabel = config.backToWorldMapLabel;
        this.amMapConfig = this._getAmMapConfigObject();
        this.areaActions = [];
        this.interceptor = interceptor;

        this.initPromise = $.Deferred();
        this.amMapInitPromise = $.Deferred();

        this.promisesToLoadMap = [
            this.mapLocalizationLoader.loadLocalizationAsync(this.language),
            this.mapSourceLoader.loadMapSourceAsync(this.mapSource),
            this.initPromise
        ];

        this.promisesToLoadMapData = [
            this.mapDataLoader.loadMapDataAsync(this.mapSource),
            this.amMapInitPromise
        ];

        $.when.apply($, this.promisesToLoadMap).done(this._mapSourceLoaded.bind(this));
        $.when.apply($, this.promisesToLoadMapData).done(this._mapDataLoaded.bind(this));

        this.createEvent('clickMapObject');
    }

    Map.prototype = Object.create(ObjectWithEvents.prototype);

    Map.prototype.constructor = Map;


    Map.prototype.init = function (mapDivId) {
        this.containerDiv = document.getElementById(mapDivId);
        if (!this.containerDiv) {
            throw 'No element found with the provided ID: ' + mapDivId;
        }

        this.initPromise.resolve();
    };


    Map.prototype.restore = function () {
        if(this.map) {
            this.map.validateNow();
        }
    };


    Map.prototype._getAmMapConfigObject = function () {
        return {
            type: 'map',
            theme: 'CMSTheme',
            language: this.language,
            addClassNames: true,
            preventDragOut: true,
            projection: this.mapProjections[this.mapSource] || 'mercator',
            balloonLabelFunction: this._balloonLabelFunction,
            valueLegend: {
                height: 16,
                width: 240,
                right: 16,
                bottom: 16,
                enabled: false
            },
            balloon: {
                fillAlpha: 1,
                borderColor: '#403e3d',
                borderThickness: 1,
                shadowAlpha: 0,
                adjustBorderColor: false,
                fontSize: 13
            },
            zoomControl: {
                buttonSize: 40,
                maxZoomLevel: 8
            },
            areasSettings: {
                color: AmCharts.themes.CMSTheme.AreasSettings.unlistedAreasColor
            },
            listeners: [{
                event: 'init',
                method: this._mapInitialized.bind(this)
            }],
            dataProvider: this._initializeDataProvider(this.mapSource)
        }
    };


    Map.prototype._balloonLabelFunction = function (area) {
        var result = area.titleTr || area.title;
        if (area.value) {
            result += ': <strong>' + area.value.toLocaleString(this.language) + '</strong>';
        }
        return result;
    };


    Map.prototype._initializeDataProvider = function (mapSource) {
        var dataProvider = {
            map: this.mapSourceLoader.getMapSourceName(mapSource),
            getAreasFromMap: true
        };

        if (mapSource !== 'world') {
            dataProvider.images = [this._getBackToWorldButton()];
        }

        return dataProvider;
    };


    Map.prototype._ensureCMSIcons = function () {
        this.mapIconConverter.convertMapIconsToCMSOnes(this.map);

        this.interceptor.after(this.map.zoomControl, 'init', function() {
            this.mapIconConverter.convertMapIconsToCMSOnes(this.map);
        }.bind(this));
    };


    Map.prototype._ensureLegendLabels = function () {
        this.mapLegendSetter.setLabels(this.map, this.language);

        this.interceptor.after(this.map.valueLegend, 'init', function() {
            this.mapLegendSetter.setLabels(this.map, this.language);
        }.bind(this));
    };


    Map.prototype._makeAreasSelectable = function () {
        this.map.dataProvider.areas.forEach(function (area) {
            if (this.clickableAreas.indexOf(area.id) !== -1) {
                area.selectable = true;
            }
        }.bind(this));
    };


    Map.prototype._getBackToWorldButton = function () {
        return {
            id: 'world',
            label: this.backToWorldMapLabel,
            rollOverColor: AmCharts.themes.CMSTheme.AreasSettings.rollOverColor,
            labelRollOverColor: AmCharts.themes.CMSTheme.AreasSettings.rollOverColor,
            useTargetsZoomValues: true,
            left: 32,
            bottom: 32,
            labelFontSize: 13,
            selectable: true
        };
    };


    Map.prototype._mapObjectClicked = function (event) {
        this.raise({
            type: 'clickMapObject',
            object: event
        });
    };


    Map.prototype._mapSourceLoaded = function () {
        AmCharts.makeChart(this.containerDiv.id, this.amMapConfig);
    };


    Map.prototype._mapDataLoaded = function (mapData) {
        this.map.valueLegend.enabled = !_.isEmpty(mapData);
        if(!mapData)
        {
            return;
        }

        this.map.areasSettings.color = AmCharts.themes.CMSTheme.AreasSettings.color;
        this.map.dataProvider.areas.forEach(function (area) {
            area.value = mapData[area.id];
        }.bind(this));

        this.map.validateNow();
        this.map.validateData();
        this._ensureLegendLabels();
    };


    Map.prototype._mapInitialized = function(e) {
        this.map = e.chart;
        this._ensureCMSIcons();
        this._makeAreasSelectable();
        this.map.addListener('clickMapObject', this._mapObjectClicked.bind(this));

        this.amMapInitPromise.resolve();
    };
        
    return Map;
});