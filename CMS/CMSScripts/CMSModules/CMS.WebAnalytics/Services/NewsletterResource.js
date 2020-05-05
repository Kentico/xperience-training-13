cmsdefine(['CMS/Application'], function (application) {
    return ['$resource', function ($resource) {
        var baseUrl = application.getData('applicationUrl') + 'cmsapi/newsletters/';

        return $resource(baseUrl, {}, {
            'getAllEmailCampaigns': { method: 'GET', url: baseUrl + 'getAllEmailCampaigns', isArray: true },
            'issues': { method: 'GET', url: baseUrl + 'getIssues', isArray: true },
            'templates': { method: 'GET', url: baseUrl + 'getTemplates', isArray: true },
            'createNewIssue': { method: 'POST', params: { newsletterId: 0}, url: baseUrl + 'createNewIssue' },
            'createEmailCampaign': { method: 'POST', url: baseUrl + 'createEmailCampaign' },
        });
    }];
});
