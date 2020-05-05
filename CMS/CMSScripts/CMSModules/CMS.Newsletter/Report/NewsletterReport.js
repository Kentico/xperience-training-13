cmsdefine(['jQuery', 'CMS/Tabs', 'CMS.Newsletter/Report/AbsoluteChart', 'CMS.Newsletter/Report/StackedChart', 'CMS/Application'], function ($, tabs, AbsoluteChart, StackedChart, application) {

    var NewsletterReport = function (config) {
        tabs.init({
            el: '.tabs',
            tabNavigationLinks: '.tab-link',
            tabContentContainers: '.tab-content'
        });


        this.loaderPromise = $.get(application.getData('applicationUrl') + 'cmsapi/NewsletterReport/GetEmailsData', config.parameters);

        config.chartInitializedCallback = this._onChartInitialized.bind(this);
        config.colors = ["#0f7abc", "#f69c04", "#518f02", "#c0282a"];

        AbsoluteChart.makeChart('absoluteChart', config);
        StackedChart.makeChart('stackedChart', config);
    };

    NewsletterReport.prototype._onChartInitialized = function (chart) {
        var that = this;

        this.loaderPromise.then(function (data) {
            that._dataLoaded(chart, data);
        });
    };

    NewsletterReport.prototype._dataLoaded = function (chart, data) {
        if (data.length) {
            chart.dataProvider = data;
            chart.validateData();
            $('.tabs').removeClass('hidden');
        } else {
            $(".newsletter-report-nodata").removeClass('hidden');
        }
    };

    return NewsletterReport;
});