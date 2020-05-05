cmsdefine(['CMS/Application'], function (application) {
    return ['$resource', function ($resource) {
        var baseUrl = application.getData('applicationUrl') + 'cmsapi/Asset';

        return $resource(baseUrl, {}, {
            'update': { method: 'PUT' },
            'create': { method: 'POST' },
            'delete': { method: 'DELETE' },
        });
    }];
});
