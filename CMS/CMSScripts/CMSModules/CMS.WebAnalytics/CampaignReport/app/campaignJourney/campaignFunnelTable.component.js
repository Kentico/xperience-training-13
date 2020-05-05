(function (angular, _) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignFunnelTable.component', [
        'cms.webanalytics/campaignreport/sourceDetailLink.component',
        'cms.webanalytics/campaignreport/campaignTableCollapseButton.component',
        'cms.webanalytics/campaignreport/campaignFunnelTable.service',
        'cms.webanalytics/campaignreport/demographicsLink/demographicsLink.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCampaignFunnelTable', report())
        .filter('percentage', ['$filter', function ($filter) {
            return function (input, decimals) {
                return $filter('number')(input * 100, decimals) + '%';
            };
        }]);

    function report() {
        return {
            bindings: {
                conversions: '<',
                sourceDetails: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/campaignJourney/campaignFunnelTable.component.html',
            controller: controller
        };
    }

    /*@ngInject*/
    function controller(campaignFunnelTableService) {
        var ctrl = this;
        ctrl.sortTypeName = 'name';
        ctrl.sortTypeContent = 'content';
        ctrl.sortTypeRate = 'rate';

        ctrl.sortType = ctrl.sortTypeRate;
        ctrl.sortDesc = true;

        ctrl.showSorting = function (type, desc) {
            return (ctrl.sortType === type) && desc;
        };

        /* Handles switching of sort column and direction */
        ctrl.sort = function (type) {
            if (ctrl.sortType === type) {
                ctrl.sortDesc = !ctrl.sortDesc;
            } else {
                ctrl.sortType = type;
                switch (ctrl.sortType) {
                    case ctrl.sortTypeName:
                    case ctrl.sortTypeContent:
                        ctrl.sortDesc = false;
                        break;
                    default:
                        ctrl.sortDesc = true;
                        break;
                }
            }
        };

        ctrl.sortValueExtractor = function (source) {
            switch (ctrl.sortType) {
                case ctrl.sortTypeName:
                    return source.name;
                case ctrl.sortTypeRate:
                    return source.conversionRate;
                default:
                    if (typeof ctrl.sortType === 'number') {
                        return source.hits[ctrl.sortType];
                    }
                    break;
            }

            return source.name;
        };

        /* Prepare items (rows) for rendered table. */
        ctrl.sources = campaignFunnelTableService.initTableData(ctrl.conversions, ctrl.sourceDetails);

        ctrl.summaryHits = campaignFunnelTableService.getSummaryConversionHits(ctrl.conversions);

        ctrl.conversionRate = campaignFunnelTableService.getTotalConversionRate(ctrl.conversions);

        /* Flag indicating that any of sources has detail link */
        ctrl.hasDetails = _.find(ctrl.sources, function (source) {
            return source.link;
        });

        ctrl.formatConversionName = function (conversion) {
            return conversion.typeName + (conversion.name ? ': ' + conversion.name : '');
        }
    }
}(angular, _));