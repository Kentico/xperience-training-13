cmsdefine([], function () {

    /**
     * Gets the decimal and thousand separators based on the current user's browser locale.
     * https://www.amcharts.com/kbase/automatically-format-numbers-according-to-users-locale/
     */
    function getNumberSeparators() {
        // default
        var result = {
            decimal: ',',
            thousand: '.'
        };

        // convert a number formatted according to locale
        var str = parseFloat(1234.56).toLocaleString();

        // if the resulting number does not contain previous number
        // (i.e. in some Arabic formats), return defaults
        if (!str.match('1')) {
            return result;
        }

        // get decimal and thousand separators
        result.decimal = str.replace(/.*4(.*)5.*/, '$1');
        result.thousand = str.replace(/.*1(.*)2.*/, '$1');

        // return results
        return result;
    }

    /**
     * Converts the input value to it's localized string form.
     *
     * @param {string|number} value to convert
     */
    function toLocaleString(value) {
        return parseFloat(value).toLocaleString();
    }

    return {
        getNumberSeparators: getNumberSeparators,
        toLocaleString: toLocaleString
    }
});
