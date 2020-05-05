cmsdefine(['jQuery', 'Underscore', 'amcharts', 'CMS.Personas/Report/dataLoader', 'CMS.Personas/Report/configuration', 'CMS.Charts/DateConverter', 'CMS.Charts/NumberFormatter', 'amcharts.serial', 'amcharts.cmstheme'], function ($, _, amchart, dataLoader, configuration, dateConverter, numberFormatter) {

    return function(config, chartContainer) {
        var getChartBaloon = function (persona) {
                var result = '';
                if (persona.personaImage) {
                    result += '<div class="persona-balloon-image">' + persona.personaImage + '</div>';
                }

                return result + '<div class="persona-balloon-text">' + persona.personaName + ':&nbsp;' + '<strong>[[value]]</strong></div>';
            },


            getChartGraphs = function (personaConfiguration) {
                var result = [];

                Object.keys(personaConfiguration)
                    .forEach(function (personaId) {
                        var persona = personaConfiguration[personaId];

                        result.push({
                            title: persona.personaName,
                            valueField: personaId,
                            fillAlphas: 0,
                            id: personaId,
                            valueAxis: 'y',
                            balloonText: getChartBaloon(persona),
                            lineThickness: 2
                        });
                    });

                return result;
            },


            hideWithoutPersonaGraph = function (chart) {
                var graphWithoutPersona = _.find(chart.graphs, function(graph) {
                    return graph.id === 'null';
                });

                chart.hideGraph(graphWithoutPersona);
            },


            categoryAxisLabelFunction = function(valueText, date, categoryAxis) {
                return dateConverter.toLocaleDateString(date, categoryAxis.currentDateFormat);
            },


            getLegendValue = function (item) {
                if(item.values && item.values.value) {
                    return item.values.value.toLocaleString();
                }

                if(item.data && item.data.length)
                {
                    var itemsWithNumber = _.filter(item.data, function(i) {return _.isNumber(i.dataContext[item.id])});

                    if(!itemsWithNumber.length)
                    {
                        return 0;
                    }

                    var itemValue = itemsWithNumber[itemsWithNumber.length-1].dataContext[item.id];

                    if(itemValue)
                    {
                        return itemValue.toLocaleString();
                    }

                    return 0;
                }

                return 0;
            },


            ensureLabelFunctionForChartScrollbar = function (chart) {
                chart.chartScrollbar.init();
                chart.chartScrollbar.categoryAxis.labelFunction = function(valueText, date, categoryAxis) {
                    return dateConverter.toLocaleDateString(date, categoryAxis.currentDateFormat);
                };
            },


            prepareChart = function (chartContainer) {
                var separators = numberFormatter.getNumberSeparators(),
                    dateFormats = [
                        {'period': 'DD', 'format': 'MMM D'}, {'period': 'WW', 'format': 'MMM D'},
                        {'period': 'MM', 'format': 'MMM'}, {'period': 'YYYY', 'format': 'YYYY'}
                    ],
                    chart = amchart.makeChart(chartContainer,
                    {
                        decimalSeparator: separators.decimal,
                        thousandsSeparator: separators.thousand,
                        legend: {
                            equalWidths: false,
                            position: 'top',
                            valueAlign: 'left',
                            valueWidth: 100,
                            labelText: '[[title]]:',
                            valueFunction: getLegendValue
                        },
                        balloon: {
                            textAlign: 'left',
                            maxWidth: 512
                        },
                        marginRight: 32,
                        autoMarginOffset: 32,
                        fontSize: 13,
                        addClassNames: true,
                        type: 'serial',
                        theme: 'CMSTheme',
                        categoryField: 'date',
                        categoryAxis: { 
                            position:'bottom',
                            parseDates: true,
                            minPeriod: 'DD',
                            labelFunction: categoryAxisLabelFunction,
                            dateFormats: dateFormats
                        },
                        valueAxes: [
                            {
                                stackType: 'none',
                                id: 'y'
                            }
                        ],
                        graphs: getChartGraphs(configuration.getPersonaConfiguration(config)),
                        chartCursor: {
                            fullWidth: true,
                            categoryBalloonFunction: function(date) {
                                return dateConverter.toLocaleDateString(date, _.find(dateFormats, function(item) { return item.period === 'DD' }).format);
                            }
                        },
                        chartScrollbar: {
                            offset: 32
                        },
                        mouseWheelZoomEnabled: true,
                        zoomOutText: config.resourceStrings['personas.personareport.showall']
                    });

                ensureLabelFunctionForChartScrollbar(chart);

                $(chart.legendDiv).css('opacity', 0);
                return chart;
            },


            chart = prepareChart(chartContainer),


            displayData = function(data) {
                $(chart.legendDiv).css('opacity', 1);
                chart.dataProvider = data;
                chart.validateData();
            },


            displayNoDataMessage = function() {
                $(".personas-report-nodata").show();
            },


            loadDataDone = function (data) {
                if(!data.length) {
                    displayNoDataMessage();
                }
                else {
                    displayData(data);
                }
            };

        hideWithoutPersonaGraph(chart);
        dataLoader.loadData().then(loadDataDone);
    };
});