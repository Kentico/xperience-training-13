(function (angular, _, EventHub) {
    'use strict';

    angular.module('cms.webanalytics/campaign/breadcrumbs.service', [
        'cms.webanalytics/campaign/serverData.service'
    ])
        .service('breadcrumbsService', breadcrumbs);

    /*@ngInject*/
    function breadcrumbs(serverDataService) {
        var that = this,
            breadcrumbs = serverDataService.getDataFromServer().breadcrumbs;

        that.updateBreadcrumbs = function (campaign, isNew) {
            breadcrumbs.data[1].text = _.escape(campaign.DisplayName);

            if (isNew) {
                breadcrumbs.pin.objectName = campaign.CodeName;
                breadcrumbs.pin.objectSiteName = campaign.SiteName;
                breadcrumbs.pin.isPinned = false;
            } else {
                breadcrumbs.pin = null;
            }

            EventHub.publish("OverwriteBreadcrumbs", breadcrumbs);
        }
    }
}(angular, _, EventHub));