cmsdefine(['amcharts.serial', 'amcharts.cmstheme'], function (chart) {

    var Module = function (config) {
        chart.makeChart(config.chartDiv, {
            type: "serial",
            theme: "CMSTheme",
            dataProvider: config.data,
            categoryField: "category",
            categoryAxis: {
                minorGridEnabled: true
            },
            valueAxes: [{
                id: "v1"
            }],
            graphs: [{
                title: "red line",
                id: "g1",
                valueAxis: "v1",
                valueField: "value",
                bullet: "round",
                balloonText: "[[category]]: <b>[[value]]</b>"
            }],
            chartCursor: {
                fullWidth: true
            },
            chartScrollbar: {
                graph: "g1"
            },
            mouseWheelZoomEnabled: true
        });
    };

    return Module;
});