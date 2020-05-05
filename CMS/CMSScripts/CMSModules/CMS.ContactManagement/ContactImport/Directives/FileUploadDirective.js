cmsdefine([
    'Underscore',
    'jQuery',
    'CMS.ContactManagement/ContactImport/FileStreamer',
    'CMS.ContactManagement/ContactImport/CSVParser'], function (
        _,
        $,
        FileStreamer,
        CSVParser) {

        var Controller = function ($scope, $element, $timeout, resourceFilter, serverData, messageHub) {
            this.$scope = $scope;
            this.$fileInput = $($element).children('.js-file-input');
            this.$timeout = $timeout;
            this.PREVIEW_ROW_COUNT = 5;
            this.CSVParser = CSVParser;
            this.getString = resourceFilter;
            this.messageHub = messageHub;
            this.$scope.onClick = this.elementClick.bind(this);
            this.$fileInput.on('change', this.fileInputChanged.bind(this));

            this.$scope.howToSmartTip = serverData.smartTips['tipHowToImport'];
            this.$scope.howToSmartTip.selector = '#tipHowToImport';

        },
            directive = function () {
                return {
                    restrict: 'A',
                    templateUrl: 'fileUploadDirectiveTemplate.html',
                    controller: [
                        '$scope',
                        '$element',
                        '$timeout',
                        'resolveFilter',
                        'serverData',
                        'messageHub',
                        Controller],
                };
            };


        /**
         * Raise click event on the hidden HTML file input.
         */
        Controller.prototype.elementClick = function () {
            this.$fileInput.click();
        };


        /**
         * Apply the change to the current scope.
         */
        Controller.prototype.fileInputChanged = function () {
            var that = this;
            this.$timeout(function () {
                that.fileSelected();
            });
        };


        /**
         * Checks whether the file is in valid format and sends event message to parent controller.
         * Creates new instance of the FileStreamer for the given file.
         */
        Controller.prototype.fileSelected = function () {
            this.$scope.sourceFileName = this.$fileInput[0].files[0];
            this.$scope.fileStream = new FileStreamer(this.$scope.sourceFileName);
            this.fileStreamLoaded();
        };


        /**
         * Shows the given message and set value of the file input to null.
         * @param  {string}  message  Message to be displayed on error.
         */
        Controller.prototype.badInputFile = function (message) {
            this.messageHub.publishError(message);

            // Set file input value to null to be able to get notified when the same file is selected again.
            this.$fileInput.val(null);
            this.$scope.$apply();
        };


        /**
         * Uses CSV parser, parse first n (PREVIEW_ROW_COUNT) rows of given file streamer and emits CSV columns with example data and file streamer to controller. 
         */
        Controller.prototype.fileStreamLoaded = function () {
            var that = this;

            this.$scope.fileStream.read(function (buffer, handle) {
                var result,
                    lines;

                handle.reset();

                if (buffer === '') {
                    that.badInputFile(that.getString('om.contact.importcsv.emptyfile'));
                    return;
                }

                try {
                    result = that._getParserResultFromBuffer(buffer, handle);
                } catch (e) {
                    that.badInputFile(that.getString(e));
                    return;
                }

                lines = _.first(result.rows, that.PREVIEW_ROW_COUNT);
                lines = that.CSVParser.filterEmptyRecords(lines);

                // Emitting to the parent controller, can use scope events.
                that.$scope.$emit('firstNRowsLoaded', {
                    parsedLines: lines,
                    fileStream: that.$scope.fileStream,
                    sourceFileName: that.$scope.sourceFileName
                });
            });
        };


        Controller.prototype._getParserResultFromBuffer = function (buffer, handle) {
            var parserError,
                batchParsedBySemicolon,
                batchParsedByComma,
                result,
                defaultIgnoreRecordLenght = this.CSVParser.IGNORE_RECORD_LENGTH,
                bothBatchesAreEmpty,
                bothBatchesContainsOnlyOneRow;

            // Does not ignore inconsistent records lengths
            this.CSVParser.IGNORE_RECORD_LENGTH = false;

            // Try find valid CSV in buffer with comma as separator.
            try {
                this.CSVParser.COLUMN_SEPARATOR = ',';
                batchParsedByComma = this.CSVParser.findValidCSVInBuffer(buffer, handle.finished);
            } catch (e) {
                parserError = e;
            }

            // If previous parsing failed try find valid CSV in buffer with semicolon separator.
            // Batch containing only one row or one column is considered as invalid/failed result. 
            // (note: the batch may be still used if CSV contains only one row, this will be determinated later)
            if (parserError || batchParsedByComma.rows.length <= 1 || batchParsedByComma.rows[0].length <= 1) {
                parserError = null;
                try {
                    this.CSVParser.COLUMN_SEPARATOR = ';';
                    batchParsedBySemicolon = this.CSVParser.findValidCSVInBuffer(buffer, handle.finished);
                } catch (e) {
                    parserError = e;
                }
            }

            // If there was error or both batches contains less than one row (or nothing) throw error. After this IF statement at least one result will have valid output.
            bothBatchesAreEmpty = !batchParsedByComma && !batchParsedBySemicolon;
            bothBatchesContainsOnlyOneRow = batchParsedByComma && batchParsedBySemicolon && (batchParsedByComma.rows.length <= 1) && (batchParsedBySemicolon.rows.length <= 1);
            if (parserError || bothBatchesAreEmpty || bothBatchesContainsOnlyOneRow) {
                throw 'om.contact.importcsv.badfiletypeorformat';
            }

            // If both results have valid output the one with more columns wins.
            if (batchParsedByComma && batchParsedBySemicolon) {
                if (batchParsedByComma.rows.length != batchParsedBySemicolon.rows.length) {
                    result = (batchParsedByComma.rows.length > batchParsedBySemicolon.rows.length) ? batchParsedByComma : batchParsedBySemicolon;
                } else {
                    result = (batchParsedByComma.rows[0].length > batchParsedBySemicolon.rows[0].length) ? batchParsedByComma : batchParsedBySemicolon;
                }
            } else {
                // One result is null and the other is filled. Filled result wins.
                result = batchParsedByComma || batchParsedBySemicolon;
            }

            this.CSVParser.COLUMN_SEPARATOR = result === batchParsedByComma ? ',' : ';';
            this.CSVParser.IGNORE_RECORD_LENGTH = defaultIgnoreRecordLenght;

            return result;
        }

        return [directive];
    });