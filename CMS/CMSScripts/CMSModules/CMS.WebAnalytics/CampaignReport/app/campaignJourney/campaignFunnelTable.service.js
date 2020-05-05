(function (angular, _) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignFunnelTable.service', [])
        .service('campaignFunnelTableService', funnelTableService);

    function funnelTableService() {
        var that = this;

        /**
         * Transforms data organized by conversions to data organized by source name
         */
        that.initTableData = function (conversions, sourceDetails) {
            if (!sourceDetails || !conversions || !conversions.length) {
                return [];
            }

            /* For each source name: collect info about that source from all campaigns, not all sources are relavant (have hits) for funnel conversion */
            /* Filer only sources with hits for funnel conversions */
            var contentRowsWithoutSummary = getContentRowsWithHits(conversions, sourceDetails);

            return addSummaryRowsAndTransformContents(contentRowsWithoutSummary, conversions);
        };

        /**
         *  Returns a number 0.0 - 1.0 calculated as last conversion hits to first conversion hits ratio
         */
        that.getTotalConversionRate = function (conversions) {
            if (conversions && conversions.length) {
                var first = conversions[0].hits;
                var last = conversions[conversions.length - 1].hits;

                return getConversionRate(first, last);
            }

            return 0;
        };


        that.getSummaryConversionHits = function (conversions) {
            var result = [];

            conversions.forEach(function (conversion) {
                result.push({
                    value: conversion.hits,
                    campaignConversionID: conversion.campaignConversionID
                })
            });

            return result;
        };


        function getContentRowsWithHits(conversions, sourceDetails) {
            return sourceDetails.map(function (sourceDetail) {
                var sourceName = sourceDetail.name;
                var sourceContent = sourceDetail.content;
                var sourceHits = addUtmParameters(getSourceHits(conversions, sourceName, sourceContent), conversions, sourceName, sourceContent);

                return {
                    name: sourceName,
                    content: sourceContent,
                    hits: sourceHits,
                    conversionRate: getConversionRate(sourceHits[0].value, sourceHits[sourceHits.length - 1].value),
                    link: sourceDetail.details
                };
            }).filter(function (source) {
                return source.hits.some(function (hit) {
                    return isBiggerThan0(hit.value);
                });
            });
        }

        function addSummaryRowsAndTransformContents(contentRowsWithoutSummary, conversions) {
            var uniqueSourceNames = getUniqueSourceNames(contentRowsWithoutSummary);

            return uniqueSourceNames.map(function (sourceName) {
                var contents = _.where(contentRowsWithoutSummary, {name: sourceName});
                var sourceHits = getSummaryOfContentHits(contents);

                return {
                    name: sourceName,
                    hits: sourceHits,
                    conversionRate: getConversionRate(sourceHits[0].value, sourceHits[sourceHits.length - 1].value),
                    collapsed: true,
                    link: getLink(contents),
                    contents: getSortedUnderlyingContents(contents)
                }
            });
        }


        function addUtmParameters(sourceHits, conversions, utmSource, utmContent) {
            var result = [];
            for (var i = 0; i < sourceHits.length; i++) {
                result.push({
                    value: sourceHits[i],
                    campaignConversionID: conversions[i].campaignConversionID,
                    utmSource: utmSource,
                    utmContent: utmContent
                });
            }

            return result;
        }


        function getUniqueSourceNames(contentRowsWithoutSummary) {
            var sourceNames = _.pluck(contentRowsWithoutSummary, 'name');
            return _.uniq(sourceNames);
        }

        /**
         *   This function is needed to create sum of hits for summary rows.
         *   We need to sum all hits for corresponding utm source from all utm contents.
         *   Easiest way to do so is to pluck only hits for each content. We can then transpose the matrix and sum each row to get
         *   total number of hits for each conversion from given source.
         *   Input:
         *       [{
         *           name: 'source',
         *           content: 'content1',
         *           hits: [{value: 10, ...}, {value: 5, ...}, {value: 1, ...}],
         *           conversionRate: 0.1,
         *           link: null
         *       }, {
         *           name: 'source',
         *           content: 'content2',
         *           hits: [{value: 5, ...}, {value: 3, ...}, {value: 1, ...}],
         *           conversionRate: 0.2,
         *           link: null
         *       }]
         *   Object after plucks and mapping:
         *       [[10, 5, 1],
         *       [5, 3, 1]]
         *   After transpose (_.zip.apply(_, hitsMatrix)):
         *       [[10 5],
         *       [5 3],
         *       [1 1]]
         *   Result:
         *       [{value: 15, ...}, {value: 8, ...}, {value: 3, ...}]
         */
        function getSummaryOfContentHits(listContents) {
            var originalHits = _.pluck(listContents, 'hits'),
                hitsMatrix = _.pluck(listContents, 'hits')
                    .map(function (hit) {
                        return _.pluck(hit, 'value');
                    }),

                transposedHits = _.zip.apply(_, hitsMatrix),

                totalHits = sumHitsForEachContent(transposedHits);

            return addUtmParameters(totalHits, originalHits[0], listContents[0].name);
        }

        function sumHitsForEachContent(transposedHits) {
            var totalHits = [];

            for (var i = 0; i < transposedHits.length; i++) {
                var rowSum = transposedHits[i].reduce(add, 0);
                totalHits.push(rowSum);
            }

            return totalHits;
        }

        /**
         * PhantomJS used in tests do not support lambda syntax, that is why this test is implemented as a method.
         */
        function add(a, b) {
            return a + b;
        }

        function getLink(contents) {
            var first = _.first(contents);
            return first.link;
        }

        function getSortedUnderlyingContents(contents) {
            var sortedContents = _.sortBy(contents, 'content');
            sortedContents = moveEmptyContentToEnd(sortedContents);

            var contentRows = sortedContents.map(function (content) {
                return {
                    content: content.content,
                    hits: content.hits,
                    conversionRate: content.conversionRate
                }
            });

            return contentRows;
        }

        function moveEmptyContentToEnd(sortedContents) {
            if (_.first(sortedContents).content === '') {
                sortedContents.push(sortedContents.shift());
            }
            return sortedContents;
        }

        /**
         * PhantomJS used in tests do not support lambda syntax, that is why this test is implemented as a method.
         */
        function isBiggerThan0(element) {
            return element > 0;
        }

        function getConversionRate(first, last) {
            if (first === 0) {
                return 0;
            }

            return (last || 0) / first;
        }

        /**
         * Returns number of conversion hits from given source
         */
        function findConversionSourceHits(conversion, sourceName, contentName) {
            var source = _.find(conversion.sources, function (source) {
                return source.name === sourceName && source.content === contentName;
            });

            return source ? source.hits : 0;
        }

        /**
         * Returns array of hit counts for given array of conversion. First number in result corresponds to first conversion etc.
         */
        function getSourceHits(conversions, sourceName, contentName) {
            return conversions.map(function (conversion) {
                return findConversionSourceHits(conversion, sourceName, contentName);
            });
        }
    }
}(angular, _));