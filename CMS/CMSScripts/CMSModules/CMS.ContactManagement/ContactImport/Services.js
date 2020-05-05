cmsdefine([
    'angular',
    'CMS.ContactManagement/ContactImport/Services/ContactResource',
    'CMS.ContactManagement/ContactImport/Services/ContactImportService',
    'CMS.ContactManagement/ContactImport/Services/ContactGroupResource'],
    function (
        angular,
        contactResource,
        contactImportService,
        contactGroupResource) {

        var moduleName = 'cms.onlinemarketing.contactimport.services';

        angular.module(moduleName, [])
            .factory('cmsContactResource', contactResource)
            .factory('cmsContactGroupResource', contactGroupResource)
            .factory('csmContactImportService', contactImportService);

        return moduleName;
    });
