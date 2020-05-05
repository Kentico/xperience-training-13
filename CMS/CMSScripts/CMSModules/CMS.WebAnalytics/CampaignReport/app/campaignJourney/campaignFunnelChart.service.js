(function (angular, _) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignFunnel.service', [
        'CMS/Filters.Resolve'
    ])
        .service('campaignFunnelService', funnelService);

    /*@ngInject*/
    function funnelService(resolveFilter) {
        var that = this,
            labelsPrefix = resolveFilter('campaign.visitors'),
            balloonTemplate = _.template(resolveFilter('campaign.report.funnel.compared') + '\n\n' +
                    resolveFilter('campaign.report.drop') + ' <strong><%= drop %></strong>\n' +
                    resolveFilter('campaign.report.droprate') + ' <strong><%= dropRate %>%</strong>'
            );

        that.initChartData = function (conversions) {
            if (!conversions || !conversions.length) {
                return;
            }

            var maxHits = getMaxHitsCount(conversions),
                chartData = mapChartData(conversions, maxHits);

            return {
                maxValue: maxHits,
                data: chartData
            };
        };

        function getMaxHitsCount (conversions) {
            return Math.max.apply(Math, conversions.map(function(c){return c.hits;}));
        }

        function mapChartData (conversions, maxHits) {
            return conversions.map(function (item, index, originals) {
                var prevItem = originals[index - 1];

                return {
                    value: item.hits,
                    legend: createLegend(item),
                    label: createLabel(item, prevItem, maxHits),
                    balloon: createBalloon(item, prevItem)
                }
            })
        }

        function createLegend (item, escape) {
            var legend = item.typeName + (item.name ? (': ' + item.name) : '');
            return escape ? _.escape(legend) : legend;
        }

        function createLabel (item, prevItem, maxHits) {
            var label = labelsPrefix + ' ' + item.hits;
            if (prevItem) {
                label += ' (' + round(getPercent(item.hits, maxHits)) + '%)';
            }
            return label;
        }

        function createBalloon (item, prevItem) {
            if (!prevItem) {
                return;
            }

            var rate = round(100.0 - getPercent(item.hits, prevItem.hits));

            return balloonTemplate({
                name: createLegend(item, true),
                drop: prevItem.hits - item.hits,
                dropRate: (rate > 0) ? rate : 0
            });
        }

        function getPercent (value, max) {
            if (max === 0) {
                return (value === 0) ? 0 : 100;
            }
            return (value / max) * 100 || 0;
        }

        function round (value) {
            return (Math.round(value * 100) / 100) || 0;
        }
    }
}(angular, _));