cmsdefine(['amcharts.serial', 'amcharts.cmstheme'], function (chart) {

    var Module = function (config) {
        var fixedColumnsLimit = 3,
            resizeChartsColumnsAndLegend = function(chart) {
                // Pixel value reserved for each category, it represents column width and padding (see less variable @column-width-with-padding).
                var categoryWidth = 250;
                var width = chart.dataProvider.length * categoryWidth;
                document.getElementById(config.chartDiv).style.width = "" + width + "px";
                document.getElementById(config.legendDiv).style.width = "" + width + "px";

                // Since the chart is already build for the initial container size we need to
                // revalidate it's size. we'll delay a little bit the call to invalidateSize()
                // so that the chart elements are created first.
                setTimeout(function () {
                    chart.invalidateSize();
                    document.getElementById(config.chartDiv).style.display = "block";
                }, 1);
            };

        chart.addInitHandler(function (chart) {
            if (config.data.length < fixedColumnsLimit) {
                resizeChartsColumnsAndLegend(chart);
            }
        }, ['serial']);

        var chartConfig = {
            type: "serial",
            theme: "CMSTheme",
            dataProvider: config.data,
            gridAboveGraphs: false,
            fontFamily: "Segoe UI",
            startDuration: 1,
            graphs: [{
                balloonText: "[[balloon]]",
                fillAlphas: 0.8,
                lineAlpha: 0.2,
                type: "column",
                valueField: "value",
                columnWidth: 0.9,
                labelText: "[[label]]",
                fontSize: 14
            }],
            balloon: {
                borderThickness: 0,
                fillAlpha: 1,
                pointerWidth: 8,
                textAlign: "left",
                pointerOrientation: "down",
                verticalPadding: 12,
                horizontalPadding: 16,
                fillColor: "#E5E0CB",
                fontSize: 14
            },
            chartCursor: {
                categoryBalloonEnabled: false,
                cursorAlpha: 0,
                zoomable: false
            },
            valueAxes: [{
                labelsEnabled: false,
                gridAlpha: 0,
                gridColor: "#FFFFFF",
                axisColor: "#FFFFFF",
                autoGridCount: false,
                minimum: 0,
                maximum: config.maxValue
            }],
            categoryField: "legend",
            categoryAxis: {
                gridPosition: "start",
                tickPosition: "start",
                gridAlpha: 0,
                gridColor: "#FFFFFF",
                axisColor: "#FFFFFF",
                autoWrap: true,
                fontSize: 14,
                boldLabels: true
            }
        };

        if (config.data.length < fixedColumnsLimit) {
            chartConfig.graphs[0].fixedColumnWidth = 200;
        }

        chart.makeChart(config.chartDiv, chartConfig);
    };

    return Module;
});