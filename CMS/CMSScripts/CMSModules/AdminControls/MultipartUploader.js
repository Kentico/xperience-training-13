cmsdefine([], function () {

    var MultipartUploader = function () {

        /**
        * Represents information about currently uploaded file.
        * @constructor
        * @param {uploadSessionID} Unique identifier for one multipart upload. 
        *                          It's acquired after uploading first part of a file from server.
        * @param {partNumber} Number of the next part of a file that will be uploaded.
        */
        var multipartData = function (uploadSessionID, partNumber) {
            this.partIdentifiers = [];
            this.uploadSessionID = uploadSessionID || "";
            this.partNumber = partNumber || 0;
        };

        // Saves information about currently uploaded file.
        var clientSideMultipartUploadData = new multipartData();


        /** 
        * Handles response after uploading one part of a file.
        * If data from server can not be parsed then the upload of one whole file is finished.
        * @param  {responseText} Response from server after uploading one part.
        */
        this.handleResponseAfterUploadingOnePart = function (responseText) {
            try {
                var obj = JSON.parse(responseText);
            } catch (e) {
                obj = null;
                clientSideMultipartUploadData = new multipartData();
            }

            if (obj) {
                [].push.apply(clientSideMultipartUploadData.partIdentifiers, obj.partIdentifiers);
                clientSideMultipartUploadData.partNumber = obj.partNumber;
                clientSideMultipartUploadData.uploadSessionID = obj.uploadSessionID;
            }
        };


        /** 
        * Returns true if the size of the maximal possible chunk that uploader can 
        * send from client to server is bigger or equal to the whole size of a file.
        * @param  {chunkSize} Size of the maximal possible chunk that uploader can send from client to server.
        * @param  {currentFileSize} Size of the currently uploaded file.
        */
        this.isFileUploadedInOneChunk = function (chunkSize, currentFileSize) {
            return chunkSize >= currentFileSize;
        };

        
        /** 
        * Sends chunk of a file to server.
        * @param  {xhr} Prepared XMLHttpRequest for uploading chunk of a file.
        * @param  {chunk} Data to be sent.
        * @param  {isLastChunk} If currently uploaded chunk is the last part of a file.
        */
        this.sendDataViaMultipartUpload = function (xhr, chunk, isLastChunk) {
            var serverSideMultipartData = new multipartData(clientSideMultipartUploadData.uploadSessionID, clientSideMultipartUploadData.partNumber);

            if (isLastChunk === true) {
                serverSideMultipartData.partIdentifiers = clientSideMultipartUploadData.partIdentifiers;
            }

            // Prepare FormData for multipart/form-data request
            var formData = new FormData();
            formData.append("multipartData", JSON.stringify(serverSideMultipartData));
            formData.append("file", chunk);

            // Sends the chunk inside multipart/form-data request
            xhr.send(formData);
        }
    };

    return MultipartUploader;
});