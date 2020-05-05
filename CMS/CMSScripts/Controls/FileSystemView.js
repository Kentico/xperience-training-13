function SetTreeRefreshAction(path) {
    SetAction('refreshtree', path);
    RaiseHiddenPostBack();
}

function FSS_FilesUploaded(files) {
    if (files != null) {
        var i, name;

        for (i = 0; i < files.length; i++) {
            name = files[i].name;
            if ((name.length > 4) && name.substr(name.length - 4, 4).toLowerCase() == ".zip") {
                SetTreeRefreshAction('');
                return;
            }
        }
    }

    SetRefreshAction();
}

function SetRefreshAction() {
    SetAction('refresh', '');
    RaiseHiddenPostBack();
}

function SetDeleteAction(argument) {
    SetAction('delete', argument);
    RaiseHiddenPostBack();
}

function SetSelectAction(argument) {
    SetAction('select', argument);
    RaiseHiddenPostBack();
}

function SetParentAction(argument) {
    SetAction('parentselect', argument);
    RaiseHiddenPostBack();
}