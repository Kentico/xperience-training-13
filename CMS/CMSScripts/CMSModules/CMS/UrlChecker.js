/**
 * Adds functionality for URL availability checking. 
 * UrlChecker follows all redirects and compares expectedStatusCode and expectedContent with final response.
 * @exports CMS/UrlChecker
 */
cmsdefine(['jQuery'], function ($) {

    /**
     * Callback after url is checked.
     * @callback UrlChecker~urlCheckCallback
     * @param {jqXHR} jqXHR
     */

    /**
     * Data for UrlChecker
     * @typedef {Object} UrlChecker~Options
     * @property {!string} url - Target URL to check.
     * @property {!number} expectedStatusCode - Expected status returned by the URL after all redirects.
     * @property {?string} expectedContent - Expected content after all redirects. If null, content is not checked. If any value (including empty string), content is expected to exactly match.
     * @property {?UrlChecker~urlCheckCallback} onSuccess - Called when URL returns expectedStatusCode and expectedContent (if provided).
     * @property {?UrlChecker~urlCheckCallback} onError - Called when URL returns unexpected status code or provided content does not exactly match.
     */

    /**
     * Creates new UrlChecker
     * @param {UrlChecker~Options} data 
     * @class UrlChecker
     */
    var UrlChecker = function (data) {
        var url = data.url,
            expectedStatusCode = data.expectedStatusCode,
            expectedContent = data.expectedContent,
            onSuccess = data.onSuccess || function () { },
            onError = data.onError || function () { },

            checkContent = expectedContent != null,
            httpMethod = checkContent ? 'GET' : 'HEAD';

        var isStatusAsExpected = function (status) {
            return status == expectedStatusCode;
        };

        var isContentAsExpected = function (content) {
            return !checkContent || (expectedContent == content);
        };

        var onComplete = function (jqXHR) {
            if (isStatusAsExpected(jqXHR.status) && isContentAsExpected(jqXHR.responseText)) {
                onSuccess(jqXHR);
            } else {
                onError(jqXHR);
            }
        };

        $.ajax(url, {
            dataType: 'text',
            type: httpMethod,
            complete: onComplete,
            async: true
        });
    };

    return UrlChecker;
});
