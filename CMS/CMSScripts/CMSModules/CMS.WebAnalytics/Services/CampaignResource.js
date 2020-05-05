cmsdefine(['CMS/Application'], function (application) {
    return ['$resource', function ($resource) {
        var baseUrl = application.getData('applicationUrl') + 'cmsapi/Campaign/';

        return $resource(baseUrl, {}, {
            'update': { method: 'PUT' }
        });
    }];
});
