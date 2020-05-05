cmsdefine(function () {
    return function () {
        var fileStream,
            parsedLines,
            sourceFileName,
            contactFieldsMapping,
            contactGroup,
            ContactImportService = {
                setFileStream: function (stream) {
                    fileStream = stream;
                },
                getFileStream: function () {
                    return fileStream;
                },
                setSourceFileName: function (name) {
                    sourceFileName = name;
                },
                getSourceFileName: function () {
                    return sourceFileName;
                },
                setParsedLines: function (lines) {
                    parsedLines = lines;
                },
                getParsedLines: function () {
                    return parsedLines;
                },
                setContactFieldsMapping: function (mapping) {
                    contactFieldsMapping = mapping;
                },
                getContactFieldsMapping: function () {
                    return contactFieldsMapping;
                },
                setContactGroup: function (group) {
                    contactGroup = group;
                },
                getContactGroup: function () {
                    return contactGroup;
                },
                clear: function() {
                    fileStream = null;
                    parsedLines = null;
                    sourceFileName = null;
                    contactFieldsMapping = null;
                    contactGroup = null;
                }
            };

        return ContactImportService;
    };
});