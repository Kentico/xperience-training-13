function ChangeDevice(device) {
    if ((parent !== this) && parent.ChangeDevice) {
        parent.ChangeDevice(device);
    }
}

function CloseSplitMode() {
    if (parent != this) {
        parent.CloseSplitMode();
    }
}

function PerformSplitViewRedirect(originalUrl, newCulture, successCallback, errorCallback, mode) {
    if (parent != this) {
        parent.PerformSplitViewRedirect(originalUrl, newCulture, successCallback, errorCallback, mode);
    }
}

// Refresh tree
function RefreshTree(expandNodeId, selectNodeId) {
    if (parent.RefreshTree) {
        parent.RefreshTree(expandNodeId, selectNodeId);
    }
}

// Select node
function SelectNode(nodeId, nodeElem, tab) {
    if ((parent !== this) && parent.SelectNode) {
        parent.SelectNode(nodeId, nodeElem, tab);
    }
}
