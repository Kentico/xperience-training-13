(function (angular, dataFromServer, smartTipModule) {
    'use strict';

    angular.module('cms.webanalytics/campaign.component', [
            'cms.webanalytics/campaign.service',
            'cms.webanalytics/campaign/assets.service',
            'cms.webanalytics/tabs.service',
            'cms.webanalytics/campaign/messages.component',
            'cms.webanalytics/campaign/description.component',
            'cms.webanalytics/campaign/inventory.component',
            'cms.webanalytics/campaign/promotion.component',
            'cms.webanalytics/campaign/reportSetup.component',
            'cms.webanalytics/campaign/statusMessage.component',
            'CMS/Filters.Resolve',
            smartTipModule(angular, dataFromServer.resources)
    ])
        .component('cmsCampaign', campaign())
        .value('activityTypes', dataFromServer.activityTypes);

    function campaign() {
        return {
            templateUrl: 'cms.webanalytics/campaign/campaign.component.html',
            controller: controller
        };
    }

    /*@ngInject*/
    function controller(campaignService, assetsService, tabsService, resolveFilter) {
        var ctrl = this,
            scheduleTabLinkText = resolveFilter('campaign.schedule.link');

        ctrl.inventoryItems = assetsService.getInventoryItems();
        ctrl.promotionItems = assetsService.getPromotionItems();

        ctrl.campaign = campaignService.getCampaign();
        ctrl.conversions = dataFromServer.conversions || [];
        ctrl.urlAssetTargetRegex = dataFromServer.urlAssetTargetRegex;

        /* Setup smart tips */
        ctrl.promotionSmartTip = dataFromServer.smartTips['promotionSmartTip'];
        ctrl.promotionSmartTip.selector = '#promotionSmartTip';

        ctrl.launchSmartTip = dataFromServer.smartTips['launchSmartTip'];
        ctrl.launchSmartTip.selector = '#launchSmartTip';

        /*
        Add Schedule tab link to the launch section smart tip.
        The link itself will be created in tabs service because the url may change
        (when campaign is saved for the first time (added 'campaignid' parameter))
         */
        ctrl.initLaunchLink = function () {
            // Create clickable link
            var link = $cmsj('<a>' + scheduleTabLinkText + '</a>').click( function() {
                tabsService.navigateScheduleTab();
            });

            $cmsj(ctrl.launchSmartTip.selector + ' .js-st-content').append(link);
        };

    }
}(angular, dataFromServer, smartTipModule));