
function InsertSelectedItem(obj) {
    if ((window.wopener) && (obj)) {
        if ((obj.editor_clientid != null) && (obj.editor_clientid != '')) {
            var editor = window.wopener.document.getElementById(obj.editor_clientid);
            if (editor != null) {
                if ((obj.doc_targetnodeid != null) && (obj.doc_targetnodeid != 'undefined') && (obj.doc_targetnodeid != '')) {
                    editor.value = obj.doc_targetnodeid;
                }
                else {
                    editor.value = obj.doc_nodealiaspath;
                }

                var evt = document.createEvent("HTMLEvents");
                evt.initEvent("change", true, true);
                editor.dispatchEvent(evt);
            }
        }
    }
}

function GetSelectedItem(editorId) {

}
