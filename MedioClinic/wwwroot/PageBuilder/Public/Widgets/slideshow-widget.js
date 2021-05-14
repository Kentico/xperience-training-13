window.medioClinic = window.medioClinic || {};

(function (slideshowWidget) {
    var swipers = [];

    /** The name of the data- HTML attribute that holds the Swiper GUID. */
    slideshowWidget.swiperGuidAttribute = "data-swiper-guid";

    /**
     * Adds a Swiper object to an internal dictionary.
     * @param {string} id The ID of the Swiper object.
     * @param {object} swiper The Swiper object to add.
     */
    slideshowWidget.addSwiper = function (id, swiper) {
        var found = window.medioClinic.slideshowWidget.getSwiper(id);

        if (!found) {
            var swiperToAdd = {
                id: id,
                swiper: swiper
            };

            swipers.push(swiperToAdd);
        }
    };

    /**
     * Retrieves a Swiper object from an internal dictionary by its ID.
     * @param {string} id The ID to search by.
     * @returns {object} Either the found Swiper object, or null.
     */
    slideshowWidget.getSwiper = function (id) {
        var found = swipers.filter(function (currentSwiper) {
            return currentSwiper.id === id;
        });

        if (found.length > 0) {
            return found[0];
        } else {
            return null;
        }
    };

    /**
     * Removes a Swiper object form an internal dictionary.
     * @param {string} id The ID to search by.
     */
    slideshowWidget.removeSwiper = function (id) {
        for (var i = swipers.length - 1; i >= 0; i--) {
            if (swipers[i].id === id) {
                swipers.splice(i, 1);
            }
        }
    };

    /**
     * Initializes a new Swiper object in the page.
     * @param {string} swiperId The ID of the future Swiper object.
     * @param {bool} editMode Indication of whether the Xperience page builder is in edit mode.
     * @param {number} transitionDelay An interval of the transition to another slide (milliseconds).
     * @param {any} transitionSpeed The duration of each transition (milliseconds).
     */
    slideshowWidget.initSwiper = function (swiperId, editMode, transitionDelay, transitionSpeed) {
        var swiperSelector = "#" + swiperId;

        var configuration = {
            loop: !editMode,
            speed: transitionSpeed,
            navigation: {
                nextEl: "#" + swiperId + " .swiper-button-next",
                prevEl: "#" + swiperId + " .swiper-button-prev"
            },
            effect: "fade",
            fadeEffect: {
                crossFade: true
            },
            autoHeight: true
        };

        if (!editMode) {
            configuration["autoplay"] = {
                delay: transitionDelay,
                disableOnInteraction: true
            };
        }

        var swiper = new Swiper(swiperSelector, configuration);
        window.medioClinic.slideshowWidget.addSwiper(swiperId, swiper);
    };

    /**
     * Gets a Swiper object for a given slideshow inline editor instance.
     * @param {HTMLElement} editor The HTML element of the inline editor.
     * @param {string} swiperGuidAttribute The name of the data- HTML attribute containing the Swiper ID.
     * @returns {object} The Swiper object.
     */
    slideshowWidget.getCurrentSwiper = function (editor, swiperGuidAttribute) {
        // Retrieving via the "swiper" property of the respective HTML element
        //return editor.parentElement.swiper;

        // Retrieving off of the global namespace container
        return window.medioClinic.slideshowWidget.getSwiper(editor.getAttribute(swiperGuidAttribute)).swiper;
    };

    /**
     * Removes any prefixes that had been previously concatedated in front of a GUID.
     * @param {string} id The GUID with the prefix.
     * @returns {string} The bare GUID value.
     */
    slideshowWidget.getGuidFromId = function (id) {
        return id.slice(-36);
    };

    /**
     * Gets an array of Dropzone HTML element IDs of a given Swiper object.
     * @param {object} swiper The parent Swiper object.
     * @returns {string[]} The array of Dropzone IDs.
     */
    slideshowWidget.collectImageIds = function (swiper) {
        var output = [];

        for (var s = 0; s <= swiper.slides.length - 1; s++) {
            var childElement = swiper.slides[s].children[0];
            output.push(childElement.id);
        }

        return output;
    };
}(window.medioClinic.slideshowWidget = window.medioClinic.slideshowWidget || {}));
