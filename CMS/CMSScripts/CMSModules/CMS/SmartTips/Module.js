cmsdefine([
    'CMS/SmartTips/SmartTipPlaceholder'
], function (
    smartTipPlaceholder
    ) {

    return function(angular, resources) {
        var moduleName = 'cms.smarttips';
        angular.module(moduleName, [])
            .value('resources', resources)
            .directive('cmsSmartTipPlaceholder', smartTipPlaceholder);

        return moduleName;
    };
})