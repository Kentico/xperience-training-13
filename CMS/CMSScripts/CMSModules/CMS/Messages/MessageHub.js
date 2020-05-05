cmsdefine(['CMS/EventHub'], function (EventHub) {

    return function () {
        var ERROR_EVENT_KEY = "MessageHubErrorEvent",
            INFO_EVENT_KEY = "MessageHubInfoEvent",
            SUCCESS_EVENT_KEY = "MessageHubSuccessEvent",
            CLEAR_EVENT_KEY = "MessageHubClearEvent",

            /**
             * Publishes given error.
             * @param  message     string  message to be published.
             * @param  description string description for the error message. Might be long text.
             */
            publishError = function (message, description) {
                EventHub.publish(ERROR_EVENT_KEY, message, description);
            },


            /**
             * Subscribes to all errors.
             * @param  callback  function callback with a parameter of an error that was published.
             */
            subscribeToError = function (callback) {
                EventHub.subscribe(ERROR_EVENT_KEY, callback);
            },


            /**
             * Publishes given info.
             * @param  message     string  message to be published.
             * @param  description string description for the info message. Might be long text.
             */
            publishInfo = function (message, description) {
                EventHub.publish(INFO_EVENT_KEY, message, description);
            },


            /**
             * Subscribes to all info messages.
             * @param  callback  function callback with a parameter of an info that was published.
             */
            subscribeToInfo = function (callback) {
                EventHub.subscribe(INFO_EVENT_KEY, callback);
            },


            /**
             * Publishes given success message.
             * @param  message     string  message to be published.
             * @param  description string  description for the success message. Might be long text.
             */
            publishSuccess = function (message, description) {
                EventHub.publish(SUCCESS_EVENT_KEY, message, description);
            },


            /**
             * Subscribes to all success messages.
             * @param  callback  function callback with a parameter of a success message that was published.
             */
            subscribeToSuccess = function (callback) {
                EventHub.subscribe(SUCCESS_EVENT_KEY, callback);
            },


            /**
             * Publishes the clear message.
             * This message is used for clearing out the messages in the message placeholder.
             */
            publishClear = function () {
                EventHub.publish(CLEAR_EVENT_KEY);
            },


            /**
             * Subscribes to clear message.
             * @param  callback  function callback performing the clearing.
             */
            subscriberToClear = function (callback) {
                EventHub.subscribe(CLEAR_EVENT_KEY, callback);
            };


        return {
            publishError: publishError,
            subscribeToError: subscribeToError,
            publishInfo: publishInfo,
            subscribeToInfo: subscribeToInfo,
            publishSuccess: publishSuccess,
            subscribeToSuccess: subscribeToSuccess,
            publishClear: publishClear,
            subscriberToClear: subscriberToClear,
        };
    };
})
