// Delete item action
function DeleteItem(nodeId, parentNodeId) {
    if (nodeId != 0) {
        location.href = "../Delete.aspx?multiple=true&dialog=1&nodeid=" + nodeId;
    }
}

function AddItem(parentNodeId) {
    wopener.NewDocument(parentNodeId, 0, targetWindow);

    if (targetWindow == null) {
        wopener.refreshPageOnClose = false;
        CloseDialog();
    }
}

// Edit item action
function EditItem(nodeId, parentNodeId) {
    if (nodeId != 0) {
        wopener.EditDocument(nodeId, targetWindow);

        if (targetWindow == null) {
            CloseDialog();
        } 
    }
}

// Select item action
function SelectItem(nodeId, parentNodeId) {
    if (nodeId != 0) {
        window.location = wopener.SelectNode(nodeId);
    }
}

function ViewItem(url) {
    wopener.location = url;
    CloseDialog();
}

// Redirect item
function RedirectItem(nodeId, culture, translated, url) {
    if (translated) {
        wopener.location = url;
        CloseDialog();
    }
    else {
        wopener.NewDocumentCulture(nodeId, culture);
        CloseDialog();
    }
}

