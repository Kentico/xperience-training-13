/**
 * Helper module able to perform WebForm specific calls to the server (callbacks and postbacks).
 *
 * NOTE: The page or control must call ScriptHelper.EnsurePostbackMethods in order to use this module, otherwise, the ASP.NET JS functions may not be available.
 */
cmsdefine([], function() {

    var
        /**
         * Performs WebForms Callback.
         *
         * @param {string} option.targetControlUniqueId - Required. The UniqueID of a server Control that handles the client callback. The control must implement the ICallbackEventHandler interface
         * @param {string} option.args - An argument passed from the client script to the server RaiseCallbackEvent method.
         * @param {function} option.successCallback - The name of the client event handler that receives the result of the successful server event.
         * @param {function} option.context - Client script that is evaluated on the client prior to initiating the callback. The result of the script is passed back to the client event handler.
         * @param {function} option.errorCallback - The name of the client event handler that receives the result when an error occurs in the server event handler.
         * @param {bool} option.useAsync - true to perform the callback asynchronously; false to perform the callback synchronously. Default is true.
         */
        doCallback = function (options) {
            WebForm_DoCallback(options.targetControlUniqueId, options.args, options.successCallback, options.context, options.errorCallback, options.useAsync);
        },

        /**
         * Performs WebForms postback.
         *
         * @param {string} option.targetControlUniqueId - Required. The UniqueID of a server Control that handles the client postback. The control must implement the IPostbackEventHandler interface
         * @param {string} option.args - An argument passed from the client script to the server RaisePostbackEvent method.
         */
        doPostback = function(options) {
            __doPostBack(options.targetControlUniqueId, options.args);
        };

    return {
        doCallback: doCallback,
        doPostback: doPostback
    };
});