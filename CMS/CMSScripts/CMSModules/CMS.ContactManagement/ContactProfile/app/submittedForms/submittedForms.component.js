(function (angular) {
    'use strict';

    angular
        .module('cms.contactmanagement/contactprofile/submittedForms.component', [
            'cms.contactmanagement/contactprofile/submittedForms.service',
            'CMS/Filters.Resolve'
        ])
        .component('cmsSubmittedForms', submittedForms());


    function submittedForms() {
        var component = {
            bindings: {
                contactId: '<'
            },
            templateUrl: 'cms.contactmanagement/contactprofile/submittedForms/submittedForms.component.html',
            controller: controller
        };

        return component;
    };


    /*@ngInject*/
    function controller(submittedFormsService) {
        var self = this;
        activate();

        self.sortTypeName = 'FormDisplayName';
        self.sortTypeSite = 'SiteDisplayName';
        self.sortTypeDate = 'FormSubmissionDate';

        self.sortType = self.sortTypeDate;
        self.sortAsc = true;

        self.sort = function (type) {
            if (self.sortType === type) {
                self.sortAsc = !self.sortAsc;
            } else {
                self.sortType = type;
                switch (type) {
                    case self.sortTypeName:
                    case self.sortTypeSite:
                        self.sortAsc = false;
                        break;
                    case self.sortTypeDate:
                        self.sortAsc = true;
                        break;
                }
            }
        };

        self.showSorting = function(type, asc) {
            return self.sortType === type && asc;
        };

        function activate() {
            submittedFormsService.getSubmittedForms(self.contactId)
                .then(onSuccess);
        };

        function onSuccess(forms) {
            self.forms = forms;
        };
    };
}(angular));