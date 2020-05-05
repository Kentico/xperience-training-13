(function (angular, _) {
    'use strict';

    angular.module('cms.webanalytics/campaign/inventory/item/getLink.service', [
        'cms.webanalytics/campaign/cms.urlHelper.service'
    ])
        .service('getLinkService', getLinkService);

    /*@ngInject*/
    function getLinkService(urlHelperService) {
        var that = this,
            urlHelper = urlHelperService.urlHelper;

        that.buildLink = function (originalLink, utmCampaign, utmSource, utmMedium, utmContent, emptyUtmSourceText) {
            if (!utmSource) {
                return emptyUtmSourceText;
            }

            var originalLinkWithoutQueryString = urlHelper.removeQueryString(originalLink),
                queryParams = urlHelper.getParameters(originalLink);

            if (utmCampaign) {
                queryParams.utm_campaign = utmCampaign;
            }

            queryParams.utm_source = utmSource;

            if (utmMedium) {
                queryParams.utm_medium = utmMedium;
            }

            if (utmContent) {
                queryParams.utm_content = utmContent;
            }

            return originalLinkWithoutQueryString + urlHelper.buildQueryString(queryParams);
        }
    }
}(angular, _));