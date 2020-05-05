/*
 * Module for formatting strings in .NET fashion (String.Format).
 */

cmsdefine([], function () {

    /**
     * Replaces {X} wildcards in first argument with values of the subsequent arguments.
     * Example: format("hello {0} {1}!", "john", "doe") returns "hello john doe!".
     *
     * @param {string} input - input string format. Can contain wildcards.
     * @return {string} formatted string
     */
    var format = function (input) {
        var args = arguments;
        return input.replace(/\{(\d+)\}/g, function(match, capture) {
            return args[parseInt(capture) + 1];
        });
    };

    return {
        format: format
    };
});