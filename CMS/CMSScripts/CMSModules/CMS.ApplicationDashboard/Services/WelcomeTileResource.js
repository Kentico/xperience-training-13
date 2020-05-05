cmsdefine(["require", "exports", 'CMS/Application'], function (cmsrequire, exports, application) {
    exports.Resource = [
        '$resource',
        function ($resource) {
            var updateAction = {
                method: 'POST',
                transformRequest: function (updated) {
                    return JSON.stringify(updated.Visible);
                }
            };

            return $resource(application.getData('applicationUrl') + 'cmsapi/WelcomeTile/', {}, {
                update: updateAction
            });
        }
    ];
});
