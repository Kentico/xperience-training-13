/**
 * Checks URLs and displays element with given id with broken URL names filled in.
 * BrokenEmailUrlNotifier follows all redirects and compares expectedStatusCode and expectedContent with final response.
 * @exports CMS/BrokenEmailUrlNotifier
 */
cmsdefine(['jQuery', 'CMS/UrlChecker'], function ($, UrlChecker) {

    /**
     * Data for BrokenEmailUrlNotifier.
     * @typedef {Object} BrokenEmailUrlNotifier~Options
     * @property {!Object[]} checkedUrls - URLs to be checked.
     * @property {!string} checkedUrls[].name - Name of URL to be displayed when broken.
     * @property {!string} checkedUrls[].url - URL to be checked.
     * @property {?string} checkedUrls[].expectedContent - Expected content after all redirects. If null, content is not checked. If any value (including empty string), content is expected to exactly match.
     * @property {!string} shownElementIdOnError - Id of element shown (as notification) when returned status or content does not exactly match.
     * @property {!string} helpUrl - URL to the documentation
     */

    /**
     * Creates new BrokenEmailUrlNotifier
     * @param {BrokenEmailUrlNotifier~Options} data 
     * @class BrokenEmailUrlNotifier
     */
    var BrokenEmailUrlNotifier = function (data) {
        var brokenUrlNames = [],
            responseCounter = 0,

        processResult = function () {
            if (++responseCounter < data.checkedUrls.length || brokenUrlNames.length === 0) {
                return;
            }

            var warning = $('#' + data.shownElementIdOnError);
            var nameList = '';

            brokenUrlNames.sort();

            brokenUrlNames.forEach(function (brokenUrlName) {
                nameList += '<li>' + brokenUrlName + '</li>';
            });

            warning.html(warning.html().replace('{0}', nameList));
            warning.html(warning.html().replace('{1}', data.helpUrl));
            warning.removeClass('hide');
        }

        data.checkedUrls.forEach(function (checkedUrl) {
            var urlCheckerOptions = {
                expectedStatusCode: 200,
                url: checkedUrl.url,
                expectedContent: checkedUrl.expectedContent,
                onSuccess: processResult,
                onError: function () {
                    brokenUrlNames.push(checkedUrl.name);
                    processResult();
                }
            };
            UrlChecker(urlCheckerOptions);
        });
    };

    return BrokenEmailUrlNotifier;
});
