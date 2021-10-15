/**
 * Service for showing information messages.
 */
cmsdefine(['jQuery'], function ($) {

    var TIMEOUT = 2000;

    function getWarning() {
        return $('.alert-warning-absolute, .alert-warning');
    }

    function showWarning(msg, append) {
        var element = getWarning();
        showMessage(element, msg, append);
    }

    function showWarningHide(msg, append) {
        var element = getWarning();
        showMessage(element, msg, append);

        setTimeout(function () {
            element.addClass('hidden');
        }, TIMEOUT);
    }

    function getError() {
        return $('.alert-error-absolute, .alert-error');
    }

    function showError(msg, append) {
        var element = getError();
        showMessage(element, msg, append);
    }

    function getInfo() {
        return $('.alert-info-absolute, .alert-info');
    }

    function showInfo(msg, append) {
        var element = getInfo();
        showMessage(element, msg, append);
    }

    function getSuccess() {
        return $('.alert-success-absolute, .alert-success');
    }

    function showSuccess(msg, append) {
        var element = getSuccess();
        showMessage(element, msg, append);

        setTimeout(function () {
            element.addClass('hidden');
        }, TIMEOUT);
    }

    function showMessage(elem, msg, append) {
        elem.removeClass('hidden');
        var label = elem.find('.alert-label');

        append && label.text().trim() ? label.append(msg) : label.text(msg);
        label.append('<br />');
    }

    function clear() {
        var elem = getInfo();
        clearMessage(elem);
        elem = getSuccess();
        clearMessage(elem);
        elem = getWarning();
        clearMessage(elem);
        elem = getError();
        clearMessage(elem);
    }

    function clearMessage(elem) {
        var label = elem.find('.alert-label');
        label.text('');
        elem.addClass('hidden');
    }

    function ensureCloseButton(elem) {
        if (!elem || !elem.hasClass('alert-dismissable')) {
            return;
        }

        var closeElem = $('<span></span>').addClass('alert-close');
        closeElem.append($('<i></i>').addClass('close icon-modal-close').click(function () {
            elem.addClass('hidden');
        }));
        closeElem.append($('<span>Close</span>').addClass('sr-only'));

        elem.append(closeElem);
    }

    var errorElement = $('.alert-error-absolute, .alert-error');
    var warningElement = $('.alert-warning-absolute, .alert-warning');
    ensureCloseButton(errorElement);
    ensureCloseButton(warningElement);

    return {
        showSuccess: showSuccess,
        showWarning: showWarning,
        showWarningHide: showWarningHide,
        showError: showError,
        showInfo: showInfo,
        clear: clear
    };
});
