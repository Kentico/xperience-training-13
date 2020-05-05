function TwitterPostTextAreaControlObj(textAreaId, labelId, hiddenFieldId) {
    this.textAreaClientId = textAreaId;
    this.labelClientId = labelId;
    this.hiddenFieldId = hiddenFieldId;
    var macroRegex = new RegExp(/[{][%].*?[%][}]/gmi);
    var urlMacroRegex = new RegExp(/[{][%]\s*(AbsoluteURL|PermanentURL)\s*[%][}]/gmi);

    var textArea = $cmsj('#' + textAreaId),
        label = $cmsj('#' + labelId),
        field = $cmsj('#' + hiddenFieldId),
        manager = window.TwitterPostTextAreaManager;

    function countCharacters() {
        var text = textArea.val();
        var match = text.match(urlMacroRegex);
        var urlMacroCount = 0;
        if (match) {
            urlMacroCount = match.length;
        }
        text = text.replace(macroRegex, '');
        var textLength = twttr.txt.getTweetLength(text, {
            short_url_length: manager.shortUrlLength,
            short_url_length_https: manager.shortUrlHttpsLength
        });
        textLength += urlMacroCount * manager.shortUrlHttpsLength;

        var charsLeft = manager.postMaxLength - textLength;
        label.text(charsLeft);
        field.val(textLength);
        if (charsLeft < 0) {
            label.addClass("Red");
        } else {
            label.removeClass("Red");
        }
    }

    function init() {
        textArea.bind('input propertychange', countCharacters);
        countCharacters();
    }

    init();
}


function TwitterPostTextAreaManagerObj(urlLength, urlHttpsLength, maxLength) {
    var controls = [];

    this.shortUrlLength = urlLength;
    this.shortUrlHttpsLength = urlHttpsLength;
    this.postMaxLength = maxLength;

    this.addControl = function (options) {
        var id = options.textAreaId;
        var control = new TwitterPostTextAreaControlObj(id, options.labelId, options.hiddenFieldId);
        controls[id] = control;
    };
}


function TwitterPostTextAreaManagerInit(options) {
    if (!window.TwitterPostTextAreaManager) {
        window.TwitterPostTextAreaManager = new TwitterPostTextAreaManagerObj(options.shortUrlLength, options.shortUrlHttpsLength, options.postMaxLength);
    }
    window.TwitterPostTextAreaManager.addControl(options.control);
}