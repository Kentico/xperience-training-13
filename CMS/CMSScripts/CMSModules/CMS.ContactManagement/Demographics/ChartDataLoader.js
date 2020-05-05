cmsdefine(['jQuery', 'CMS/Application'], function ($, application) {

    function ChartDataLoader(controller, action, parameters, titleField, valueField) {

        if (!controller) {
            throw 'Controller (first argument) has to be specified';
        }

        if (!action) {
            throw 'Action (second argument) has to be specified';
        }

        this.controller = controller;
        this.action = action;
        this.parameters = parameters;
        this.titleField = titleField || 'title';
        this.valueField = valueField || 'value';
    }


    ChartDataLoader.prototype.loadDataAsync = function () {
        return $.get(application.getData('applicationUrl') + 'cmsapi/' + this.controller + '/' + this.action, this.parameters).then(this._prepareData.bind(this));
    };


    ChartDataLoader.prototype._prepareData = function (data) {
        data.map(function (item) {
            item.title = item[this.titleField];
            item.value = item[this.valueField];
        }.bind(this));

        return data;
    };


    return ChartDataLoader;
});
