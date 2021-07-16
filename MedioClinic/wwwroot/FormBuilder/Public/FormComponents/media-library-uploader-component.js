window.medioClinic = window.medioClinic || {};

(function (mediaLibraryUploaderFormComponent) {
    /**
     * Displays file size and MIME type of the selected file.
     * @param {HTMLElement} target HTML element that invoked the function.
     */
    mediaLibraryUploaderFormComponent.onFileSelected = function (target) {
        var mbSize = 1048576;
        var file = target.files[0];
        var fileSizeString = "";
        var fileMimeTypeString = "";
        var detailsElement = target.parentElement.parentElement.parentElement.parentElement.querySelector(".mc-upload-file-details");

        var uploadButton =
            target.parentElement.parentElement.parentElement.parentElement.parentElement.querySelector(".upload-button button");

        if (file) {
            if (file.type === "image/jpeg" || file.type === "image/png" || file.type === "image/gif") {
                var fileSize = 0;
                uploadButton.disabled = false;

                if (file.size > mbSize) {
                    fileSize = (Math.round(file.size * 100 / mbSize) / 100).toString() + 'MB';
                } else {
                    fileSize = (Math.round(file.size * 100 / 1024) / 100).toString() + 'kB';
                }

                fileSizeString = fileSize.toString();
                fileMimeTypeString = file.type.toString();
            } else {
                fileMimeTypeString = null;
            }
        }

        detailsElement.querySelector(".mc-file-size").innerHTML =
            "<strong>Size:</strong> "
            + fileSizeString;

        var fileTypeElement = detailsElement.querySelector(".mc-file-type");

        if (fileMimeTypeString) {
            fileTypeElement.innerHTML =
                "<strong>Type:</strong> "
                + fileMimeTypeString;
        } else {
            fileTypeElement.innerHTML
                = "<strong>Type:</strong> Invalid file type. Please upload a .jpg or .png file.";
            uploadButton.disabled = true;
        }
    };

    /**
     * Makes sure the user is warned about the need to re-upload a file in the form.
     * @param {bool} isFileRequired Indicates whether the form field is required.
     * @param {string} fileGuidHiddenElementId ID of the hidden element with file GUID.
     */
    mediaLibraryUploaderFormComponent.checkForUnuploadedFile = function (isFileRequired, fileGuidHiddenElementId) {
        var fileGuidHiddenElement = document.getElementById(fileGuidHiddenElementId);

        if (fileGuidHiddenElement) {
            var parentForm = getParentForm(fileGuidHiddenElement, null);

            if (parentForm) {
                var submitButton = getFormSubmitButton(parentForm);

                if (isFileRequired && !fileGuidHiddenElement.value) {
                    submitButton.disabled = true;
                } else {
                    submitButton.disabled = false;
                }
            }
        }
    }

    /**
     * Finds a submit button inside an element.
     * @param {HTMLElement} element The element to search through.
     * @returns {HTMLElement} The button element.
     */
    var getFormSubmitButton = function (element) {
        var childInputElements = element.getElementsByTagName("input");
        var foundSubmitButton = null;

        for (var i = 0; i < childInputElements.length - 1; i++) {
            if (childInputElements[i].type === "submit") {
                foundSubmitButton = childInputElements[i];

                break;
            }
        }

        return foundSubmitButton;
    }

    /**
     * Uploads the selected file using XHR.
     * @param {HTMLElement} target HTML element that invoked the function.
     * @param {string} url URL to upload the file to.
     */
    mediaLibraryUploaderFormComponent.uploadFile = function (target, url, isFileRequired, fileGuidHiddenElementId) {
        if (url && url.length > 0) {
            var xhr = new XMLHttpRequest();
            var parentForm = getParentForm(target, null);
            var formData = new FormData(parentForm);
            xhr.addEventListener("load", onUploadCompleted.bind(null, xhr, isFileRequired, fileGuidHiddenElementId), false);
            xhr.addEventListener("progress", onUploadProgressChange, false);
            xhr.addEventListener("error", onUploadFailed.bind(null, xhr, isFileRequired, fileGuidHiddenElementId), false);
            xhr.open("POST", url);
            xhr.send(formData);
        }
    };

    /**
     * Searches for nearest parent form HTML element.
     * @param {HTMLElement} target HTML element that invoked the function.
     * @param {HTMLElement=} mostParentElement An optional HTML element where searching should stop.
     * @returns {HTMLElement} The parent form element, or null.
     */
    var getParentForm = function (target, mostParentElement) {
        if (!mostParentElement) {
            mostParentElement = document.getElementsByTagName("body")[0];
        }

        if (target !== mostParentElement) {
            var parent = target.parentElement;

            if (parent.tagName === "FORM") {
                return parent;
            } else {
                return getParentForm(parent, mostParentElement);
            }
        } else {
            return null;
        }
    };

    /**
     * Handles error codes
     * @param {number} statusCode HTTP status code.
     * @param {string} errorMessage Error message.
     * @param {HTMLElement} [targetElement] The HTML element containing the form message element.
     */
    var processErrors = function (statusCode, errorMessage, targetElement) {
        var errorFlag = "error";
        var completeMessage = "HTTP error " + statusCode + ". " + errorMessage;

        processMessage(completeMessage, errorFlag, targetElement);
    };

    /**
     * Logs a console message and displays it in the page, if possible.
     * @param {string} message The message.
     * @param {string} type The type of the message.
     * @param {HTMLElement} [targetElement] The HTML element containing the form message element.
     */
    var processMessage = function (message, type, targetElement) {
        var cssClasses = "";
        var isNamespaceAvailable = typeof window.medioClinic.showMessage === "function";

        if (isNamespaceAvailable) {
            window.medioClinic.showMessage(message, type, false);
        }

        if (type === "info") {
            cssClasses = "light-blue lighten-5";
            console.info(message);
        } else if (type === "warning") {
            cssClasses = "yellow lighten-3";
            console.warn(message);
        } else if (type === "error") {
            cssClasses = "red lighten-3";
            console.error(message);
        }

        if (targetElement && isNamespaceAvailable) {
            var messageElement = targetElement.querySelector(".mc-form-messages");
            messageElement.appendChild(window.medioClinic.buildMessageMarkup(message, cssClasses));
        }
    };

    /**
     * Gets the media library file GUID and puts its value into another form input element.
     * @param {Event} e Event invoked when an upload is complete.
     */
    var onUploadCompleted = function (xhr, isFileRequired, fileGuidHiddenElementId) {
        var responseObject = JSON.parse(xhr.response);
        var fileInputElement = document.getElementById(responseObject.fileInputElementId);
        fileInputElement.value = responseObject.fileGuid;
        var detailsElement = fileInputElement.parentElement;

        if (xhr.status >= 200 && xhr.status < 300) {
            var message = "Upload of the image is complete. File GUID: " + responseObject.fileGuid;
            processMessage(message, "info", detailsElement);
        } else {
            processErrors(xhr.status, responseObject.error, detailsElement);
        }

        window.medioClinic.mediaLibraryUploaderFormComponent.checkForUnuploadedFile(isFileRequired, fileGuidHiddenElementId);
    };

    /**
     * Logs the file upload progress into the console.
     * @param {Event} e Event invoked when the upload progress changes.
     */
    var onUploadProgressChange = function (e) {
        var percentComplete = Math.round(e.loaded * 100 / e.total);
        console.info(
            "Upload progress: "
            + percentComplete + "%");
    };

    /**
     * Logs the failed upload to the console.
     * @param {Event} e Event invoked when the upload fails.
     */
    var onUploadFailed = function (xhr, isFileRequired, fileGuidHiddenElementId) {
        processErrors(xhr.status, null, null);

        window.medioClinic.mediaLibraryUploaderFormComponent.checkForUnuploadedFile(isFileRequired, fileGuidHiddenElementId);
    };
}(window.medioClinic.mediaLibraryUploaderFormComponent = window.medioClinic.mediaLibraryUploaderFormComponent || {}));