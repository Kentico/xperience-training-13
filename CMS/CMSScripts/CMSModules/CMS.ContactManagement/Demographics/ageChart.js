cmsdefine([
    'CMS.ContactManagement/Demographics/ChartDataLoaderExtender',
    'CMS.ContactManagement/Demographics/ColumnChart',
    'CMS.ContactManagement/Demographics/percentageDataConverter'], function (ChartDataLoaderExtender,
                                                                             ColumnChart,
                                                                             percentageDataConverter) {


    var changeUnknownToGray = function (item) {
            if (item.title === 'unknown') {
                item.color = '#bdbbbb';
            }
        },


        dataExtender = function (data) {
            data.forEach(function (item) {
                changeUnknownToGray(item);
            });

            percentageDataConverter.convertDataToPercents(data, 'value');
            return data;
        };


    return function (config) {
        var columnChart = new ColumnChart({
            chartDiv: config.chartDiv,
            chartDataLoader: new ChartDataLoaderExtender('ContactDemographics', 'GetGroupedByAge', config.parameters, 'Category', 'NumberOfContacts', dataExtender),
            noDataLabel: config.resourceStrings['om.contact.demographics.graphicalrepresentation.nodata']
        });

        columnChart.init();
    }
});
