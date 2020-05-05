(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/confirmation.service', [
        'cms.webanalytics/campaign.service',
        'CMS/Filters.Resolve'
    ])
        .service('confirmationService', confirmationService);

    /*@ngInject*/
    function confirmationService(campaignService, resolveFilter) {
        var that = this,
            config = getConfig();

        that.canRemoveAsset = can.bind(this, 'remove', 'asset');
        that.canRemoveObjective = can.bind(this, 'remove', 'objective');
        that.canEditObjective = can.bind(this, 'edit', 'objective');

        that.canRemoveConversion = function (isFunnel) {
            return isFunnel ? can('remove', 'funnelStep') : can('remove', 'conversion');
        };

        that.canEditConversion = function (isFunnel) {
            return isFunnel ? can('edit', 'funnelStep') : can('edit', 'conversion');
        };

        function can(action, object) {
            var status = campaignService.getCampaign().status,
                message = config[status] && config[status][action] && config[status][action][object];

            return !message || confirm(resolveFilter(message));
        }

        function getConfig() {
            var config = {
                Draft: {
                    remove: {
                        asset: 'campaign.asset.deleteconfirmation',
                        conversion: 'campaign.conversion.deleteconfirmation',
                        funnelStep: 'campaign.funnelstep.deleteconfirmation',
                        objective: 'campaign.objective.deleteconfirmation'
                    }
                },
                Running: {
                    remove: {
                        asset: 'campaign.asset.deleteconfirmation.running',
                        conversion: 'campaign.conversion.deleteconfirmation.running',
                        funnelStep: 'campaign.funnelstep.deleteconfirmation.running',
                        objective: 'campaign.objective.deleteconfirmation.running'
                    },
                    edit: {
                        conversion: 'campaign.conversion.editconfirmation.running',
                        funnelStep: 'campaign.funnelstep.editconfirmation.running',
                        objective: 'campaign.objective.editconfirmation.running'
                    }
                }
            };

            /* Both Draft and Scheduled campaign have the same confirmation messages */
            config.Scheduled = config.Draft;
            return config;
        }
    }
}(angular));