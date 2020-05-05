/**
* Returns current page address (URL).
*/
cmsdefine([], function () {

    /**
     * Gets current URL.
     *
     * @returns {string} Current URL
     */
    var getCurrentUrl = function () {
        return window.location.href;
    };


    /**
    * Gets current URL Location object.
    *
    * @returns {Location} Current URL
    */
    var getCurrentUrlLocation = function () {
        return window.location;
    };

    return {
        getCurrentUrl: getCurrentUrl,
        getCurrentUrlLocation: getCurrentUrlLocation
    };
});
