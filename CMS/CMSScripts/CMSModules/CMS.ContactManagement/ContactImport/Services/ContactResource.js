cmsdefine(['CMS/Application'], function (application) {
    return ['$resource', 'cmsContactInterceptor', function ($resource, contactInterceptor) {
        var baseUrl = application.getData('applicationUrl') + 'cmsapi/ContactImport/';

        return $resource(baseUrl, {}, {
            'import': {
                method: 'POST',
                url: baseUrl + 'post',
                interceptor: contactInterceptor
            }
        });
    }];
});
