cmsdefine(['jQuery'], function ($) {

    /* This limitation is just to prevent the preheader of being too long and span across multiple rows. 
    * (Preview looks bad when there is too long preheader spanning over multiple lines)
    * It will be truncated and full text of preheader will be available in the tooltip. */
    var PREHEADER_MAX_LENGTH = 200;

    var previewFrame,
        mobilePreviewFrame,
        subjectElement,
        preheaderElement,
        recipientsElement,   
        designPreviewButtonElement,     
        codePreviewButtonElement,
        designPreviewElement,
        subjects,
        preheaders,
        emails,
        frameUrl,
        codePreviewElement,
        preheaderMaxLength;

    /**
    * Sets preview frames and changes frames content on change of subscribers dropdown.
    */
    var EmailPreviewModule = function (data) {

        previewFrame = $('#preview');
        mobilePreviewFrame = $('#mobile-preview');
        subjectElement = $('#' + data.lblSubjectId);
        preheaderElement = $('#' + data.lblPreheaderId);
        recipientsElement = $('#' + data.dropDownListId);    
        designPreviewButtonElement = $('#btnDesignPreview');      
        codePreviewButtonElement = $('#btnCodePreview');  
        designPreviewElement = $('#design-preview');
        codePreviewElement = $('#code-preview');

        subjects = data.subjects;
        preheaders = data.preheaders;
        emails = data.emails;
        frameUrl = data.url;
        preheaderMaxLength = PREHEADER_MAX_LENGTH;

        recipientsElement.change(this.subscriberChanged.bind(this));
        designPreviewButtonElement.click(this.displayDesignPreview);
        codePreviewButtonElement.click(this.displayCodePreview);
        
        this.setPreview.call(this, 0);
    };

    EmailPreviewModule.prototype.setPreheader = function (index) {
        var preheader = preheaders[index].substring(0, preheaderMaxLength);

        if (preheaders[index].length > preheaderMaxLength) {
            preheader += '&hellip;';
            preheaderElement.attr('title', preheaders[index]);
        }

        preheaderElement.html(preheader);
    }

    EmailPreviewModule.prototype.setPreview = function (index) {
        subjectElement.html(subjects[index]);
        preheaderElement.html(preheaders[index]);
        var frameSrc = frameUrl + '&recipientemail=' + emails[index];
        previewFrame.attr('src', frameSrc);
        mobilePreviewFrame.attr('src', frameSrc);

        this.setEmailSource.call(this, frameSrc);
        this.setPreheader.call(this, index);
    }

    EmailPreviewModule.prototype.subscriberChanged = function () {
        if (recipientsElement.val()) {
            var index = recipientsElement.val();
            this.setPreview.call(this, index);
        }
    }

    EmailPreviewModule.prototype.displayDesignPreview = function () {
        designPreviewElement.show();
        codePreviewElement.hide();
        codePreviewButtonElement.removeClass('active');
        designPreviewButtonElement.addClass('active');
    }

    EmailPreviewModule.prototype.displayCodePreview = function () {
        designPreviewElement.hide();
        codePreviewElement.show();
        codePreviewButtonElement.addClass('active');
        designPreviewButtonElement.removeClass('active');
    }

    EmailPreviewModule.prototype.setEmailSource = function (sourceUrl) {
        $.ajax({
            url: sourceUrl,
            dataType: 'html'
        }).done(function(data) {
            var editor = $('.code-mirror')[0].CodeMirror;
            editor.setValue(data);
        });
    }

    return EmailPreviewModule;
});