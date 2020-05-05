(function (angular) {
    'use strict';

    angular.module('cms.contactmanagement/contactprofile/detail/accounts.component', [
        'cms.contactmanagement/contactprofile/detail/accounts/account.component',
        'CMS/Filters.Resolve'
    ])
    .component('cmsAccounts', accountList());

    function accountList() {
        var component = {
            bindings: {
                field: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/detail/accounts/accounts.component.html',
        };

        return component;
    };
}(angular));