cmsdefine(['CMS.ContactManagement/Demographics/ChartDataLoader'], function(ChartDataLoader) {

    function ChartDataLoaderExtender(controller, action, parameters, titleField, valueField, callback) {
        ChartDataLoader.call(this, controller, action, parameters, titleField, valueField);
        this.callback = callback;
    }

    ChartDataLoaderExtender.prototype = Object.create(ChartDataLoader.prototype);

    ChartDataLoaderExtender.prototype.constructor = ChartDataLoaderExtender;

    ChartDataLoaderExtender.prototype.loadDataAsync = function() {
        return Object.getPrototypeOf(ChartDataLoaderExtender.prototype).loadDataAsync.call(this).then(this.callback);
    };

    return ChartDataLoaderExtender;
});