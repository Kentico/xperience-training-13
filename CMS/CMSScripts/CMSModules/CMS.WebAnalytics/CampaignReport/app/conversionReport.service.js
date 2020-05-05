(function(angular, _) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/conversionReport.service', [])
        .service('conversionReportService', conversionReportService);

    function conversionReportService() {
        var that = this;

        /**
         * Function adds summary rows for each utm source in conversion and transforms entry structure.
         * @conversion {structure} entry structure.
         *           [{
         *               "name": "utmSource",
         *               "content": "post1",
         *               "hits": 700,
         *               "link": null
         *           } , {
         *               "name": "utmSource",
         *               "content": "",
         *               "hits": 200,
         *               "link": null
         *           }]
         * @returns {structure} structure with summary for each utm source and underlying utm contents.
         *           [{
         *               "name": utmSource,
         *               "hits": 900,             // number of total hits for each conversion for given utm source
         *               "link": null,       // in case of email we will have link details here
         *               "collapsed": true
         *               "contents": [{           // list of different utm contents under given utm source
         *                   "content": "post1",
         *                   "hits": { value: 700, ... },
         *               }, {
         *                   "content": "",
         *                   "hits": { value: 200, ... }
         *               }]
         *           }]
         */
        that.initTableData = function (conversion) {
            var uniqueSourceNames = getUniqueSourceNames(conversion.sources);
            conversion.hitsWithUtmParameters = addUtmParameters(conversion.hits, conversion.campaignConversionID);

            return uniqueSourceNames.map(
                function (sourceName) {
                    var contents = getContentsWithSourceName(conversion, sourceName);

                    return {
                        name: sourceName,
                        hits: addUtmParameters(sumAllHitsForSource(contents), conversion.campaignConversionID, sourceName),
                        link: getLink(contents),
                        collapsed: true,
                        contents: getSortedUnderlyingContents(contents)
                    }
                });
        }


        function addUtmParameters(sourceHit, campaignConversionID, utmSource, utmContent) {
            return {
                value: sourceHit,
                campaignConversionID: campaignConversionID,
                utmSource: utmSource,
                utmContent: utmContent
            }
        }


        function getUniqueSourceNames(conversionSources) {
            var sourceNames = _.pluck(conversionSources, 'name');
            return _.uniq(sourceNames);
        }

        function getContentsWithSourceName(conversions, sourceName) {
            return _.where(conversions.sources, { name: sourceName }).map(function(conversionSource) {
                conversionSource.campaignConversionID = conversions.campaignConversionID;
                return conversionSource;
            });
        }

        function getLink(contents) {
            return _.first(contents).link;
        }

        function sumAllHitsForSource(contents) {
            var hitsArray = _.pluck(contents, 'hits');
            return reduceTotalHits(hitsArray);
        }

        function reduceTotalHits(hitsArray) {
            return hitsArray.reduce(add, 0);
        }

        function add(a, b) {
             return a + b;
        }

        function getSortedUnderlyingContents(contents) {
            var sortedContents = _.sortBy(contents, 'content');
            sortedContents = moveEmptyContentToEnd(sortedContents);

            var contentRows = sortedContents.map(function (content) {
                return {
                    content: content.content,
                    hits: addUtmParameters(content.hits, content.campaignConversionID, content.name, content.content)
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
    }
}(angular, _));