(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/card/marketingEmailStatus.component', [
            'cms.contactmanagement/contactprofile/card/marketingEmailStatus.service',
            'CMS/Filters.Resolve'
        ])
        .component('cmsCardMarketingEmailStatus', cardMarketingEmail());
    

    function cardMarketingEmail() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/card/marketingEmailStatus/marketingEmailStatus.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(marketingEmailStatusService) {
        activate.apply(this);

        function activate() {
            marketingEmailStatusService.getMarketingEmailStatus(this.contactId)
                .then(onSuccess.bind(this));
        };

        function onSuccess(marketingEmailStatusViewModel) {
            this.marketingEmailStatus = marketingEmailStatusViewModel.MarketingEmailStatus;
        };
    };
}(angular));