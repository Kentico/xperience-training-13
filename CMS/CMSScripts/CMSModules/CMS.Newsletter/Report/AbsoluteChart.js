cmsdefine(['amcharts', 'CMS.Newsletter/Report/ReportChart', 'CMS.Charts/NumberFormatter', 'amcharts.serial', 'amcharts.cmstheme'], function (amcharts, ReportChart, numberFormatter) {

    function makeChart(chartDiv, config) {
        var balloonTextHandler = function (item) {
            var data = item.serialDataItem.dataContext;
            var name = data.Name;
            var sent = numberFormatter.toLocaleString(data.Sent);
            var opens = numberFormatter.toLocaleString(data.Opens);
            var clicks = numberFormatter.toLocaleString(data.Clicks);
            var unsubscribed = numberFormatter.toLocaleString(data.Unsubscribed);

            return "<p><b>" + name + "</b></p>" +
                   "<p>" + config.resourceStrings.sent + ": <b>" + sent + "</b><br>"
                         + config.resourceStrings.opens + ": <b>" + opens + "</b><br>"
                         + config.resourceStrings.clicks + ": <b>" + clicks + "</b><br>"
                         + config.resourceStrings.unsubscribed + ": <b>" + unsubscribed + "</b>" +
                   "</p>";
        };

        var chart = new ReportChart(chartDiv, {
            categoryField: "Date",
            legend: {
                useGraphSettings: true
            },
            graphs: [{
                balloonFunction: balloonTextHandler,
                bullet: "round",
                hideBulletsCount: 30,
                title: config.resourceStrings.sent,
                lineColor: config.colors[0],
                valueField: "Sent"
            }, {
                balloonFunction: balloonTextHandler,
                showBalloon: false,
                bullet: "round",
                hideBulletsCount: 30,
                title: config.resourceStrings.opens,
                lineColor: config.colors[1],
                valueField: "Opens"
            }, {
                balloonFunction: balloonTextHandler,
                showBalloon: false,
                bullet: "round",
                hideBulletsCount: 30,
                title: config.resourceStrings.clicks,
                lineColor: config.colors[2],
                valueField: "Clicks"
            }, {
                balloonFunction: balloonTextHandler,
                showBalloon: false,
                bullet: "round",
                hideBulletsCount: 30,
                title: config.resourceStrings.unsubscribed,
                lineColor: config.colors[3],
                valueField: "Unsubscribed"
            }],
            zoomOutText: config.resourceStrings.chartZoomButtonText
        });

        chart.registerInitializededCallback(config.chartInitializedCallback);
    }

    return {
        makeChart: makeChart
    };
});