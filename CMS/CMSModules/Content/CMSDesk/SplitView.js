var IsCMSDesk = true; 

function RefreshTree(expandNodeId, selectNodeId) {
    parent.RefreshTree(expandNodeId, selectNodeId);
}

function SelectNode(nodeId) {
    parent.SelectNode(nodeId);
}

function NewDocument(parentNodeId, className) {
    parent.NewDocument(parentNodeId, className);
}

function DeleteDocument(nodeId) {
    parent.DeleteDocument(nodeId);
}

function EditDocument(nodeId, tab) {
    parent.EditDocument(nodeId, tab);
}

function PerformSplitViewRedirect(originalUrl, newCulture, successCallback, errorCallback, mode) {
    parent.PerformSplitViewRedirect(originalUrl, newCulture, successCallback, errorCallback, mode);
}

function ChangeDevice(device) {
    parent.ChangeDevice(device);
}