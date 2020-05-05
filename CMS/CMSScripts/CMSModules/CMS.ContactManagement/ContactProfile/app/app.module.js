(function(angular, moduleName, urlHelper) {
    'use strict';
    angular.module(moduleName, [
            moduleName + "app.templates",
            'cms.contactmanagement/contactProfile.component',
            'cms.contactmanagement/contactprofile/card.component.wrapper'
        ])
        .controller('app', controller);
    

    function controller($location) {
        this.contactId = urlHelper.getParameters($location.absUrl()).contactid;
    }

})(angular, moduleName, urlHelper);

