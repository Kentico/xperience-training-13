window.medioClinic = window.medioClinic || {};

(function (newsletterSubscriptionWidget) {
    /**
     * Subscribes to a newsletter and displays a "thank you" message.
     * @param {string} url URL of the communications controller.
     * @param {string} newsletterGuid GUID of a newsletter to subscribe to.
     * @param {string} contactGuid GUID of the current contact.
     * @param {string} contactEmail Email address of the current contact.
     * @param {string} elementId GUID used in element IDs of the current widget instance.
     */
    newsletterSubscriptionWidget.subscribe = function (url, newsletterGuid, contactGuid, contactEmail, elementId) {
        var formElement = document.getElementById("newsletter-form-" + elementId);
        var emailInput = document.getElementById("email-" + elementId);
        var firstNameInput = document.getElementById("first-name-" + elementId);
        var lastNameInput = document.getElementById("last-name-" + elementId);
        var consentCheckbox = document.getElementById("consent-checkbox-" + elementId);
        var thankyouElement = document.getElementById("newsletter-thankyou-" + elementId);
        var errorMessages = [];

        if (!contactGuid) {
            errorMessages.push("You haven't agreed to using analytical cookies yet. Please do so in the blue bar in the bottom of the page.")
        }

        if (!contactEmail && emailInput && !emailInput.value || emailInput && emailInput.value && !isEmailValid(emailInput.value)) {
            errorMessages.push("You haven't provided a valid email address and we couldn't determine it from our records. Please enter your email address.");
        }

        if (consentCheckbox && !consentCheckbox.checked) {
            errorMessages.push("You haven't agreed to receiving newsletters. Please select the checkbox.");
        }

        if (errorMessages.length > 0) {
            messageText = errorMessages.join("\n");
            showMessage(messageText, formElement);
        } else {
            // Send the POST request.
            var xhr = new XMLHttpRequest();
            var formData = new FormData();
            formData.append("NewsletterGuid", newsletterGuid);
            formData.append("ContactGuid", contactGuid);

            if (emailInput) {
                formData.append("Email", emailInput.value);
            }

            if (firstNameInput && lastNameInput) {
                formData.append("FirstName", firstNameInput.value);
                formData.append("LastName", lastNameInput.value);
            }

            xhr.addEventListener("load", onLoad.bind(null, xhr, formElement, thankyouElement), false);
            xhr.addEventListener("error", onError.bind(null, xhr, formElement), false);
            xhr.open("POST", url);
            xhr.send(formData);
        }

        return true;
    };

    /**
     * Validates an email address.
     * @param {string} emailAddress An email address to validate.
    */
    var isEmailValid = function (emailAddress) {
        var pattern = /^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;

        return emailAddress && String(emailAddress).toLowerCase().match(pattern);
    }

    /**
     * On success, hide the form element and reveal the "thank you" element.
     * @param {XMLHttpRequest} xhr XMLHttpRequest.
     * @param {HTMLElement} formElement Form element.
     * @param {HTMLElement} thankyouElement Thank you element.
    */
    var onLoad = function (xhr, formElement, thankyouElement) {
        if (formElement && thankyouElement) {
            if (xhr.status !== 200) {
                showMessage(xhr.responseText, formElement);
            } else {
                formElement.style.display = "none";
                thankyouElement.style.display = "block";
            }
        }
    };

    /**
     * On failure, fill formElementId.innerHtml with error message.
     * @param {XMLHttpRequest} xhr XMLHttpRequest.
     * @param {HTMLElement} formElement Form element.
    */
    var onError = function (xhr, formElement) {
        if (xhr && formElement) {
            var responseObject = JSON.parse(xhr.response);
            showMessage(responseObject.error, formElement);
        }
    };

    /**
     * Shows an error message within the subscription widget's body.
     * @param {string} message Error message.
     * @param {HTMLElement} formElement Form element.
    */
    var showMessage = function (message, formElement) {
        if (message && formElement) {
            var errorMessage = "The subscription to the newsletter failed. Error message(s): \n" + message;
            window.medioClinic.showMessage(errorMessage, "error", true);
            var divElement = formElement.querySelector(".mc-error-messages");
            divElement.innerHTML = "";
            var innerSection = window.medioClinic.buildMessageMarkup(messageText, "deep-orange lighten-2");
            divElement.appendChild(innerSection);
        }
    }
}(window.medioClinic.newsletterSubscriptionWidget = window.medioClinic.newsletterSubscriptionWidget || {}));