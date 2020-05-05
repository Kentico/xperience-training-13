cmsdefine([
    'angular',
    'angular-route',
    'angular-resource',
    'angular-animate',
    'angularSortable',
    'angular-ellipsis',
    'CMS.ApplicationDashboard/DashboardController',
    'CMS.ApplicationDashboard/DashboardDirectives',
    'CMS.ApplicationDashboard/DashboardServices'
], function(
    angular,
    angularRoute,
    angularResource,
    angularAnimate,
    angularSortable,
    angularEllipsis,
    cmsDashboardController,
    cmsDashboardDirectives,
    cmsDashboardServices
) {
    return function () {
        // Create cms.dashboard angular module
        var moduleName = 'cms.ApplicationDashboard',
            module = angular.module(moduleName, [
                cmsDashboardDirectives.Module,
                cmsDashboardServices.Module,
                'ngRoute',
                'ngResource',
                'ngAnimate',
                'ui.sortable',
                'dibari.angular-ellipsis'
        ]);

        // Configure routes
        module.config([
            '$routeProvider', function($routeProvider) {
                $routeProvider.when('/', {
                    templateUrl: 'dashboard.html',
                    controller: 'cms.dashboardController'
                });

                $routeProvider.otherwise({
                    redirectTo: '/'
                });
            }
        ]);

        // Inject controllers
        module.controller('cms.dashboardController', cmsDashboardController.Controller);
        return moduleName;
    };
});