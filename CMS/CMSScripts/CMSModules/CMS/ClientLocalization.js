/*
 * Provides methods for working with localized strings. 
 */
cmsdefine([], function () {

    /**
     * Get localized string for specific resource key.
     * 
     * @param {string} Resource key 
     * @returns {string} Localized string
     */
    function getString(key) {
        var translation,
            localizationStrings = window.CMS['localizationStrings'];

        if (localizationStrings !== undefined) {    
            translation = localizationStrings[key];
        }

        return (translation !== undefined) ? translation : key;
    }

    return {
        getString: getString
    };
});