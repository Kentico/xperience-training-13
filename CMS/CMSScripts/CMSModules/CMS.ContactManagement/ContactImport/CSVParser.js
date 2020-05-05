cmsdefine(['Underscore', 'csv-parser', 'CMS/StringHelper'], function (_, csvParser, StringHelper) {
    var BUFFER_DOES_NOT_CONTAIN_CSV = 'BUFFER_DOES_NOT_CONTAIN_CSV';
    csvParser.IGNORE_RECORD_LENGTH = true;
    csvParser.DETECT_TYPES = false;

    return {
        COLUMN_SEPARATOR: csvParser.COLUMN_SEPARATOR,
        IGNORE_RECORD_LENGTH: csvParser.IGNORE_RECORD_LENGTH,

        /**
         * Checks if length of every row in given input match the given length. If not, row is either spliced or filled up with empty values.
         * @param  input         string[[]]  Input to be checked
         * @param   columnsCount     int     Number of columns every row has to match 
         *                                   if any row is longer, it is spliced to match the length
         *                                   if any row is shorter, empty strings are added at the end of the line
         */
        fixLineColumnsCount: function (input, columnsCount) {
            var linesWithWrongColumnsCount = input.filter(function (line) {
                return line.length !== columnsCount;
            });

            linesWithWrongColumnsCount.forEach(function (line) {
                if (line.length > columnsCount) {
                    line.splice(columnsCount, Number.MAX_VALUE);
                }
                while (line.length < columnsCount) {
                    line.push('');
                }
            });
        },

        /**
        * Checks whether the length of given parsed lines match.
        * @param  input  string[string[]]  Array containing the parsed values
        * @return                          True, if length of all nested arrays match; otherwise, false
        */
        checkLineLength: function (input) {
            var lengths = input.map(function (line) {
                return line.length;
            }),
                firstRowLength,
                result = true;

            if (!lengths || !lengths.length) {
                return false;
            }

            firstRowLength = lengths[0];
            lengths.forEach(function (length) {
                result = result && (length === firstRowLength);
            });

            return result;
        },


        /**
         * Filters empty records from arrays of array.
         * @param  records  string[string[]]  Array of records.
         * @return          string[string[]]  Array of records without empty records.
         */
        filterEmptyRecords: function(records) {
            return records.filter(function (record) {
                return (record.length !== 1) || (record[0] !== '');
            });
        },

        /**
         * Find Valid CSV in buffer which may contains incomplete CSV. 
         * @param buffer           string buffer possibly with invalid csv.
         * @param tryWholeBuffer   try parse whole buffer at first attempt.
         * @return {
         *            rows: string[string[]],     // parsed CSV
         *            remainder: int              // number of characters not used for parsing in buffer
         *         }
         */
        findValidCSVInBuffer: function (buffer, tryWholeBuffer) {
            var lastEndOfLine = tryWholeBuffer ? {
                    start: buffer.length,
                    end: buffer.length,
                } : StringHelper.getLastEndOfLinePosition(buffer),
                originalLength = buffer.length,
                rows;

            csvParser.COLUMN_SEPARATOR = this.COLUMN_SEPARATOR;
            csvParser.IGNORE_RECORD_LENGTH = this.IGNORE_RECORD_LENGTH;

            while (lastEndOfLine.start > 0) {
                try {
                    buffer = buffer.substring(0, lastEndOfLine.start);
                    // Parse may throw exception
                    rows = csvParser.parse(buffer);
                    return {
                        rows: rows,
                        // Remainder of the text to read without the new line
                        remainder: originalLength - lastEndOfLine.end - (lastEndOfLine.end - lastEndOfLine.start),
                    };
                } catch (e) {
                    // Only UNEXPECTED_CHARACTER (ERROR_CHAR) is considered as exception
                    if (e !== csvParser.ERROR_EOF && e !== csvParser.ERROR_EOL) {
                        throw e;
                    }
                }
                lastEndOfLine = StringHelper.getLastEndOfLinePosition(buffer);
            }

            throw BUFFER_DOES_NOT_CONTAIN_CSV;
        }
    };
});