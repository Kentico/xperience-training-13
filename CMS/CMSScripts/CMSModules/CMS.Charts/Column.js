cmsdefine(['amcharts.serial', 'amcharts.cmstheme'], function (chart) {

    var Module = function (config) {
        chart.makeChart(config.chartDiv, {
            type: "serial",
            theme: "CMSTheme",
            dataProvider: config.data,
            valueAxes: [{
                dashLength: 0
            }],
            gridAboveGraphs: true,
            startDuration: 1,
            graphs: [{
                balloonText: "[[category]]: <b>[[value]]</b>",
                fillAlphas: 0.8,
                lineAlpha: 0.2,
                type: "column",
                valueField: "value"
            }],
            chartCursor: {
                categoryBalloonEnabled: false,
                cursorAlpha: 0,
                zoomable: false
            },
            categoryField: "category",
            categoryAxis: {
                gridPosition: "start",
                tickPosition: "start",
                tickLength: 20
            },
        });
    };

    return Module;
});