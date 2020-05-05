var editor = null, lastPos = null, lastFindPos = null, lastQuery = null, marked = null;

/* Search */
function unmark() {
    if (marked == null) return;
    for (var i = 0; i < marked.length; ++i) {
        marked[i].clear();
    }
    marked.length = 0;
}

function search(editor, text, matchCase) {
    unmark();

    if (!text) return true;

    if (lastQuery != text) {
        lastPos = null;
        lastFindPos = null;
    }

    lastQuery = text;

    var findPos = lastPos || editor.getCursor();
    for (var cursor = editor.getSearchCursor(text, findPos, !matchCase); cursor.findNext(); ) {
        var from = cursor.from();
        var to = cursor.to();
        lastPos = { line: to.line, ch: to.ch + 1 };
        lastFindPos = from;
        editor.setSelection(from, to);
        marked.push(editor.markText(from, to, "code-mirror-searched"));
        return true;
    }

    return false;
}

function focusOnTextBox(textBoxId) {
    var txtBox = document.getElementById(textBoxId);
    if (txtBox != null) {
        try { txtBox.focus(); }
        catch (e) { }
    }
}

function init() {
    if (editor == null) {
        if (window.location.search.indexOf("editorName=") != 1)
            return false;

        var editorName = window.location.search.substring(12, window.location.search.length);
        editor = wopener[editorName];
        marked = editor._searchMarked;
    }
}

function findText() {
    init();

    var text = txtSearch.value;
    var matchCase = chkMatchCase.checked;

    if (!search(editor, text, matchCase)) {
        lastFindPos = lastPos = null;
        editor.setCursor({ line: 0, ch: 0 });
        window.alert(editor.toolbar.getLocalizedString("TextNotFound", false) + "\n\n" + text);
    }

    btnSearch.focus();

    return false;
}

/* Replace */
function replace(editor, text, replace, matchCase, all) {
    unmark();

    if (!text) return;

    if (lastQuery != text) {
        lastPos = null;
        lastFindPos = null;
    }

    var count = 0;
    if (!all) {
        var findPos = lastFindPos || lastPos || editor.getCursor();
        var cursor = editor.getSearchCursor(text, findPos, !matchCase);
        if (cursor.findNext()) {
            cursor.replace(replace);
            if (cursor.findNext()) {
                editor.setSelection(cursor.from(), cursor.to());
            }
            lastPos = cursor.to();
            lastFindPos = cursor.from();

            count++;
        }
        else {
            lastFindPos = null;
        }
    }
    else {
        for (var cursor = editor.getSearchCursor(text, { line: 0, ch: 0 }, !matchCase); cursor.findNext(); ) {
            cursor.replace(replace);
            count++;
        }

        lastFindPos = null;
    }

    lastQuery = text;

    return count;
}

function replaceText() {
    init();

    var from = txtSearch.value;
    var to = txtReplace.value;
    var replaceAll = rbAll.checked;
    var matchCase = chkMatchCase.checked;

    var replaced = replace(editor, from, to, matchCase, replaceAll);
    if (replaced == 0) {
        lastFindPos = lastPos = null;
        editor.setCursor({ line: 0, ch: 0 });
        window.alert(editor.toolbar.getLocalizedString("TextNotFound", true) + "\n\n" + from);
    }
    else if (replaceAll) {
        window.alert(replaced + " " + editor.toolbar.getLocalizedString("XOccurencesReplaced", false));
    }

    return false;
}

/* Edit source */
var containsWPchar = false;
var WP_CHAR = "□";
var WPR_CHAR = "▫";

function getSource() {
    var s = window.location.search;
    if (s.indexOf("editorName=") == 1) {
        var editorName = s.substring(12, s.length);
        var editor = window.wopener[editorName];

        var code = editor.getValue();
        containsWPchar = (code.indexOf(WP_CHAR) >= 0);

        txtSource.value = code;
    }
}

function setSource() {
    var s = window.location.search;
    if (s.indexOf("editorName=") == 1) {
        var editorName = s.substring(12, s.length);

        var editor = window.wopener[editorName];

        var code = txtSource.value;

        if (containsWPchar) {
            code = code.replace(WP_CHAR, WPR_CHAR);
            code = code.replace(new RegExp(WP_CHAR, "g"), "");
            code = code.replace(new RegExp(WPR_CHAR, "g"), WP_CHAR);

            if (code.indexOf(WP_CHAR) < 0) {
                code += WP_CHAR;
            }
        }

        editor.setValue(code);

        var inputElem = editor.getInputField();
        if (inputElem.onchange) {
            inputElem.onchange();
        }
    }
}

function doResize() {
    var container = document.getElementById('divContent');

    txtSource.style.height = txtSource.style.maxHeight = (container.clientHeight - 64) + 'px';
    txtSource.style.width = txtSource.style.maxWidth = (container.clientWidth - 32) + 'px';
    txtSource.style.resize = 'none';
}