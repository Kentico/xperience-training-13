cmsdefine(["CMS/StringFormatter"], function (stringFormatter) {

    return function (angular) {

        var moduleName = 'CMS/Filters.StringFormat',
            module = angular.module(moduleName, []);

        module.filter('stringFormat', function() {
            return stringFormatter.format;
        });

        return moduleName;
    };
})