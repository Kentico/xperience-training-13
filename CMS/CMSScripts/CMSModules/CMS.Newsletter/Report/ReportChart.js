/**
 * A base chart for newsletter report charts.
 */
cmsdefine(['jQuery', 'amcharts', 'CMS.Charts/DateConverter', 'CMS.Charts/NumberFormatter', 'amcharts.serial', 'amcharts.cmstheme'], function ($, amcharts, dateConverter, numberFormatter) {
    
    var INITIAL_ZOOM_ITEM_COUNT = 20;

    var ReportChart = function (chartDiv, config) {
        this.chartInitPromise = $.Deferred();
        var options = $.extend(true, this._getChartConfiguration(), config);
        
        this.chart = amcharts.makeChart(chartDiv, options);
        this.chart.addListener("dataUpdated", this.zoomChart.bind(this));
    };

    ReportChart.prototype.registerInitializededCallback = function (callback) {
        var that = this;
        this.chartInitPromise.then(function () {
            callback(that.chart);
        });
    }

    ReportChart.prototype.zoomChart = function () {
        var itemCount = this.chart.dataProvider.length;
        var fromIndex = itemCount <= INITIAL_ZOOM_ITEM_COUNT ? 0 : itemCount - INITIAL_ZOOM_ITEM_COUNT;
        var toIndex = itemCount - 1;

        this.chart.zoomToIndexes(fromIndex, toIndex);
    };

    ReportChart.prototype._getChartConfiguration = function () {
        var dateFormats = [
            { "period": "mm", "format": "hh:mm" }, { "period": "hh", "format": "hh:mm" },
            { 'period': 'DD', 'format': 'MMM D' }, { 'period': 'WW', 'format': 'MMM D' },
            { 'period': 'MM', 'format': 'MMM' }, { 'period': 'YYYY', 'format': 'YYYY' }
        ];
        var separators = numberFormatter.getNumberSeparators();

        return {
            type: "serial",
            theme: "CMSTheme",
            decimalSeparator: separators.decimal,
            thousandsSeparator: separators.thousand,
            legend: {
                align: "center",
                listeners: [{
                    event: "hideItem",
                    method: this._onGraphHide.bind(this)
                }, {
                    event: "showItem",
                    method: this._onGraphShow.bind(this)
                }]
            },
            listeners: [{
                event: 'init',
                method: this._chartInitialized.bind(this)
            }],
            valueAxes: [{
                axisAlpha: 0
            }],
            chartScrollbar: {},
            chartCursor: {
                cursorAlpha: 1,
                oneBalloonOnly: true,
                categoryBalloonFunction: function (date) {
                    return dateConverter.toLocaleDateString(date, _.find(dateFormats, function (item) { return item.period === 'DD' }).format);
                }
            },
            categoryAxis: {
                startOnAxis: true,
                parseDates: true,
                axisColor: "#DADADA",
                minPeriod: "mm",
                dateFormats: dateFormats,
                labelFunction: function (valueText, date, categoryAxis) {
                    if (categoryAxis.currentDateFormat === "hh:mm") {
                        // Display only localized hh:mm format. Do not display YYYY/MM
                        return dateConverter.toLocaleTimeString(date, categoryAxis.currentDateFormat);
                    }
                    else {
                        return dateConverter.toLocaleDateString(date, categoryAxis.currentDateFormat);
                    }
                }
            }
        };
    };

    ReportChart.prototype._chartInitialized = function () {
        this.chartInitPromise.resolve();
    };

    ReportChart.prototype._onGraphHide = function (event) {
        var hiddenGraph = event.dataItem;
        var graphs = event.chart.graphs;
        hiddenGraph.showBalloon = false;
        
        var visibleGraph = this._findFirstVisibleGraph(graphs);
        if (visibleGraph && (visibleGraph.index > hiddenGraph.index)) {
            visibleGraph.showBalloon = true;
        }
    };

    ReportChart.prototype._onGraphShow = function (event) {
        var shownGraph = event.dataItem;
        var graphs = event.chart.graphs;
        var graphsWithoutShown = graphs.slice();
        graphsWithoutShown.splice(shownGraph.index, 1);

        var visibleGraph = this._findFirstVisibleGraph(graphsWithoutShown);
        if (visibleGraph) {
            if (visibleGraph.index > shownGraph.index) {
                shownGraph.showBalloon = true;
                visibleGraph.showBalloon = false;
            }
        } else {
            shownGraph.showBalloon = true;
        }
    }

    ReportChart.prototype._findFirstVisibleGraph = function (graphs) {
        for (var i = 0; i < graphs.length; i++) {
            if (!graphs[i].hidden) {
                return graphs[i];
            }
        }

        return undefined;
    }

    return ReportChart;
});