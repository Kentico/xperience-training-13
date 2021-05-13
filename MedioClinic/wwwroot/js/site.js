// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    var sidenavElements = document.querySelectorAll(".sidenav");
    M.Sidenav.init(sidenavElements);
    var dropdownElements = document.querySelectorAll(".dropdown-trigger");

    M.Dropdown.init(dropdownElements, {
        hover: false
    });

    var selectElements = document.querySelectorAll("select");
    M.FormSelect.init(selectElements);
    var datepickerElements = document.querySelectorAll("datepicker");
    M.Datepicker.init(datepickerElements);
});

(function (medioClinic) {
    /**
     * Shows a system message to the user via the ".kn-system-messages" element.
     * @param {string} message The system message.
     * @param {string} type Either "info", "warning", or "error".
     * @param {bool} logToConsole Instructs to also log to console.
     */
    medioClinic.showMessage = function (message, type, logToConsole) {
        var messageElement = document.querySelector(".mc-system-messages");

        if (message && type) {
            if (type === "info") {
                messageElement.appendChild(medioClinic.buildMessageMarkup(message, "light-blue lighten-5"));

                if (logToConsole) {
                    console.info(message);
                }
            } else if (type === "warning") {
                messageElement.appendChild(medioClinic.buildMessageMarkup(message, "yellow lighten-4"));

                if (logToConsole) {
                    console.warn(message);
                }
            } else if (type === "error") {
                messageElement.appendChild(medioClinic.buildMessageMarkup(message, "deep-orange lighten-2"));

                if (logToConsole) {
                    console.error(message);
                }
            }
        }
    };

    /**
     * Builds an HTML element of a system message.
     * @param {string} message The system message.
     * @param {string} cssClasses The CSS class selectors.
     * @returns {HTMLElement} The <p> element.
     */
    medioClinic.buildMessageMarkup = function (message, cssClasses) {
        var paragraph = document.createElement("p");
        paragraph.classList = "mc-user-message " + cssClasses;
        paragraph.innerText = message;
        var div = document.createElement("div");
        div.classList = "col xl5 l9 m12 s12";
        div.appendChild(paragraph);
        var section = document.createElement("section");
        section.classList = "row section";
        section.appendChild(div);

        return section;
    };
}(window.medioClinic = window.medioClinic || {}));