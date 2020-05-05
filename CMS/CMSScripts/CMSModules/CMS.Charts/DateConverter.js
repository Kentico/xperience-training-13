/**
 * Converts given date to the locale string using the AmCharts localization pattern.
 */

cmsdefine([], function() {

    var getCurrentBCP47 = function() {
            return navigator.language;
        },


        contains = function(format, pattern) {
            if (!format) {
                return false;
            }
            return format.indexOf(pattern) !== -1;
        },


        tryParseYear = function(amDateFormat) {
            if(contains(amDateFormat, 'YYYY')) {
                return 'numeric';
            } else if(contains(amDateFormat, 'YY')) {
                return '2-digit'
            }

            return undefined;
        },


        tryParseMonth = function(amDateFormat) {
            if(contains(amDateFormat, 'MMMM'))
            {
                return 'long';
            } else if(contains(amDateFormat, 'MMM'))
            {
                return 'short';
            } else if(contains(amDateFormat, 'MM')) {
                return '2-digit';
            } else if(contains(amDateFormat, 'M')) {
                return 'numeric';
            }

            return undefined;
        },


        tryParseDay = function(amDateFormat) {
            if(contains(amDateFormat, 'DD')) {
                return '2-digit';
            } else if(contains(amDateFormat, 'D')) {
                return 'numeric';
            }

            return undefined;
        },

        tryParseHour = function (amDateFormat) {
            if (contains(amDateFormat, 'hh')) {
                return '2-digit';
            }

            return undefined;
        },

        tryParseMinute = function (amDateFormat) {
            if (contains(amDateFormat, 'mm')) {
                return '2-digit';
            }

            return undefined;
        },


        parseFormat = function (amDateFormat) {
            return {
                year: tryParseYear(amDateFormat),
                month: tryParseMonth(amDateFormat),
                day: tryParseDay(amDateFormat),
                hour: tryParseHour(amDateFormat),
                minute: tryParseMinute(amDateFormat)
            };
        };

    return {
        toLocaleDateString: function (date, format) {
            return date.toLocaleDateString(getCurrentBCP47(), parseFormat(format));
        },
        toLocaleTimeString: function (date, format) {
            return date.toLocaleTimeString(getCurrentBCP47(), parseFormat(format));
        }
    };
});
