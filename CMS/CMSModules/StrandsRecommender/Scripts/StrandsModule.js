var STRANDS = (function (my, $) {

    /**
     * Performs asynchronous web call using jQuery Ajax and ASP.NET WebMethod.
     * Logs error message to JS console when call is unsuccessful.
     * @param {string} method Web method to be called.
     * @param {object} data Data input to be sent to web method.
     */
    my.webMethodCall = function (method, data) {
        return $.ajax({
            type: "POST",
            url: window.applicationUrl + "CMSModules/StrandsRecommender/Pages/WebMethods.aspx/" + method,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(data),
            dataType: "json"
        });
    };


    /**
     * Builds styled error label containing given message.
     * @param {message} message to be shown.
     */
    my.buildErrorLabel = function(message) {
        return $("<span class=\"ErrorLabel\" ></span>").html(message);
    };

    return my;
}(STRANDS || {}, $cmsj))