(function () {
    window.kentico.pageBuilder.registerInlineEditor("slideshow-editor", {
        init: function (options) {
            var imageGuidPrefix = "image-";
            var slideGuidPrefix = "slide-";
            var messageGuidPrefix = "message-";
            var editor = options.editor;
            var mediaLibraryName = editor.getAttribute("data-media-library-name");
            var allowedImageExtensions = editor.getAttribute("data-allowed-image-extensions");
            var plusButton = editor.parentElement.querySelector("ul.mc-slideshow-buttons .mc-swiper-plus");
            var minusButton = editor.parentElement.querySelector("ul.mc-slideshow-buttons .mc-swiper-minus");

            var swiper = window.medioClinic
                .slideshowWidget
                .getCurrentSwiper(editor, window.medioClinic.slideshowWidget.swiperGuidAttribute);

            // Image GUID retrieval: Alternative 1 (begin)
            /*var slideIds = window.medioClinic.slideshowWidget.collectImageIds(swiper);

            var imageGuids = slideIds.map(function (slideId) {
                return window.medioClinic.slideshowWidget.getGuidFromId(slideId);
            });*/
            // Image GUID retrieval: Alternative 1 (end)

            // Image GUID retrieval: Alternative 2 (begin)
            var imageGuids = editor.getAttribute("data-image-guids").split(";");
            imageGuids.splice(-1, 1);
            // Image GUID retrieval: Alternative 2 (end)

            /** Adds a new slide to the Swiper object. */
            var addSlide = function () {
                var tempGuid = generateUuid();

                var markup =
                    buildSlideMarkup(
                        tempGuid,
                        options.localizationService.getString("MedioClinic.InlineEditor.Slideshow.PickImage"));

                var slide = createNewSlide(swiper, imageGuids, tempGuid, markup);
                var pickingLink = slide.querySelector("div.mc-slideshow-message");

                pickingLink.addEventListener("click", function () {
                    if (!mediaLibraryName) {
                        showErrorMessage(slide, tempGuid,
                            options.localizationService.getString("MedioClinic.InlineEditor.Slideshow.NoLibrarySpecified"));
                    } else {
                        var dialogOptions = createDialogOptions(slide, pickingLink);
                        window.kentico.modalDialog.contentSelector.open(dialogOptions);
                    }
                });
            };

            /**
             * Displays an error message
             * @param {HTMLElement} slide The slide to insert the message into.
             * @param {string} tempGuid Temporary GUID assigned to a new image.
             * @param {string} message Resource string key.
             */
            var showErrorMessage = function (slide, tempGuid, message) {
                var messageElement = document.createElement("div");
                messageElement.classList.add("mc-slideshow-message", "mc-slideshow-message-other");
                messageElement.id = messageGuidPrefix + tempGuid;
                messageElement.innerText = message;
                slide.appendChild(messageElement);
            };

            /** Creates dialog options for the Xperience content selector
             * @param {HTMLElement} slide The slide to inser the new image into.
             * @param {HTMLElement} pickingLink The link anchor to pick an image.
             * @returns {object} The dialog options object. */
            var createDialogOptions = function (slide, pickingLink) {
                var outputOptions = {
                    tabs: ["media"],
                    mediaOptions: {
                        libraryName: mediaLibraryName,
                        allowedExtensions: allowedImageExtensions,
                    },
                    selectedItemsLimit: 1,
                    selectedItems: buildSelectedItems(options.propertyValue),
                    applyCallback: function (files) {
                        var newFile = files.items[0];
                        var childElementIndex = getChildElementIndex(pickingLink.parentElement);
                        imageGuids.splice(childElementIndex, 1, newFile.fileGuid);

                        // Image drawing: Client side (begin)
                        dispatchBuilderEvent(imageGuids, false);
                        // Image drawing: Client side (end)

                        // Image drawing: Server side (begin)
                        /* dispatchBuilderEvent(imageGuids, true); */
                        // Image drawing: Server side (end)

                        // Image drawing: Client side (begin)
                        var imgElement = document.createElement("img");
                        imgElement.classList.add("responsive-img");
                        imgElement.src = newFile.url;
                        imgElement.id = imageGuidPrefix + newFile.fileGuid;
                        slide.appendChild(imgElement);
                        swiper.updateSize();
                        replaceId(slide, slideGuidPrefix + newFile.fileGuid);
                        pickingLink.remove();
                        // Image drawing: Client side (end)

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
            }

            /**
             * Creates a new Swiper slide.
             * @param {object} swiper The Swiper object.
             * @param {string[]} imageGuids GUIDs of images in slides.
             * @param {string} tempGuid Temporary GUID assigned to a new image.
             * @param {string} markup HTML markup of the new slide.
             * @returns {HTMLElement} The new slide.
             */
            var createNewSlide = function (swiper, imageGuids, tempGuid, markup) {
                var activeIndexWhenAdded = swiper.slides.length > 0 ? swiper.activeIndex + 1 : 0;
                imageGuids.splice(activeIndexWhenAdded, 0, tempGuid);
                swiper.addSlide(activeIndexWhenAdded, markup);
                swiper.slideNext();
                var slide = editor.parentElement.querySelector("#" + slideGuidPrefix + tempGuid);

                return slide;
            };

            /**
             * Gets the position (index) of a given HTML element in the parent Swiper element.
             * @param {HTMLElement} childElement The HTML element of the child object.
             * @returns {number} The position in the parent Swiper.
             */
            var getChildElementIndex = function (childElement) {
                return Array.prototype.slice.call(childElement.parentElement.children)
                    .indexOf(childElement);
            };

            /**
             * Crafts an HTML markup of a new Swiper slide.
             * @param {string} id The ID of the future HTML element of the slide.
             * @param {string} text The instructional text for the new slide.
             * @returns {string} The complete HTML markup of the Swiper slide.
             */
            var buildSlideMarkup = function (id, text) {
                return "<div class=\"swiper-slide\" id=\""
                    + slideGuidPrefix + id
                    + "\"><div class=\"mc-slideshow-message mc-slideshow-message-first\"><a>"
                    + text + "</a></div></div>";
            };

            /** 
             *  Generates an UUID (GUID).
             *  @returns {string} The UUID.
             * */
            var generateUuid = function () {
                return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
                    var r = Math.random() * 16 | 0, v = c === "x" ? r : r & 0x3 | 0x8;

                    return v.toString(16);
                });
            };

            /**
             * Replaces an ID of a given HTML element.
             * @param {HTMLElement} htmlElement The HTML element, which ID should be swapped.
             * @param {string} newId The new ID.
             */
            var replaceId = function (htmlElement, newId) {
                htmlElement.id = newId;
            };

            /**
             * Dispatches the Xperience page builder event that updates state of the widget in the browser store.
             * @param {string[]} imageGuids The GUIDs of the images in the Swiper object.
             * @param {boolean} refreshMarkup Indicates whether widget markup should be redrawn by the server upon event dispatch.
             */
            var dispatchBuilderEvent = function (imageGuids, refreshMarkup) {
                var customEvent = new CustomEvent("updateProperty", {
                    detail: {
                        name: options.propertyName,
                        value: imageGuids,
                        refreshMarkup: refreshMarkup
                    }
                });

                editor.dispatchEvent(customEvent);
            };

            /** Removes a slide from the current Swiper object. */
            var removeSlide = function () {
                var slideChildElement = swiper.slides[swiper.activeIndex];

                if (slideChildElement) {
                    var childElementIndex = getChildElementIndex(slideChildElement);
                    imageGuids.splice(childElementIndex, 1);
                    swiper.removeSlide(swiper.activeIndex);

                    // Image drawing: Client side (begin)
                    dispatchBuilderEvent(imageGuids, false);
                    // Image drawing: Client side (end)

                    // Image drawing: Server side (begin)
                    /* dispatchBuilderEvent(imageGuids, true); */
                    // Image drawing: Client side (end)
                }
            };

            plusButton.addEventListener("click", addSlide);
            minusButton.addEventListener("click", removeSlide);
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


