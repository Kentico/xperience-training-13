cmsdefine(['CMS/Application'], function(application) {

    var Licenses = function (reloadApplication) {
        if (reloadApplication) {
            application.reload();
        }
    };
    
    return Licenses;
});