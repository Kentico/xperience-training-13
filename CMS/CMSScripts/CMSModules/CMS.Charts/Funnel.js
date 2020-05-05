cmsdefine(['amcharts.funnel', 'amcharts.cmstheme'], function (chart) {

    var Module = function(config) {
        chart.makeChart(config.chartDiv, {
            type: "funnel",
            theme: "CMSTheme",
            dataProvider: config.data,
            titleField: "title",
            valueField: "value",
            labelPosition: "right",
            marginRight: 240,
            marginBottom: 35,
            neckWidth: "40%",
            neckHeight: "0",
            balloonText: "<span class=\"overview-funnel-balloon\">[[title]]:&nbsp;[[value]]<br>[[rateText]]</span>",
            labelText: "[[title]]: [[value]]\n[[rateText]]",
            addClassNames: "true",
            fontSize: 16,
            maxLabelWidth: 400,
            labelTickColor: "#e5e5e5",
            labelTickAlpha: 1,
            outlineThickness: 0,
            balloon: {
                adjustBorderColor: false,
                cornerRadius: 0,
                borderThickness: 0,
            },
    });
    };

    return Module;
});