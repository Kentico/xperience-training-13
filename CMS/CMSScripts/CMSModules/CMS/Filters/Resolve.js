cmsdefine([], function () {

    return function (angular, resources) {

        var moduleName = 'CMS/Filters.Resolve',
            module = angular.module(moduleName, []);

        module.filter('resolve', function () {
            return function(key) {
                return resources[key] || key;
            };
        });

        return moduleName;
    };
})