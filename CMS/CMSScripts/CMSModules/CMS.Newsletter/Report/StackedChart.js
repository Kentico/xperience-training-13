cmsdefine(['amcharts', 'CMS.Newsletter/Report/ReportChart', 'CMS.Charts/NumberFormatter', 'amcharts.serial', 'amcharts.cmstheme'], function (amcharts, ReportChart, numberFormatter) {

    function makeChart(chartDiv, config) {
        var openRateGraphColor = config.colors[1];
        var clickRateGraphColor = config.colors[2];
        var unsubscribedRateGraphColor = config.colors[3];

        var balloonTextHandler = function (item) {
            var data = item.serialDataItem.dataContext;
            var name = data.Name;
            var openRate = numberFormatter.toLocaleString(data.OpenRate);
            var clickRate = numberFormatter.toLocaleString(data.ClickRate);
            var unsubscribedRate = numberFormatter.toLocaleString(data.UnsubscribedRate);

            return "<p><b>" + name + "</b></p>" +
                   "<p>" + config.resourceStrings.openRate + ": <b>" + openRate + " %</b><br>"
                         + config.resourceStrings.clickRate + ": <b>" + clickRate + " %</b><br>"
                         + config.resourceStrings.unsubscribedRate + ": <b>" + unsubscribedRate + " %</b><br>" +
                   "</p>";
        };

        var chart = new ReportChart(chartDiv, {
            categoryField: "Date",
            legend: {
                valueText: "[[value]] %",
            },
            valueAxes: [{
                axisAlpha: 0,
                gridAlpha: 0.07,
                unit: " %"
            }],
            graphs: [{
                id: "openRateGraph",
                balloonFunction: balloonTextHandler,
                bullet: "round",
                fillColors: openRateGraphColor,
                fillAlphas: 0.6,
                lineColor: openRateGraphColor,
                lineAlpha: 0.9,
                title: config.resourceStrings.openRate,
                valueField: "OpenRate"
            }, {
                id: "clickRateGraph",
                balloonFunction: balloonTextHandler,
                showBalloon: false,
                bullet: "round",
                fillColors: clickRateGraphColor,
                fillAlphas: 0.8,
                lineColor: clickRateGraphColor,
                lineAlpha: 0.9,
                title: config.resourceStrings.clickRate,
                valueField: "ClickRate"
            }, {
                id: "unsubscribedRateGraph",
                balloonFunction: balloonTextHandler,
                showBalloon: false,
                bullet: "round",
                fillColors: unsubscribedRateGraphColor,
                fillAlphas: 0.8,
                lineColor: unsubscribedRateGraphColor,
                lineAlpha: 0.9,
                title: config.resourceStrings.unsubscribedRate,
                valueField: "UnsubscribedRate"
            }],
            zoomOutText: config.resourceStrings.chartZoomButtonText
        });

        chart.registerInitializededCallback(config.chartInitializedCallback);
    }

    return {
        makeChart: makeChart
    };
});