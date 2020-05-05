cmsdefine(['CMS.ContactManagement/Demographics/ChartDataLoader', 'CMS.ContactManagement/Demographics/PieChart'], function (ChartDataLoader, PieChart) {

    return function (config) {
        var pieChart = new PieChart({
            chartDiv: config.chartDiv,
            chartDataLoader: new ChartDataLoader('ContactPersonaDemographics', 'GetGroupedByPersona', config.parameters, 'PersonaName', 'NumberOfContacts'),
            noDataLabel: config.resourceStrings['om.contact.demographics.graphicalrepresentation.nodata'],
        });

        pieChart.init();
    }
});
