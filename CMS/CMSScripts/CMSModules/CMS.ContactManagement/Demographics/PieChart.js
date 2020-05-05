cmsdefine(['amcharts', 'jQuery', 'CMS/stringShortener', 'CMS/Application', 'CMS/localizationHelper', 'amcharts.pie', 'amcharts.cmstheme'], function (amchart, $, stringShortener, application, localizationHelper) {

    function PieChart(config) {
        if (!config) {
            throw 'Config object is required';
        }

        if (!config.chartDataLoader) {
            throw 'Chart dataloader has to be specified';
        }

        if (!config.chartDiv) {
            throw 'Chart div has to be specified';
        }

        this.noDataLabel = config.noDataLabel;
        this.language = application.getData('language') || 'en';
        this.chartDataLoader = config.chartDataLoader;
        this.chartDiv = document.getElementById(config.chartDiv);
        this.chartInitPromise = $.Deferred();

        this.chartInitPromise.then(this._initializeDataLoading.bind(this));
    }

    PieChart.prototype.init = function () {
        amchart.makeChart(this.chartDiv.id, this._getChartConfigurationObject());
    };


    PieChart.prototype._initializeDataLoading = function () {
        this.chartDataLoader.loadDataAsync().then(this._dataLoaded.bind(this));
    };


    PieChart.prototype._getChartConfigurationObject = function () {
        return {
            type: 'pie',
            theme: 'CMSTheme',
            valueField: 'value',
            titleField: 'title',
            innerRadius: '30%',
            maxLabelWidth: 200,
            radius: 128,
            balloonFunction: this._balloonFunction.bind(this),
            adjustPrecision: true,
            labelFunction: this._labelFunction.bind(this),
            addClassNames: true,
            pullOutRadius: 0,
            legend: {
                position: 'right',
                valueWidth: 60,
                labelWidth: 256,
                labelText: '[[title]]:',
                valueFunction: this._legendValueFunction.bind(this)
            },
            listeners: [{
                event: 'init',
                method: this._chartInitialized.bind(this)
            }]
        }
    };


    PieChart.prototype._legendValueFunction = function (slice) {
        // Need to use no-break space at the end of the value, otherwise would be the percent sign partially hidden
        return localizationHelper.getLocalizedPercents(slice.percents, this.language) + ' ';
    };


    PieChart.prototype._balloonFunction = function (slice) {
        return slice.title + ': <strong>' + localizationHelper.getLocalizedPercents(slice.percents, this.language) + '</strong> (' + slice.value.toLocaleString(this.language) + ')';
    };


    PieChart.prototype._labelFunction = function (slice) {
        return stringShortener.shortenString(slice.title, 160, this.chartDiv) + ': ' + localizationHelper.getLocalizedPercents(slice.percents, this.language);
    };


    PieChart.prototype._chartInitialized = function (e) {
        this.chart = e.chart;
        this.chartInitPromise.resolve();
    };


    PieChart.prototype._displayNoDataLabel = function () {
        this.chart.allLabels = [{
            text: this.noDataLabel,
            align: 'center',
            y: 220
        }];
        this.chart.validateNow();
    };


    PieChart.prototype._dataLoaded = function (data) {
        if (!data.length) {
            this._displayNoDataLabel();
            this.chart.legend.enabled = false;
            this.chart.validateNow();
            return;
        }

        this.chart.dataProvider = data;
        this.chart.validateData();
        this.chart.validateNow();
    };


    return PieChart;
});
