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