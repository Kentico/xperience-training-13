cmsdefine(['amcharts.ammap', 'require', 'jQuery'], function (ammap, require, $) {

    var lowResMapSuffix = 'Low',
        mappings = {
            'world': 'world',
            'US': 'usa'
        },


        getLowResMapFileName = function (code) {
            if (!code) {
                throw 'Incorrect argument, map code expected';
            }
            if (!mappings[code]) {
                throw 'Cannot find map for code "' + code + '"';
            }

            return mappings[code] + lowResMapSuffix;
        },


        getLowResMapModuleName = function (resFileName) {
            return 'CMS.Maps/ammap/maps/js/' + resFileName;
        },


        mapFileIsAlreadyLoaded = function (resMapFile) {
            return !!AmCharts.maps[resMapFile];
        },


        loadMapSourceAsync = function (code) {
            var deferred = $.Deferred(),
                lowResMapFile = getLowResMapFileName(code),
                lowResMapModuleName = getLowResMapModuleName(lowResMapFile);

            if (mapFileIsAlreadyLoaded(lowResMapFile)) {
                deferred.resolve();
                return deferred;
            }

            require([lowResMapModuleName], function () {
                deferred.resolve(lowResMapModuleName);
            });

            return deferred;
        };

    return {
        loadMapSourceAsync: loadMapSourceAsync,
        getMapSourceName: getLowResMapFileName
    }
});
