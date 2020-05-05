/** 
 * Attachments count updater module
 * 
 * Subscribes to event fired by MetafileDialog when dialog is closing.
 * Updates text in element specified by selector. Appends number indicating count of the metafiles (attachments)
 */
cmsdefine(['CMS/EventHub', 'jQuery'], function (hub, $) {

    /**
    * Updates inner html of element with text containing given items count. 
    *
    * @param {jQuery element} element - element to update
    * @param {String} text - element inner text
    * @param {frame} count - items count
    */
    var updateAttachmentCount = function(element, text, count) {
        if (element) {
            if (count > 0) {
                element.html(text + ' (' + count + ')');
            }
            else if (count == 0) {
                element.html(text);
            }
        }
    };
    

    /**
    * Module constructor
    */
    var AttachmentsCountModule = function (data) {
        var that = this;
       
        this.element = $(data.selector);
        this.text = data.text;

        hub.subscribe('UpdateAttachmentsCount', function (count) {
            updateAttachmentCount(that.element, that.text, count);
        });
    };
 
    return AttachmentsCountModule;
});