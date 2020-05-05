var editDialogOpened = 'true';
var pendingEditExecuted = 0;

// Function called from COPY dialog
function RefreshPage(newPath, action, filename) {
    if (newPath != null) {
        if (action == null) {
            action = 'copymovefolder';
        }

        // Special threatment when called by ImageEditor
        if (action == 'edit') {
            pendingEditExecuted = 0;
            if (editDialogOpened == 'false') {
                setTimeout('RaiseEdit(\'' + filename + '\')', 300);
                return;
            }
        }

        SetAction(action, newPath);
        RaiseHiddenPostBack();
    }
}

function UploaderOnClick(fileName) {
    if ((fileName != null) && (fileName != '')) {
        $cmsj('#hdnFileOrigName').attr('value', fileName);
    }
}

function RefreshLibrary(newPath) {
    newPath = newPath.replace(/\\/g, "\\\\").replace(/'/g, "\\'").replace(/"/g, "\\\"").replace(/</g, "\\<").replace(/>/g, "\\>");
    setTimeout("RefreshLibraryOrig('" + newPath + "')", 300);
}

function RefreshLibraryOrig(newPath) {
    SetAction('copymovefinished', newPath);
    RaiseHiddenPostBack();
}

// Confirm mass delete
function MassConfirm(dropdown, msg) {
    var drop = document.getElementById(dropdown);
    if (drop != null) {
        if (drop.value == "delete") {
            return confirm(msg);
        }
        return true;
    }
    return true;
}

function EditDialogStateUpdate(isOpened) {
    editDialogOpened = isOpened;
}

function imageEdit_Refresh(guid) {
    pendingEditExecuted = 0;
    if (editDialogOpened == 'false') {
        setTimeout('RaiseEdit(\'' + guid + '\')', 300);
    }
}

function RaiseEdit(guid) {
    if ((editDialogOpened == 'false') && (pendingEditExecuted == 0)) {
        pendingEditExecuted = 1;
        SetAction('edit', guid);
        RaiseHiddenPostBack();
    }
}