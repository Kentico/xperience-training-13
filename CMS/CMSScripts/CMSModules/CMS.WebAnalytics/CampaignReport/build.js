cmsdefine(['angular','CMS/Filters/Resolve','CMS/Filters/NumberShortener','CMS/UrlHelper','CMS.WebAnalytics/Components/CampaignStatusComponent','CMS.Charts/CampaignColumnChart','Underscore'], function(angular,resolve,numberShortener,urlHelper,campaignStatusComponent,cmsChart,_) { 
var moduleName = 'cms.webanalytics/campaignreport/';
return function(dataFromServer) { 
if(angular && resolve && dataFromServer && dataFromServer.resources) { 
resolve(angular, dataFromServer.resources); 
} 
(function (angular, moduleName, urlHelper, numberShortener) {
    'use strict';
    controller.$inject = ["$location"];
    angular.module(moduleName, [
            moduleName + "app.templates",
            'cms.webanalytics/campaignreport/campaignReport.component'
    ])
    .controller('app', controller);

    numberShortener(angular);
    
    function controller($location) {
        this.campaignId = urlHelper.getParameters($location.absUrl()).campaignid;
    }

})(angular, moduleName, urlHelper, numberShortener);
(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignObjective.component', [
        'CMS/Filters.Resolve',
        'CMS/Filters.NumberShortener'
    ])
        .component('cmsCampaignObjective', objectiveComponent());

    function objectiveComponent() {
        return {
            bindings: {
                objective: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/campaignObjective.component.html'
        };
    }
}(angular));
(function (angular, dataFromServer) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignReport.component', [
        'cms.webanalytics/campaignreport/campaignReportHeader.component',
        'cms.webanalytics/campaignreport/reportTabs.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCampaignReport', report());

    function report() {
        var component = {
            bindings: {
                campaignId: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/campaignReport.component.html',
            controller: controller
        };

        return component;
    }

    function controller() {
        this.report = dataFromServer.report;
    }
}(angular, dataFromServer));
(function (angular, campaignStatusComponent) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignReportHeader.component', [
        'cms.webanalytics/campaignreport/campaignObjective.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCampaignStatus', campaignStatusComponent)
        .component('cmsCampaignReportHeader', header());

    function header() {
        var component = {
            bindings: {
                report: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/campaignReportHeader.component.html'
        };

        return component;
    }
}(angular, campaignStatusComponent));
(function (angular) {
	'use strict';

	angular.module('cms.webanalytics/campaignreport/campaignTableCollapseButton.component', [])
        .component('cmsCollapseButton', button());

	function button() {
		return {
			bindings: {
			    collapsed: '='
			},
			templateUrl: 'cms.webanalytics/campaignreport/campaignTableCollapseButton.component.html'
		};
	}

}(angular));
(function(angular, _) {
    'use strict';

    controller.$inject = ["conversionReportService"];
    angular.module('cms.webanalytics/campaignreport/conversionReport.component',[
            'cms.webanalytics/campaignreport/sourceDetailLink.component',
            'cms.webanalytics/campaignreport/campaignTableCollapseButton.component',
            'cms.webanalytics/campaignreport/conversionReport.service',
            'CMS/Filters.Resolve'
        ])
        .component('cmsConversionReport', report());

    function report() {
        var component = {
            bindings: {
                conversion: '<',
                sourceDetails: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/conversionReport.component.html',
            controller: controller
        };

        return component;
    }
    
    /*@ngInject*/
    function controller(conversionReportService) {
        var self = this,
            prepareSourceLink = function (source) {
                var detail = _.find(self.sourceDetails, function (d) {
                    return d.name === source.name;
                });

                source.link = detail && detail.details;
            },

            init = function () {
                if (self.conversion) {
                    self.conversion.sources.map(prepareSourceLink);
                    self.transformedData = conversionReportService.initTableData(self.conversion);
                }
                
                self.sortTypeName = 'name';
                self.sortTypeHits = 'hits';

                self.sortType = self.sortTypeHits;
                self.sortDesc = true;
            };

        init();

        self.sort = function (type) {
            if (self.sortType === type) {
                self.sortDesc = !self.sortDesc;
            } else {
                self.sortType = type;
                switch (type) {
                    case self.sortTypeName:
                        self.sortDesc = false;
                        break;
                    case self.sortTypeHits:
                        self.sortDesc = true;
                        break;
                }
            }
        };

        self.showSorting = function (type, desc) {
            return (self.sortType === type) && desc;
        };
    }
}(angular, _));
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
(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/sourceDetailLink.component', [])
        .component('cmsSourceDetailLink', report());

    function report() {
       return {
            bindings: {
                link: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/sourceDetailLink.component.html'
        };
    }
}(angular));
(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/campaignFunnel.component', [
        'cms.webanalytics/campaignreport/campaignFunnelChart.component',
        'cms.webanalytics/campaignreport/campaignFunnelTable.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsCampaignFunnel', funnel());

    function funnel() {
        return {
            bindings: {
                report: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/campaignJourney/campaignFunnel.component.html',
            controller: controller
        };
    }

    function controller() {
        var ctrl = this;

        /* Filter only conversions which belong to funnel */
        ctrl.conversions = ctrl.report.conversions.filter(function(conversion) {
            return conversion.isFunnelStep;
        });
    }

}(angular));
(function (angular, cmsChart) {
    'use strict';

    controller.$inject = ["$timeout", "campaignFunnelService"];
    angular.module('cms.webanalytics/campaignreport/campaignFunnelChart.component', [
        'cms.webanalytics/campaignreport/campaignFunnel.service'
    ])
        .component('cmsCampaignFunnelChart', funnel());

    function funnel() {
        return {
            bindings: {
                chartId: '@',
                conversions: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/campaignJourney/campaignFunnelChart.component.html',
            controller: controller
        };
    }

    /*@ngInject*/
    function controller($timeout, campaignFunnelService) {
        var ctrl = this,
            chartData = campaignFunnelService.initChartData(ctrl.conversions);

        ctrl.legendId = ctrl.chartId + 'legend';

        ctrl.hasData = function () {
            return chartData && chartData.maxValue;
        };

        // Chart requires fully rendered HTML (to locate containers by ID) - this is why it's called from $timeout
        $timeout(function () {
            if (ctrl.hasData()) {
                createChart();
            }
        });

        function createChart () {
            cmsChart({
                chartDiv: ctrl.chartId,
                legendDiv: ctrl.legendId,
                data: chartData.data,
                maxValue: chartData.maxValue
            });
        }
    }
}(angular, cmsChart));
(function (angular, _) {
    'use strict';

    funnelService.$inject = ["resolveFilter"];
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
(function (angular, _) {
    'use strict';

    controller.$inject = ["campaignFunnelTableService"];
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
(function (angular, dataFromServer, urlHelper) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/demographicsLink/demographicsLink.component', [])
        .component('cmsDemographicsLink', demographicsLinkComponent());

    function demographicsLinkComponent() {
        return {
            bindings: {
                data: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/demographicsLink/demographicsLink.component.html',
            controller: controller
        };
    }


    /*@ngInject*/
    function controller() {
        var ctrl = this,
            data = ctrl.data;

        var parameters = {
            campaignConversionID: data.campaignConversionID
        };

        if(data.utmSource)
        {
            parameters.utmSource = data.utmSource === dataFromServer.defaultUTMSourceName ? '' : data.utmSource;
        }

        if(typeof data.utmContent !== 'undefined')
        {
            parameters.utmContent = data.utmContent;
        }

        var queryString = urlHelper.buildQueryString(parameters).substring(1);

        ctrl.href = dataFromServer.demographicsLink + '&' + queryString;
        ctrl.value = ctrl.data.value;
    }
}(angular, dataFromServer, urlHelper));
(function (angular) {
    'use strict';

    controller.$inject = ["resolveFilter"];
    angular.module('cms.webanalytics/campaignreport/reportTabs.component', [
        'cms.webanalytics/campaignreport/conversionReport.component',
        'cms.webanalytics/campaignreport/tabSwitcher.component',
        'cms.webanalytics/campaignreport/campaignFunnel.component',
        'CMS/Filters.Resolve'
    ])
        .component('cmsReportTabs', reportTabs());

    function reportTabs() {
        return {
            bindings: {
                report: '<'
            },
            templateUrl: 'cms.webanalytics/campaignreport/reportTabs/reportTabs.component.html',
            controller: controller
        };
    }

    /*@ngInject*/
    function controller(resolveFilter) {
        var ctrl = this;

        ctrl.nonFunnelConversions = ctrl.report.conversions.filter(function (c) {
            return !c.isFunnelStep
        });
        
        ctrl.tabIndex = 0;
        ctrl.tabs = [
            resolveFilter('campaign.conversions'),
            resolveFilter('campaign.journey')
        ];

        ctrl.changeTab = function (index) {
            ctrl.tabIndex = index;
        }
    }
}(angular));
(function (angular) {
    'use strict';

    angular.module('cms.webanalytics/campaignreport/tabSwitcher.component', [])
        .component('cmsTabSwitcher', tabSwitcher());

    function tabSwitcher() {
        return {
            bindings: {
                selectedIndex: '<',
                tabs: '<',
                onTabChange: '&'
            },
            templateUrl: 'cms.webanalytics/campaignreport/reportTabs/tabSwitcher.component.html',
            controller: controller
        };
    }

    function controller() {
        var ctrl = this;

        ctrl.activeTab = this.selectedIndex || 0;

        ctrl.changeTab = function (index) {
            ctrl.activeTab = index;
            ctrl.onTabChange({index: index});
        }
    }
}(angular));
angular.module("cms.webanalytics/campaignreport/app.templates", []).run(["$templateCache", function($templateCache) {$templateCache.put("cms.webanalytics/campaignreport/additionalResources.html","<!-- Resources used in non-html files (resolveFilter(resString)...) so they would not be automagically resolved by Gulp -->\r\n{{ \"campaigns.campaign.status.running\" | resolve }}\r\n{{ \"campaigns.campaign.status.scheduled\" | resolve }}\r\n{{ \"campaigns.campaign.status.draft\" | resolve }}\r\n{{ \"campaigns.campaign.status.finished\" | resolve }}\r\n{{ \"campaign.conversions\" | resolve }}\r\n{{ \"campaign.journey\" | resolve }}\r\n{{ \"campaign.visitors\" | resolve }}\r\n{{ \"campaign.report.drop\" | resolve }}\r\n{{ \"campaign.report.droprate\" | resolve }}\r\n{{ \"campaign.report.funnel.compared\" | resolve }}");
$templateCache.put("cms.webanalytics/campaignreport/campaignObjective.component.html","<div class=\'campaign-objective content-block-50\' data-ng-if=\'$ctrl.objective\' data-ng-class=\'{met : $ctrl.objective.actual >= $ctrl.objective.target}\'>\r\n    <div class=\'objective-header col-md-8 col-sm-8 col-xs-8\'>\r\n        <h4>{{ \"campaign.objective\" | resolve }}</h4>\r\n        <span>{{::$ctrl.objective.name}}</span>\r\n    </div>\r\n    <div class=\'objective-result col-md-2 col-sm-2\'>\r\n        <span>{{::100 * $ctrl.objective.actual / $ctrl.objective.target | number : 1}}%</span>\r\n    </div>\r\n    <div class=\'objective-detail col-md-1 col-sm-1 col-xs-6\'>\r\n        <dl>\r\n            <dt>{{::$ctrl.objective.actual | shortNumber}}</dt>\r\n            <dd>{{ \"campaign.objective.actual\" | resolve }}</dd>\r\n        </dl>\r\n    </div>\r\n    <div class=\'objective-detail col-md-1 col-sm-1\'>\r\n        <dl>\r\n            <dt>{{::$ctrl.objective.target | shortNumber}}</dt>\r\n            <dd>{{ \"campaign.objective.target\" | resolve }}</dd>\r\n        </dl>\r\n    </div>\r\n</div>\r\n\r\n");
$templateCache.put("cms.webanalytics/campaignreport/campaignReport.component.html","<div class=\'campaign-report\'>\r\n    <h2 data-ng-bind=\'::$ctrl.report.name\'></h2>\r\n    <cms-campaign-report-header report=\'$ctrl.report\'></cms-campaign-report-header>\r\n\r\n    <cms-report-tabs report=\'$ctrl.report\'></cms-report-tabs>\r\n\r\n    <footer data-ng-if=\'::$ctrl.report.updateDate\'>\r\n        <p>{{ \"campaign.report.lastupdate\" | resolve }} {{::$ctrl.report.updateDate | date:\'medium\'}}</p>\r\n    </footer>\r\n</div>\r\n");
$templateCache.put("cms.webanalytics/campaignreport/campaignReportHeader.component.html","<div class=\'content-block-50\'>\r\n    <cms-campaign-status status=\'$ctrl.report.status\'></cms-campaign-status>\r\n</div>\r\n<div class=\'campaign-description\'>\r\n    <p>{{::$ctrl.report.description}}</p>\r\n</div>\r\n<dl>\r\n    <dt data-ng-if=\'::$ctrl.report.launchDate\'>{{ \"campaign.basicinfo.start\" | resolve }}</dt>\r\n    <dd data-ng-if=\'::$ctrl.report.launchDate\'>{{::$ctrl.report.launchDate | date:\'medium\'}}</dd>\r\n    <dt data-ng-if=\'::$ctrl.report.finishDate\'>{{ \"campaign.basicinfo.end\" | resolve }}</dt>\r\n    <dd data-ng-if=\'::$ctrl.report.finishDate\'>{{::$ctrl.report.finishDate | date:\'medium\'}}</dd>\r\n</dl>\r\n<cms-campaign-objective objective=\'$ctrl.report.objective\'></cms-campaign-objective>");
$templateCache.put("cms.webanalytics/campaignreport/campaignTableCollapseButton.component.html","<button class=\'icon-only btn btn-icon blue\' data-ng-click=\'$ctrl.collapsed = !$ctrl.collapsed\'>\r\n    <i aria-hidden=\'true\' class=\'icon-plus-square \' title=\'{{ \"general.expand\" | resolve }}\' data-ng-if=\'$ctrl.collapsed\'></i>\r\n    <i aria-hidden=\'true\' class=\'icon-minus-square\' title=\'{{ \"general.collapse\" | resolve }}\' data-ng-if=\'!$ctrl.collapsed\'></i>\r\n</button>\r\n");
$templateCache.put("cms.webanalytics/campaignreport/conversionReport.component.html","<div class=\'conversion-report content-block\'>\r\n    <h4>{{::$ctrl.conversion.typeName + ($ctrl.conversion.name ? \': \' + $ctrl.conversion.name : \'\')}}</h4>\r\n    <table class=\'table table-hover\' data-ng-if=\'::$ctrl.conversion.sources.length\'>\r\n        <thead>\r\n            <tr class=\'unigrid-head\'>\r\n                <th class=\'main-column-icon-size\'>&nbsp;</th>\r\n                <th class=\'main-column-40\'>\r\n                    <a href=\'#\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeName)\'>{{ \"campaign.conversion.trafficchannel\" | resolve }}</a>\r\n                    <i aria-hidden=\'true\' class=\'icon-caret-down\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeName, $ctrl.sortDesc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeName)\'></i>\r\n                    <i aria-hidden=\'true\' class=\'icon-caret-up\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeName, !$ctrl.sortDesc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeName)\'></i>\r\n                </th>\r\n                <th class=\'main-column-30\'>\r\n                    {{ \"campaign.report.emailreports\" | resolve }}\r\n                </th>\r\n                <th class=\'text-right main-column-30\'>\r\n                    <a href=\'#\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeHits)\'>{{ \"conversions\" | resolve }}</a>\r\n                    <i aria-hidden=\'true\' class=\'icon-caret-down\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeHits, $ctrl.sortDesc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeHits)\'></i>\r\n                    <i aria-hidden=\'true\' class=\'icon-caret-up\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeHits, !$ctrl.sortDesc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeHits)\'></i>\r\n                </th>\r\n            </tr>\r\n        </thead>\r\n        <tbody class=\'tbody-hover\'>\r\n            <tr class=\'summary-row\' data-ng-repeat-start=\'source in $ctrl.transformedData | orderBy:$ctrl.sortType:$ctrl.sortDesc\'>\r\n                <td>\r\n                    <cms-collapse-button collapsed=\'source.collapsed\'></cms-collapse-button>\r\n                </td>\r\n                <td>{{::source.name}}</td>\r\n                <td>\r\n                    <cms-source-detail-link link=\'source.link\'></cms-source-detail-link>\r\n                </td>\r\n                <td class=\'text-right\'><cms-demographics-link data=\'source.hits\'></cms-demographics-link></td>\r\n            </tr>\r\n            <tr data-ng-repeat-end data-ng-repeat-start=\'content in source.contents\' data-ng-if=\'!source.collapsed\'>\r\n                <td>&nbsp;</td>\r\n                <td data-ng-if=\'!content.content\'>{{ \"campaign.conversion.trafficnocontent\" | resolve }}</td>\r\n                <td data-ng-if=\'content.content\'>{{::content.content}}</td>\r\n                <td>&nbsp;</td>\r\n                <td class=\'text-right\'>\r\n                    <cms-demographics-link data=\'content.hits\'></cms-demographics-link>\r\n                </td>\r\n            </tr>\r\n            <tr class=\'separator-row\' data-ng-repeat-end data-ng-if=\'!source.collapsed\'>\r\n                <td colspan=\'4\'>\r\n                    &nbsp;\r\n                </td>\r\n            </tr>\r\n            <tr class=\'summary-row\'>\r\n                <td>&nbsp;</td>\r\n                <td>{{ \"general.summary\" | resolve }}</td>\r\n                <td></td>\r\n                <td class=\'text-right\'><cms-demographics-link data=\'$ctrl.conversion.hitsWithUtmParameters\'></cms-demographics-link></td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n    <span data-ng-if=\'::!$ctrl.conversion.sources.length\'>{{ \"campaign.nodata\" | resolve }}</span>\r\n</div>");
$templateCache.put("cms.webanalytics/campaignreport/sourceDetailLink.component.html","<span>\r\n    <a target=\'_blank\' data-ng-href=\'{{::$ctrl.link.url}}\'>{{::$ctrl.link.text}}</a>\r\n</span>\r\n");
$templateCache.put("cms.webanalytics/campaignreport/campaignJourney/campaignFunnel.component.html","<h3>{{ \"campaign.journey\" | resolve }}</h3>\r\n<cms-campaign-funnel-chart conversions=\'$ctrl.conversions\' chart-id=\'campaignFunnelChart\'></cms-campaign-funnel-chart>\r\n<cms-campaign-funnel-table conversions=\'$ctrl.conversions\' source-details=\'$ctrl.report.sourceDetails\'></cms-campaign-funnel-table>");
$templateCache.put("cms.webanalytics/campaignreport/campaignJourney/campaignFunnelChart.component.html","<div class=\'campaign-table-chart-container\' data-ng-if=\'$ctrl.hasData()\'>\r\n    <div id=\'{{::$ctrl.chartId}}\' class=\'column-chart\'></div>\r\n    <div id=\'{{::$ctrl.legendId}}\' class=\'legend\'></div>\r\n</div>");
$templateCache.put("cms.webanalytics/campaignreport/campaignJourney/campaignFunnelTable.component.html","<div class=\'journey-report content-block\'>\r\n    <div data-ng-if=\'::$ctrl.conversions.length && $ctrl.sources.length\'>\r\n        <h4>{{ \"campaign.report.channelperformance\" | resolve }}</h4>\r\n        <table class=\'table table-hover\'>\r\n            <thead>\r\n                <tr class=\'unigrid-head\'>\r\n                    <th class=\'main-column-icon-size\'>\r\n                        &nbsp;\r\n                    </th>\r\n                    <th class=\'main-column-20\'>\r\n                        <a href=\'#\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeName)\'>{{ \"campaign.conversion.trafficchannel\" | resolve }}</a>\r\n                        <i aria-hidden=\'true\' class=\'icon-caret-down\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeName, $ctrl.sortDesc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeName)\'></i>\r\n                        <i aria-hidden=\'true\' class=\'icon-caret-up\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeName, !$ctrl.sortDesc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeName)\'></i>\r\n                    </th>\r\n                    <th class=\'main-column-20\' data-ng-if=\'::$ctrl.hasDetails\'>\r\n                        {{ \"campaign.report.emailreports\" | resolve }}\r\n                    </th>\r\n                    <th class=\'text-right\' data-ng-repeat=\'conversion in $ctrl.conversions track by $index\'>\r\n                        <a href=\'#\' title=\'{{::$ctrl.formatConversionName(conversion)}}\' data-ng-click=\'$ctrl.sort($index)\'>{{:: conversion.name || conversion.typeName }}</a>\r\n                        <i aria-hidden=\'true\' class=\'icon-caret-down\' data-ng-show=\'$ctrl.showSorting($index, $ctrl.sortDesc)\' data-ng-click=\'$ctrl.sort($index)\'></i>\r\n                        <i aria-hidden=\'true\' class=\'icon-caret-up\' data-ng-show=\'$ctrl.showSorting($index, !$ctrl.sortDesc)\' data-ng-click=\'$ctrl.sort($index)\'></i>\r\n                    </th>\r\n                    <th class=\'text-right\'>\r\n                        <a href=\'#\' title=\'{{ \"campaign.report.conversionrateexplanation\" | resolve }}\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeRate)\'>{{ \"campaign.conversionrate\" | resolve }}</a>\r\n                        <i aria-hidden=\'true\' class=\'icon-caret-down\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeRate, $ctrl.sortDesc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeRate)\'></i>\r\n                        <i aria-hidden=\'true\' class=\'icon-caret-up\' data-ng-show=\'$ctrl.showSorting($ctrl.sortTypeRate, !$ctrl.sortDesc)\' data-ng-click=\'$ctrl.sort($ctrl.sortTypeRate)\'></i>\r\n                    </th>\r\n                </tr>\r\n            </thead>\r\n            <tbody class=\'tbody-hover\'>\r\n                <tr class=\'summary-row\' data-ng-repeat-start=\'source in $ctrl.sources | orderBy:$ctrl.sortValueExtractor:$ctrl.sortDesc\'>\r\n                    <td>\r\n                        <cms-collapse-button collapsed=\'source.collapsed\'></cms-collapse-button>\r\n                    </td>\r\n                    <td>\r\n                        {{::source.name}}\r\n                    </td>\r\n                    <td data-ng-if=\'::$ctrl.hasDetails\'>\r\n                        <cms-source-detail-link link=\'source.link\'></cms-source-detail-link>\r\n                    </td>\r\n                    <td class=\'text-right\' data-ng-repeat=\'hitCount in source.hits track by $index\'>\r\n                        <cms-demographics-link data=\'hitCount\'></cms-demographics-link>\r\n                    </td>\r\n                    <td class=\'text-right\'>{{::source.conversionRate | percentage:2}}</td>\r\n                </tr>\r\n                <tr data-ng-repeat-end data-ng-repeat-start=\'content in source.contents\' data-ng-if=\'!source.collapsed\'>\r\n                    <td>&nbsp;</td>\r\n                    <td data-ng-if=\'!content.content\'>{{ \"campaign.conversion.trafficnocontent\" | resolve }}</td>\r\n                    <td data-ng-if=\'content.content\'>{{::content.content}}</td>\r\n                    <td data-ng-if=\'::$ctrl.hasDetails\'>&nbsp;</td>\r\n                    <td class=\'text-right\' data-ng-repeat=\'hitCount in content.hits track by $index\'>\r\n                        <cms-demographics-link data=\'hitCount\'></cms-demographics-link>\r\n                    </td>\r\n                    <td class=\'text-right\'>{{::content.conversionRate | percentage:2}}</td>\r\n                </tr>\r\n                <tr class=\'separator-row\' data-ng-repeat-end data-ng-if=\'!source.collapsed\'>\r\n                    <td colspan=\'{{::source.hits.length + ($ctrl.hasDetails ? 4 : 3)}}\'>\r\n                        &nbsp;\r\n                    </td>\r\n                </tr>\r\n                <tr class=\'summary-row\'>\r\n                    <td>&nbsp;</td>\r\n                    <td>{{ \"general.summary\" | resolve }}</td>\r\n                    <td data-ng-if=\'::$ctrl.hasDetails\'></td>\r\n                    <td class=\'text-right\' data-ng-repeat=\'summaryHit in $ctrl.summaryHits track by $index\'>\r\n                        <cms-demographics-link data=\'summaryHit\'></cms-demographics-link>\r\n                    </td>\r\n                    <td class=\'text-right\'>\r\n                        {{::$ctrl.conversionRate | percentage:2}}\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n    <span data-ng-if=\'::!$ctrl.conversions.length\'>{{ \"campaign.nojourneystep\" | resolve }}</span>\r\n    <span data-ng-if=\'::$ctrl.conversions.length && !$ctrl.sources.length\'>{{ \"campaign.nodata\" | resolve }}</span>\r\n</div>\r\n");
$templateCache.put("cms.webanalytics/campaignreport/demographicsLink/demographicsLink.component.html","<a href=\'{{::$ctrl.href}}\' target=\'_blank\' data-ng-if=\'::$ctrl.value > 0\'>{{::$ctrl.value}}</a>\r\n{{ ::$ctrl.value == 0 ? \'0\' : \'\' }}");
$templateCache.put("cms.webanalytics/campaignreport/reportTabs/reportTabs.component.html","<cms-tab-switcher tabs=\'::$ctrl.tabs\' selected-index=\'$ctrl.tabIndex\' on-tab-change=\'$ctrl.changeTab(index)\'></cms-tab-switcher>\r\n\r\n<div on=\'$ctrl.tabIndex\' data-ng-switch>\r\n    <div data-ng-switch-when=\'0\'>\r\n        <h3>{{ \"campaign.conversions\" | resolve }}</h3>\r\n        <div data-ng-repeat=\'conversion in ::$ctrl.nonFunnelConversions\'>\r\n            <cms-conversion-report conversion=\'::conversion\' source-details=\'::$ctrl.report.sourceDetails\'></cms-conversion-report>\r\n        </div>\r\n        <span data-ng-if=\'::!$ctrl.nonFunnelConversions.length\'>{{ \"campaign.noconversion\" | resolve }}</span>\r\n    </div>\r\n\r\n    <div data-ng-switch-when=\'1\'>\r\n        <cms-campaign-funnel report=\'::$ctrl.report\'></cms-campaign-funnel>\r\n    </div>\r\n</div>");
$templateCache.put("cms.webanalytics/campaignreport/reportTabs/tabSwitcher.component.html","<div class=\'content-block campaign-tab-switcher\'>\r\n    <div class=\'btn-group\'>\r\n        <button type=\'button\' class=\'btn btn-default\' data-ng-repeat=\'tab in ::$ctrl.tabs\' data-ng-class=\'{active: $ctrl.activeTab === $index}\' data-ng-click=\'$ctrl.changeTab($index)\'>\r\n            {{::tab}}\r\n        </button>\r\n    </div>\r\n</div>");}]);return 'cms.webanalytics/campaignreport/';}})