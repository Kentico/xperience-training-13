(function (angular, cmsModalDirective, cmsCreatePageDialog, cmsSelectItemDialog) {
    "use strict";

    angular.module('cms.webanalytics/campaign/cms.dialogs.module', [])
        .directive('cmsModalDialog', cmsModalDirective)
        .directive('cmsCreatePageDialog', cmsCreatePageDialog)
        .directive('cmsSelectItemDialog', cmsSelectItemDialog);

}(angular, cmsModalDirective, cmsCreatePageDialog, cmsSelectItemDialog));