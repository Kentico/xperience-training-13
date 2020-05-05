(function(angular, _) {
    'use strict';

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