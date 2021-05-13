(function () {
    window.kentico.pageBuilder.registerInlineEditor("image-uploader", {
        init: function (options) {
            var editor = options.editor;
            var zone = editor.querySelector(".dz-uploader");
            var clickable = editor.querySelector(".dz-clickable");

            var dropzone = new Dropzone(zone, {
                acceptedFiles: editor.getAttribute("data-allowed-image-extensions"),
                maxFiles: 1,
                url: editor.getAttribute("data-upload-url"),
                createImageThumbnails: false,
                clickable: clickable,
                dictInvalidFileType: options.localizationService.getString(
                    "InlineEditor.ImageUploader.Dropzone.InvalidFileType")
            });

            /**
             * Handles error codes
             * @param {number} statusCode HTTP status code
             * @param {string} statusText HTTP status text
             */
            var processErrors = function (statusCode, statusText) {
                var errorFlag = "error";
                var errorMessage = "";

                if (statusCode >= 500) {
                    errorMessage = options.localizationService.getString("InlineEditor.ImageUploader.Dropzone.UploadFailed");
                } else if (statusCode === 401) {
                    errorMessage = options.localizationService.getString("InlineEditor.ImageUploader.Dropzone.Unauthorized");
                } else if (statusCode === 403) {
                    errorMessage = options.localizationService.getString("InlineEditor.ImageUploader.Dropzone.Forbidden");
                } else if (statusCode === 422) {
                    errorMessage = options.localizationService.getString("InlineEditor.ImageUploader.Dropzone.UploadUnprocessable");
                } else {
                    errorMessage = options.localizationService.getString("InlineEditor.ImageUploader.Dropzone.UploadUnknownError");
                }

                errorMessage += " " + options.localizationService.getString("InlineEditor.ImageUploader.Dropzone.Details") + " " + statusText;
                window.medioClinic.showMessage(errorMessage, errorFlag, true);
            };

            dropzone.on("success",
                function (event) {
                    var content = JSON.parse(event.xhr.response);

                    var customEvent = new CustomEvent("updateProperty",
                        {
                            detail: {
                                value: content.guid,
                                name: options.propertyName
                            }
                        });

                    editor.dispatchEvent(customEvent);
                });

            dropzone.on("error",
                function (event) {
                    document.querySelector(".dz-preview").style.display = "none";
                    processErrors(event.xhr.status, event.xhr.responseText);
                });
        },

        destroy: function (options) {
            var dropzone = options.editor.querySelector(".dz-uploader").dropzone;

            if (dropzone) {
                dropzone.destroy();
            }
        }
    });
})();
