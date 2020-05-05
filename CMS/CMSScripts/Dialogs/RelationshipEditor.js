
function InsertSelectedItem(obj) {
    if ((window.wopener) && (obj)) {
        if ((obj.editor_clientid != null) && (obj.editor_clientid != '')) {
            var split = obj.editor_clientid.split(';');
            if (split.length == 2) {
                var editorClientId = split[0];
                var hiddenFieldId = split[1];

                // Get elements
                var editor = window.wopener.document.getElementById(editorClientId);
                var hiddenField = window.wopener.document.getElementById(hiddenFieldId);

                // Set node alias path
                if (editor != null) {
                    editor.value = obj.doc_nodealiaspath;
                }

                // Set node id
                if (hiddenFieldId != null) {
                    hiddenField.value = obj.doc_targetnodeid;
                }

                // Refresh
                if (window.wopener.RefreshRelatedPanel) {
                    window.wopener.RefreshRelatedPanel(editorClientId);
                }
            }
        }
    }
}

function GetSelectedItem(editorId) {

}
