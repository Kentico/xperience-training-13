cmsdefine(["require", "exports", 'angular', 'CMS/Application'], function (cmsrequire, exports, angular, application) {
    var Service = (function () {
        function Service($interval, $http) {
            var _this = this;
            this.$interval = $interval;
            this.$http = $http;
            /**
            * Polling interval in seconds.
            */
            this.POLLING_INTERVAL = 20;
            this.start = function (applicationGuid, newDataCallback, noDataCallback) {
                var oldData = null, getData = function () {
                    _this.$http.post(application.getData('applicationUrl') + 'cmsapi/Tile/UpdateTile?tileModelType=applicationLiveTileModel', { applicationGuid: applicationGuid }).success(function (newLiveTileData) {
                        if (!newLiveTileData || isNaN(newLiveTileData.Value)) {
                            oldData = null;
                            noDataCallback();
                            return;
                        }

                        var data = {
                            Value: newLiveTileData.Value,
                            Description: newLiveTileData.Description
                        };

                        if (!angular.equals(oldData, data)) {
                            oldData = data;
                            newDataCallback(data);
                        }
                    }).error(function (data, status) {
                        if (status == 403) {
                            window.top.location.href = data.LogonPageUrl;
                        }
                        oldData = null;
                        noDataCallback();
                    });
                };

                getData();

                return _this.$interval(getData, _this.POLLING_INTERVAL * 1000);
            };
            }
        Service.$inject = [
            '$interval',
            '$http'
        ];
        return Service;
    })();
    exports.Service = Service;
});
