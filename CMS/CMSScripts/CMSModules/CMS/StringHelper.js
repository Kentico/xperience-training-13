/**
 * Adds functionality for manipulating with strings.
 */

cmsdefine([], function () {

    /**
     * Gets characters that represent new line in the given text.
     * @param   text    string  text to find line separator in
     * @return  string          new line characters
     */
    var getEndOfLine = function (text) {
        var endsOfLine = ['\r\n', '\n', '\r'];

        for (var i = 0, len = endsOfLine.length; i < len; i++) {
            if (text.indexOf(endsOfLine[i]) >= 0) {
                return endsOfLine[i];
            }
        }

        return null;
    },


    /**
     * Returns position of the last return character in the given text.
     * @param  text  string  string to be checked. Can use both Windows/UNIX new line characters.
     * @return               start and end position of the last return character (if \r\n is used, start and end index differs)
     */
        getLastEndOfLinePosition = function (text) {
            var start,
                end,
                endOfLine = getEndOfLine(text);

            if (!endOfLine) {
                return { start: -1, end: -1 };
            }

            start = text.lastIndexOf(endOfLine);
            end = start + endOfLine.length - 1;

            return {
                start: start,
                end: end
            };
        };


    // Expose String helper API
    return {
        getLastEndOfLinePosition: getLastEndOfLinePosition
    };
});