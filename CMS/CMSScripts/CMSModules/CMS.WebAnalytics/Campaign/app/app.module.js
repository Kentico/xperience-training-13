(function (angular, moduleName, shortNumber) {
    'use strict';
    angular.module(moduleName, [
            moduleName + 'app.templates',
            'cms.webanalytics/campaign/cms.forms.module',
            'cms.webanalytics/campaign.component',
            'ui.bootstrap'
    ])
        .controller('app', controller)
        .factory('authorizeInterceptor', ['$q', authorizeInterceptor])
        .factory('httpStatusInterceptor', ['$q', 'resolveFilter', httpStatusInterceptor])
        .factory('httpMethodInterceptor', httpMethodInterceptor)
        .config(['$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push('authorizeInterceptor');
            $httpProvider.interceptors.push('httpStatusInterceptor');
            $httpProvider.interceptors.push('httpMethodInterceptor');
        }]);

    shortNumber(angular);

    function controller() {
    }

    // CMSApi does not support PUT and DELETE requests due to security reasons
    // Transform those request to the GET and POST one with appropriate URLs
    function httpMethodInterceptor() {
        return {
            request: function (config) {
                if (config.url.indexOf('cmsapi/newsletters/') >= 0) {
                    return config;
                }

                if (config.method === 'POST') {
                    config.url += '/Post';
                }

                if (config.method === 'PUT') {
                    config.method = 'POST';
                    config.url += '/Put';
                }

                if (config.method === 'DELETE') {
                    config.method = 'POST';
                    config.url += '/Delete';
                }

                return config;
            }
        };
    }

    function authorizeInterceptor($q) {
        return {
            'responseError': function (rejection) {
                // User was signed off, need to redirect to the login page
                if (rejection.status === 403) {
                    var logonPageUrl = rejection.headers('logonpageurl');

                    if (logonPageUrl) {
                        window.top.location.href = logonPageUrl;
                    }
                }

                return $q.reject(rejection);
            }
        };
    }

    function httpStatusInterceptor($q, resolveFilter) {
        var CLIENT_ERROR_CATEGORY = 4,
            SERVER_ERROR_CATEGORY = 5;
        return {
            'responseError': function (rejection) {
                if (isStatusInCategory(rejection.status, CLIENT_ERROR_CATEGORY) && !isSpecialClientErrorStatus(rejection.status)) {
                    rejection.data = resolveFilter('campaign.autosave.failed');
                }

                if (isStatusInCategory(rejection.status, SERVER_ERROR_CATEGORY)) {
                    rejection.data = resolveFilter('campaign.autosave.server.error');
                }

                return $q.reject(rejection);
            }
        };
    }

    function isSpecialClientErrorStatus(status) {
        return status === 403 || status === 400;
    }

    function isStatusInCategory(status, category) {
        return (status / 100|0) === category;
    }

})(angular, moduleName, shortNumber);