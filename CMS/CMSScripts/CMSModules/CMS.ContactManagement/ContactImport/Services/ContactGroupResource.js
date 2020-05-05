cmsdefine(['CMS/Application'], function (application) {
    return ['$resource', 'cmsContactInterceptor', function ($resource, contactInterceptor) {
        var baseUrl = application.getData('applicationUrl') + 'cmsapi/ContactGroup/';

        return $resource(baseUrl, {}, {
            'create': {
                method: 'POST',
                url: baseUrl + 'post',
                interceptor: contactInterceptor
            }
        });
    }];
});
