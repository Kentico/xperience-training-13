cmsdefine(['CMS.ContactManagement/Demographics/ChartDataLoader', 'CMS.ContactManagement/Demographics/PieChart'], function(ChartDataLoader, PieChart) {

    return function(config)
    {
        var pieChart = new PieChart({
            chartDiv: config.chartDiv,
            chartDataLoader: new ChartDataLoader('ContactDemographics', 'GetGroupedByGender', config.parameters, 'Gender', 'NumberOfContacts'),
            noDataLabel: config.resourceStrings['om.contact.demographics.graphicalrepresentation.nodata']
        });

        pieChart.init();
    }
});
