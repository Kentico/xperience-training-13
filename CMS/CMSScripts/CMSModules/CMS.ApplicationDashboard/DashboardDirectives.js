cmsdefine(["require", "exports", 'angular', 'CMS.ApplicationDashboard/Directives/SingleObjectTileDirective', 'CMS.ApplicationDashboard/Directives/ApplicationTileDirective', 'CMS.ApplicationDashboard/Directives/WelcomeTileDirective', 'CMS.ApplicationDashboard/Directives/TileIconDirective'], function (cmsrequire, exports, angular, singleObjectTileDirective, applicationTileDirective, welcomeTileDirective, tileIconDirective) {
    var ModuleName = 'cms.dashboard.directives';

    angular.module(ModuleName, []).directive('singleObjectTile', singleObjectTileDirective.Directive).directive('applicationTile', applicationTileDirective.Directive).directive('welcometile', welcomeTileDirective.Directive).directive('tileIcon', tileIconDirective);

    exports.Module = ModuleName;
});
