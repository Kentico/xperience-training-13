cmsdefine(['CMS/NumberShortener'], function (numberShortener) {
    /**
     * Makes number shorter i.e. 10 000 to 10k, 1 000 000 to 1M. Number is always rounded to interger. 
     * After shortening two decimal places are kept: e.g. 5 263 to 5.26k.
     */
    return function (angular) {
        var DECIMAL_FRACTION = 2,
            LABELS = { thousand: 'k', million: 'M', billion: 'G' },
            moduleName = 'CMS/Filters.NumberShortener',
            module = angular.module(moduleName, []);

        module.filter('shortNumber', function () {
            return function (number) {
                return numberShortener.shortenNumber(Math.round(number), DECIMAL_FRACTION, LABELS);;
            }
        });

        return moduleName;
    };
})