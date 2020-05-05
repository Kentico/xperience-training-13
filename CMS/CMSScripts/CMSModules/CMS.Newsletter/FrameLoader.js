/**
 * Module providing frameLoaded method to ensure that iframe has finished its loading
 */
cmsdefine(['jQuery'], function ($) {

    /**
     * Fires the doneFunction callback once the iframe is loaded.
     * 
     * @param {Element} iframe (DOM element or jQuery)
     * @callback doneFunction 
     */
    function frameLoaded(iframe, doneFunction) {
   
        if (!$(iframe).contents().find('body').html()) {
            $(iframe).on('load', function () {
                doneFunction();
            });
        } else {
            doneFunction();
        }
    }

    return {
        frameLoaded: frameLoaded
    };
});