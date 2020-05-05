-cmsdefine(['jQuery', 'CMS.Personas/Report/chart'], function ($, chart) {

    return function(config)
    {
        var chartContainerID = 'chartDiv',

            prepareChartContainer = function () {
                var chartContainer = $('<div class="personas-chart-container"></div>').attr('id', chartContainerID),
                    reportContainer = $('<div class="personas-report"></div>'),
                    header = $('<h3></h3>').text(config.resourceStrings['personas.personareport.header']),
                    noDataToDisplay = $('<span class="personas-report-nodata"></span>').text(config.resourceStrings['personas.personareport.nodata']);

                $('.UIContent').append(reportContainer.append(header).append(noDataToDisplay).append(chartContainer));
            };

        prepareChartContainer();
        chart(config, chartContainerID);
    };
});