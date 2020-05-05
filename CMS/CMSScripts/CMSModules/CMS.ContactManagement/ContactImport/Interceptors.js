cmsdefine([
    'angular',
    'CMS.ContactManagement/ContactImport/Interceptors/ContactInterceptor'],
    function (
        angular,
        contactInterceptor) {

        var moduleName = 'cms.onlinemarketing.contactimport.interceptors';
        angular.module(moduleName, [])
            .factory('cmsContactInterceptor', contactInterceptor);

        return moduleName;
    });
