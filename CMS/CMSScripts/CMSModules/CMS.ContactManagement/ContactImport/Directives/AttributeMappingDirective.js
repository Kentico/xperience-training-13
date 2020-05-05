cmsdefine(['Underscore'], function (_) {

    var Controller = function ($scope, messageHub, resolveFilter) {
        this.$scope = $scope;
        this.PREVIEW_ROW_COUNT = 5;
        this.messageHub = messageHub;
        this.getString = resolveFilter;
        $scope.resolveFilter = resolveFilter;

        if (!$scope.fileStream || !$scope.parsedLines || !$scope.sourceFileName) {
            return;
        }

        // Initialize contact field mapping that will be distributed to subdirectives to fill it
        this.$scope.processingContactFieldsMapping = angular.copy(this.$scope.contactFields) || [];

        this.processContactFieldsMapping();

        this.$scope.onMappingFinished = this.onMappingFinished.bind(this);
        this.$scope.csvColumnNamesWithExamples = this.getCSVColumnNamesWithExamples(this.$scope.parsedLines);
        this.parseSourceFileName();
    },
        directive = function () {
            return {
                restrict: 'A',
                templateUrl: 'attributeMappingDirectiveTemplate.html',
                controller: ['$scope', 'messageHub', 'resolveFilter', Controller]
            };
        };


    /**
     * Iterates the contact fields collection and set mappedIndex to -1 for every contact field.
     */
    Controller.prototype.processContactFieldsMapping = function () {
        // Initialize contact field mapping that will be distributed to subdirectives to fill it
        this.$scope.processingContactFieldsMapping.forEach(function (category) {
            category.categoryMembers.forEach(function (field) {
                field.mappedIndex = -1;
            });
        });
    };


    /**
     * Removes contact groups from contact fields mapping.
     */
    Controller.prototype.onMappingFinished = function () {
        var contactFieldsWithoutCategories = this.getFieldsWithoutCategories(this.$scope.processingContactFieldsMapping),
            contactFieldsMapping = this.filterContactFieldsMapping(contactFieldsWithoutCategories);

        // Removes all records from the message placeholder to ensure only one error message is displayed at time
        this.messageHub.publishClear();

        if (!this.checkMappingValidity(contactFieldsMapping)) {
            return;
        }

        if (!this.validateContactGroup()) {
            return;
        }

        this.$scope.$emit("mappingFinished", {
            mapping: contactFieldsMapping,
            contactGroup: this.$scope.contactGroup
        });
    };


    /**
     * Validates and repairs contact group object based on segmentation type.
     */
    Controller.prototype.validateContactGroup = function () {
        switch (this.$scope.segmentationType) {
            case 'new':
                if (!this.$scope.contactGroup.name) {
                    this.messageHub.publishError(this.getString('om.contact.importcsv.segmentation.nocontactgroupname'));
                    return false;
                }
                this.$scope.contactGroup.guid = null;
                break;

            case 'existing':
                if (!this.$scope.contactGroup.guid) {
                    this.messageHub.publishError(this.getString('om.contact.importcsv.segmentation.nocontactgroupselected'));
                    return false;
                }
                this.$scope.contactGroup.name = null;
                break;

            case 'none':
            default:
                this.$scope.contactGroup = {};
        }

        return true;
    };


    /**
     * Sets the new contact group name for the current date and file name.
     * Finds unique name based on contact groups in scope. Adds _x if not unique.
     */
    Controller.prototype.parseSourceFileName = function () {
        var fileName = this.$scope.sourceFileName.name,
            today = new Date().toISOString().substring(0, 10),
            indexOfDot,
            name,
            i = 1;

        indexOfDot = fileName.lastIndexOf('.');
        if (indexOfDot > 0) {
            fileName = fileName.substring(0, indexOfDot);
        }

        name = today + '-' + fileName;
        while (_.find(this.$scope.contactGroups, function (cg) { return cg.contactGroupDisplayName === name; })) {
            name = today + '-' + fileName + '_' + i;
            i++;
        }

        this.$scope.contactGroup.name = name;
    };


    /**
     * From the given array of parsed lines creates new object suitable for displaying column names with their examples.
     * @param  parsedLines  string[]  array of input lines
     * @return object                 object suitable for displaying column names and the examples
     */
    Controller.prototype.getCSVColumnNamesWithExamples = function (parsedLines) {
        return _.first(parsedLines).map(function (data, index) {
            return {
                ColumnIndex: index,
                ColumnName: data,
                ColumnExamples: _.rest(parsedLines).map(function (restData) {
                    return restData[index];
                })
            };
        });
    };


    /**
     * Flatten given contact fields and return them without categories.
     * @param   contactFieldsMappingWithCategories  contactFieldMapping[]  collection of contact field mappings to be flattened.
     * @return  contactFieldMapping[]                                      collection of contact field mappings without categories
     */
    Controller.prototype.getFieldsWithoutCategories = function (contactFieldsMappingWithCategories) {
        return _.flatten(
            _.pluck(contactFieldsMappingWithCategories, 'categoryMembers')
        );
    };


    /**
     * Checks whether given collection of mapped contact fields is valid and can be pass to the importer. If not, binds proper error message.
     * @param   contactFieldsMapping  contactFieldMapping[]  collection of contact field mappings to be checked.
     * @return  boolean                                      true, if collection is valid; otherwise, false.
     */
    Controller.prototype.checkMappingValidity = function (contactFieldsMapping) {
        if (!this.checkEmailIsMapped(contactFieldsMapping)) {
            this.messageHub.publishError(this.getString('om.contact.importcsv.noemailmapping'));
            return false;
        }
        return true;
    };


    /**
     * Checks whether given collection of mapped contact fields map also ContactEmail field. 
     * @param   contactFieldsMapping  contactFieldMapping[]  collection of contact field mappings to be checked.
     * @return  boolean                                      true, if collection contains ContactEmail field; otherwise, false.
     */
    Controller.prototype.checkEmailIsMapped = function (contactFieldsMapping) {
        if (contactFieldsMapping && contactFieldsMapping.length > 0) {
            return _.findWhere(contactFieldsMapping, { 'name': 'ContactEmail' });
        }
        return false;
    };


    /**
     * Checks whether each element in given collection has set mappedIndex. If not, the element is filtered out.
     * @param   contactFieldsMapping  contactFieldMapping[]  collection of contact field mappings to be checked.
     * @return  contactFieldMapping[]                        collection of filtered contact field mappings
     */
    Controller.prototype.filterContactFieldsMapping = function (contactFieldsMapping) {
        return contactFieldsMapping.filter(function (elem) {
            return elem.mappedIndex > -1;
        });
    };

    return [directive];
});