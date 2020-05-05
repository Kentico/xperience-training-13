(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactprofile/detail/accounts/account.component', [
        'CMS/Filters.Resolve'
    ])
    .component('cmsAccount', account());

    function account() {
        var component = {
            bindings: {
                account: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/detail/accounts/account/account.component.html',
        };

        return component;
    };
}(angular));