cmsdefine(['CMS/Application'], function (app) {

    return {
        bindings: {
            status: '<'
        },
        replace : true,
        templateUrl: app.getData('applicationUrl') + 'CMSScripts/CMSModules/CMS.WebAnalytics/Components/CampaignStatusComponent.html'
    };
});