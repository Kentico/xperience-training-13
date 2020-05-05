cmsdefine(['Underscore', 'CMS.ContactManagement/ContactImport/CSVParser', 'CMS/Messages/MessageHub'], function (_, CSVParser, MessageHub) {

    var Controller = function ($scope, $timeout, $q, cmsContactResource, cmsContactGroupResource, serverData, messageHub, resolveFilter) {
        var that = this;

        this.messageHub = new MessageHub();
        this.$scope = $scope;
        this.$timeout = $timeout;
        this.$q = $q;
        this.parser = CSVParser;
        this.messageHub = messageHub;
        this.getString = resolveFilter;

        if (!$scope.contactFieldsMapping || !$scope.contactGroup || !$scope.fileStream) {
            return;
        }

        this.$scope.result = {
            imported: 0,
            duplicities: 0,
            failures: 0,
            processed: 0,
            csvData: false,
        };

        this.$scope.isImporting = false;
        this.$scope.numberOfAllRecords = 0;
        this.$scope.finished = false;
        this.contactResource = cmsContactResource;
        this.contactGroupResource = cmsContactGroupResource;

        this.$scope.continueToSmartTip = serverData.smartTips['tipContinueTo'];
        this.$scope.continueToSmartTip.selector = '#tipContinueTo';
        

        this.$scope.importedTotalCount = 0;

        // If contact mapping is defined, run the importing process.
        if ($scope.contactFieldsMapping) {
            this.$scope.$emit('importStarted');
            this.$scope.isImporting = true;
            this.$scope.unfinishedRequests = 0;

            if (this.$scope.contactGroup.name) {
                this.createContactGroup(this.$scope.contactGroup.name, function (contactGroupGuid) {
                    that.importContacts(that.$scope.contactFieldsMapping, contactGroupGuid, that.$scope.fileStream);
                });
            } else {
                this.importContacts(this.$scope.contactFieldsMapping, this.$scope.contactGroup.guid, this.$scope.fileStream);
            }
        }

        messageHub.subscribeToError(function () {
            that._importFinished();
        });
    },
        directive = function () {
            return {
                restrict: 'A',
                controller: ['$scope', '$timeout', '$q', 'cmsContactResource', 'cmsContactGroupResource', 'serverData', 'messageHub', 'resolveFilter', Controller],
                templateUrl: 'importProcessDirectiveTemplate.html'
            };
        };


    /**
     * Parse contacts according to the given mappings and send them to the server.
     * @param  contactFieldsMapping  object[]      collection of contact fields mapping set by the user
     * @param  contactGroupGuid      string        contact group guid
     * @param  fileStream            FileStreamer  instance of FileStreamer
     */
    Controller.prototype.importContacts = function (contactFieldsMapping, contactGroupGuid, fileStream) {
        var that = this,
            names = contactFieldsMapping.map(function (elem) { return elem.name; }),
            selectedIndexes = contactFieldsMapping.map(function (elem) { return elem.mappedIndex; }),
            skipFirstRow = true,
            columnsCount = 0;

        fileStream.read(function (buffer, handle) {
            var result,
                filtered;

            that.handle = handle;
            try {
                result = that.parser.findValidCSVInBuffer(buffer, handle.finished);

                handle.move(-result.remainder);

                if (skipFirstRow) {
                    columnsCount = result.rows[0].length;
                    result.rows.shift();
                }
                skipFirstRow = false;

                result.rows = that.parser.filterEmptyRecords(result.rows);
                that.parser.fixLineColumnsCount(result.rows, columnsCount);
                filtered = that.filterColumns(result.rows, selectedIndexes);

                that.sendBulkToServer(names, contactGroupGuid, filtered);
                that.$scope.numberOfAllRecords += filtered.length;
            }
            catch (e) {
                that.$scope.finished = true;
                that.messageHub.publishError(that.getString('om.contact.importcsv.importerror') + ' ' + that.getString('om.contact.importcsv.invalidcsv'));
                that.$scope.$apply();
            }
        });
    };


    /**
     * Creates contact group by calling remote call.
     * @param name      string                              Contact group name
     * @param callback  function(string contactGroupGuid)   Callback to invoke if contact group was successfully created 
     */
    Controller.prototype.createContactGroup = function (name, callback) {
        var promise = this.contactGroupResource.create({
            contactGroupName: name,
        }, {}).$promise;

        promise.then(function (data) {
            if (!data) {
                return;
            }
            callback(data.ContactGroupGuid);
        });
    };


    /**
     * Filters out all columns from the given input which index is not present in the given included columns array.
     * @param  input            string[[]]  input to be modified
     * @param  includedColumns  int[]    array of indexes which should be included from the parsed results
     * @return                  input without filtered columns
     */
    Controller.prototype.filterColumns = function (input, includedColumns) {
        return input.map(function (item) {
            var resultItem = [];
            includedColumns.forEach(function (index) {
                resultItem.push(item[index]);
            });
            return resultItem;
        });
    };


    /**
     * Called when the request is finished.
     */
    Controller.prototype.onRequestFinished = function () {
        if (this.handle) {
            if (this.handle.finished) {
                this._importFinished();
            }
            this.handle.continue();
        }
    };


    Controller.prototype._importFinished = function () {
        this.$scope.finished = true;
        this.$scope.$emit('importFinished');
    }


    /**
     * Performs sending of the contacts to the server.
     * @param  fieldNames       string[]         array of field names obtained from the CSV header
     * @param  contactGroupGuid string           contact group guid
     * @param  contactLines     array[string[]]  array of arrays containing the data itself
     */
    Controller.prototype.sendBulkToServer = function (fieldNames, contactGroupGuid, contactLines) {
        var that = this,
            importPromise = this.getImportPromise(fieldNames, contactGroupGuid, contactLines);

        importPromise.then(function (data) {
            that.onImportBulkSuccess(data, contactLines.length);
        });
    };


    /**
     * Creates new import request to the server and returns the resource promise.
     * @param  fieldNames        string[]        array of field names obtained from the CSV header
     * @param  contactGroupGuid  string          contact group guid
     * @param  contactLines      array[string[]] array of arrays containing the data itself
     * @return                                   promise of the import request
     */
    Controller.prototype.getImportPromise = function (fieldNames, contactGroupGuid, contactLines) {
        return this.contactResource.import({
            contactGroupGuid: contactGroupGuid,
        }, {
            fieldsOrder: fieldNames,
            fieldValues: contactLines,
        }).$promise;
    };


    /**
     * Import request success handler. Sets number of results to the UI.
     * @param  data   object  result object returned from the server
     */
    Controller.prototype.onImportBulkSuccess = function (data, processed) {
        var that = this;

        if (!data) {
            return;
        }

        this.$timeout(function () {
            var notImported,
                includeHeader,
                rows;

            that.$scope.result.processed += processed;
            that.$scope.result.imported += data.imported;
            that.$scope.result.duplicities += data.duplicities;
            that.$scope.result.failures += data.failures;

            notImported = data.notImportedContacts;

            if (notImported && notImported.messages && notImported.messages.length > 0) {
                includeHeader = !that.$scope.result.csvData;
                that.$scope.result.csvData = that.$scope.result.csvData || '';

                // Append message to each row (contact)
                rows = _.map(notImported.fieldValues, function (row, i) { return row.concat([notImported.messages[i]]); });

                // Prepend header
                if (includeHeader) {
                    rows = [notImported.fieldsOrder.concat(['Explanation'])].concat(rows);
                }

                that.$scope.result.csvData += (includeHeader ? '' : '\r\n') + that._createCsv(rows);
            }

            that.onRequestFinished();
        });
    };


    Controller.prototype._createCsv = function (rows) {
        var escape = function (value) { return '"' + value.replace(/"/g, '""') + '"'; },
            result = [],
            i;

        for (i = 0; i < rows.length; i += 1) {
            result.push(_.map(rows[i], escape).join(','));
        }
        return result.join('\r\n');
    };

    return [directive];
});