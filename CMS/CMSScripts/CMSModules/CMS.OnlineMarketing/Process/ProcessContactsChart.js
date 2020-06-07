cmsdefine(['amcharts', 'jQuery', 'CMS/stringShortener', 'CMS/Application', 'CMS/localizationHelper', 'amcharts.pie', 'amcharts.cmstheme'], function (amchart, $, stringShortener, application, localizationHelper) {

    function PieChart(config) {
        if (!config) {
            throw 'Config object is required';
        }

        if (!config.chartDiv) {
            throw 'Chart div has to be specified';
        }

        if (!config.data.length) {
            return;
        }

        this.chartEnabled = true;
        this.language = application.getData('language') || 'en';
        this.chartDiv = document.getElementById(config.chartDiv);
        this.dataProvider = config.data;
    }


    PieChart.prototype.init = function () {
        if (this.chartEnabled) {
            amchart.makeChart(this.chartDiv.id, this._getChartConfigurationObject());
        }
    };


    PieChart.prototype._getChartConfigurationObject = function () {
        return {
            type: 'pie',
            theme: 'CMSTheme',
            valueField: 'value',
            titleField: 'title',
            pullOutRadius: 20,
            pullOutDuration: 0.2,
            startDuration: 0,
            labelsEnabled: false,
            adjustPrecision: true,
            balloonFunction: this._balloonFunction.bind(this),
            dataProvider: this.dataProvider,
            legend: {
                maxColumns: 1,
                align: "center",
                labelText: '[[title]]:',
                valueFunction: this._legendValueFunction.bind(this)
            }
        }
    };


    PieChart.prototype._legendValueFunction = function (slice) {
        // Need to use no-break space at the end of the value, otherwise would be the percent sign partially hidden
        return localizationHelper.getLocalizedPercents(slice.percents, this.language) + ' ';
    };


    PieChart.prototype._balloonFunction = function (slice) {
        return slice.title + ': <strong>' + localizationHelper.getLocalizedPercents(slice.percents, this.language) + '</strong> (' + slice.value.toLocaleString(this.language) + ')';
    };


    return function (config) {
        var pieChart = new PieChart({
            chartDiv: config.chartElement,
            data: config.data
        });

        pieChart.init();
    }
});