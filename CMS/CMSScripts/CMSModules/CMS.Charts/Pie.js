cmsdefine(['amcharts.pie', 'amcharts.cmstheme'], function (chart) {

    var Module = function (config) {
        chart.makeChart(config.chartDiv, {
            type: "pie",
            theme: "CMSTheme",
            dataProvider: config.data,
            valueField: "value",
            titleField: "title",
        });
    };

    return Module;
});