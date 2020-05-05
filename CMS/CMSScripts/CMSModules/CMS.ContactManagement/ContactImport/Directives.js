cmsdefine([
    'angular',    
    'CMS.ContactManagement/ContactImport/Directives/FileUploadDirective',
    'CMS.ContactManagement/ContactImport/Directives/AttributeMappingDirective',
    'CMS.ContactManagement/ContactImport/Directives/AttributeMappingControlDirective',
    'CMS.ContactManagement/ContactImport/Directives/ImportProcessDirective',
    'CMS.ContactManagement/ContactImport/Directives/DownloadDataDirective',
    'CMS/Messages/Module'],
    function (
        angular,
        fileUploadDirective,
        attributeMappingDirective,
        attributeMappingControlDirective,
        importProcessDirective,
        downloadDataDirective,
        messageModule) {

        var moduleName = 'cms.onlinemarketing.contactimport.directives';

        angular.module(moduleName, [messageModule(angular)])
            .directive('cmsFileUploadDirective', fileUploadDirective)
            .directive('cmsAttributeMappingDirective', attributeMappingDirective)
            .directive('cmsAttributeMappingControlDirective', attributeMappingControlDirective)
            .directive('cmsImportProcessDirective', importProcessDirective)
            .directive('cmsDownloadDataDirective', downloadDataDirective);
    
        return moduleName;
    });