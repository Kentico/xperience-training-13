cmsdefine(['CMS/Context'], function (Context) {

    var Core = function (data, defaultData) {
        this.ctx = new Context(data, defaultData);
    };

    return Core;
})