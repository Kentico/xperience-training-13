
function InsertSelectedItem(obj){
    if ((window.wopener) && (obj)) {
        if ((obj.editor_clientid != null) && (obj.editor_clientid != '')) {
            var editor = window.wopener.document.getElementById(obj.editor_clientid);
            if (editor != null) {
                var bbcode = null;
                if ((obj.img_url != null) && (obj.img_url != '')) {
                    var size = null;
                    if ((obj.img_width) && (obj.img_height)) {
                        size = '=' + obj.img_width + 'x' + obj.img_height;
                    }
                    bbcode = '[img' + (size ? size : '') + ']' + obj.img_url + '[/img]';
                }
                else {
                    bbcode = CreateBBLink(obj);
                }
                if (bbcode) {
                    insertText(editor, bbcode);
                }
            }
        }
    }
}

function GetSelectedItem(){
    
}


function CreateBBLink(obj){
    var bbcode = null;
    if ((obj.link_url) && (obj.link_url != '')) {
        var url = obj.link_url;
        if ((obj.link_protocol) && (obj.link_protocol != '')) {
            url = obj.link_protocol + url.replace(/^\s*\//ig, '');
        }

        if ((obj.link_text) && (obj.link_text != '')) {
            bbcode = '[url=' + url + ']' + obj.link_text + '[/url]';
        }
        else {
            bbcode = '[url]' + url + '[/url]';
        }
    }
    else if ((obj.anchor_name) && (obj.anchor_name != '')) {
        if ((obj.anchor_linktext) && (obj.anchor_linktext != '')) {
            bbcode = '[url=#' + obj.anchor_name + ']' + obj.anchor_linktext + '[/url]';
        }
        else {
            bbcode = '[url]#' + obj.anchor_name + '[/url]';
        }
    }
    else if ((obj.email_to) && (obj.email_to != '')) {
        var sUrl = 'mailto:';
        // Create mailto href
        sUrl += obj.email_to;
        if ((obj.email_cc) || (obj.email_bcc) || (obj.email_subject) || (obj.email_body)) {
            sUrl += '?';
        }
        if ((obj.email_cc) && (obj.email_cc != '')) {
            sUrl += 'cc=' + obj.email_cc + '&';
        }
        if ((obj.email_bcc) && (obj.email_bcc != '')) {
            sUrl += 'bcc=' + obj.email_bcc + '&';
        }
        if ((obj.email_subject) && (obj.email_subject != '')) {
            sUrl += 'subject=' + escapeUrl(obj.email_subject) + '&';
        }
        if ((obj.email_body) && (obj.email_body != '')) {
            sUrl += 'body=' + escapeUrl(obj.email_body) + '&';
        }
        sUrl = sUrl.replace(/(&)$/, '');
        if ((obj.email_linktext) && (obj.email_linktext != '')) {
            bbcode = '[url=' + sUrl + ']' + obj.email_linktext + '[/url]';
        }
        else {
            bbcode = '[url=' + sUrl + ']' + obj.email_to + '[/url]';
        }
    }
    return bbcode;
}


function insertText(editor, text){

    //IE support
    if (editor.selection) {
        editor.focus();
        sel = editor.selection.createRange();
        sel.text = text;
    }
    //MOZILLA/NETSCAPE support
    else if (editor.selectionStart || editor.selectionStart == '0') {
        var startPos = editor.selectionStart;
        var endPos = editor.selectionEnd;
        editor.value = editor.value.substring(0, startPos) +
        text +
        editor.value.substring(endPos, editor.value.length);
    }
    else {
        editor.value += text;
    }
}

function escapeUrl(text) {
    return encodeURIComponent(decodeURIComponent(text));
}
