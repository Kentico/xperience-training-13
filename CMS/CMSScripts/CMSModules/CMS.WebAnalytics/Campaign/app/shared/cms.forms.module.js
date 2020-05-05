(function (angular, cmsTextboxDirective, cmsRadioButtonDirective, cmsSelectDirective, cmsSelect2Directive, cmsTextareaDirective) {
    'use strict';

    // Wraps all directives from CMS Forms into one module
    angular.module('cms.webanalytics/campaign/cms.forms.module', [])
        .directive('cmsTextbox', cmsTextboxDirective)
        .directive('cmsTextarea', cmsTextareaDirective)
        .directive('cmsRadioButton', cmsRadioButtonDirective)
        .directive('cmsSelect', cmsSelectDirective)        
        .directive('cmsSelect2', cmsSelect2Directive);

}(angular, cmsTextboxDirective, cmsRadioButtonDirective, cmsSelectDirective, cmsSelect2Directive, cmsTextareaDirective));