cmsdefine([
        "angular",
        "angular-resource",
        "angular-sanitize",
        "CMS.WebAnalytics/Services/CampaignResource",
        "CMS.WebAnalytics/Controllers/CampaignListController",
        "CMS.WebAnalytics/Directives/CampaignDetailDirective",
        "CMS.WebAnalytics/Components/CampaignStatusComponent",
        "CMS/Filters/Resolve",
        "CMS/Filters/StringFormat",
        "CMS/Filters/NumberShortener",
        "CMS/Messages/Module"
    ],
    function (
        angular,
        ngResource,
        ngSanitize,
        campaignResource,
        campaignListController,
        campaignDetailDirective,
        campaignStatusComponent,
        resolveFilter,
        stringFormatFilter,
        numberShortener,
        messagesModule
    ) {

        return function (dataFromServer) {
            var moduleName = "Campaigns",
                module = angular.module(moduleName, [
                    "ngResource",
                    "ngSanitize",
                    resolveFilter(angular, dataFromServer.resources),
                    stringFormatFilter(angular),
                    numberShortener(angular),
                    messagesModule(angular),
                ]);

            module.controller("CampaignList", campaignListController());

            module.directive("campaignDetail", campaignDetailDirective);

            module.component('cmsCampaignStatus', campaignStatusComponent);

            module.factory("cmsCampaignResource", campaignResource);
            
            module.factory("authorizeInterceptor", ["$q", function($q) {
                return {
                    'responseError': function (rejection) {
                        // User was signed off, need to redirect to the login page
                        if (rejection.status === 403) {
                            var logonPageUrl = rejection.headers("logonpageurl");

                            if (logonPageUrl) {
                                window.top.location.href = logonPageUrl;
                            }
                        }
                        
                        return $q.reject(rejection);
                    }
                };
            }]);

            // CMSApi does not support PUT and DELETE requests due to security reasons
            // Transform those request to the GET and POST one with appropriate URLs
            module.factory("httpMethodInterceptor", function () {

                return {
                    request: function (config) {
                        if (config.url.indexOf('cmsapi/newsletters/') >= 0) {
                            return config;
                        }

                        if (config.method === "POST") {
                            config.url += "/Post";
                        }

                        if (config.method === "PUT") {
                            config.method = "POST";
                            config.url += "/Put";
                        }
                        
                        if (config.method === "DELETE") {
                            config.method = "POST";
                            config.url += "/Delete";
                        }
                        
                        return config;
                    }
                };
            });
            
            module.config(["$httpProvider", function ($httpProvider) {
                $httpProvider.interceptors.push("authorizeInterceptor");
                $httpProvider.interceptors.push("httpMethodInterceptor");
            }]);

            // Create constant for server data
            module.constant("serverData", dataFromServer);

            return moduleName;
        };
});