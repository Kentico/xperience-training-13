/**
 * Registers collection of localized strings to global 'CMS' namespace.
 */
cmsdefine([], function () {

    var RegisterClientLocalization = function (data) {
        window.CMS = window.CMS || {};

        if (data !== undefined) {
            window.CMS['localizationStrings'] = data;
        }
    };

    return RegisterClientLocalization;
});