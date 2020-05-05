(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/statusMessage.component', [
        'cms.webanalytics/campaign.service',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCampaignStatusMessage', statusMessage());

    function statusMessage() {
        return {
            templateUrl: 'cms.webanalytics/campaign/statusMessage/statusMessage.component.html',
            replace: true,
            controller: controller
        }
    }

    /*@ngInject*/
    function controller(campaignService, resolveFilter) {
        var ctrl = this,
            campaign = campaignService.getCampaign();

        ctrl.closed = campaign.isDraft();

        ctrl.close = function () {
            ctrl.closed = true;
        };

        if (campaign.isFinished() || campaign.isScheduled()) {
            ctrl.alertMessage = {
                messageText: campaign.isFinished() ? resolveFilter('campaign.message.finished') : resolveFilter('campaign.message.scheduled'),
                cssClass : 'alert-info',
                icon : 'icon-i-circle'
            }
        } 
        else if (campaign.isLaunched()) {
            ctrl.alertMessage = {
                messageText: resolveFilter('campaign.message.running'),
                cssClass: 'alert-warning',
                icon: 'icon-exclamation-triangle'
            }
        }
    }
}(angular));