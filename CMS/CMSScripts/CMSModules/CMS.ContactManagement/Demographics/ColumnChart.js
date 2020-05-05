cmsdefine(['amcharts', 'jQuery', 'CMS/Application', 'CMS/localizationHelper', 'Underscore', 'amcharts.serial', 'amcharts.cmstheme'], function (amchart, $, application, localizationHelper, _) {

    function ColumnChart(config) {
        if (!config) {
            throw 'Config object is required';
        }

        if (!config.chartDataLoader) {
            throw 'Chart dataloader has to be specified';
        }

        if (!config.chartDiv) {
            throw 'Chart div has to be specified';
        }

        this.language = application.getData('language') || 'en';
        this.noDataLabel = config.noDataLabel;
        this.chartDataLoader = config.chartDataLoader;
        this.chartDiv = document.getElementById(config.chartDiv);
        this.chartInitPromise = $.Deferred();

        this.chartInitPromise.then(this._initializeDataLoading.bind(this));
    }


    ColumnChart.prototype.init = function () {
        amchart.makeChart(this.chartDiv.id, this._getChartConfigurationObject());
    };


    ColumnChart.prototype._initializeDataLoading = function () {
        this.chartDataLoader.loadDataAsync().then(this._dataLoaded.bind(this));
    };


    ColumnChart.prototype._getChartConfigurationObject = function () {
        return {
            type: 'serial',
            theme: 'CMSTheme',
            categoryField: 'title',
            addClassNames: true,
            rotate: true,
            listeners: [{
                event: 'init',
                method: this._chartInitialized.bind(this)
            }],
            graphs: [{
                balloonFunction: this._balloonFunction.bind(this),
                fillAlphas: 0.8,
                lineAlpha: 0,
                type: 'column',
                valueField: 'percentage',
                colorField: 'color'
            }]
        }
    };


    ColumnChart.prototype._labelFunction = function (value) {
        // Need to use no-break space at the end of the value, otherwise would be the percent sign partially hidden
        return localizationHelper.getLocalizedPercents(value, this.language) + '  ';
    };


    ColumnChart.prototype._balloonFunction = function (graphItem) {
        return graphItem.category + ': <strong>' + localizationHelper.getLocalizedPercents(graphItem.dataContext.percentage, this.language) + '</strong> (' + graphItem.dataContext.value.toLocaleString(this.language) + ')';
    };


    ColumnChart.prototype._chartInitialized = function (e) {
        this.chart = e.chart;

        this._setValueAxisLabelFunction();
        this.chartInitPromise.resolve();
    };


    ColumnChart.prototype._setValueAxisLabelFunction = function () {
        this.chart.graphs[0].valueAxis.labelFunction = this._labelFunction.bind(this);
    };


    ColumnChart.prototype._displayNoDataLabel = function () {
        this.chart.allLabels = [{
            text: this.noDataLabel,
            align: 'center',
            y: 220
        }];
        this.chart.validateNow();
    };


    ColumnChart.prototype._setAxisMaximum = function (data) {
        var max = _.max(data, function (a) {
            return a.percentage
        });
        this.chart.graphs[0].valueAxis.maximum = max.percentage;
    };


    ColumnChart.prototype._dataLoaded = function (data) {
        if (!data.length) {
            this._displayNoDataLabel();
            this.chart.validateNow();
            return;
        }

        this.chart.dataProvider = data;

        this._setAxisMaximum(data);
        this.chart.validateData();
        this.chart.validateNow();
    };

    return ColumnChart;
});
