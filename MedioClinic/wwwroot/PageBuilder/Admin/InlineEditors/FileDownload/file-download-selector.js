(function () {
    window.kentico.pageBuilder.registerInlineEditor("file-download-selector", {
        init: function (options) {
            var editor = options.editor;
            var mediaLibraryName = editor.getAttribute("data-media-library-name");
            var fileGuid = editor.getAttribute("data-file-guid");
            var pickingDiv = editor.querySelector("div.mc-file-download-selector");
            var pickingButton = pickingDiv.querySelector("button");

            if (pickingButton) {
                pickingButton.addEventListener("click", function () {
                    if (!mediaLibraryName) {
                        showErrorMessage(pickingDiv,
                            options.localizationService.getString("MedioClinic.NoLibrarySpecified"));
                    } else {
                        var dialogOptions = createDialogOptions(pickingDiv);
                        window.kentico.modalDialog.contentSelector.open(dialogOptions);
                    }
                });
            }

            /**
             * Displays an error message
             * @param {HTMLElement} pickingDiv The <div> element to insert the message into.
             * @param {string} message Resource string key.
             */
            var showErrorMessage = function (pickingDiv, message) {
                var messageElement = document.createElement("div");
                messageElement.classList.add("mc-slideshow-message", "mc-slideshow-message-other");
                //messageElement.id = messageGuidPrefix + tempGuid;
                messageElement.innerText = message;
                pickingDiv.appendChild(messageElement);
            };

            /** Creates dialog options for the Xperience content selector
             * @param {HTMLElement} pickingDiv The link anchor to pick an image.
             * @returns {object} The dialog options object. */
            var createDialogOptions = function (pickingDiv) {
                var outputOptions = {
                    tabs: ["media"],
                    mediaOptions: {
                        libraryName: mediaLibraryName,
                    },
                    selectedItemsLimit: 1,
                    selectedItems: buildSelectedItems(options.propertyValue),
                    applyCallback: function (files) {
                        var newFile = files.items[0];
                        fileGuid = newFile.fileGuid;
                        dispatchBuilderEvent(fileGuid, false);

                        var guidElement = pickingDiv.querySelector("p.mc-file-download-guid");
                        guidElement.textContent = "File GUID: " + fileGuid;

                        return {
                            closeDialog: true
                        };
                    }
                };

                return outputOptions;
            };

            /**
             * Builds the 'selectedItems' contentSelector property.
             * @param {string[]} guids Image GUID identifiers.
             * @returns {object} The selectedItems object.
             */
            var buildSelectedItems = function (guids) {
                if (Array.isArray(guids)) {
                    var output = {
                        type: "media",
                        items: new Array()
                    };
                    guids.forEach(function (guid) {
                        var value = {
                            value: guid
                        }
                        output.items.push(value);
                    })
                } else {
                    return null;
                }
            };

            /**
             * Dispatches the Xperience page builder event that updates state of the widget in the browser store.
             * @param {string[]} imageGuids The GUIDs of the images in the Swiper object.
             * @param {boolean} refreshMarkup Indicates whether widget markup should be redrawn by the server upon event dispatch.
             */
            var dispatchBuilderEvent = function (fileGuid, refreshMarkup) {
                var customEvent = new CustomEvent("updateProperty", {
                    detail: {
                        name: options.propertyName,
                        value: fileGuid,
                        refreshMarkup: refreshMarkup
                    }
                });

                editor.dispatchEvent(customEvent);
            };
        },

        destroy: function (options) {
            var swiper = window.medioClinic
                .slideshowWidget
                .getCurrentSwiper(options.editor, window.medioClinic.slideshowWidget.swiperGuidAttribute);

            if (swiper) {
                window.medioClinic.slideshowWidget.removeSwiper(swiper.el.id);
                swiper.destroy();
            }
        }
    });
})();