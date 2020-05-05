function InsertSelectedItem(obj) {
    if (window.wopener && obj) {
        var path = null;
        if ((obj.item_path) && (obj.item_path != '')) {
            path = obj.item_path;
        }
        if ((obj.editor_clientid != null) && (obj.editor_clientid != '')) {
            var editor = window.wopener.document.getElementById(obj.editor_clientid);
            if (editor != null) {
                if (path != null) {
                    if (editor.src != null) {
                        editor.src = obj.item_resolved_path;
                    }
                    if ((editor.style.backgroundImage != null) && (editor.style.backgroundImage != '')) {
                        editor.style.backgroundImage = obj.item_resolved_path;
                    }
                    if (editor.value != null) {
                        editor.value = path;
                        if (editor.onchange) {
                            editor.onchange();
                        }
                    }
                }
            }
        }
    }
}

function GetSelectedItem(editorId) {
    var obj = null;
    if ((editorId) && (editorId != '')) {
        if (window.wopener) {
            var editor = window.wopener.document.getElementById(editorId);
            if ((editor != null) && (editor.value) && (editor.value != '')) {
                obj = new Object();
                obj.item_path = editor.value;
                
            }
        }
    }
    return obj;
}