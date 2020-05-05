cmsdefine([
    'angular',
    'angular-route',
    'angular-resource',
    'angular-sanitize',
    'CMS.ContactManagement/ContactImport/Controllers/RequirementsCheckerController',
    'CMS.ContactManagement/ContactImport/Controllers/FileUploadController',
    'CMS.ContactManagement/ContactImport/Controllers/AttributeMappingController',
    'CMS.ContactManagement/ContactImport/Controllers/ImportProcessController',
    'CMS.ContactManagement/ContactImport/Directives',
    'CMS.ContactManagement/ContactImport/Services',
    'CMS.ContactManagement/ContactImport/Interceptors',
    'CMS/Filters/Resolve',
    'CMS/Filters/StringFormat',
    'CMS/Messages/Module',
    'CMS/SmartTips/Module'
], function (
    angular,
    ngRoute,
    ngResource,
    ngSanitize,
    requirementsCheckerController,
    fileUploadController,
    attributeMappingController,
    importProcessController,
    directives,
    services,
    interceptors,
    resolveFilter,
    stringFormatFilter,
    messageModule,
    smartTipModule
) {

    return function (dataFromServer) {

        // Create ...
        var moduleName = 'cms.OnlineMarketing.ContactImport',
            module = angular.module(moduleName, [
                directives,
                services,
                interceptors,
                resolveFilter(angular, dataFromServer.resourceStrings),
                stringFormatFilter(angular),
                'ngRoute',
                'ngResource',
                'ngSanitize',
                messageModule(angular),
                smartTipModule(angular, dataFromServer.resourceStrings)
            ]);

        // Configure routes
        module.config([
            '$routeProvider', function ($routeProvider) {
                $routeProvider.when('/', {
                    templateUrl: 'requirementsCheckerTemplate.html',
                    controller: 'CMS.OnlineMarketing.ContactImport.Controllers.RequirementsCheckerController'
                });

                $routeProvider.when('/upload', {
                    templateUrl: 'fileUploadTemplate.html',
                    controller: 'CMS.OnlineMarketing.ContactImport.Controllers.FileUploadController'
                });

                $routeProvider.when('/mapping', {
                    templateUrl: 'attributeMappingTemplate.html',
                    controller: 'CMS.OnlineMarketing.ContactImport.Controllers.AttributeMappingController'
                });

                $routeProvider.when('/import', {
                    templateUrl: 'importProcessTemplate.html',
                    controller: 'CMS.OnlineMarketing.ContactImport.Controllers.ImportProcessController'
                });

                $routeProvider.otherwise({
                    redirectTo: '/'
                });
            }
        ]);

        // Create constant for server data
        module.constant("serverData", dataFromServer);

        // Inject controllers
        module.controller('CMS.OnlineMarketing.ContactImport.Controllers.RequirementsCheckerController', requirementsCheckerController);
        module.controller('CMS.OnlineMarketing.ContactImport.Controllers.FileUploadController', fileUploadController);
        module.controller('CMS.OnlineMarketing.ContactImport.Controllers.AttributeMappingController', attributeMappingController);
        module.controller('CMS.OnlineMarketing.ContactImport.Controllers.ImportProcessController', importProcessController);

        return moduleName;
    };
})