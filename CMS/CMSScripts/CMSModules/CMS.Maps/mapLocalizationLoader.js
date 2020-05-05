cmsdefine(['amcharts.ammap', 'require', 'jQuery'], function (ammap, require, $) {

    var availableLanguages = ['af', 'ar', 'cs', 'da', 'de', 'el', 'es', 'et', 'fi', 'fr', 'hi', 'hr', 'hu', 'is', 'it', 'ja', 'ko', 'lt', 'lv', 'nb', 'nl', 'pl', 'pt', 'ru', 'sk', 'sl', 'sq', 'sv', 'th', 'zh'],

        getLocalizationModuleName = function (languageCode) {
            return 'CMS.Maps/ammap/lang/' + languageCode;
        },


        languageIsAlreadyLoaded = function (languageCode) {
            return !!AmCharts.mapTranslations[languageCode];
        },


        loadLocalizationAsync = function (languageCode) {
            var deferred = $.Deferred();

            if (!languageCode) {
                throw 'Incorrect argument, language code expected';
            }

            if (availableLanguages.indexOf(languageCode) === -1) {
                deferred.resolve('Given language is not available');
                return deferred;
            }

            if (languageIsAlreadyLoaded(languageCode)) {
                deferred.resolve();
            }
            else {
                require([getLocalizationModuleName(languageCode)], function () {
                    deferred.resolve();
                });
            }

            return deferred;
        };


    return {
        loadLocalizationAsync: loadLocalizationAsync
    }
});
