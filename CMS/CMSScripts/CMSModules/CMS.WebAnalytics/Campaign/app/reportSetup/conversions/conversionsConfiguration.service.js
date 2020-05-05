(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaign/reportSetup/conversionsConfiguration.service', [
        'CMS/Filters.Resolve'
    ])
        .service('conversionsConfigurationService', configurationService);

    /*@ngInject*/
    function configurationService(resolveFilter) {
        var configuration = initConfiguration();

        this.getConfiguration = function (isFunnel) {
            return isFunnel ? configuration.funnel : configuration.conversions;
        };

        function initConfiguration() {
            return {
                conversions: {
                    heading: resolveFilter('campaign.conversions'),
                    description: resolveFilter('campaign.conversions.description'),
                    dialogHeading: resolveFilter('campaign.conversions.defineconversion'),
                    addButtonLabel: resolveFilter('campaign.conversions.add'),
                    addButtonTitle: resolveFilter('campaign.conversions.add.title'),
                },

                funnel: {
                    heading: resolveFilter('campaign.journey'),
                    description: resolveFilter('campaign.journey.description'),
                    dialogHeading: resolveFilter('campaign.journey.definestep'),
                    addButtonLabel: resolveFilter('campaign.journey.add'),
                    addButtonTitle: resolveFilter('campaign.journey.add.title'),
                    additionalClass: 'separator',
                }
            }
        }
    }
}(angular));